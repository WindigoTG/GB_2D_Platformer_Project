using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Quest/QuestChainConfig", fileName = "QuestChainConfig", order = 2)]
public class QuestChainConfig : ScriptableObject
{
    public int Id;
    public QuestConfig[] Quests;
    public QuestStoryType QuestChainType;
}
public enum QuestStoryType
{
    Common,
    Resettable
}

