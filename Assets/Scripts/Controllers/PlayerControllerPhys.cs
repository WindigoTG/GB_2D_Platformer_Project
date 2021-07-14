using UnityEngine;

public class PlayerControllerPhys : IUpdateable, IUpdatableFixed
{
    PlayerModelPhys _player;
    IPlayerFactory _playerfactory;

    float _inputHor;

    public PlayerControllerPhys(IPlayerFactory playerfactory)
    {
        _playerfactory = playerfactory;
        _player = _playerfactory.CreatePlayerPhys();

        _player.StartAtPosition(new Vector3(0, 1, 0));
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
    }

    public void FixedUpdate()
    {
        _player.Move(_inputHor);
    }

    public Transform PlayerTransform => _player.Transform;
}
