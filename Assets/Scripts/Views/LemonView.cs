using UnityEngine;

public class LemonView
{
    Rigidbody2D _rigidBody;

    float _lifetime = 2f;
    float _firedAt;

    public LemonView(GameObject lemon)
    {
        _rigidBody = lemon.GetComponent<Rigidbody2D>();
    }

    public Rigidbody2D RigidBody => _rigidBody;
    public Transform Transform => _rigidBody.transform;
    public float Lifetime => _lifetime;
    public float FiredAt
    {
        get => _firedAt;
        set => _firedAt = value;
    }

    public void Activate()
    {
        _rigidBody.gameObject.SetActive(true);
        _rigidBody.velocity = Vector2.zero;
    }
    public void Deactivate()
    {
        _rigidBody.gameObject.SetActive(false);
    }
}
