using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for a directional ability, an ability that's cast and hits and area in a given direction.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Directional Ability", fileName = "directionalAbility.asset")]
    public class DirectionalAbilitySO : AbilityBaseSO
    {
        [SerializeField] private float _angle = 45;
        [SerializeField] private float _distance = 3;

        public float Distance => _distance;
        public float Angle => _angle;

        public AbilityBase AddTo(GameObject gameObject)
        {
            var component = gameObject.AddComponent<DirectionalAbility>();
            component.SetAbility(this);
            component.GetInvokerFromHierarchy();
            return component;
        }

        public override bool TargetIsInSight(AbilityInvoker invoker, IAbilityTarget potentialTarget, Vector3 _,
            Vector3 lookDirection)
        {
            var casterPos = invoker.transform.position;
            casterPos.y = 0;
            var directionToTarget = potentialTarget.Transform.position - casterPos;
            directionToTarget.y = 0;
            float deltaAngle = Vector3.Angle(directionToTarget, lookDirection);
            if (this.Angle < deltaAngle)
            {
                return false;
            }
            
            if(!this.LineOfSightCheck)
                return true;
            var targetPos = potentialTarget.Transform.position.With(y: 0);
            return !Physics.Linecast(casterPos, targetPos, this.LosBlockingLayers);
        }
    }
}