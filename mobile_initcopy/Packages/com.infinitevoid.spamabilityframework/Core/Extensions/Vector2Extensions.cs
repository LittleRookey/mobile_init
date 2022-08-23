using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Extensions
{
    public static class Vector2Extensions
    {

        /// <summary>
        /// Returns a new Vector2 with overriden values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 With(this Vector2 source, float? x = null, float? y = null)
            => new Vector3(x ?? source.x, y ?? source.y);

    }
}