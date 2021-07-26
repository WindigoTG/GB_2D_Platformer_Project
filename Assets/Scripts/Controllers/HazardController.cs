using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : IUpdateable
{
    Transform _player;
    MainController _mainController;

    SpikeModel[] _spikes;
    LightningTrapModel[] _lightningTraps;
    SpinningBladeModel[] _spinningBladeTraps;

    public HazardController(Transform player, MainController mainController)
    {
        _player = player;
        _mainController = mainController;

        SetSpikes();
        SetUpLightningTraps();
        SetUpSpinningBlades();
    }

    public void Update()
    {
        if (Time.frameCount % 2 == 0)
        {
            for (int i = 0; i < _lightningTraps.Length; i++)
                _lightningTraps[i].Update();


            for (int i = 0; i < _spinningBladeTraps.Length; i++)
                _spinningBladeTraps[i].Update();
        }
        if (Time.frameCount % 3 == 0)
            for (int i = 0; i < _spikes.Length; i++)
                _spikes[i].Update();
    }

    void SetSpikes()
    {
        var spikes = GameObject.FindGameObjectsWithTag("Spike");
        _spikes = new SpikeModel[spikes.Length];
        for (int i = 0; i < _spikes.Length; i++)
            _spikes[i] = new SpikeModel(spikes[i], _player);
    }

    void SetUpLightningTraps()
    {
        var traps = GameObject.FindGameObjectsWithTag("LightningTrap");
        _lightningTraps = new LightningTrapModel[traps.Length];

        SpriteAnimatorController lightningAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("LightningAnimationsConfig"));
        _mainController.AddUpdatable(lightningAnimatorController);

        for (int i = 0; i < _lightningTraps.Length; i++)
            _lightningTraps[i] = new LightningTrapModel(traps[i].gameObject, _player, lightningAnimatorController);
    }

    void SetUpSpinningBlades()
    {
        var traps = GameObject.FindGameObjectsWithTag("SpinningBladeTrap");
        _spinningBladeTraps = new SpinningBladeModel[traps.Length];

        SpriteAnimatorController spinningBladeAnimatorController = new SpriteAnimatorController(
                                                                Resources.Load<SpriteAnimationsConfig>("SpinningBladeAnimationsConfig"));
        _mainController.AddUpdatable(spinningBladeAnimatorController);

        for (int i = 0; i < _spinningBladeTraps.Length; i++)
            _spinningBladeTraps[i] = new SpinningBladeModel(traps[i].gameObject, _player, spinningBladeAnimatorController);
    }

    public void RegisterTarget(PlayerHealth playerHealth)
    {
        for (int i = 0; i < _spikes.Length; i++)
            _spikes[i].TargetHit += playerHealth.TakeDamage;

        for (int i = 0; i < _lightningTraps.Length; i++)
            _lightningTraps[i].TargetHit += playerHealth.TakeDamage;

        for (int i = 0; i < _spinningBladeTraps.Length; i++)
            _spinningBladeTraps[i].TargetHit += playerHealth.TakeDamage;
    }
}
