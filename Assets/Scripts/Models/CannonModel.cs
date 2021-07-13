using UnityEngine;

public class CannonModel
{
    CannonView _cannon;
    private PlayerPosition _targetPosition;
    float _rotationSpeed = 30f;

    BulletModel[] _bullets;
    int _bulletsAmmount = 5;

    float _fireDelay = 3f;
    float _fireCoolDown;
    int _currentBullet = 0;

    public CannonModel(PlayerPosition targetPosition)
    {
        _cannon = new CannonView(
            Object.Instantiate(Resources.Load<GameObject>("Cannon")));

        _cannon.SetPosition(new Vector3(0, 2, 0));

        _targetPosition = targetPosition;

        InitializeBullets();
        _fireCoolDown = _fireDelay;
    }

    public void Update()
    {
        TrackTarget();

        if (_fireCoolDown > 0)
            _fireCoolDown -= Time.deltaTime;
        else
        {
            _bullets[_currentBullet++].Shoot(_cannon.MuzzlePosition, _cannon.MuzzleRotation);

            if (_currentBullet >= _bullets.Length)
                _currentBullet = 0;

            _fireCoolDown = _fireDelay;
        }

        UpdateBullets();
    }

    private void UpdateBullets()
    {
        for (int i = 0; i < _bullets.Length; i++)
        {
            _bullets[i].Move();
        }
    }

    private void TrackTarget()
    {
        var dir = _targetPosition.Position - _cannon.BarrelPosition;
        var angle = Vector3.Angle(Vector3.right, dir);
        var axis = Vector3.Cross(Vector3.right, dir);
        _cannon.RotateToward(Quaternion.AngleAxis(angle, axis), _rotationSpeed);
    }

    private void InitializeBullets()
    {
        _bullets = new BulletModel[_bulletsAmmount];
        GameObject bulletPrefab = Resources.Load<GameObject>("Bullet");
        for (int i = 0; i < _bullets.Length; i++)
        {
            _bullets[i] = new BulletModel(Object.Instantiate(bulletPrefab));
        }
    }
}
