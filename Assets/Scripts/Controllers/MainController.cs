using System;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] Transform _testPlayer;
    [SerializeField] Transform _testEnemy;

    List<IUpdateable> _updatables = new List<IUpdateable>();

    private void Awake()
    {
        InitializeUpdatables();
    }

    private void Update()
    {
        for (int i = 0; i < _updatables.Count; i++)
            _updatables[i].Update();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _updatables.Count; i++)
            if (_updatables[i] is IDisposable)
                (_updatables[i] as IDisposable).Dispose();
    }

    void InitializeUpdatables()
    {
        SpriteAnimatorController playerAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("PlayerAnimationsConfig"));
        _updatables.Add(playerAnimatorController);

        PlaceholderPlayerController playerController = new PlaceholderPlayerController(_testPlayer, playerAnimatorController);
        _updatables.Add(playerController);

        SpriteAnimatorController enemyAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("MetAnimationsConfig"));
        _updatables.Add(enemyAnimatorController);

        PlaceholderEnemyController enemyController = new PlaceholderEnemyController(_testEnemy, enemyAnimatorController);
        _updatables.Add(enemyController);
    }
}
