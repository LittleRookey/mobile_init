using InfiniteVoid.SpamFramework.Core.ExternalSystems;

namespace InfiniteVoid.SpamFramework.Core.Common
{
    /// <summary>
    /// A targeted ability can be cast on a target <see cref="IAbilityTarget"/> or a given point if <see cref="RequiresTarget"/> is false 
    /// </summary>
    public interface ITargetedAbility : IAbility
    {
        bool RequiresTarget { get; }
        void Cast(IAbilityTarget target);
        bool CastOnSelf { get; }
    }
}