using UnityEngine;

public class UntransformedPlayerView : PlayerView
{
    public UntransformedPlayerView(SpriteAnimatorController animationController, Transform viewTransform) : base(animationController, viewTransform) { }

    public override void StartJumpAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Jump, true, _animationSpeed);
    }

    public override void StartFallAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Fall, true, _animationSpeed);
    }
}
