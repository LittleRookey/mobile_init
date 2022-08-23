namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// An actor that can be damaged. Basic implementation supplied in HealthComponent in example game
    /// </summary>
    public interface IDamageable
    {
        void Damage(int damageAmount);
    }
}