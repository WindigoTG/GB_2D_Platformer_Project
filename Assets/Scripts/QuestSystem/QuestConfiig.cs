using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Quest/QuestConfig", fileName = "QuestConfig", order = 0)]
public class QuestConfig : ScriptableObject
{
    public int Id;
    public QuestType QuestType;
}

public enum QuestType
{
    Switch,
}

