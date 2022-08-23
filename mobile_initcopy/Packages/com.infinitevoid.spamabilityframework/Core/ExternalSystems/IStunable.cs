namespace InfiniteVoid.SpamFramework.Core.ExternalSystems
{
    /// <summary>
    /// A stunnable character can be stunned, i.e have it's movement impaired in some way.
    /// </summary>
    public interface IStunable
    {
        void Stun(float stuntime);
    }
}