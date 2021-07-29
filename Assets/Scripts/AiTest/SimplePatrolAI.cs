using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class SimplePatrolAI
{
    private readonly SimplePatrolAIView _view;
    private readonly SimplePatrolAIModel _model;
    private readonly ExplosionView _explosion;

    private Transform _target;
    public event Action<int> TargetHit;

    bool _isExploding;
    bool _hasExploded;
    float _explosionTime;

    public SimplePatrolAI(SimplePatrolAIView view, SimplePatrolAIModel model, Transform target, GameObject explosionPrefab, SpriteAnimatorController explosionAnimatorController)
    {
        _view = view;
        _model = model;
        _target = target;
        _view.Transform.position = _model.SpawnPosition;

        var explosion = Object.Instantiate(explosionPrefab);
        _explosion = new ExplosionView(explosion, explosionAnimatorController);
    }

    public void FixedUpdate()
    {
        if (_isExploding && _explosion.CheckForAnimationComplition())
        {
            _explosion.Deactivate();
            _isExploding = false;
            _hasExploded = true;
            _explosionTime = Time.time;
        }

        if (_hasExploded && 
            (Time.time - _explosionTime > _model.RespawnTime))
        {
            _hasExploded = false;
            _view.Transform.position = _model.SpawnPosition;
            _view.Gameobject.SetActive(true);
        }

        if (!_isExploding && !_hasExploded)
        {
            var newVelocity = _model.CalculateVelocity(_view.Transform.position) * Time.fixedDeltaTime;
            _view.Rigidbody2D.velocity = newVelocity;

            CheckForTarget();
        }
    }

    private void CheckForTarget()
    {
        if (_view.CheckForTarget(_target))
        {
            _explosion.ActivateAtPosition(_view.Transform.position);
            _isExploding = true;
            TargetHit?.Invoke(_model.Damage);

            _view.Gameobject.SetActive(false);
            _view.Rigidbody2D.velocity = Vector2.zero;
        }
    }
}

