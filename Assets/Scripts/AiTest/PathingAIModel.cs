using UnityEngine;
using Pathfinding;

public class PathingAIModel
{
    private readonly AIConfig _config;
    private Path _path;
    private int _currentPointIndex;

    private Transform _wayPoint;
    private int _currentWayPointIndex;

    public PathingAIModel(AIConfig config)
    {
        _config = config;
        _wayPoint = GetNextWaypoint();
    }

    public void UpdatePath(Path p)
    {
        _path = p;
        _currentPointIndex = 0;
    }

    public Vector2 CalculateVelocity(Vector2 fromPosition)
    {
        if (_path == null) return Vector2.zero;
        if (_currentPointIndex >= _path.vectorPath.Count) return Vector2.zero;

        var direction = ((Vector2)_path.vectorPath[_currentPointIndex] - fromPosition).normalized;
        var result = _config.Speed * direction;
        var sqrDistance = Vector2.SqrMagnitude((Vector2)_path.vectorPath[_currentPointIndex] - fromPosition);
        if (sqrDistance <= _config.MinSqrDistanceToTarget)
        {
            _currentPointIndex++;
        }
        return result;
    }

    public Transform GetWaypoint(Vector2 fromPosition)
    {
        var sqrDistance = Vector2.SqrMagnitude((Vector2)_wayPoint.position - fromPosition);
        if (sqrDistance <= _config.MinSqrDistanceToTarget)
        {
            _wayPoint = GetNextWaypoint();
        }

        return _wayPoint;
    }

    private Transform GetNextWaypoint()
    {
        _currentWayPointIndex = (_currentWayPointIndex + 1) % _config.PatrolData.Waypoints.Length;
        return _config.PatrolData.Waypoints[_currentWayPointIndex];
    }

    public Vector3 SpawnPosition => _config.PatrolData.SpawnPoint.position;
    public int Damage => _config.Damage;
    public float RespawnTime => _config.RespawnTime;
    public float SqrAwarenessRadius => _config.SqrAwarenessRadius;
}

