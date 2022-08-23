using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// Base for all ability effects, allowing them to be handled generically
    /// </summary>
    [Serializable]
    public abstract class AbilityEffectSO : ScriptableObject
    {
        /// <summary>
        /// The help description for the object. This is shown in the inspector
        /// </summary>
        protected abstract string _metaHelpDescription { get; }

        [SerializeField] private string _name;
        [Multiline] [SerializeField] private string _description;

        [HelpBox(
            "If this is turned off then the effect won't be included in the list of effects an ability publicly has. Useful for removing an effect from the abilities public interface so the effect can be listen in the GUI.")]
        [SerializeField]
        private bool _includedInAbilityEffects = true;
        
        /// <summary>
        /// Gets the help description. Used in AbilityEffectSOEditor.
        /// </summary>
        public string HelpDescription => _metaHelpDescription;

        public string Description => _description;
        public string Name => _name;
        public bool IncludedInAbilityEffects => _includedInAbilityEffects;


        public abstract void ApplyTo(IAbilityTarget target, Vector3 abilityPos, AbilityBaseSO ability,
            AbilityInvoker invoker);

#if UNITY_EDITOR
        /// <summary>
        /// Logs a warning to the console if currently running in the editor
        /// </summary>
        /// <param name="warning"></param>
        protected void LogWarning(string warning)
        {
            Debug.LogWarning(warning);
        }
#endif
    }
}