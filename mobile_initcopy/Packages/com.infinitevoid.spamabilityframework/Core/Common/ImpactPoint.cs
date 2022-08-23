using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// A point in the world where an impact happened.
    /// </summary>
    public struct ImpactPoint
    {
        public Vector3 Position { get; }
        public Vector3 HitNormal { get; }

        public ImpactPoint(Vector3 pos)
        {
            this.Position = pos;
            this.HitNormal = Vector3.forward;
        }

        public ImpactPoint(Vector3 pos, Vector3 hitNormal)
        {
            this.Position = pos;
            this.HitNormal = hitNormal;
        }
    }
}