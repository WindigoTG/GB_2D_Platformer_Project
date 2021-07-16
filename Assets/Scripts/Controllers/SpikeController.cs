using UnityEngine;
using System;

public class SpikeController : IUpdateable
{
    Transform _player;
    SpikeModel[] _spikes;
    int _spikeDamage = 1;

    public event Action<int> Contact;

    public SpikeController(Transform player)
    {
        _player = player;

        var spikes = GameObject.FindGameObjectsWithTag("Spike");
        _spikes = new SpikeModel[spikes.Length];
        for (int i = 0; i < _spikes.Length; i++)
            _spikes[i] = new SpikeModel(spikes[i]);

    }

    public void Update()
    {
        if (Time.frameCount % 2 == 0)
            for (int i = 0; i < _spikes.Length; i++)
                if (_spikes[i].CheckCollision(_player))
                {
                    Contact?.Invoke(_spikeDamage);
                    break;
                }
    }
}
