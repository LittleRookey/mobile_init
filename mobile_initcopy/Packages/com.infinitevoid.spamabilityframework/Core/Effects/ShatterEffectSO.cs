using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Shatter", fileName = "shatterEffect.asset")]
    public class ShatterEffectSO : AbilityEffectSO
    {
        protected override string _metaHelpDescription => $@"Shatters the target into pieces. Requires that the target has an {nameof(IShatterable)}-component. You can create your own or use the supplied {nameof(ProcedualShatter)}-component."; 
        
        [Tooltip("Number of times each piece should be cut. A higher number equals more pieces but also more instantiations and destroys")]
        [SerializeField] private int _cutCascades = 3;
        [SerializeField] private int _explodeForce = 500;
        [SerializeField] private int _minPartDespawnSeconds = 3;
        [SerializeField] private int _maxPartDespawnSeconds = 5;

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            if (target.Transform == null) return;
            var shatter = target.Transform.GetComponent<IShatterable>();
            if (shatter == null)
            {
                
#if UNITY_EDITOR
                LogWarning($"{name}: {target.Transform.name} has no {nameof(IShatterable)}-component");
    #endif
                return;
            }
            shatter.DestroyMesh(_cutCascades, _explodeForce, _minPartDespawnSeconds, _maxPartDespawnSeconds);
        }
    }
}