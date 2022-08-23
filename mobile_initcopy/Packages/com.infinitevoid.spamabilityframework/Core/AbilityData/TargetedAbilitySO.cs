using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for a targeted ability
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Targeted Ability", fileName = "targetedAbility.asset")]
    public class TargetedAbilitySO : AbilityBaseSO
    {
        [SerializeField] private bool _castOnSelf;

        public bool CastOnSelf => _castOnSelf;

        public AbilityBase AddTo(GameObject gameObject, Transform warmupVfxSpawn)
        {
            TargetedAbility component = gameObject.AddComponent<TargetedAbility>();
            component.SetAbility(this);
            component.SetWarmupVFXSpawn(warmupVfxSpawn);
            component.GetInvokerFromHierarchy();
            return component;
        }

        public override bool TargetIsInSight(AbilityInvoker _, IAbilityTarget potentialTarget, Vector3 hitPosition,
            Vector3 lookDirection)
        {
            if(!this.LineOfSightCheck)
                return true;
                
            return !Physics.Linecast(hitPosition, potentialTarget.Transform.position, this.LosBlockingLayers);
        }
    }
}