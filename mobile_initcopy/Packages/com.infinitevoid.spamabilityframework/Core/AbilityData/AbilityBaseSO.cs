using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.Common.Enums;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Base data for an ability
    /// </summary>
    public abstract class AbilityBaseSO : ScriptableObject
    {
        public abstract bool TargetIsInSight(AbilityInvoker invoker, IAbilityTarget potentialTarget,
            Vector3 hitPosition, Vector3 lookDirection);
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("Ability settings")] 
        [HelpBox("Automatic uses Time.deltaTime, Manual ticks when Ability.TickCooldown() is called")]
        [SerializeField] private TimeHandlingEnum _cooldownType;
        [SerializeField] private float _abilityCooldown;
        [Tooltip("The range in unit an ability can be cast. Set to 0 if infinite")]
        [SerializeField] private float _castRange;
        [SerializeField] private bool _requiresAbilityTarget = false;
        [SerializeField] private TelegraphSettings _telegraph = TelegraphSettings.None;

        [Header("Effects")] 
        [SerializeField]
        private List<EffectAndTime> _abilityEffects = new List<EffectAndTime>();
        
        [Header("Area-Of-Effect")]
        [HelpBox("If Radius is greater than 0 the effects will be applied to all targets up to Max Effect Targets in range in the given layer(s). Effects are always applied to the main target.")]
        [SerializeField] private float _effectRadius = 0;
        [SerializeField] private int _maxEffectTargets = 1;
        [SerializeField] private bool _lineOfSightCheck;
        [SerializeField] private LayerMask _losBlockingLayers;
        [SerializeField] private LayerMask _effectLayers;

        [HelpBox("Setting this makes the effects to be applied per target instead of per-effect, usable when you want a chain of effects on an AOE-effect. Timing between effects are still respected.")] 
        [SerializeField] private EffectApplicationEnum _effectApplication;
        [SerializeField] private float _perTargetEffectApplicationDelay;
        
        [Header("Animation & timings")]
        [SerializeField] private AbilityAnimationTimingsSO _animationTimings;
       
        [Header("VFX")] 
        [SerializeField] private AbilityVFXSO _abilityVfx;

        [Header("SFX")] 
        [SerializeField] private AbilitySFXSO _abilitySfx;

        
        public string Name => _name;

        public string Description => _description;

        public Sprite Icon => _icon;

        public AbilityVFXSO VFX => _abilityVfx;
        public AbilitySFXSO SFX => _abilitySfx;
        public float CastRange => _castRange;
        public float Cooldown => _abilityCooldown;
        public AbilityAnimationTimingsSO AnimationTimings => _animationTimings;
        public bool RequiresAbilityTarget => _requiresAbilityTarget;
        public List<EffectAndTime> AbilityEffects => _abilityEffects;
        public float EffectRadius => _effectRadius;
        public bool AOEAbility => 0 < _effectRadius || this is DirectionalAbilitySO;
        public LayerMask EffectLayers => _effectLayers;
        public int MaxEffectTargets => _maxEffectTargets;
        public float PerTargetEffectApplicationDelay => _perTargetEffectApplicationDelay;
        public bool ApplyPerTarget => _effectApplication == EffectApplicationEnum.PerTarget;
        public bool HasWarmup => _animationTimings?.WarmupTime > 0;
        /// <summary>
        /// The cast time of the ability, i.e. the warmup time.
        /// </summary>
        public float CastTime => _animationTimings?.WarmupTime ?? 0;
        public bool AutomaticCooldown => this._cooldownType == TimeHandlingEnum.Automatic;
        public bool Telegraphed => this._telegraph != TelegraphSettings.None;

        public bool TelegraphOnWarmup => this._telegraph == TelegraphSettings.DuringWarmup;
        public bool TelegraphOnCast => this._telegraph == TelegraphSettings.OnCast;

        public bool LineOfSightCheck => _lineOfSightCheck;

        public LayerMask LosBlockingLayers => _losBlockingLayers;

        public void CacheAnimationTriggers() => _animationTimings?.CacheTriggers();

        private enum EffectApplicationEnum
        {
            PerEffect,
            PerTarget,
        }

        private enum TelegraphSettings
        {
            None = 0,
            DuringWarmup = 1,
            OnCast = 2,
        }

    }
}