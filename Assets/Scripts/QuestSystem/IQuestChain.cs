using System;

public interface IQuestChain : IDisposable
{
    void Update();
    bool IsDone { get; }
    int Id { get; }
    event Action Completed;
}

