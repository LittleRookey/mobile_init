using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Extensions;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting
    /// of an <see cref="ProjectileAbilitySO"/> at a given <see cref="IAbilityTarget"/> or point 
    /// </summary>
    public class TargetedProjectileAbility : ProjectileAbility, ITargetedAbility
    {
        private IAbilityTarget _target;
        private Vector3 _position;
        public bool RequiresTarget => Ability.RequiresAbilityTarget;
        public bool CastOnSelf => false;
        
        
        
        public void Cast(IAbilityTarget target)
        {
            _target = target;
            base.CastAbility(target.Transform.position);
        }
        
        public override void Cast(Vector3 position)
        {
            _position = ProjectileAbilitySo.MoveIn3d ? position : position.With(y: SpawnPoint.y);
            base.CastAbility(position);
        }

        protected override void OnCast()
        {
            base.PlayCastSFX();
            var projectile = base.GetProjectile(base.SpawnPoint);
            if(_target != null)
                projectile.MoveToTarget(_target, base.ProjectileAbilitySo, base.Invoker);
            else projectile.MoveTo(_position, base.ProjectileAbilitySo, base.Invoker, ProjectileAbilitySo.DistanceCheckRange);
            _target = null;
        }

    }
}