using System;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] Transform _testEnemy;
    [SerializeField] Transform _testObject;
    [SerializeField] Transform _testObject2;

    List<IUpdateable> _updatables = new List<IUpdateable>();

    PlayerController _playerController;

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
        SpriteAnimatorController enemyAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("MetAnimationsConfig"));
        _updatables.Add(enemyAnimatorController);

        PlaceholderEnemyController enemyController = new PlaceholderEnemyController(_testEnemy, enemyAnimatorController);
        _updatables.Add(enemyController);


        if (_testObject.gameObject.activeSelf)
        {
            SpriteAnimatorController testAnimatorController = new SpriteAnimatorController(
                                                                    Resources.Load<SpriteAnimationsConfig>("VentAnimationsConfig"));
            _updatables.Add(testAnimatorController);
            _testObject.GetComponent<Test>().SetAnimator(testAnimatorController);
        }

        if (_testObject2.gameObject.activeSelf)
        {
            SpriteAnimatorController test2AnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("ModelXAnimationsConfig"));
            _updatables.Add(test2AnimatorController);
            _testObject2.GetComponent<Test>().SetAnimator(test2AnimatorController);
        }

        _playerController = new PlayerController(new PlayerFactory(this));
        _updatables.Add(_playerController);

        CannonController cannonController = new CannonController(_playerController.PlayerPosition);
        _updatables.Add(cannonController);
    }

    public void AddUpdatable(IUpdateable updatable)
    {
        _updatables.Add(updatable);
    }
}
