using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Track _animation;
    [SerializeField] float _animationSpeed = 10;
    Track _currentAnimation;

    [SerializeField] SpriteRenderer _spriteRenderer;
    SpriteAnimatorController _animatorController;

    bool _isActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            if (_currentAnimation != _animation)
            {
                _currentAnimation = _animation;
                _animatorController.StartAnimation(_spriteRenderer, _animation, true, _animationSpeed);
            }
        }
    }

    public void SetAnimator(SpriteAnimatorController controller)
    {
        _animatorController = controller;
        _isActive = true;
        var animation = _animation;
        _animatorController.StartAnimation(_spriteRenderer, Track.Idle, true, _animationSpeed);
        _currentAnimation = _animation;
    }
}
