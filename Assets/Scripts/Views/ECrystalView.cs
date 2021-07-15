using UnityEngine;

public class ECrystalView 
{
    SpriteAnimatorController _animatorController;
    SpriteRenderer _spriteRenderer;
    Transform _transform;
    float _animationSpeed = 10f;

    public ECrystalView(Transform transform, SpriteAnimatorController animationController)
    {
        _transform = transform;
        _animatorController = animationController;
        _spriteRenderer = _transform.GetComponent<SpriteRenderer>();
        
    }

    public void StartAnimation()
    {
        _animatorController.StartAnimation(_spriteRenderer, Track.Idle, true, _animationSpeed);
    }

    public void Deactivate()
    {
        _animatorController.StopAnimation(_spriteRenderer);
        _transform.gameObject.SetActive(false);
    }
}
