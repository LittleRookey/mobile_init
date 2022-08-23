using System;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.VFX
{
    public class NullAbilityVFXHandler : IAbilityVFXHandler
    {
        public static NullAbilityVFXHandler Instance => _instance ??= new NullAbilityVFXHandler();
        private static NullAbilityVFXHandler _instance;

        private NullAbilityVFXHandler()
        {
            _instance = this;
        }

        public void ExecuteWarmupVFX() { }
        public void TickOnHitVFX(float tickAmount) { }
        public void Update(float deltaTime) { }
        public bool VfxApplied => true;
        public void ExecuteOnHitVFX(ImpactPoint impactPoint, IAbilityTarget[] hitTargets) {}
        public void Destroy(Action<GameObject> destroyAction) {}
        public void StopWarmupVfx() { }
    }
}