using UnityEngine;

public class CannonModel
{
    CannonView _cannon;
    private Transform _targetTransform;
    float _rotationSpeed = 30f;
    

    BulletModel[] _bullets;
    int _bulletsAmmount = 5;

    float _fireDelay = 3f;
    float _fireCoolDown;
    float _fireRange = 12;  //–ассто€ние, в пределах которого должна находитьс€ цель.
                            //ƒл€ предотвращени€ беспор€дочной пальбы, когда игрок в другой части уровн€.
    int _currentBullet = 0;
    int _damage = 1;

    public event System.Action<int> TargetHit;

    public CannonModel(Transform targetTransform, Vector3 position)
    {
        _cannon = new CannonView(
            Object.Instantiate(Resources.Load<GameObject>("Cannon")));

        _cannon.SetPosition(position);

        _targetTransform = targetTransform;

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
            if ((_targetTransform.position - _cannon.BarrelPosition).magnitude <= _fireRange)
            {
                _bullets[_currentBullet++].Shoot(_cannon.MuzzlePosition, _cannon.MuzzleRotation);

                if (_currentBullet >= _bullets.Length)
                    _currentBullet = 0;

                _fireCoolDown = _fireDelay;
            }
        }

        for (int i = 0; i < _bullets.Length; i++)
            if (_bullets[i].CheckCollision(_targetTransform))
                TargetHit?.Invoke(_damage);
    }

    private void TrackTarget()
    {
        var dir = _targetTransform.position - _cannon.BarrelPosition;
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
