using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Stun", fileName = "stuneffect.asset")]
    public class StunEffectSO : AbilityEffectSO
    {
        [SerializeField] private float _stunTime;

        protected override string _metaHelpDescription =>
            $"Stuns the target for the given time. Requires the target to have an {nameof(IStunable)}-component.";

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            var stunable = target.Transform.GetComponent<IStunable>();
            if (stunable == null)
            {
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} does not have an {nameof(IStunable)}-component");
#endif
                return;
            }

            stunable.Stun(_stunTime);
        }
    }
}