using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Abilities
{
    /// <summary>
    /// Base class for projectile abilities.
    /// </summary>
    public abstract class ProjectileAbility : AbilityBase
    {
        [Space(20)]
        [SerializeField] private ProjectilePool _projectilePool;
        
        [SerializeField] private Transform _spawnPoint;

        private ProjectileAbilitySO _currentAbility;

        protected override AbilityBaseSO Ability => _currentAbility;
        protected override bool CacheOnHitVfx => false;
        protected ProjectileAbilitySO ProjectileAbilitySo => _currentAbility;
        protected Vector3 SpawnPoint => _spawnPoint.position;

        protected Projectile GetProjectile(Vector3 position)
        {
            return _projectilePool.GetProjectile(position);
        }

        protected sealed override void OnStart()
        {
            #if UNITY_EDITOR
            if (_projectilePool == null)
            {
                Debug.LogWarning($"Pool is null in {gameObject.name}", this);
                return;
            }
            #endif
            _currentAbility =  _projectilePool.ProjectileAbilitySo;
        }

        public void SetProjectileSpawnPoint(Transform spawnPoint)
        {
            this._spawnPoint = spawnPoint;
        }

        /// <summary>
        /// Sets the projectile-pool for this ability.
        /// </summary>
        /// <param name="pool"></param>
        public void SetPool(ProjectilePool pool)
        {
            this._projectilePool = pool;
        }

        public GameObject CreateAndSetPool(ProjectileAbilitySO projectileAbilitySO)
        {
            var poolGo = new GameObject($"{projectileAbilitySO.Name}_pool");
            this._projectilePool = poolGo.AddComponent<ProjectilePool>();
            this._projectilePool.SetProjectile(projectileAbilitySO);
            return poolGo;
        }

        protected override void CleanupOnDestroy()
        {
            if(_projectilePool)
                this._projectilePool.DestroySelf();
        }
    }
}