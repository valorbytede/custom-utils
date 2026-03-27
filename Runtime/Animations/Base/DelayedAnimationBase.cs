using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Base
{
    public abstract class DelayedAnimationBase<TTarget, TContent, TState> : IAnimation<TState>
        where TState : unmanaged, Enum
    {
        [SerializeField] protected TTarget target;
        [SerializeField] private float _delay;
        [SerializeField] private EnumArray<TState, TContent> _states;

        private Tween _currentAnimation;

        protected TContent targetSource;

        public Tween PlayAnimation(TState state, bool isInstant)
        {
            targetSource = _states[state];

            if (isInstant)
            {
                UpdateState();
                return default;
            }

            if (_currentAnimation.isAlive)
                _currentAnimation.Stop();

            return _currentAnimation = Tween.Delay(this, _delay, static self => self.UpdateState());
        }

        protected abstract void UpdateState();

        public void CancelAnimation()
        {
            _currentAnimation.Stop();
        }
    }
}