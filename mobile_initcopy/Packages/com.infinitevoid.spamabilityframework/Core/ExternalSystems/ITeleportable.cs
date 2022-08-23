using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// An actor which can teleport to a given position
    /// </summary>
    public interface ITeleportable
    {
        void TeleportTo(Vector3 targetPos);
    }
}