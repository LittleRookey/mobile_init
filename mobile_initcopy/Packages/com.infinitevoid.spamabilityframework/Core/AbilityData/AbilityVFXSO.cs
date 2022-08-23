using InfiniteVoid.SpamFramework.Core.Common.Enums;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Data for an ability's VFX (Visual Effects)
    /// </summary>
    [CreateAssetMenu(menuName = "Ability VFX", fileName = "abilityVfx.asset")]
    public class AbilityVFXSO : ScriptableObject
    {
        [Header("Warmup")]
        [SerializeField] private GameObject _warmupVfx;
        [SerializeField] private float _warmupLifeTime = 1;
        [SerializeField] private Vector3 _warmupCustomScale = Vector3.one;
        
        [Header("On Hit")]
        [SerializeField] private GameObject _onHitVfx;
        [SerializeField] private OnHitParticleCountEnum _numOfOnHitInstances;
        [SerializeField] private int _onHitInstances = 0;
        
        [SerializeField] private OnHitSpawnPointEnum _onHitSpawnPoint;
        [SerializeField] private Vector3 _onHitPositionOffset = Vector3.zero;
        [SerializeField] private OnHitRotationEnum _onHitRotation = OnHitRotationEnum.Initial;
        [SerializeField] private bool _spawnOnHitAtTargetBase;
        
        [SerializeField] private TimeHandlingEnum _lifeTimeHandling;
        [SerializeField] private float _onHitLifeTime = 3;
        
        public GameObject WarmupVfx => _warmupVfx;
        public float WarmupLifeTime => _warmupLifeTime;
        public GameObject OnHitVfx => _onHitVfx;
        public Vector3 OnHitPositionOffset => _onHitPositionOffset;
        public float OnHitLifeTime => _onHitLifeTime;
        public bool SpawnOnHitAtTargetBase => _spawnOnHitAtTargetBase;
        public Vector3 WarmupCustomScale => _warmupCustomScale;
        public bool AutomaticOnHitLifetime => _lifeTimeHandling == TimeHandlingEnum.Automatic;
        public int OnHitInstances => _onHitInstances;
        public bool OnHitSpawnOnHitAtImpactPoint => _onHitSpawnPoint == OnHitSpawnPointEnum.ImpactPoint;
        public bool SameOnHitCountAsMaxTargets => _numOfOnHitInstances == OnHitParticleCountEnum.SameAsMaxTargets;
        public bool RotateToNormal => _onHitRotation == OnHitRotationEnum.TargetNormal;


        private enum OnHitSpawnPointEnum
        {
            Targets,
            ImpactPoint
        }

        private enum OnHitParticleCountEnum
        {
            SameAsMaxTargets,
            Custom
        }
        private enum OnHitRotationEnum
        {
            Initial,
            TargetNormal
        }
    }
}