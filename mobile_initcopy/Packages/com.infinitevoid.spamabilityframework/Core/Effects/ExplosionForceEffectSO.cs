using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// Applies an explosion force when the ability hits. The ability has to be an AOE-ability for this to work
    /// </summary>
    [CreateAssetMenu(menuName = "Ability effects/Explosion force (AOE)", fileName = "explosionForceEffect.asset")]
    public class ExplosionForceEffectSO : AbilityEffectSO
    {
        [SerializeField] private float _force;
        [SerializeField] private float _upModifier;

        protected override string _metaHelpDescription =>
            "(AOE-Effect) Adds explosion force to the targets. Requires the ability to have an effect radius greater than 0 and the targets to have non-kinematic rigidbodies.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
#if UNITY_EDITOR
            if (!ability.AOEAbility)
            {
                LogWarning(
                    $"An Explosioneffect ({this.name}) is added to {ability.name} but the ability is not an AOE ability.");
                return;
            }
#endif

            var rby = target.Transform.GetComponent<Rigidbody>();
            if (rby == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} has no rigidbody attached.");
#endif
                return;
            }

            rby.AddExplosionForce(_force, abilityPos, ability.EffectRadius, _upModifier);
        }
    }
}