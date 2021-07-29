using System;
using UnityEngine;

[Serializable]
public struct AIConfig
{
    public float Speed;
    public float MinSqrDistanceToTarget;
    [HideInInspector] public PatrolData PatrolData;
    public int Damage;
    public float RespawnTime;
    public float SqrAwarenessRadius;
}

