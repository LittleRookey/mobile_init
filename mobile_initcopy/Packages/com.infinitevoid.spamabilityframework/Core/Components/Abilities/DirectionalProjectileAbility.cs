using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Extensions;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// The component that's added to a gameobject to allow casting of
    /// an <see cref="ProjectileAbilitySO"/> in a given direction
    /// </summary>
    public class DirectionalProjectileAbility : ProjectileAbility
    {
        private Vector3 _direction;

        public override void Cast(Vector3 pointInDirection)
        {
            _direction = ProjectileAbilitySo.MoveIn3d ? pointInDirection : pointInDirection.With(y: SpawnPoint.y);
            base.CastAbility(_direction);
        }

        protected override void OnCast()
        {
            base.PlayCastSFX();
            var projectile = base.GetProjectile(base.SpawnPoint);
            projectile.MoveInDirection(_direction, base.ProjectileAbilitySo, base.Invoker);
        }
    }
}