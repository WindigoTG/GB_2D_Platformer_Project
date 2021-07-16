using UnityEngine;
using System;

public class ECrystalController : IUpdateable
{
    ECrystalModel[] _crystals;
    public event Action CrystalCollected;

    public ECrystalController(SpriteAnimatorController animationController)
    {
        var crystals = GameObject.FindGameObjectsWithTag("ECrystal");

        _crystals = new ECrystalModel[crystals.Length];

        for (int i = 0; i < _crystals.Length; i++)
        {
            _crystals[i] = new ECrystalModel(crystals[i].transform, animationController);
        }
    }

    public void Update()
    {
        if (Time.frameCount %3 == 0)
            for (int i = 0; i < _crystals.Length; i++)
            {
                if (_crystals[i].IsActive && _crystals[i].CheckCollisions())
                    CrystalCollected?.Invoke();
            }
    }
}
