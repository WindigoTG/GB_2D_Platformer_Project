using UnityEngine;
using System;

[Serializable]
public class PairedTeleporterData 
{
    [SerializeField] Transform _first;
    [SerializeField] Transform _second;

    bool _isUnlocked;

    public bool IsUnlocked => _isUnlocked;

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

    public void SetUnlocked()
    {
        _isUnlocked = true;
    }
}

