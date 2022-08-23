using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Tries to get the component T in parents or children of the given transform 
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentInHierarchy<T>(this Transform t) where T : Component
        {
            T component = t.GetComponentInParent<T>();
            return component ? component : t.GetComponentInChildren<T>();
        }
    }
}