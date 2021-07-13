using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderEnemyController : IUpdateable
{
    private Transform _enemy;
    private SpriteAnimatorController _animatorController;
    private SpriteRenderer _spriteRenderer;
    float _animationSpeed = 10.0f;

    private float _speed = 2.5f;
    private State _currentSate;
    private float _minStateDuration = 2.0f;
    private float _maxStateDuration = 5.0f;
    private float _currentDuration;

    public PlaceholderEnemyController(Transform enemy, SpriteAnimatorController animatorController)
    {
        _enemy = enemy;
        _animatorController = animatorController;
        _spriteRenderer = _enemy.GetComponent<SpriteRenderer>();

        _currentSate = State.Hidden;
        RandomDuration();
    }

    public void Update()
    {
        if (_currentSate == State.Run)
            Move();

        if (_currentDuration > 0)
            _currentDuration -= Time.deltaTime;
        else
            NewState();
    }

    void Move()
    {
        _enemy.Translate(Vector2.right * _speed * Time.deltaTime);

        if (_enemy.position.x > 7)
        {
            _enemy.position = new Vector3(7, _enemy.position.y, _enemy.position.z);
            _currentDuration = 0;
        }

        if (_enemy.position.x < -7)
        {
            _enemy.position = new Vector3(-7, _enemy.position.y, _enemy.position.z);
            _currentDuration = 0;
        }

    }

    void Hide()
    {
        _currentSate = State.Hidden;
        _animatorController.StartAnimation(_spriteRenderer, Track.Hide, false, _animationSpeed);
    }

    void PopUp()
    {
        _currentSate = State.Idle;
        _animatorController.StartAnimation(_spriteRenderer, Track.PopUp, false, _animationSpeed);
    }

    void StartMoving()
    {
        if (_enemy.position.x < 7 && _enemy.position.x > -7)
        {
            int random = Random.Range(0, 2);
            if (random == 1)
                _enemy.Rotate(Vector2.up, 180);
        }
        else
            _enemy.Rotate(Vector2.up, 180);

        _currentSate = State.Run;
        _animatorController.StartAnimation(_spriteRenderer, Track.Run, true, _animationSpeed);
    }

    void RandomDuration()
    {
        _currentDuration = Random.Range(_minStateDuration, _maxStateDuration);
    }

    void NewState()
    {
        switch (_currentSate)
        {
            case State.Idle:
                {
                    int random = Random.Range(0, 2);

                    switch (random)
                    {
                        case 0:
                        default:
                            {
                                Hide();
                                break;
                            }
                        case 1:
                            {
                                StartMoving();
                                break;
                            }
                    }
                    break;
                }

            case State.Hidden:
                {
                    PopUp();
                    break;
                }

            case State.Run:
                {
                    Hide();
                    break;
                }
        }

        RandomDuration();
    }
}

public enum State
{
    Idle = 0,
    Run = 1,
    Hidden = 2,
}