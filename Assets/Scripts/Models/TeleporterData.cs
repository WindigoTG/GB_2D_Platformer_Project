using UnityEngine;
using System;

[Serializable]
public class TeleporterData 
{
    [SerializeField] Transform _first;
    [SerializeField] Transform _second;

    public SpriteRenderer FirstSpriteRenderer => _first.GetComponent<SpriteRenderer>();
    public SpriteRenderer SecondSpriteRenderer => _second.GetComponent<SpriteRenderer>();

    public Transform GetPair(Transform teleporter)
    {
        if (_first == teleporter)
            return _second;
        if (_second == teleporter)
            return _first;
        return null;
    }
}

