using UnityEngine;

[System.Serializable]
public class PatrolData
{
    [SerializeField] Transform _spawnPoint;

    [SerializeField ]Transform[] _waypoints;

    public Transform SpawnPoint => _spawnPoint;
    public Transform[] Waypoints => _waypoints;
}
