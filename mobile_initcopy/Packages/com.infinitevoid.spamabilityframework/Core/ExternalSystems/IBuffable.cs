namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// A buffable actor is able to recieve buffs.
    /// </summary>
    public interface IBuffable
    {
        void AddBuff(IBuff buff);
    }
}