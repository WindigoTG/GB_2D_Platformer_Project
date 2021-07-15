using UnityEngine;

public class PlayerControllerPhys : IUpdateable, IUpdatableFixed
{
    PlayerModelPhys _player;
    IPlayerFactory _playerfactory;

    float _inputHor;

    float _crystalCount;

    public PlayerControllerPhys(IPlayerFactory playerfactory)
    {
        _playerfactory = playerfactory;
        _player = _playerfactory.CreatePlayerPhys();

        _player.StartAtPosition(TeleporterHandler.Instance.GetStart().position);

        TeleporterHandler.Instance.Finish += GetToFinish;
    }

    public void Update()
    {
        _player.Update();

        if (Input.GetMouseButton(1))
            _player.ChangeForm();

        _inputHor = Input.GetAxisRaw("Horizontal");
        _player.Crouch(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
            _player.Jump();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_crystalCount >= 3)
            {
                var teleporter = _player.FindInteractableTeleporter();
                if (teleporter != null)
                {
                    var destination = TeleporterHandler.Instance.GetDestination(teleporter);
                    if (destination != null)
                    {
                        _player.TeleportToPosition(destination.position);
                        _crystalCount -= 3;
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

    public void SetSpikeController(SpikeController spikeController)
    {
        _player.SetSpikeController(spikeController);
    }

    public void SetCannonController(CannonController cannonController)
    {
        _player.SetCannonController(cannonController);
    }

    private void GetToFinish()
    {
        _player.Finish();
    }

    public Transform PlayerTransform => _player.Transform;

    public PlayerModelPhys Player => _player;
}
