using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Utils
{
    public static class Distance
    {
        /// <summary>
        /// Compares the distance between two positions and returns true if they are closer together than the specified distance
        /// </summary>
        /// <returns><c>true</c>, if closer than was ised, <c>false</c> otherwise.</returns>
        /// <param name="pointA">Point a.</param>
        /// <param name="pointB">Point b.</param>
        /// <param name="distance">Distance.</param>
        public static bool IsLessThan(Vector3 pointA, Vector3 pointB, float distance)
            => (pointA - pointB).sqrMagnitude < distance * distance;

        public static float Between(Vector3 pointA, Vector3 pointB) => (pointA - pointB).magnitude;
    }
}
