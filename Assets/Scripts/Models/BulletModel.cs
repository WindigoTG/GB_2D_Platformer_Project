using UnityEngine;

public class BulletModel
{
    BulletView _bullet;

    float _force = 25f;

    public BulletModel(GameObject bullet)
    {
        _bullet = new BulletView(bullet);
        _bullet.Deactivate();
    }


    public void Shoot(Vector3 position, Quaternion rotation)
    {
        _bullet.Transform.position = position;
        _bullet.Transform.rotation = rotation;
        _bullet.Activate();
        _bullet.RigidBody.AddForce(_bullet.Transform.right * _force, ForceMode2D.Impulse);
    }
}
