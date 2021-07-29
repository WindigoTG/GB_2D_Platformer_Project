using UnityEngine;

public class PlayerControllerPhys : IUpdateable, IUpdateableFixed
{
    PlayerModelPhys _player;
    IPlayerFactory _playerfactory;
    TeleporterHandler _teleporterHandler;

    float _inputHor;

    float _crystalCount;

    public PlayerControllerPhys(IPlayerFactory playerfactory, TeleporterHandler teleporterHandler)
    {
        _playerfactory = playerfactory;
        _player = _playerfactory.CreatePlayerPhys();
        _teleporterHandler = teleporterHandler;

        _player.StartAtPosition(_teleporterHandler.GetStart().position);

        _teleporterHandler.Finish += GetToFinish;
    }

    public void Update()
    {
        _player.Update();

        if (Input.GetMouseButton(1))
            _player.ChangeForm();

        if (Input.GetMouseButton(0))
            _player.Shoot();

        _inputHor = Input.GetAxisRaw("Horizontal");
        _player.Crouch(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
            _player.Jump();

        if (Input.GetKeyDown(KeyCode.E))
        {
            
            var teleporter = _player.FindInteractableTeleporter();
            if (teleporter != null)
            {
                if (_crystalCount >= 3 || _teleporterHandler.IsTeleporterUnlocked(teleporter))
                {
                    var destination = _teleporterHandler.GetDestination(teleporter);
                    if (destination != null)
                    {
                        if (!_teleporterHandler.IsTeleporterUnlocked(destination))
                        {
                            _crystalCount -= 3;
                            _teleporterHandler.Unlock(destination);
                        }
                        _player.TeleportToPosition(destination.position);
                    }
                }
            }
        }
    }

    public void FixedUpdate()
    {
        _player.Move(_inputHor);
    }

    public void CollectCrystal()
    {
        _crystalCount++;
    }

    public void RegisterHazards(HazardController hazardController, CannonController cannonController, AIController AIController)
    {
        _player.RegisterHazards(hazardController, cannonController, AIController);
    }

    private void GetToFinish()
    {
        _player.Finish();
    }

    public Transform PlayerTransform => _player.Transform;

    public PlayerModelPhys Player => _player;
}
