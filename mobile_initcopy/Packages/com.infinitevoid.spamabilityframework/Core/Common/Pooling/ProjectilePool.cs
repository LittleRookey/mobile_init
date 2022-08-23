using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// A specialized pool for handling projectiles 
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private ProjectileAbilitySO _projectileInPool;
        [SerializeField] private int _noInPool = 1;
        [HelpBox("When abilities that reference this pool are destroyed they will call Destroy on this pool. Check this if you want to manage its lifetime manually.")]
        [SerializeField] private bool _keepAlive;
        private Pool<Projectile> _projectilePool;
        private Pool<Transform> _onHitVfxPool;
        private GameObject _onHitVFXToSpawn;
        private Func<Transform> _spawnVfxAction;
        private Transform _transform;
        private int _numProjectilesAlive = 0;
        private Action _decreaseNumProjectileAliveAction;

        public ProjectileAbilitySO ProjectileAbilitySo => _projectileInPool;

        private void Awake()
        {
            _transform = this.transform;
            _decreaseNumProjectileAliveAction = () => this._numProjectilesAlive--;
            this.enabled = false;
            if (_projectileInPool != null)
                InitPool();
            // _spawnVfxAction = InstantiateOnHitVFX;
        }

        private void Update()
        {
            if (0 < this._numProjectilesAlive)
                return;
            
            Destroy(this.gameObject);
        }

        private void InitPool()
        {
            _projectilePool = new Pool<Projectile>(_noInPool, () =>
            {
                var projectileInstance = Instantiate(_projectileInPool.Prefab, this._transform);
                projectileInstance.gameObject.SetActive(false);
                projectileInstance.Init(ProjectileAbilitySo, _transform, _decreaseNumProjectileAliveAction);
                return projectileInstance;
            });
        }

        public void SetProjectile(ProjectileAbilitySO projectileAbilitySo, bool createPoolImmediately = false)
        {
            _projectileInPool = projectileAbilitySo;
            if(createPoolImmediately)
                InitPool();
        }


        public Projectile GetProjectile(Vector3 position)
        {
            _numProjectilesAlive++;
            return _projectilePool.GetNext(position, true, Vector3.forward);
        }

        public void DestroySelf()
        {
            if (_keepAlive)
                return;
            if (this)
            {
                this.enabled = true;
            }
        }
    }
}