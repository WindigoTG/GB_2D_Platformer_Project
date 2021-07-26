using UnityEngine;

public class LightningTrapModel
{
    CapsuleCollider2D[] _lighnings;
    int _damage = 1;
    Collider2D[] _contacts = new Collider2D[8];
    Transform _target;

    public event System.Action<int> TargetHit;

    public LightningTrapModel(GameObject trap, Transform target, SpriteAnimatorController animationController)
    {
        _lighnings = trap.GetComponentsInChildren<CapsuleCollider2D>();
        _target = target;
        foreach (var lightning in _lighnings)
            animationController.StartAnimation(lightning.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
    }

    public void Update()
    {
        for (int i = 0; i < _lighnings.Length; i++)
        {
            System.Array.Clear(_contacts, 0, _contacts.Length);
            _lighnings[i].GetContacts(_contacts);
            for (int j = 0; j < _contacts.Length; j++)
                if (_contacts[j] != null && _contacts[j].transform == _target)
                {
                    TargetHit?.Invoke(_damage);
                    break;
                }
        }
    }
}
