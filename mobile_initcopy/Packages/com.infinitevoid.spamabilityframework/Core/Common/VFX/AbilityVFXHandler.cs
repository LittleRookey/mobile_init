using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public class AbilityVFXHandler : IAbilityVFXHandler
    {
        private readonly AbilityVFXSO _abilityVfx;
        private GameObject _warmupVfx;
        private ParticlePool _onHitVfxPool;
        private float _warmupLifetime;


        public bool VfxApplied => (!_warmupVfx || !_warmupVfx.activeSelf) &&
                                  (_onHitVfxPool == null || _onHitVfxPool.AllParticlesInactive);

        public AbilityVFXHandler(AbilityBaseSO ability,
            Func<GameObject> getWarmupVfxInstance,
            Func<Transform> getOnHitVfxInstance)
        {
            if (!ability.VFX) return;
            _abilityVfx = ability.VFX;

            CacheVFX(getWarmupVfxInstance, getOnHitVfxInstance);
        }

        public void ExecuteWarmupVFX()
        {
            if (_warmupVfx == null) return;

            _warmupVfx.SetActive(true);
            _warmupLifetime = _abilityVfx.WarmupLifeTime;
        }

        public void ExecuteOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets)
        {
            if (!_abilityVfx.OnHitVfx) return;
            var lifeTime = _abilityVfx.AutomaticOnHitLifetime ? 0 : _abilityVfx.OnHitLifeTime;
            var vfxForward = _abilityVfx.RotateToNormal ? impactPoint.HitNormal : _abilityVfx.OnHitVfx.transform.forward;
            if (_abilityVfx.OnHitSpawnOnHitAtImpactPoint)
            {
                _onHitVfxPool.SpawnParticleAt(impactPoint.Position + _abilityVfx.OnHitPositionOffset, vfxForward);
                return;
            }

            for (var i = 0; i < hitTargets.Length; i++)
            {
                if (hitTargets[i] == null) return;

                if (_abilityVfx.SpawnOnHitAtTargetBase)
                    _onHitVfxPool.SpawnParticleAt(hitTargets[i].BasePosition + _abilityVfx.OnHitPositionOffset, vfxForward,
                        lifeTime);
                else
                    _onHitVfxPool.SpawnParticleAt(hitTargets[i].Transform.position + _abilityVfx.OnHitPositionOffset, vfxForward,
                        lifeTime);
            }
        }

        public void Destroy(Action<GameObject> destroyAction)
        {
            _onHitVfxPool?.Clear(destroyAction);
            _onHitVfxPool = null;
            if (_warmupVfx)
                destroyAction(_warmupVfx);
        }

        public void StopWarmupVfx()
        {
            if(_warmupVfx)
                _warmupVfx.SetActive(false);
        }

        /// <summary>
        /// Ticks the OnHitVFX lifetime. Does nothing if the OnHitVfx has automatic lifetime management.
        /// </summary>
        /// <param name="tickAmount"></param>
        public void TickOnHitVFX(float tickAmount)
        {
            if (!_abilityVfx.OnHitVfx || _abilityVfx.AutomaticOnHitLifetime) return;
            _onHitVfxPool?.TickLifetimes(tickAmount);
        }

        public void Update(float deltaTime)
        {
            _warmupLifetime -= deltaTime;
            if (_warmupVfx && _warmupLifetime <= 0)
                _warmupVfx.SetActive(false);

            if (!_abilityVfx.AutomaticOnHitLifetime || !_abilityVfx.OnHitVfx) return;
            _onHitVfxPool?.TickLifetimes(deltaTime);
        }


        private void CacheVFX(Func<GameObject> getWarmupVfxInstance, Func<Transform> getOnHitVfxInstance)
        {
            if (_abilityVfx.WarmupVfx && getWarmupVfxInstance != null)
            {
                _warmupVfx = getWarmupVfxInstance.Invoke();
                _warmupVfx.transform.localScale = _abilityVfx.WarmupCustomScale;
                _warmupVfx.SetActive(false);
            }

            if (_abilityVfx.OnHitVfx && getOnHitVfxInstance != null)
            {
                _onHitVfxPool = new ParticlePool(_abilityVfx.OnHitInstances, getOnHitVfxInstance);
            }
        }
    }
}