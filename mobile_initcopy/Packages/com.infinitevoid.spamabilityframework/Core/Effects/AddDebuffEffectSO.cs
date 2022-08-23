using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Add Debuff", fileName = "debuffeffect.asset")]
    public class AddDebuffEffectSO : AbilityEffectSO
    {
        [SerializeField] private ScriptableObject _debuff;

        protected override string _metaHelpDescription =>
            $"Adds a debuff to the target. Requires the target to have an {nameof(IDebuffable)}-component and the debuff to implement {nameof(IDebuff)}.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var debuffable = target.Transform.GetComponent<IDebuffable>();
            if (debuffable == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} has no {nameof(IDebuffable)}-component");
#endif
                return;
            }

            if (!(_debuff is IDebuff debuff))
            {
#if UNITY_EDITOR
                LogWarning($"{name}: the Buff supplies does not inherit from {nameof(IDebuff)}");
#endif
                return;
            }

            debuffable.AddDebuff(debuff);
        }
    }
}