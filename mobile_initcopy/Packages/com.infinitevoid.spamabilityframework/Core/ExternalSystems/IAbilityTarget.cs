using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// Implement this to allow a gameobject to be targeted by abilities
    /// </summary>
    public interface IAbilityTarget
    {
        Transform Transform { get; }
        /// <summary>
        /// An audiosource on the target. Can be null if no sounds are to be played from the target
        /// </summary>
        AudioSource AudioSource { get; }
        /// <summary>
        /// The position of the target's feet or base, i.e. where it's grounded
        /// </summary>
        Vector3 BasePosition { get; }
    }
}