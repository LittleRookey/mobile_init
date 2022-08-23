using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// A map of an ability's timings and animation triggers. This should be in sync with animation lengths
    /// to get a proper timing of the different stages of casting an ability.
    /// </summary>
    [CreateAssetMenu(menuName= "Ability Animation & timings", fileName = "abilityanims.asset")]
    public class AbilityAnimationTimingsSO : ScriptableObject
    {
        // Since its faster to set trigger by hash we cache the hashes.
        private int _castTriggerHash;
        private int _warmupTriggerHash;
        private int _cooldownTriggerHash;
        private int _castBoolHash;
        
        [Header("Timings")]
        [SerializeField] private float _animationWarmupTime;
        [SerializeField] private float _animationCastTime;
        [SerializeField] private float _animationCooldownTime;
        
        [Header("Animator parameters")]
        [SerializeField] private string _warmupTriggerName;
        [SerializeField] private string _castTriggerName;
        [SerializeField] private string _castBoolName;
        [SerializeField] private string _cooldownTriggerName;
        
        [Header("Animations")] 
        [SerializeField] private AnimationClip _warmupAnimation;
        [SerializeField] private AnimationClip _castAnimation;
        [SerializeField] private AnimationClip _cooldownAnimation;


        public float WarmupTime => _animationWarmupTime;
        public float CastTime => _animationCastTime;
        public float CooldownTime => _animationCooldownTime;
        public float TotalAnimationTime => _animationWarmupTime + _animationCastTime + _animationCooldownTime;
        public bool ContinuousCast => _castBoolHash != 0;

        public void CacheTriggers()
        {
            _castTriggerHash = Animator.StringToHash(_castTriggerName);
            _castBoolHash = Animator.StringToHash(_castBoolName);
            _warmupTriggerHash = Animator.StringToHash(_warmupTriggerName);
            _cooldownTriggerHash = Animator.StringToHash(_cooldownTriggerName);
        }

        public void PlayCastAnimation(Animator animator)
        {
            if(_castTriggerHash != 0)
                animator.SetTrigger(_castTriggerHash);
            if(_castBoolHash != 0)
                animator.SetBool(_castBoolHash, true);
        }

        public void PlayWarmupAnimation(Animator animator)
        {
            if(_warmupTriggerHash != 0)
                animator.SetTrigger(_warmupTriggerHash);
        }
        
        public void PlayCooldownAnimation(Animator animator)
        {
            if(_cooldownTriggerHash != 0)
                animator.SetTrigger(_cooldownTriggerHash);
        }

        /// <summary>
        /// Sets timings from the given animation clips. Only used in the editor 
        /// </summary>
        [ContextMenu("Set timings from animations")]
        public void SetTimings()
        {
            if(_warmupAnimation)
                _animationWarmupTime = _warmupAnimation.length;
            if(_castAnimation)
                _animationCastTime = _castAnimation.length;
            if(_cooldownAnimation)
                _animationCooldownTime = _cooldownAnimation.length;
        }

        
        public void StopAllAnimations(Animator animator)
        {
            if(_castBoolHash != 0)
               animator.SetBool(_castBoolHash, false);
        }
    }
}