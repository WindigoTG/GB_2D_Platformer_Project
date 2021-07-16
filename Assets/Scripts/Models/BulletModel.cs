using UnityEngine;

public class BulletModel
{
    BulletView _bullet;

    float _force = 25f;
    float _lethalVelocity = 5f; //Скорость при которой контакт с пулей опасен для игрока,
                                //чтобы избежать получения урона от лежащих неподвижно пуль 

    Collider2D[] _contacts = new Collider2D[8];

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

    public bool CheckCollision(Transform target)
    {
        if (_bullet.RigidBody.velocity.magnitude > _lethalVelocity)
        {
            System.Array.Clear(_contacts, 0, _contacts.Length);
            _bullet.Collider.GetContacts(_contacts);
            for (int i = 0; i < _contacts.Length; i++)
                if (_contacts[i] != null && _contacts[i].transform == target)
                    return true;
        }

        return false;
    }
}
