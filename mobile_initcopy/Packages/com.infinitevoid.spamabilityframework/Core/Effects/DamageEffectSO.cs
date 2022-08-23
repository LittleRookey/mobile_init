using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Damage", fileName = "damageeffect.asset")]
    public class DamageEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _damageValue;
        #if UNITY_EDITOR
        [SerializeField] private bool _debugLogging = false;
        #endif

        protected override string _metaHelpDescription =>
            $"Deals damage to the target. Requires the target to have an {nameof(IDamageable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var damageable = target.Transform.GetComponent<IDamageable>();

            if (damageable == null)
            {
#if UNITY_EDITOR
                if(_debugLogging)
                    LogWarning($"{target.Transform.name} does not have an {nameof(IDamageable)}-component");
#endif
                return;
            }
            damageable.Damage(_damageValue);
        }
    }
}