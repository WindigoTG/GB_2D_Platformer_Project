using UnityEngine;

public class PlayerModel 
{
    PlayerView _currentForm;
    PlayerView _transformedForm;
    PlayerView _untransformedForm;
    Transform _player;

    private float _defaultSpeed = 5f;
    private float _currentSpeed;

    private const float _inputThreshold = 0.1f;
    private const float _spriteOffset = 0.728f;

    private Vector3 _leftScale = new Vector3(-1, 1, 1);
    private Vector3 _rightScale = new Vector3(1, 1, 1);

    private float _groundLevel = -1.0f;
    private float _verticalVelocity;
    private float _jumpForce = 8.0f;
    private float _gravity = 15.0f;

    bool _isReady;
    bool _isMoving;
    bool _isGrounded;
    bool _isJumping;
    bool _isFalling;
    bool _isCrouching;
    bool _isTeleporting;
    bool _isChangingFormPhaseOne;
    bool _isChangingFormPhaseTwo;

    BoxCollider2D _collider;
    Vector2 _raycastStart;
    Vector2 _raycastEnd;
    Vector2 _raycastDirection;

    public PlayerModel(PlayerView untransformedForm, PlayerView transformedForm)
    {
        _transformedForm = transformedForm;
        _untransformedForm = untransformedForm;
        _currentForm = untransformedForm;

        _player = new GameObject("Player").transform;
        _collider = _player.gameObject.AddComponent<BoxCollider2D>();
        _collider.size = new Vector2(1f, 0.1f);
        _collider.isTrigger = true;

        _untransformedForm.Transform.parent = _player;
        _untransformedForm.Transform.localPosition = new Vector3(0, _spriteOffset, 0);
        _untransformedForm.Deactivate();

        _transformedForm.Transform.parent = _player;
        _transformedForm.Transform.localPosition = new Vector3(0, _spriteOffset, 0);
        _transformedForm.Deactivate();

        _currentSpeed = _defaultSpeed;
        _isGrounded = true;
    }

    public void StartAtPosition(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _player.position = new Vector3(position.x, hit.collider.bounds.max.y, 0);
        _currentForm.Activate();

        if (_currentForm.CanTeleport)
        {
            _currentForm.StartTeleportInAnimation();
            _isTeleporting = true;
        }
        else
        {
            _isReady = true;
            _currentForm.StartIdleAnimation();
        }
    }

    public void Update()
    {
        if (_isTeleporting)
            if (_currentForm.IsAnimationDone)
            {
                _isTeleporting = false;
                _isReady = true;
                _currentForm.StartIdleAnimation();
            }

        if (_isChangingFormPhaseOne && _currentForm.IsAnimationDone)
            ChangingFormPhaseTwo();

        if (_isChangingFormPhaseTwo && _currentForm.IsAnimationDone)
            FinishFormChange();

        if (!_isGrounded)
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
            VerticalMove();
        }

        CheckForGroung();
    }

    private void CheckForGroung()
    {
        //В случае если персонаж не совершает прыжок, делаем рейкаст для проверки касается ли он земли
        if (!_isJumping)
        {
            //Сначала делаем рейкаст по центру персонажа
            _raycastStart = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y);
            _raycastEnd = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
            _raycastDirection = _raycastEnd - _raycastStart;
            RaycastHit2D hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _collider.bounds.size.y, LayerMask.GetMask("Ground"));
            Debug.DrawLine(_raycastStart, _raycastEnd, Color.red);

            //Проверяем на успешность
            if (hit.collider == null)
            {
                //В случае неудачи на всякий случай проверяем касается ли условная "левая нога" земли
                _raycastStart = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
                _raycastEnd = new Vector2(_collider.bounds.min.x, _collider.bounds.min.y);
                _raycastDirection = _raycastEnd - _raycastStart;
                hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _collider.bounds.size.y, LayerMask.GetMask("Ground"));
                Debug.DrawLine(_raycastStart, _raycastEnd, Color.cyan);

                if (hit.collider == null)
                {
                    //После чего на всякий случай проверяем условную "правую ногу"
                    _raycastStart = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);
                    _raycastEnd = new Vector2(_collider.bounds.max.x, _collider.bounds.min.y);
                    _raycastDirection = _raycastEnd - _raycastStart;
                    hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _collider.bounds.size.y, LayerMask.GetMask("Ground"));
                    Debug.DrawLine(_raycastStart, _raycastEnd, Color.magenta);
                }
            }

            //Если какой-либо контакт есть и персонаж находится в падении, приземляем персонажа
            if (_isFalling && hit.collider)
            {
                Land();
                //_player.position.Change(y: hit.point.y);
                //_player.position = new Vector3(_player.position.x, hit.point.y, _player.position.z);
                _player.position = new Vector3(_player.position.x, hit.collider.bounds.max.y, _player.position.z);
            }

            //Если персонаж находился на земле и вдруг потерял контакт, начинаем падение
            if (_isGrounded && hit.collider == null)
            {
                Fall();
            }

        }
    }

    public void ChangeForm()
    {
        if (_isReady && _isGrounded && !_isChangingFormPhaseOne && !_isChangingFormPhaseTwo)
        {
            ResetState();
            _currentForm.StartHenshinPhaseOneAnimation();
            _isChangingFormPhaseOne = true;
            _isReady = false;
        }
    }

    private void ChangingFormPhaseTwo()
    {
        _isChangingFormPhaseOne = false;
        _isChangingFormPhaseTwo = true;

        _currentForm.Deactivate();
        if (_currentForm == _untransformedForm)
            _currentForm = _transformedForm;
        else
            _currentForm = _untransformedForm;
        _currentForm.Activate();

        _currentForm.StartHenshinPhaseTwoAnimation();
    }

    private void FinishFormChange()
    {
        _isChangingFormPhaseTwo = false;
        _isReady = true;
        _currentForm.StartIdleAnimation();
    }

    public void Move(float inputHor)
    {
        if (_isReady)
        {

            if (Mathf.Abs(inputHor) > _inputThreshold)
            {
                _player.localScale = (inputHor < 0 ? _leftScale : _rightScale);

                if (!CheckForWall())
                    _player.position += Vector3.right * (Time.deltaTime * _currentSpeed * (inputHor < 0 ? -1 : 1));
                

                if (!_isMoving)
                {
                    if (_isGrounded)
                    {
                        if (!_isCrouching)
                            _currentForm.StartRunAnimation();
                        else
                            _currentForm.StartCrawlAnimation();
                    }
                    _isMoving = true;
                }
            }
            else if (_isMoving)
            {
                _isMoving = false;
                if (_isGrounded)
                {
                    if (!_isCrouching)
                        _currentForm.StartIdleAnimation();
                    else
                        _currentForm.StartCrouchAnimation();
                }
            }
        }
    }

    private bool CheckForWall()
    {
        if (_player.localScale.x > 0)
        {
            _raycastStart = new Vector2(_collider.bounds.max.x, _collider.bounds.center.y + 0.5f);
            _raycastEnd = new Vector2(_collider.bounds.max.x + _collider.size.y, _collider.bounds.center.y + 0.5f);
            _raycastDirection = _raycastEnd - _raycastStart;
        }
        else
        {
            _raycastStart = new Vector2(_collider.bounds.min.x, _collider.bounds.center.y + 0.5f);
            _raycastEnd = new Vector2(_collider.bounds.min.x - _collider.size.y, _collider.bounds.center.y + 0.5f);
            _raycastDirection = _raycastEnd - _raycastStart;
        }

        RaycastHit2D hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _collider.bounds.size.y, LayerMask.GetMask("Wall"));
        Debug.DrawLine(_raycastStart, _raycastEnd, Color.green);

        if (hit.collider != null)
            return true;

        return false;
    }

    private void VerticalMove()
    {
        _player.position += Vector3.up * (Time.deltaTime * _verticalVelocity);
        if (_verticalVelocity < 0 && !_isFalling)
        {
            Fall();
        }
    }

    private void Fall()
    {
        _isGrounded = false;
        _isJumping = false;
        _isFalling = true;
        _currentForm.StartFallAnimation();
    }

    public void Crouch(float inputVer)
    {
        if (_isReady && _isGrounded && _currentForm.CanCrouch)
        {
            if ((Mathf.Abs(inputVer) > _inputThreshold) && inputVer < 0)
            {
                if (!_isCrouching)
                {
                    _isCrouching = true;
                    _currentSpeed = _defaultSpeed / 3;
                    if (!_isMoving)
                        _currentForm.StartCrouchAnimation();
                    else
                        _currentForm.StartCrawlAnimation();
                }
            }
            else if (_isCrouching)
            {
                _isCrouching = false;
                _currentSpeed = _defaultSpeed;
                if (!_isMoving)
                    _currentForm.StartIdleAnimation();
                else
                    _currentForm.StartRunAnimation();
            }
        }
    }

    private void Land()
    {
        _player.position.Change(y: _groundLevel);
        _verticalVelocity = 0f;
        _isGrounded = true;
        _isFalling = false;
        if (_isMoving)
            _currentForm.StartRunAnimation();
        else
            _currentForm.StartIdleAnimation();
    }

    public void Jump()
    {
        if (_isReady && _isGrounded)
        {
            _isGrounded = false;
            _isJumping = true;
            _verticalVelocity = _jumpForce;
            _currentForm.StartJumpAnimation();
            ResetState();
        }
    }

    private void ResetState()
    {
        _isMoving = false;
        _isCrouching = false;
        _currentSpeed = _defaultSpeed;
    }

    public PlayerPosition PlayerPosition => new PlayerPosition(_player, _spriteOffset);
}


public struct PlayerPosition
{
    Transform _playerTransform;
    float _offset;

    public PlayerPosition(Transform transform, float offset)
    {
        _playerTransform = transform;
        _offset = offset;
    }

    public Vector3 Position => new Vector3(_playerTransform.position.x, _playerTransform.position.y + _offset, _playerTransform.position.z);
}