namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// A shieldable actor can either recieve a shield for a fixed amount or can have it's shield repaired by the given amount.
    /// Basic implementation supplied in HealthComponent in example game.
    /// </summary>
    public interface IShieldable
    {
        void Shield(int shieldAmount);
    }
}