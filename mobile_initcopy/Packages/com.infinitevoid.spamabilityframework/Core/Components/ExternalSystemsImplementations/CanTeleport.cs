using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations
{
    /// <summary>
    /// Simple component that allows an object to teleport to a given position
    /// </summary>
    public class CanTeleport : MonoBehaviour, ITeleportable
    {
        public void TeleportTo(Vector3 targetPos)
        {
            transform.position = targetPos;
        }
    }
}