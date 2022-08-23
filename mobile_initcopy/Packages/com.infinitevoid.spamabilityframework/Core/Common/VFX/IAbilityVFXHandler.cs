using System;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public interface IAbilityVFXHandler
    {
        void ExecuteWarmupVFX();
        /// <summary>
        /// Ticks the OnHitVFX lifetime. Does nothing if the OnHitVfx has automatic lifetime management.
        /// </summary>
        /// <param name="tickAmount"></param>
        void TickOnHitVFX(float tickAmount);
        void Update(float deltaTime);
        bool VfxApplied { get;}
        void ExecuteOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets);
        void Destroy(Action<GameObject> destroyAction);
        void StopWarmupVfx();
    }
}