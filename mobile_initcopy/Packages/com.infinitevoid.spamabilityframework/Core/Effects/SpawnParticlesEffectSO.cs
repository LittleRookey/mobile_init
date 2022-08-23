using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    [CreateAssetMenu(menuName = "Ability effects/Spawn particles", fileName = "spawnparticles.asset")]
    public class SpawnParticlesEffectSO : AbilityEffectSO
    {
        protected override string _metaHelpDescription => $"Instantiates particles on the target.";
        
        [SerializeField] private GameObject _particlePrefab;
        [SerializeField] private Vector3 _positionOffset = Vector3.zero;
        [SerializeField] private bool _spawnAtTargetBase;
        [SerializeField] private float _particleLifeTime = 3;
        [HelpBox("Pooled particles are shared between all uses of the effect.")]
        [SerializeField] private int _pooledParticles = 1;

        private ParticlePool _particlePool;
        private GameObject _particlePoolParent;

        public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker)
        {
            if (_particlePool == null)
                SetupParticlePool();
            Vector3 spawnPos;
            if (_spawnAtTargetBase)
                spawnPos = target.BasePosition + _positionOffset;
            else
                spawnPos = target.Transform.position + _positionOffset;

            _particlePool.SpawnParticleAt(spawnPos,Vector3.forward ,_particleLifeTime);
        }

        private void SetupParticlePool()
        {
            _particlePoolParent = new GameObject($"{this.name}_pool");
            _particlePool = new ParticlePool(_pooledParticles, () =>
            {
                var instance = Instantiate(_particlePrefab, _particlePoolParent.transform);
                instance.SetActive(false);
                return instance.transform;
            });
        }
    }
}