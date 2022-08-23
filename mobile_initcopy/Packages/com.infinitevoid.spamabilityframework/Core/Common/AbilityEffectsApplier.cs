using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Handles applying an ability's effects to a given list of targets.
    /// </summary>
    public class AbilityEffectsApplier
    {
        public bool EffectsApplied => _curState == ApplyEffectsState.Done;
        public float MaxEffectTime { get; private set; }

        private IAbilityTarget[] _validTargets;
        private readonly AbilityBaseSO _ability;
        private AbilityInvoker _invoker;
        private Vector3 _hitpos;
        private float _timeElapsed = 0;
        private ApplyEffectsState _curState = ApplyEffectsState.Done;
        private int _curTargetIndex = 0;
        private IAbilityTarget _curTarget;
        private EffectAndTime _curEffect;
        private int _curEffectIndex = 0;

        public AbilityEffectsApplier(AbilityBaseSO ability)
        {
            this._ability = ability;
            float timePerEffect = 0f;
            for (int i = 0; i < ability.AbilityEffects.Count; i++)
            {
                timePerEffect += ability.AbilityEffects[i].EffectTime;
            }
            MaxEffectTime = ability.ApplyPerTarget ? 
                (ability.PerTargetEffectApplicationDelay * ability.MaxEffectTargets) + (timePerEffect * ability.MaxEffectTargets)
                : timePerEffect * ability.MaxEffectTargets;
        }


        public void ApplyEffects(Vector3 hitPosition, IAbilityTarget[] targets, AbilityInvoker invoker)
        {
            _invoker = invoker;
            _hitpos = hitPosition;
            _validTargets = targets;
            _curState = ApplyEffectsState.Initialize;
        }

        public void Update(float deltaTime)
        {
            // This was first implemented as a coroutine but that generated ~80b of garbage every cast even with MEC.
            // To keep lib garbage free this is now a state machine, which makes it less readable but has other upsides
            // as all operations are spread over frames for less calculations each frame.
            switch (_curState)
            {
                case ApplyEffectsState.Done:
                    return;
                case ApplyEffectsState.Initialize:
                    _curTargetIndex = 0;
                    _curEffectIndex = 0;
                    if (_curTargetIndex < _validTargets.Length)
                        _curTarget = _validTargets[_curTargetIndex];
                    if (_curEffectIndex < _ability.AbilityEffects.Count)
                        _curEffect = _ability.AbilityEffects[_curEffectIndex];
                    if (_curTarget == null || _curEffect == null || !_curEffect.Effect)
                    {
                        _curState = ApplyEffectsState.Done;
                        return;
                    }

                    _timeElapsed = 0;
                    _curEffectIndex++;
                    _curTargetIndex++;

                    _curState = ApplyEffectsState.ApplyEffectToTarget;
                    return;

                case ApplyEffectsState.GetNextTarget:
                    if (_curTargetIndex < _validTargets.Length)
                    {
                        _curTarget = _validTargets[_curTargetIndex];
                        _curTargetIndex++;
                        if (_curTarget == null)
                            _curState = ApplyEffectsState.NoMoreTargets;
                        else
                            _curState = ApplyEffectsState.ApplyEffectToTarget;
                    }
                    else
                    {
                        _curState = ApplyEffectsState.NoMoreTargets;
                    }

                    return;
                case ApplyEffectsState.NoMoreTargets:
                    // If we apply effects per target and there's no more targets we're done
                    if (_ability.ApplyPerTarget)
                        _curState = ApplyEffectsState.Done;
                    // Effects are applied per effect and there's no more targets. Current effect is done
                    // as there's no more targets. Get the next effect.
                    else
                    {
                        _curTargetIndex = 0;
                        _curTarget = _validTargets[_curTargetIndex];
                        _curState = ApplyEffectsState.GetNextEffect;
                    }

                    return;
                case ApplyEffectsState.GetNextEffect:
                    // If there's a next effect we get it and apply it to the current target
                    if (_curEffectIndex < _ability.AbilityEffects.Count)
                    {
                        _curEffect = _ability.AbilityEffects[_curEffectIndex];
                        _curEffectIndex++;
                        if(!_curEffect.Effect)
                            _curState = ApplyEffectsState.NoMoreEffects;
                        else 
                            _curState = ApplyEffectsState.ApplyEffectToTarget;
                    }
                    else
                    {
                        _curState = ApplyEffectsState.NoMoreEffects;
                    }

                    return;
                case ApplyEffectsState.NoMoreEffects:
                    if (_ability.ApplyPerTarget)
                    {
                        // There's no more effects to apply to the current target which means we're done
                        // with the current target. Get first effect and wait for next target
                        _curEffectIndex = 0;
                        _curEffect = _ability.AbilityEffects[_curEffectIndex];
                        _curState = ApplyEffectsState.WaitForNextTarget;
                    }
                    else
                    {
                        // If there's no more effects to apply and we don't apply effects to each target in order
                        // we're done
                        _curState = ApplyEffectsState.Done;
                    }

                    return;
                case ApplyEffectsState.ApplyEffectToTarget:
                    _curEffect.Effect.ApplyTo(_curTarget, _hitpos, _ability, _invoker);
                    _curState = _ability.ApplyPerTarget
                        ? ApplyEffectsState.WaitForNextEffect
                        : ApplyEffectsState.WaitForNextTarget;
                    return;
                case ApplyEffectsState.WaitForNextEffect:
                    _timeElapsed += deltaTime;
                    if (_timeElapsed < _curEffect.EffectTime)
                        return;
                    _timeElapsed = 0;
                    _curState = ApplyEffectsState.GetNextEffect;
                    return;
                case ApplyEffectsState.WaitForNextTarget:
                    _timeElapsed += deltaTime;
                    if (_timeElapsed < _ability.PerTargetEffectApplicationDelay)
                        return;
                    _timeElapsed = 0;
                    _curState = ApplyEffectsState.GetNextTarget;
                    return;
            }
        }

        private enum ApplyEffectsState
        {
            Done,
            Initialize,
            ApplyEffectToTarget,
            GetNextTarget,
            GetNextEffect,
            WaitForNextEffect,
            WaitForNextTarget,
            NoMoreTargets,
            NoMoreEffects
        }
    }
}