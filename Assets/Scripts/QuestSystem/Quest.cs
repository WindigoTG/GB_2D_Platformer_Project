using System;
using UnityEngine;

public sealed class Quest : IQuest
{
    private readonly QuestObjectView _view;
    private readonly IQuestModel _model;

    private bool _active;
    private Transform _target;

    public Quest(QuestObjectView view, IQuestModel model, Transform target)
    {
        _view = view != null ? view : throw new ArgumentNullException(nameof(view));
        _model = model != null ? model : throw new ArgumentNullException(nameof(model));
        _target = target;
    }

    public void Update()
    {
        if (_view.CheckInteraction(_target))
            Complete();
    }

    private void OnContact(GameObject obj)
    {
        var completed = _model.TryComplete(obj);
        if (completed) Complete();
    }

    private void Complete()
    {
        if (!_active) return;
        _active = false;
        IsCompleted = true;
        _view.ProcessComplete();
        OnCompleted();
    }

    private void OnCompleted()
    {
        Completed?.Invoke(this, this);
    }

    public event EventHandler<IQuest> Completed;
    public bool IsCompleted { get; private set; }

    public void Reset()
    {
        if (_active) return;
        _active = true;
        IsCompleted = false;
        _view.ProcessActivate();
    }

    public void Dispose()
    {

    }
}

