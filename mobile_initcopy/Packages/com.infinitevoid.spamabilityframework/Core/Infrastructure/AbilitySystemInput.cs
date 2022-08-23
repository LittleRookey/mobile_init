using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure
{
    /// <summary>
    /// Handles player input.
    /// This abstraction will support both legacy and new input system in a future update.
    /// </summary>
    public class AbilitySystemInput : MonoBehaviour
    {
        public static Vector3 MousePosition => Input.mousePosition;

        public float GetHorizontalRaw() => Input.GetAxisRaw("Horizontal");

        public float GetVerticalRaw() => Input.GetAxisRaw("Vertical");

        public static bool GetLeftMouseButton() => Input.GetMouseButton(0);

        public static bool GetRightMouseButtonDown() => Input.GetMouseButtonDown(1);

        public static bool GetRightMouseButtonUp() => Input.GetMouseButtonUp(1);

        public static bool ShouldCastQAbility() => Input.GetKeyDown(KeyCode.Q);
    }
}