using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// Base interface for all abilities
    /// </summary>
    public interface IAbility
    {
        bool IsOnCooldown { get; }
        float CastTime { get; }
        void Cast(Vector3 pos);
    }
}