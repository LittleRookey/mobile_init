namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// An actor that can be healed. Basic implementation supplied in HealthComponent in example game.
    /// </summary>
    public interface IHealable
    {
        void Heal(int healAmount);
    }
}