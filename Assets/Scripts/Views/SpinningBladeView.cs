using UnityEngine;

public class SpinningBladeView 
{
    Collider2D _collider;

    Collider2D[] _contacts = new Collider2D[8];

    public SpinningBladeView(Collider2D collider, SpriteAnimatorController animationController)
    {
        _collider = collider;

        animationController.StartAnimation(_collider.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
    }

    public bool CheckContacts(Transform target)
    {
        System.Array.Clear(_contacts, 0, _contacts.Length);
        _collider.GetContacts(_contacts);
        for (int i = 0; i < _contacts.Length; i++)
            if (_contacts[i] != null && _contacts[i].transform == target)
                return true;

        return false;
    }
}
