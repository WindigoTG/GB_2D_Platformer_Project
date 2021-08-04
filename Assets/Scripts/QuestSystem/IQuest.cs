using System;

public interface IQuest : IDisposable
{
    event EventHandler<IQuest> Completed;
    bool IsCompleted { get; }
    void Reset();
    void Update();
}

