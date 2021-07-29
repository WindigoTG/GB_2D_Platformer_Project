using UnityEngine;

public class SimplePatrolAIModel
{
    private readonly AIConfig _config;
    private Transform _target;
    private int _currentPointIndex;

    public SimplePatrolAIModel(AIConfig config)
    {
        _config = config;
        _target = GetNextWaypoint();
    }

    public Vector2 CalculateVelocity(Vector2 fromPosition)
    {
        var sqrDistance = Vector2.SqrMagnitude((Vector2)_target.position - fromPosition);
        if (sqrDistance <= _config.MinSqrDistanceToTarget)
        {
            _target = GetNextWaypoint();
        }

        var direction = ((Vector2)_target.position - fromPosition).normalized;
        return _config.Speed * direction;
    }

    private Transform GetNextWaypoint()
    {
        _currentPointIndex = (_currentPointIndex + 1) % _config.PatrolData.Waypoints.Length;
        return _config.PatrolData.Waypoints[_currentPointIndex];
    }

    public Vector3 SpawnPosition => _config.PatrolData.SpawnPoint.position;
    public int Damage => _config.Damage;
    public float RespawnTime => _config.RespawnTime;
}

