namespace InfiniteVoid.SpamFramework.Core.Common.Enums
{
    /// <summary>
    /// Time handling enum, which can be either manual or automatic.
    /// This is used to check if the system should update itself or if an external system will handle updating
    /// the given behaviour like cooldown and lifetime.
    /// </summary>
    public enum TimeHandlingEnum
    {
        Automatic = 0,
        Manual = 1,
    }
}