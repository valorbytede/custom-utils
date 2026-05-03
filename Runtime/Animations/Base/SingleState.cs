using JetBrains.Annotations;

namespace CustomUtils.Runtime.Animations.Base
{
    /// <summary>
    /// A single-value enum used as a state type for animations that don't require multiple states.
    /// </summary>
    [PublicAPI]
    public enum SingleState
    {
        Default = 0
    }
}