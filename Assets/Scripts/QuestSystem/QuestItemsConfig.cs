using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Quest/QuestItemsConfig", fileName = "QuestItemsConfig", order = 1)]
public class QuestItemsConfig : ScriptableObject
{
    public int QuestId;
    public List<int> QuestItemIdCollection;
}

