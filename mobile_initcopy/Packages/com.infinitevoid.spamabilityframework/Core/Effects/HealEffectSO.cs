using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Heal", fileName = "healeffect.asset")]
    public class HealEffectSO : AbilityEffectSO
    {
        [SerializeField] private int _healValue;

        protected override string _metaHelpDescription =>
            $"Heals the target. Requires the target to have an {nameof(IHealable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var healable = target.Transform.GetComponent<IHealable>();
            if (healable == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IHealable)}-component");
#endif
                return;
            }

            healable.Heal(_healValue);
        }
    }
}