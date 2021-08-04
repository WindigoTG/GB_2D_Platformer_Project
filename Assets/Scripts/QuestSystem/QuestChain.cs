using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class QuestChain : IQuestChain
{
    private readonly List<IQuest> _questsCollection;
    private readonly int _id;

    public event Action Completed;

    public QuestChain(List<IQuest> questsCollection, int id)
    {
        _questsCollection = questsCollection ?? throw new ArgumentNullException(nameof(questsCollection));
        _id = id;

        Subscribe();

        ResetQuest(0);
    }

    public void Update()
    {
        for (int i = 0; i < _questsCollection.Count; i++)
            _questsCollection[i].Update();
    }

    private void Subscribe()
    {
        foreach (var quest in _questsCollection) quest.Completed += OnQuestCompleted;
    }

    private void Unsubscribe()
    {
        foreach (var quest in _questsCollection) quest.Completed -= OnQuestCompleted;
    }

    private void OnQuestCompleted(object sender, IQuest quest)
    {
        var index = _questsCollection.IndexOf(quest);
        if (IsDone)
        {
            Debug.LogWarning("Quest chain is done!");
            Completed?.Invoke();
        }
        else
        {
            // если очередной квест выполнен, запускаем следующий квест
            ResetQuest(++index);
        }
    }

    private void ResetQuest(int index)
    {
        if (index < 0 || index >= _questsCollection.Count) return;
        var nextQuest = _questsCollection[index];
        if (nextQuest.IsCompleted) OnQuestCompleted(this, nextQuest);
        else _questsCollection[index].Reset();
    }

    public bool IsDone => _questsCollection.All(value => value.IsCompleted);

    public int Id => _id;

    public void Dispose()
    {
        Unsubscribe();
        foreach (var quest in _questsCollection) quest.Dispose();
    }
}

