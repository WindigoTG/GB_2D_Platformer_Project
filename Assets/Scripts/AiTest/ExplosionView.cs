using UnityEngine;

public class ExplosionView
{
    SpriteAnimatorController _animatorController;
    GameObject _gameObject;
    SpriteRenderer _spriteRenderer;

    public ExplosionView(GameObject gameObject, SpriteAnimatorController animatorController)
    {
        _gameObject = gameObject;
        Deactivate();
        _animatorController = animatorController;
        _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
    }

    public void ActivateAtPosition(Vector3 position)
    {
        _gameObject.transform.position = position;
        _gameObject.SetActive(true);
        _animatorController.StartAnimation(_spriteRenderer, Track.Idle, false, 10);
    }

    public void Deactivate()
    {
        _gameObject.SetActive(false);
    }

    public bool CheckForAnimationComplition()
    {
        return _animatorController.IsAnimationFinished(_spriteRenderer);
    }
}
