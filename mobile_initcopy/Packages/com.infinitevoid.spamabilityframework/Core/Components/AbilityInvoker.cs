using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InfiniteVoid.SpamFramework.Core.Components
{
    /// <summary>
    /// Handles all the basic coordination of invoking abilities: Cooldown, triggers, events.
    /// <para></para>
    /// Must be attached this to a GameObject that should be able to cast abilities.
    /// </summary>
    public class AbilityInvoker : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioSource _audioSource;
#if UNITY_EDITOR
        [Header("Debug")] [SerializeField] private bool _logEvents = true;
#endif
        private float _time;
        private bool _casting => _curState != CastState.Idle;
        private Transform _transform;

        private AbilityBaseSO _ability;
        private CastState _curState;
        private Action _warmupAction;
        private Action _castAction;
        private Action _cooldownAction;

        public Vector3 Position => _transform.position;
        public Vector3 Forward => _transform.forward;
        public AudioSource AudioSource => _audioSource;
        public bool DoneInvoking => !this.enabled;

        private void Awake()
        {
            _transform = transform;
#if UNITY_EDITOR
            if (!_animator)
                Debug.LogWarning($"{this.gameObject.name} has no animator and won't play animations");
            if (!_audioSource)
                Debug.LogWarning($"{this.gameObject.name} has no audio source and won't play casting sounds");
#endif
        }

        public bool CanCastAbility(AbilityBaseSO ability, Vector3 target)
        {
            if (_casting)
            {
#if UNITY_EDITOR
                if (_logEvents)
                    Debug.Log($"Already casting, cancelling", this.gameObject);
#endif
                AbilitySystemEvents.RaiseAlreadyCasting();
                return false;
            }

            if (0 < ability.CastRange && !TargetIsWithinRange(Position, target, ability.CastRange))
            {
#if UNITY_EDITOR
                if (_logEvents)
                    Debug.Log($"Target is not within casting distance", gameObject);
#endif
                AbilitySystemEvents.RaiseTargetNotInCastRange();
                return false;
            }

            return true;
        }

        public void CastAbility(AbilityBaseSO ability, Action warmupAbilityAction, Action castAbilityAction,
            Action cooldownAction)
        {
            _ability = ability;
            if (!_ability.AnimationTimings)
            {
                warmupAbilityAction.Invoke();
                castAbilityAction.Invoke();
                cooldownAction?.Invoke();
            }
            else
            {
                _warmupAction = warmupAbilityAction;
                _castAction = castAbilityAction;
                _cooldownAction = cooldownAction;
                this.enabled = true;
                _curState = CastState.StartWarmup;
            }
        }

        private bool TargetIsWithinRange(Vector3 casterPosition, Vector3 targetPosition, float abilityCastRange)
        {
            return casterPosition.IsWithinRangeTo(targetPosition, abilityCastRange);
        }

        private void Update()
        {
            switch (this._curState)
            {
                case CastState.Idle:
                    this.enabled = false;
                    return;
                case CastState.StartWarmup:
                    _warmupAction?.Invoke();
                    if (_animator)
                        _ability.AnimationTimings.PlayWarmupAnimation(_animator);
                    _time = 0;
                    _curState = CastState.WaitForWarmupAnimation;
                    return;
                case CastState.WaitForWarmupAnimation:
                    _time += Time.deltaTime;
                    if (_time < _ability.AnimationTimings.WarmupTime)
                        return;
                    _curState = CastState.CastAbility;
                    return;
                case CastState.CastAbility:
                    _castAction.Invoke();
                    _cooldownAction?.Invoke();
                    if (_animator)
                        _ability.AnimationTimings.PlayCastAnimation(_animator);
                    _time = 0;
                    _curState = CastState.WaitForCastAnimation;
                    return;
                case CastState.WaitForCastAnimation:
                    _time += Time.deltaTime;
                    if (_time < _ability.AnimationTimings.CastTime)
                        return;
                    _time = 0;
                    _curState = CastState.PlayCooldownAnimation;
                    return;
                case CastState.PlayCooldownAnimation:
                    if (_animator)
                        _ability.AnimationTimings.PlayCooldownAnimation(_animator);
                    _curState = CastState.WaitForCooldownAnimation;
                    return;
                case CastState.WaitForCooldownAnimation:
                    _time += Time.deltaTime;
                    if (_time < _ability.AnimationTimings.CooldownTime)
                        return;
                    _time = 0;
                    _curState = CastState.Idle;
                    this.enabled = false;
                    return;
            }
        }

        public void StopCasting()
        {
            _curState = CastState.Idle;
            if (_animator && _ability && _ability.AnimationTimings)
            {
                _ability.AnimationTimings.StopAllAnimations(_animator);
            }

            this.enabled = false;
        }

        private enum CastState
        {
            Idle,
            StartWarmup,
            WaitForWarmupAnimation,
            CastAbility,
            WaitForCastAnimation,
            PlayCooldownAnimation,
            WaitForCooldownAnimation
        }
    }
}