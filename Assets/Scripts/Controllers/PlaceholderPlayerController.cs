using UnityEngine;

public class PlaceholderPlayerController : IUpdateable
{
    private Transform _player;
    private SpriteAnimatorController _animatorController;
    private SpriteRenderer _spriteRenderer;
    float _animationSpeed = 10.0f;

    private Rigidbody2D _rigidBody;
    float _jumpForce = 15.0f;

    private float _speed = 6.5f;
    private float _inputHor;
    bool _isRunning;
    bool _isGrounded;
    bool _isFalling;

    public PlaceholderPlayerController(Transform player, SpriteAnimatorController animatorController)
    {
        _player = player;
        _animatorController = animatorController;
        _spriteRenderer = _player.GetComponent<SpriteRenderer>();
        _rigidBody = _player.GetComponent<Rigidbody2D>();
        StartIdle();
    }

    public void Update()
    {
        _inputHor = Input.GetAxisRaw("Horizontal");
        if (_inputHor != 0)
        {
            Move();
            Flip();
        }
        else if (_isRunning)
            StartIdle();

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
            Jump();

        if (!_isGrounded && _rigidBody.velocity.y < 0 && !_isFalling)
            Fall();

        if (!_isGrounded && _rigidBody.velocity.y == 0 && _isFalling)
            StartIdle();
    }

    void Move()
    {
        _player.Translate(Vector3.right * _inputHor * _speed * Time.deltaTime);
        if (!_isRunning && _isGrounded)
        {
            _isRunning = true;
            _animatorController.StartAnimation(_spriteRenderer, Track.Run, true, _animationSpeed);
        }
    }

    void Flip()
    {
        if ((_inputHor > 0 && _player.localScale.x < 0) ||
            (_inputHor < 0 && _player.localScale.x > 0))
        {
            _player.localScale = new Vector3(-_player.localScale.x, 1, 1);
        }
    }

    void StartIdle()
    {
        _isGrounded = true;
        _isRunning = false;
        _isFalling = false;
        _animatorController.StartAnimation(_spriteRenderer, Track.Idle, true, _animationSpeed);
    }

    void Jump()
    {
        _isGrounded = false;
        _isRunning = false;
        _rigidBody.AddRelativeForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _animatorController.StartAnimation(_spriteRenderer, Track.Jump, false, _animationSpeed);
    }

    void Fall()
    {
        _isFalling = true;
        _isRunning = false;
        _animatorController.StartAnimation(_spriteRenderer, Track.Fall, false, _animationSpeed);
    }
}
