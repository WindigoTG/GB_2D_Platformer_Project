using UnityEngine;

public class SpikeModel
{
    Collider2D _collider;
    Collider2D[] _contacts = new Collider2D[8];

    public SpikeModel (GameObject spike)
    {
        _collider = spike.GetComponent<Collider2D>();
    }

    public bool CheckCollision(Transform player)
    {
        System.Array.Clear(_contacts, 0, _contacts.Length);
        _collider.GetContacts(_contacts);
        for (int i = 0; i < _contacts.Length; i++)
            if (_contacts[i] != null && _contacts[i].transform == player)
                return true;

        return false;
    }
}
