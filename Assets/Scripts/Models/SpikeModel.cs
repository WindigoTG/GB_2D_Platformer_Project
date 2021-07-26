using UnityEngine;
using System;

public class SpikeModel
{
    Collider2D _collider;
    Collider2D[] _contacts = new Collider2D[8];

    Transform _player;
    int _spikeDamage = 1;
    public event Action<int> TargetHit;

    public SpikeModel (GameObject spike, Transform player)
    {
        _collider = spike.GetComponent<Collider2D>();
        _player = player;
    }

    public void Update()
    {
        System.Array.Clear(_contacts, 0, _contacts.Length);
        _collider.GetContacts(_contacts);
        for (int i = 0; i < _contacts.Length; i++)
            if (_contacts[i] != null && _contacts[i].transform == _player)
            {
                TargetHit?.Invoke(_spikeDamage);
                break;
            }
    }
}
