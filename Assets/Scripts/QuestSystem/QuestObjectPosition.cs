using UnityEngine;

[System.Serializable]
public class QuestObjectPosition
{
    [SerializeField] Transform _objectPosition;
    [SerializeField] int _id;

    public Vector3 Position => _objectPosition.position;
    public int Id => _id;
}
