using UnityEngine;

public class BulletView
{
    GameObject _bullet;
    Transform _transform;
    TrailRenderer _trail;
    Rigidbody2D _rigidBody;
    Collider2D _collider;

    public BulletView(GameObject bullet) 
    {
        _bullet = bullet;
        _rigidBody = _bullet.GetComponent<Rigidbody2D>();
        _transform = bullet.transform;
        _trail = bullet.GetComponent<TrailRenderer>();
        _collider = bullet.GetComponent<Collider2D>();
    }

    public Transform Transform => _transform;
    public Rigidbody2D RigidBody => _rigidBody;
    public Collider2D Collider => _collider;
    public bool IsActive => _bullet.activeSelf;

    public void Activate()
    {
        _trail.Clear();
        _bullet.SetActive(true);
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.angularVelocity = 0;
    }

    public void Deactivate()
    {
        _bullet.SetActive(false);
    }
}
