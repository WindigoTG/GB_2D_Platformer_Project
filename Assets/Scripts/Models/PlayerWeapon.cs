using UnityEngine;

public class PlayerWeapon
{
    Transform _weaponMuzzle;
    LemonView[] _lemons = new LemonView[3];
    int _currentLemon = 0;
    float _force = 50;

    public PlayerWeapon(Transform weaponMuzzle)
    {
        _weaponMuzzle = weaponMuzzle;

        var lemon = Resources.Load<GameObject>("Lemon");

        for (int i = 0; i < _lemons.Length; i++)
        {
            _lemons[i] = new LemonView(Object.Instantiate(lemon));
            _lemons[i].Deactivate();
        }
    }

    public void Shoot(float direction)
    {
        if ((Time.time - _lemons[_currentLemon].FiredAt) > _lemons[_currentLemon].Lifetime)
        {
            _lemons[_currentLemon].Transform.position = _weaponMuzzle.position;
            _lemons[_currentLemon].Transform.localScale = _lemons[_currentLemon].Transform.localScale.Change(x: direction);
            _lemons[_currentLemon].Activate();
            _lemons[_currentLemon].RigidBody.velocity = Vector2.right * (_force * direction);
            _lemons[_currentLemon].FiredAt = Time.time;

            _currentLemon++;
            if (_currentLemon >= _lemons.Length)
                _currentLemon = 0;
        }
    }
}
