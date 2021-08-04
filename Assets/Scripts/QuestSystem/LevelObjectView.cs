using System;
using UnityEngine;

public class LevelObjectView : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Transform Transform;
    public Collider2D Collider2D;
    public Rigidbody2D Rigidbody2D;

    public Action<GameObject> OnLevelObjectContact { get; set; }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var levelObject = collider.gameObject;
        OnLevelObjectContact?.Invoke(levelObject);
    }
}
