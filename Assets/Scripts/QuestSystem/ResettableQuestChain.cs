using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ResettableQuestChain : IQuestChain
{
    private readonly List<IQuest> _questsCollection;
    private readonly int _id;
    private int _currentIndex;

    public event Action Completed;

    public ResettableQuestChain(List<IQuest> questsCollection, int id)
    {
        _questsCollection = questsCollection ?? throw new ArgumentNullException(nameof(questsCollection));
        _id = id;

        Subscribe();

        ResetQuests();
    }

    public void Update()
    {
        for (int i = 0; i < _questsCollection.Count; i++)
            _questsCollection[i].Update();
    }

    private void Subscribe()
    {
        foreach (var quest in _questsCollection)
        {
            quest.Completed += OnQuestCompleted;
        }
    }

    private void Unsubscribe()
    {
        foreach (var quest in _questsCollection)
        {
            quest.Completed -= OnQuestCompleted;
        }
    }

    private void OnQuestCompleted(object sender, IQuest quest)
    {
        var index = _questsCollection.IndexOf(quest);

        if (_currentIndex == index)
        {
            _currentIndex++;
            if (IsDone)
            {
                Debug.LogWarning("Quest chain is done!");
                Completed?.Invoke();
            }
        }
        else
        {
            ResetQuests();
        }
    }

    private void ResetQuests()
    {
        _currentIndex = 0;
        foreach (var quest in _questsCollection)
        {
            quest.Reset();
        }
    }

    public bool IsDone => _questsCollection.All(value => value.IsCompleted);

    public int Id => _id;

    public void Dispose()
    {
        Unsubscribe();
        foreach (var quest in _questsCollection)
        {
            quest.Dispose();
        }
    }
}

