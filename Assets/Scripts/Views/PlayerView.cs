using UnityEngine;

public class PlayerView
{
    protected SpriteAnimatorController _animatorController;
    protected SpriteRenderer _spriteRenderer;
    protected float _animationSpeed = 7.5f;
    protected Transform _transform;

    protected bool _canCrouch;
    protected bool _canTeleport;
    protected bool _canWallCling;

    public PlayerView(SpriteAnimatorController animationController, Transform viewTransform)
    {
        _transform = viewTransform;
        _animatorController = animationController;
        _spriteRenderer = _transform.GetComponent<SpriteRenderer>();

        _canCrouch = _animatorController.HasAnimation(Track.Crouch);
        _canTeleport = _animatorController.HasAnimation(Track.TeleportIn);
        _canWallCling = _animatorController.HasAnimation(Track.WallCling);
    }

    public Transform Transform => _transform;

    public bool CanCrouch => _canCrouch;
    public bool CanTeleport => _canTeleport;
    public bool CanWallCling => _canWallCling;
    public bool IsAnimationDone => _animatorController.IsAnimationFinished(_spriteRenderer);

    public virtual void StartIdleAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Idle, true, _animationSpeed);
    }

    public virtual void StartRunAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Run, true, _animationSpeed);
    }

    public virtual void StartJumpAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Jump, false, _animationSpeed);
    }

    public virtual void StartFallAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Fall, false, _animationSpeed);
    }

    public virtual void StartHenshinPhaseOneAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.HenshinPhaseOne, false, _animationSpeed);
    }

    public virtual void StartHenshinPhaseTwoAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.HenshinPhaseTwo, false, _animationSpeed);
    }

    public virtual void StartCrouchAnimation()
    {
        if (_canCrouch)
            _animatorController.StartAnimation(_spriteRenderer, Track.Crouch, true, _animationSpeed);
    }

    public virtual void StartCrawlAnimation()
    {
        if (_canCrouch)
            _animatorController.StartAnimation(_spriteRenderer, Track.Crawl, true, _animationSpeed);
    }

    public virtual void StartTeleportInAnimation()
    {
        if (_canTeleport)
            _animatorController.StartAnimation(_spriteRenderer, Track.TeleportIn, false, _animationSpeed);
    }

    public virtual void StartTeleportOutAnimation()
    {
        if (_canTeleport)
            _animatorController.StartAnimation(_spriteRenderer, Track.TeleportOut, false, _animationSpeed);
    }

    public virtual void StartWallClingAnimation()
    {
        if (_canWallCling)
            _animatorController.StartAnimation(_spriteRenderer, Track.WallCling, false, _animationSpeed);
    }

    public virtual void StartHurtAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.TakeHit, false, _animationSpeed);
    }

    public virtual void StartHurtCrouchAnimation()
    {
        if (_canCrouch)
            _animatorController.StartAnimation(_spriteRenderer, Track.TakeHitCrouch, false, _animationSpeed);
    }

    public void Activate()
    {
        _transform.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _animatorController.StopAnimation(_spriteRenderer);
        _transform.gameObject.SetActive(false);
    }
}
