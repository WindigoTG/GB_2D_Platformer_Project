using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBladeModel
{
    SpinningBladeView[] _blades;
    Transform _target;

    int _damage = 1;
    public event System.Action<int> TargetHit;

    public SpinningBladeModel(GameObject trap, Transform target, SpriteAnimatorController animationController)
    {
        _target = target;

        var colliders = trap.GetComponentsInChildren<BoxCollider2D>();
        _blades = new SpinningBladeView[colliders.Length];

        for (int i = 0; i < _blades.Length; i++)
            _blades[i] = new SpinningBladeView(colliders[i], animationController);
    }

    public void Update()
    {
        for (int i = 0; i < _blades.Length; i++)
            if (_blades[i].CheckContacts(_target))
                TargetHit?.Invoke(_damage);
    }
}
