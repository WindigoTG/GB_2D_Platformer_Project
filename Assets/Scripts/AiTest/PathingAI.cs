using UnityEngine;
using Pathfinding;

public class PathingAI
{
    private readonly PathingAIView _view;
    private readonly PathingAIModel _model;
    private readonly Seeker _seeker;
    private readonly ExplosionView _explosion;

    readonly Transform _target;

    public event System.Action<int> TargetHit;

    bool _isExploding;
    bool _hasExploded;
    float _explosionTime;

    float _recalculationInterval = 1.0f;
    float _lastRecalculationTime;

    bool _isChasing;

    public PathingAI(PathingAIView view, PathingAIModel model, Transform target, GameObject explosionPrefab, SpriteAnimatorController explosionAnimatorController)
    {
        _view = view;
        _model = model;
        _seeker = _view.Gameobject.GetComponent<Seeker>();
        _target = target;
        _view.Transform.position = _model.SpawnPosition;

        var explosion = Object.Instantiate(explosionPrefab);
        _explosion = new ExplosionView(explosion, explosionAnimatorController);
    }

    public void FixedUpdate()
    {
        if ((_target.position - _view.Transform.position).sqrMagnitude <= _model.SqrAwarenessRadius)
            _isChasing = true;
        else
            _isChasing = false;

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
            if (Time.time - _lastRecalculationTime > _recalculationInterval)
            {
                RecalculatePath();
                _lastRecalculationTime = Time.time;
            }

            var newVelocity = _model.CalculateVelocity(_view.Transform.position) * Time.fixedDeltaTime;
            _view.Rigidbody2D.velocity = newVelocity;

            CheckForTarget();
        }
    }

    public void RecalculatePath()
    {
        if (_seeker.IsDone())
        {
            var target = _isChasing ? _target : _model.GetWaypoint(_view.Transform.position);
            _seeker.StartPath(_view.Rigidbody2D.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;
        _model.UpdatePath(p);
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

