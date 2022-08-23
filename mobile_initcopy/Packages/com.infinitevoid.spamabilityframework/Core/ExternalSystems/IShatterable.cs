using System;

namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    public interface IShatterable
    {
        void DestroyMesh(int cutCascades, float explodeForce, int partDespawnSecondsMin, int partDespawnSecondsMax);
        event Action WasShattered;
    }
}