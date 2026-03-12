using System;
using CustomUtils.Runtime.Animations.Base;
using CustomUtils.Runtime.Animations.Settings;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Anchor
{
    /// <summary>
    /// Animates the anchorMax of a RectTransform based on state.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [PublicAPI]
    [Serializable]
    public sealed class AnchorMaxAnimation<TState> : AnimationBase<TState, Vector2, Vector2AnimationSettings>
        where TState : unmanaged, Enum
    {
        [SerializeField] private RectTransform _target;

        protected override void SetValueInstant(Vector2 value)
        {
            _target.anchorMax = value;
        }

        protected override Tween CreateTween(Vector2AnimationSettings animationSettings)
            => Tween.UIAnchorMax(_target, animationSettings.TweenSettings);
    }
}