using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Shield", fileName = "shieldeffect.asset")]
    public class ShieldEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _shieldValue;

        protected override string _metaHelpDescription =>
            $"Adds shield to the target. Requires the target to have an {nameof(IShieldable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var shieldable = target.Transform.GetComponent<IShieldable>();
            if (shieldable == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IShieldable)}-component");
#endif
                return;
            }

            shieldable.Shield(_shieldValue);
        }
    }
}