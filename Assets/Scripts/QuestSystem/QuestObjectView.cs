using UnityEngine;

public class QuestObjectView
{
    public int Id => _id;

    private Color _completedColor;
    private Color _defaultColor;
    private int _id;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    Collider2D[] _overlappingColliders = new Collider2D[4];
    ContactFilter2D _filter;

    public QuestObjectView(GameObject questObject, Color defaultColor, Color completedColor)
    {
        _spriteRenderer = questObject.GetComponent<SpriteRenderer>();
        _collider = questObject.GetComponent<Collider2D>();
        _defaultColor = defaultColor;
        _completedColor = completedColor;

        ProcessActivate();

        _filter = new ContactFilter2D();
        _filter.layerMask = LayerMask.GetMask("Player");
    }

    public void ProcessComplete()
    {
        _spriteRenderer.color = _completedColor;
    }

    public void ProcessActivate()
    {
        _spriteRenderer.color = _defaultColor;
    }

    public bool CheckInteraction(Transform target)
    {
        System.Array.Clear(_overlappingColliders, 0, _overlappingColliders.Length);

        _collider.OverlapCollider(_filter, _overlappingColliders);

        for (int i = 0; i < _overlappingColliders.Length; i++)
        {
            if (_overlappingColliders[i] != null && _overlappingColliders[i].transform == target)
                return true;
        }

        return false;
    }
}
