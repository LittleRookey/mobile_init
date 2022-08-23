using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Checks if the current vector3 is in the given range to the given other vector.
        /// This uses a quicker distance comparision compared to Vector3.<seealso cref="Vector3.Distance"/>
        /// </summary>
        /// <param name="this"></param>
        /// <param name="other"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsWithinRangeTo(this Vector3 @this, Vector3 other, float range)
        {
            Vector3 offset = other - @this;
            float sqrLen = offset.sqrMagnitude;
            return sqrLen <= range * range;
        }

        /// <summary>
        /// Returns a new Vector3 with overriden values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 With(this Vector3 source, float? x = null, float? y = null, float? z = null)
            => new Vector3(x ?? source.x, y ?? source.y, z ?? source.z);

        /// <summary>
        /// Returns a new Vector3 with the given values added to the previous values.
        /// Ex: Vector3(1,1,0).Add(y: 1) = new Vector3(1,2,0);
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Add(this Vector3 source, float? x = null, float? y = null, float? z = null)
            => new Vector3(
                x != null ? source.x + x.Value : source.x,
                y != null ? source.y + y.Value : source.y,
                z != null ? source.z + z.Value : source.z);

        /// <summary>
        /// Returns a new Vector3 with the given values subtracted from the previous values.
        /// Ex: Vector3(1,1,0).Subtract(y: 1) = new Vector3(1,0,0);
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Subtract(this Vector3 source, float? x = null, float? y = null, float? z = null)
            => new Vector3(
                x != null ? source.x - x.Value : source.x,
                y != null ? source.y - y.Value : source.y,
                z != null ? source.z - z.Value : source.z);
    }
}