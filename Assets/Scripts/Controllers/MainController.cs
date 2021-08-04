using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using LevelGeneration;
#endif

public class MainController : MonoBehaviour
{
    [SerializeField] Transform _testEnemy;
    [SerializeField] Transform _testObject;
    [SerializeField] Transform _testObject2;
    [Space]
    [SerializeField] Transform[] _cannonPositions;
    [Space]
    [SerializeField] TeleporterData _teleporterData;
    [Space]
    [SerializeField] PatrolData[] _patrolData;
    [SerializeField] PatrolData[] _smartPatrolData;
    [Space]
    [SerializeField] QuestsConfigurator _questConfigurator;
    [Space]
    [SerializeField] int _exitQuestChainId;

    List<IUpdateable> _updatables = new List<IUpdateable>();
    List<IUpdateableFixed> _updatablesFixed = new List<IUpdateableFixed>();

    PlayerController _playerController;
    PlayerControllerPhys _playerControllerPhys;

    private void Awake()
    {
        InitializeUpdatables();

        var questChain = _questConfigurator.GetQuestChain(_exitQuestChainId);
        if (questChain != null)
        {
            DisableExit();
            questChain.Completed += EnableExit;
        }

        new LevelGenerator(Resources.Load<LevelGenerationConfig>("LevelGenerationConfig"));
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
        TeleporterHandler teleporterHandler = new TeleporterHandler(_teleporterData ,teleporterAnimationController);

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

        SpriteAnimatorController platformAnimationController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("PlatformAnimationsConfig"));
        _updatables.Add(platformAnimationController);
        MovingPlatformController platformController = new MovingPlatformController(platformAnimationController);
        _updatablesFixed.Add(platformController);

        _playerControllerPhys = new PlayerControllerPhys(playerFactory, teleporterHandler);
        _updatables.Add(_playerControllerPhys);
        _updatablesFixed.Add(_playerControllerPhys);
        crystalController.CrystalCollected += _playerControllerPhys.CollectCrystal;

        CannonController cannonController = new CannonController(_cannonPositions, _playerControllerPhys.PlayerTransform);
        _updatables.Add(cannonController);

        HazardController hazardController = new HazardController(_playerControllerPhys.PlayerTransform, this);
        _updatables.Add(hazardController);


        SpriteAnimatorController tellyAnimationController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("TellyAnimationsConfig"));
        _updatables.Add(tellyAnimationController);
        SpriteAnimatorController explosionAnimationController = new SpriteAnimatorController(
                                                                 Resources.Load<SpriteAnimationsConfig>("ExplosionAnimationsConfig"));
        _updatables.Add(explosionAnimationController);
        AIController AIController = new AIController(_patrolData, _smartPatrolData, tellyAnimationController, explosionAnimationController, _playerControllerPhys.PlayerTransform);
        _updatablesFixed.Add(AIController);


        _playerControllerPhys.RegisterHazards(hazardController, cannonController, AIController);

        FindObjectOfType<CinemachineVirtualCamera>().Follow = _playerControllerPhys.PlayerTransform;

        _playerControllerPhys.Player.End += End;

        StartFans();

        _questConfigurator.Initialize(_playerControllerPhys.PlayerTransform);
        _updatablesFixed.Add(_questConfigurator);
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

    private void StartFans()
    {
        SpriteAnimatorController fanAnimationController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("FanAnimationsConfig"));
        _updatables.Add(fanAnimationController);
        var fans = FindObjectsOfType<WindZone>();
        foreach (var fan in fans)
        {
            fanAnimationController.StartAnimation(fan.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
        }
    }

    void DisableExit()
    {
        _teleporterData.FinishTeleporter.gameObject.SetActive(false);
    }

    void EnableExit()
    {
        _teleporterData.FinishTeleporter.gameObject.SetActive(true);
    }
}
