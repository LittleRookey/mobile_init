using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Handles getting valid <see cref="IAbilityTarget"/>s for a given ability.
    /// </summary>
    public class AbilityTargetsResolver
    {
        /// <summary>
        /// Contains the valid targets gotten after calling <see cref="ResolveTargets"/>.
        /// Targets will be added from the beggining up to the max number of targets for the ability, and if
        /// less targets than max are found then remaining elements will be null.
        /// I.e. if 2 targets were resolved then Targets[2] - Targets[max] will be null.
        /// </summary>
        public IAbilityTarget[] ValidTargets { get; private set; }
        private readonly AbilityBaseSO _ability;
        private AbilityInvoker _invoker;
        private readonly Collider[] _colliders;
        // private readonly Func<IAbilityTarget, Vector3, Vector3, bool> _isInSightStrategy;
        
        public AbilityTargetsResolver(AbilityBaseSO ability)
        {
            this._ability = ability;
            if (ability.AOEAbility)
                _colliders = new Collider[ability.MaxEffectTargets];

            ValidTargets = new IAbilityTarget[_ability.MaxEffectTargets];
        }

        public void ResolveTargets(AbilityInvoker invoker, Vector3 hitPosition, IAbilityTarget target = null)
        {
            this._invoker = invoker;
            if (_ability.AbilityEffects == null || _ability.AbilityEffects.Count == 0)
            {
                return;
            }
#if UNITY_EDITOR
            // This is only done in editor as the number of targets should be changeable during runtime
            ValidTargets = new IAbilityTarget[_ability.MaxEffectTargets];
#else
                Array.Clear(ValidTargets,0,ValidTargets.Length);
#endif
            if (_ability.AOEAbility)
            {
                GetValidTargetsInRadius(hitPosition);
            }
            else if (target != null)
                ValidTargets[0] = target;
        }
        
        private void GetValidTargetsInRadius(Vector3 hitPosition)
        {
            int numColliders;
            float radius;
            if (_ability is DirectionalAbilitySO directionalAbility)
                radius = directionalAbility.Distance;
            else
                radius = _ability.EffectRadius;
            
            numColliders = Physics.OverlapSphereNonAlloc(hitPosition, radius, _colliders,
                _ability.EffectLayers);

            var lookDirection = Vector3.zero;
            if(_invoker)
                lookDirection = hitPosition - _invoker.transform.position;
            int validTargetIdx = 0;
            for (int i = 0; i < numColliders; i++)
            {
                if(!_colliders[i].TryGetComponent<IAbilityTarget>(out var potentialTarget))
                    continue;

                if (_ability.TargetIsInSight(_invoker, potentialTarget, hitPosition, lookDirection))
                {
                    ValidTargets[validTargetIdx] = potentialTarget;
                    validTargetIdx++;
                }
            }
        }
    }
}