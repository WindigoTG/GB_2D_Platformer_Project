using UnityEngine;

public class BulletView
{
    GameObject _bullet;
    Transform _transform;
    TrailRenderer _trail;

    public BulletView(GameObject bullet) 
    {
        _bullet = bullet;
        _transform = bullet.transform;
        _trail = bullet.GetComponent<TrailRenderer>();
    }

    public Transform Transform => _transform;
    public bool IsActive => _bullet.activeSelf;

    public void Activate()
    {
        _trail.Clear();
        _bullet.SetActive(true);
    }

    public void Deactivate()
    {
        _bullet.SetActive(false);
    }
}
