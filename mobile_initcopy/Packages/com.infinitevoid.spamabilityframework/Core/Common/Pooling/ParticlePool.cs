using System;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// A specialized pool for handling particles
    /// </summary>
    public class ParticlePool : Pool<Transform>
    {
        private float[] _lifetimes;
        private int _activeParticlesCount;

        public ParticlePool(int poolCount, Func<Transform> createPooledItem)
            : base(poolCount, createPooledItem)
        {
            _lifetimes = new float[poolCount];
        }

        public bool AllParticlesInactive => _activeParticlesCount == 0; 

        public void SpawnParticleAt(Vector3 position, Vector3 vfxForward, float lifeTime = 0f)
        {
            if (0 < lifeTime)
            {
                _lifetimes[CurIndex] = lifeTime;
                _activeParticlesCount++;
            }
            base.GetNext(position, true, vfxForward);
        }

        public void TickLifetimes(float deltaTime)
        {
            if (_activeParticlesCount == 0) return;
            for (int i = 0; i < this._lifetimes.Length; i++)
            {
                _lifetimes[i] -= deltaTime;
                if (_lifetimes[i] <= 0)
                {
                    this[i].gameObject.SetActive(false);
                    _activeParticlesCount--;
                }
            }
        }

        public void Clear(Action<GameObject> destroyAction)
        {
            for (int i = 0; i < this._lifetimes.Length; i++)
            {
                if(this[i])
                    destroyAction?.Invoke(this[i].gameObject);
            }
        }
    }
}