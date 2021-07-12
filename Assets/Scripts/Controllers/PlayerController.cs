using UnityEngine;

public class PlayerController : IUpdateable
{
    PlayerModel _player;
    IPlayerFactory _playerfactory;

    public PlayerController(IPlayerFactory playerfactory)
    {
        _playerfactory = playerfactory;
        _player = _playerfactory.CreatePlayer();

        _player.StartAtPosition(new Vector3(2, 1, 0));
    }

    public void Update()
    {
        _player.Update();

        if (Input.GetMouseButton(1))
            _player.ChangeForm();

        _player.Move(Input.GetAxisRaw("Horizontal"));
        _player.Crouch(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
            _player.Jump();
    }

    public PlayerPosition PlayerPosition => _player.PlayerPosition;
}
