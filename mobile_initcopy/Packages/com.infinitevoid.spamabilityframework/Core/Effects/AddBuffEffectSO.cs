using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Add buff", fileName = "buffeffect.asset")]
    public class AddBuffEffectSO : AbilityEffectSO
    {
        [SerializeField] private ScriptableObject _buff;

        protected override string _metaHelpDescription =>
            $"Adds a buff to the target. Requires the target to have an {nameof(IBuffable)}-component and the buff to implement {nameof(IBuff)}.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var buffable = target.Transform.GetComponent<IBuffable>();
            if (buffable == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} has no {nameof(IBuffable)}-component");
#endif
                return;
            }

            if (!(_buff is IBuff buff))
            {
#if UNITY_EDITOR
                LogWarning($"{name}: the Buff supplies does not inherit from {nameof(IBuff)}");
#endif
                return;
            }

            buffable.AddBuff(buff);
        }
    }
}