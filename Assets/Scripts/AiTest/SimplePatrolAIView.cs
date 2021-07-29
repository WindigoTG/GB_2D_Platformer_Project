using UnityEngine;

public class SimplePatrolAIView
{
    Rigidbody2D _rigidBody;
    Transform _transform;
    ContactPoint2D[] _contacts = new ContactPoint2D[8];

    public SimplePatrolAIView(GameObject viewObject)
    {
        _rigidBody = viewObject.GetComponent<Rigidbody2D>();
        _transform = viewObject.transform;
    }

    public Rigidbody2D Rigidbody2D => _rigidBody;
    public Transform Transform => _transform;
    public GameObject Gameobject => _transform.gameObject;

    public bool CheckForTarget(Transform target)
    {
        System.Array.Clear(_contacts, 0, _contacts.Length);

        if (_rigidBody.GetContacts(_contacts) > 0)
            for (int i = 0; i < _contacts.Length; i++)
                if (_contacts[i].collider != null && _contacts[i].collider.transform == target)
                    return true;

        return false;
    }
}
