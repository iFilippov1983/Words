using System;

namespace Game
{
    public interface IAnimatableObjectParent
    {
        public event Action OnAnimationInitialize;
        public event Action OnAnimationFinish;

        public void AnimationFinishCallback();
    }
}
