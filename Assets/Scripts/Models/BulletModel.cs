using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class BulletModel
{
    BulletView _bullet;
    float _colliderRadius;
    float _colliderOffset;

    float _gravity = -10f;
    Vector3 _velocity;
    float _force = 10f;

    RaycastHit2D[] _hits = new RaycastHit2D[16];
    Vector3 _lastPosition;

    public BulletModel(GameObject bullet)
    {
        _bullet = new BulletView(bullet);
        CalculateColliderData(bullet);
        _bullet.Deactivate();
    }

    private void CalculateColliderData(GameObject bullet)
    {
        CapsuleCollider2D collider = bullet.AddComponent<CapsuleCollider2D>();
        collider.direction = CapsuleDirection2D.Horizontal;
        _colliderOffset = collider.size.x / (collider.size.x / collider.size.y);
        _colliderRadius = collider.size.y / 2;
        Object.Destroy(collider);
    }

    public void Shoot(Vector3 position, Quaternion rotation)
    {
        _bullet.Transform.position = position;
        _bullet.Transform.rotation = rotation;
        SetVelocity(_bullet.Transform.right * _force);
        _bullet.Activate();
    }

    public void Move()
    {
        if (_bullet.IsActive)
        {
            _lastPosition = _bullet.Transform.position;
            SetVelocity(_velocity + Vector3.up * _gravity * Time.deltaTime);

            _bullet.Transform.position += _velocity * Time.deltaTime;

            CheckForCollision();
        }
    }

    private void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
        var angle = Vector3.Angle(Vector3.left, _velocity);
        var axis = Vector3.Cross(Vector3.left, _velocity);
        _bullet.Transform.rotation = Quaternion.AngleAxis(angle, axis);

    }

    private void CheckForCollision()
    {
        Array.Clear(_hits, 0, _hits.Length);

        Vector3 origin = _lastPosition + (_bullet.Transform.right * _colliderOffset);
        Vector3 direction = _bullet.Transform.position - origin;
        float maxDistance = direction.magnitude;

        Physics2D.CircleCastNonAlloc(origin, _colliderRadius, direction.normalized, _hits, maxDistance);

        bool checkGround = true;
        bool checkWall = true;

        foreach (var h in _hits)
            if (h.collider != null)
            {
                if (checkGround && h.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    checkGround = false;
                    SetVelocity(_velocity.Change(y: -_velocity.y));
                    if ((h.point.y - _bullet.Transform.position.y) < 0)
                        _bullet.Transform.position = _bullet.Transform.position.Change(y: h.collider.bounds.max.y + (_colliderOffset + _colliderRadius));
                    else
                        _bullet.Transform.position = _bullet.Transform.position.Change(y: h.collider.bounds.min.y - (_colliderOffset + _colliderRadius));
                }
                if (checkWall && h.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    checkWall = false;
                    SetVelocity(_velocity.Change(x: -_velocity.x));
                    if ((h.point.x - _bullet.Transform.position.x) > 0)
                        _bullet.Transform.position = _bullet.Transform.position.Change(x: h.collider.bounds.min.x - (_colliderOffset + _colliderRadius));
                    else
                        _bullet.Transform.position = _bullet.Transform.position.Change(x: h.collider.bounds.max.x + (_colliderOffset + _colliderRadius));
                }

                if (!checkWall && !checkGround)
                    break;
            }
    }
}
