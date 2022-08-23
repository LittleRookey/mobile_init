namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// A debuffable actor is able to recieve debuffs.
    /// </summary>
    public interface IDebuffable
    {
        void AddDebuff(IDebuff buff);
    }
}