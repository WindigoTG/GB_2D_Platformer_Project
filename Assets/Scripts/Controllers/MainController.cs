using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainController : MonoBehaviour
{
    [SerializeField] Transform _testEnemy;
    [SerializeField] Transform _testObject;
    [SerializeField] Transform _testObject2;
    [Space]
    [SerializeField] Transform[] _cannonPositions;

    List<IUpdateable> _updatables = new List<IUpdateable>();
    List<IUpdatableFixed> _updatablesFixed = new List<IUpdatableFixed>();

    PlayerController _playerController;
    PlayerControllerPhys _playerControllerPhys;

    private void Awake()
    {
        InitializeUpdatables();
    }

    private void Update()
    {
        for (int i = 0; i < _updatables.Count; i++)
            _updatables[i].Update();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _updatablesFixed.Count; i++)
            _updatablesFixed[i].FixedUpdate();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _updatables.Count; i++)
            if (_updatables[i] is IDisposable)
                (_updatables[i] as IDisposable).Dispose();
    }

    void InitializeUpdatables()
    {
        SpriteAnimatorController crystalAnimationController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("ECrystalAnimationsConfig"));
        _updatables.Add(crystalAnimationController);
        ECrystalController crystalController = new ECrystalController(crystalAnimationController);
        _updatables.Add(crystalController);

        SpriteAnimatorController teleporterAnimationController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("TeleporterAnimationsConfig"));
        _updatables.Add(teleporterAnimationController);
        TeleporterHandler.Instance.SetAnimationController(teleporterAnimationController);

        SpriteAnimatorController enemyAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("MetAnimationsConfig"));
        _updatables.Add(enemyAnimatorController);

        PlaceholderEnemyController enemyController = new PlaceholderEnemyController(_testEnemy, enemyAnimatorController);
        _updatables.Add(enemyController);

        #region Animation Test Objects
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
        #endregion

        PlayerFactory playerFactory = new PlayerFactory(this);

        //_playerController = new PlayerController(playerFactory);
        //_updatables.Add(_playerController);

        _playerControllerPhys = new PlayerControllerPhys(playerFactory);
        _updatables.Add(_playerControllerPhys);
        _updatablesFixed.Add(_playerControllerPhys);
        crystalController.CrystalCollected += _playerControllerPhys.CollectCrystal;
        
        if (_cannonPositions.Length > 0)
        {
            CannonController cannonController = new CannonController(_cannonPositions, _playerControllerPhys.PlayerTransform);
            _updatables.Add(cannonController);
            _playerControllerPhys.SetCannonController(cannonController);
        }

        SpikeController spikeController = new SpikeController(_playerControllerPhys.PlayerTransform);
        _updatables.Add(spikeController);
        _playerControllerPhys.SetSpikeController(spikeController);

        FindObjectOfType<CinemachineVirtualCamera>().Follow = _playerControllerPhys.PlayerTransform;

        _playerControllerPhys.Player.End += End;
    }

    public void AddUpdatable(IUpdateable updatable)
    {
        _updatables.Add(updatable);
    }

    public void End()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }
}
