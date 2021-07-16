using UnityEngine;

public class ECrystalModel
{
    ECrystalView _view;
    float _radius;
    Vector2 _position;
    RaycastHit2D[] _hits = new RaycastHit2D[8];
    bool _isActive;

    public ECrystalModel(Transform transform, SpriteAnimatorController animationController)
    {
        _view = new ECrystalView(transform, animationController);
        _view.StartAnimation();

        var collider = transform.gameObject.AddComponent<CapsuleCollider2D>();
        _radius = collider.size.x / 2;
        Object.Destroy(collider);

        _position = new Vector2(transform.position.x, transform.position.y);

        _isActive = true;
    }
    public bool CheckCollisions()
    {
        System.Array.Clear(_hits, 0, _hits.Length);
        Physics2D.CircleCastNonAlloc(_position, _radius, _position, _hits, 0, LayerMask.GetMask("Player"));

        for (int i = 0; i < _hits.Length; i++)
            if (_hits[i].collider != null)
            {
                _view.Deactivate();
                _isActive = false;
                return true;
            }

        return false;
    }

    public bool IsActive => _isActive;
}
