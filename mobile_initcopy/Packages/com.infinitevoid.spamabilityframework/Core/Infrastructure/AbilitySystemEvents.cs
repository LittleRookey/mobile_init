using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure
{
    /// <summary>
    /// Static mediator or event-hub for event-handling.
    /// This is used instead of events-classes that derive from a common interface as there's no need to use
    /// any kind of reflective invocation (which results in garbage generation) and also no instantiating and caching of event-classes.
    /// It's up the utilizing classes to make sure they unsubscribe properly, or some sort of game-manager
    /// to call <see cref="Reset"/> on appropriate times to not run into nullreference-exceptions.
    /// One common way to handle this is in OnEnabled/OnDisabled or Awake/OnDestroy.
    ///
    /// This class introduces a static dependency which makes the classes harder to test, but most of the
    /// core logic should be in non-monobehaviours anyway for easy testing.
    /// The upside to this is that systems like UI can be decoupled from the rest of the game, and
    /// events can be traced in code with "find references" which is easier to track than unity-events.
    /// </summary>
    public static class AbilitySystemEvents
    {
        public static void Reset()
        {
            AbilityCooldownStart = null;
            AbilityCooldownTick = null;
            AbilityIsOnCooldown = null;
            AbilityUsed = null;
            AlreadyCasting = null;
            PlayerSelectedAbility = null;
            TargetNotInCastRange = null;
        }

        /// <summary>
        /// Rasied when an ability goes on cooldown.
        /// </summary>
        public static event Action<AbilityBaseSO> AbilityCooldownStart;
        public static void RaiseAbilityCooldownStart(AbilityBaseSO ability) => AbilityCooldownStart?.Invoke(ability);
        
        /// <summary>
        /// Raised then an ability's cooldown is manually ticked
        /// </summary>
        public static event Action<AbilityBaseSO, float> AbilityCooldownTick;
        public static void RaiseAbilityCooldownTick(AbilityBaseSO ability, float tickTime) =>
            AbilityCooldownTick?.Invoke(ability, tickTime);

        /// <summary>
        /// Raised when trying to use an ability that is on cooldown
        /// </summary>
        public static event Action<AbilityBaseSO> AbilityIsOnCooldown;
        public static void RaiseAbilityIsOnCooldown(AbilityBaseSO ability) => AbilityIsOnCooldown?.Invoke(ability);

        /// <summary>
        /// Raised when an ability is used (cast), This is Raised at the same time as the warmup starts.
        /// </summary>
        public static event Action<AbilityBaseSO,Vector3> AbilityUsed;
        public static void RaiseAbilityUsed(AbilityBaseSO ability, Vector3 targetPosition) => AbilityUsed?.Invoke(ability, targetPosition);

        /// <summary>
        /// Raised when trying to use an ability but the invoker is already casting an ability
        /// </summary>
        public static event Action AlreadyCasting;
        public static void RaiseAlreadyCasting() => AlreadyCasting?.Invoke();

        /// <summary>
        /// Raised when a player selects and ability to use.
        /// </summary>
        public static event Action<AbilityBaseSO> PlayerSelectedAbility;
        public static void RaisePlayerSelectedAbility(AbilityBaseSO ability) => PlayerSelectedAbility?.Invoke(ability);

        public static event Action PlayerDeselectedAbility;
        public static void RaisePlayerDeselectedAbility() => PlayerDeselectedAbility?.Invoke();
        
        /// <summary>
        /// Raised by an <see cref="AbilityInvoker"/> when attempting to cast an ability on a target which is out of range
        /// </summary>
        public static event Action TargetNotInCastRange;
        public static void RaiseTargetNotInCastRange() => TargetNotInCastRange?.Invoke();
        
        
    }
}