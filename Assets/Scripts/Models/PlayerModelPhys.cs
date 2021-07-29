using UnityEngine;

public class PlayerModelPhys 
{
    #region Fields
    PlayerView _currentForm;
    PlayerView _transformedForm;
    PlayerView _untransformedForm;

    Rigidbody2D _player;
    Collider2D _collider;           
    Collider2D _colliderStand;

    private readonly ContactsPoller _contactsPoller;

    private PlayerHealth _health;
    private PlayerWeapon _weapon;

    private float _defaultSpeed = 200f;
    private float _currentSpeed;

    private const float _inputThreshold = 0.1f;
    private const float _fallThreshold = -1f;
    private float _jumpForce = 350.0f;

    private Vector3 _leftScale = new Vector3(-1, 1, 1);
    private Vector3 _rightScale = new Vector3(1, 1, 1);

    bool _isReady;
    bool _isMoving;
    bool _isGrounded;
    bool _isJumping;
    bool _isFalling;
    bool _isCrouching;
    bool _isTeleportingIn;
    bool _isTeleportingOut;
    bool _isChangingFormPhaseOne;
    bool _isChangingFormPhaseTwo;
    bool _isWallClinging;
    bool _isHurt;
    bool _isDead;
    bool _isAtFinish;
    bool _isShooting;

    float _shootingCD = 0.3f;
    float _currentShootingCD;

    Vector2 _raycastStart;
    Vector2 _raycastEnd;
    Vector2 _raycastDirection;
    private float _heightDifference;

    PhysicsMaterial2D _normalMaterial;      //Поскольку персонаж имеет две формы, одна из которых умеет цепляться к стенам,
    PhysicsMaterial2D _wallClingMaterial;   //а другая нет, используются два разных физический материала, которые подставляются
                                            //в коллайдер при смене формы
                                            //Сначала думал сделать коллайдеры с заранее заданным материалом частью вьюшек каждой
                                            //формы, но поскольку одна из форм умеет приседать, показалось целесообразней сделать
                                            //коллайдер независимый от форм, с изменяемым размером и сменным материалом.

    ContactPoint2D[] _contacts = new ContactPoint2D[16];
    Vector3 _destination;
    Vector3 _lastCheckPoint;

    public event System.Action End;
    #endregion

    public Transform Transform => _player.transform;

    public PlayerModelPhys(GameObject player, PlayerView untransformedForm, PlayerView transformedForm)
    {
        _transformedForm = transformedForm;
        _untransformedForm = untransformedForm;
        _currentForm = untransformedForm;

        _player = player.GetComponent<Rigidbody2D>();
        var colliders = player.GetComponents<BoxCollider2D>();
        _colliderStand = colliders[0];
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].offset.y > _colliderStand.offset.y)
                _colliderStand = colliders[i];
        }
        _collider = player.GetComponent<CompositeCollider2D>();

        _heightDifference = _collider.bounds.max.y + (_collider as CompositeCollider2D).edgeRadius - _player.transform.position.y;

        _untransformedForm.Transform.parent = player.transform;
        _untransformedForm.Transform.localPosition = Vector3.zero;
        _untransformedForm.Deactivate();

        _transformedForm.Transform.parent = player.transform;
        _transformedForm.Transform.localPosition = Vector3.zero;
        _transformedForm.Deactivate();

        _health = new PlayerHealth();
        _health.Damage += TakeHit;
        _health.Death += Die;

        _contactsPoller = new ContactsPoller(_collider);

        _normalMaterial = Resources.Load<PhysicsMaterial2D>("NormalMaterial");
        _wallClingMaterial = Resources.Load<PhysicsMaterial2D>("WallClingMaterial");
        SetPhysMaterial();

        _weapon = new PlayerWeapon(player.GetComponentInChildren<Grid>().transform);

        _currentSpeed = _defaultSpeed;
        _isGrounded = true;
    }

    private void SetPhysMaterial()
    {
        _collider.sharedMaterial = _currentForm.CanWallCling ? _wallClingMaterial : _normalMaterial;
    }

    public void StartAtPosition(Vector3 position)
    {
        _lastCheckPoint = position;

        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

        _player.transform.position = new Vector3(position.x, hit.collider.bounds.max.y, 0);
        
        _currentForm.Activate();

        if (_currentForm.CanTeleport)
        {
            _currentForm.StartTeleportInAnimation();
            _isTeleportingIn = true;
        }
        else
        {
            _isReady = true;
            _currentForm.StartIdleAnimation();
        }

        if (_isDead)
        {
            _health.Reset();
            _isDead = false;
        }
    }

    public void Update()
    {
        if (_isTeleportingIn)
            if (_currentForm.IsAnimationDone)
            {
                _isTeleportingIn = false;
                _isReady = true;
                _currentForm.StartIdleAnimation();
            }
        if (_isTeleportingOut)
            if (_currentForm.IsAnimationDone)
            {
                _isTeleportingOut = false;
                if (_isAtFinish)
                {
                    End?.Invoke();
                }
                else
                {
                    StartAtPosition(_destination);
                }
            }

        if (_isShooting)
        {
            if (_currentShootingCD > 0)
                _currentShootingCD -= Time.deltaTime;
            else
            {
                _isShooting = false;

                if (!_isGrounded)
                    _currentForm.StartFallAnimation();
                else if (_isMoving)
                    _currentForm.StartRunAnimation();
                else
                    _currentForm.StartIdleAnimation();
            }
        }

        if (_isChangingFormPhaseOne && _currentForm.IsAnimationDone)
            ChangingFormPhaseTwo();

        if (_isChangingFormPhaseTwo && _currentForm.IsAnimationDone)
            FinishFormChange();

        if (_isHurt && _currentForm.IsAnimationDone)
        {
            Recover();
        }

        VerticalCheck();
    }

    private bool CheckForCeiling()
    {
        _raycastStart = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y);
        _raycastEnd = new Vector2(_collider.bounds.center.x, _player.transform.position.y + _heightDifference);
        _raycastDirection = _raycastEnd - _raycastStart;
        RaycastHit2D hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
        Debug.DrawLine(_raycastStart, _raycastEnd, Color.red);

        if (hit.collider == null)
        {
            _raycastStart = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
            _raycastEnd = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y + _heightDifference);
            _raycastDirection = _raycastEnd - _raycastStart;
            hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
            Debug.DrawLine(_raycastStart, _raycastEnd, Color.blue);

            if (hit.collider == null)
            {
                _raycastStart = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);
                _raycastEnd = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y + _heightDifference);
                _raycastDirection = _raycastEnd - _raycastStart;
                hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
                Debug.DrawLine(_raycastStart, _raycastEnd, Color.green);
            }
        }

        return hit.collider != null;
    }

 
    #region Form Change
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

        SetPhysMaterial();
    }

    private void FinishFormChange()
    {
        _isChangingFormPhaseTwo = false;
        _isReady = true;
        _currentForm.StartIdleAnimation();
    }
    #endregion

    public void Move(float inputHor)
    {
        if (_isReady)
        {
            _contactsPoller.Update();

            var newVelocity = 0f;

            if (Mathf.Abs(inputHor) > _inputThreshold)
            {
                _player.transform.localScale = (inputHor < 0 ? _leftScale : _rightScale);

                if ((inputHor > 0 && !_contactsPoller.HasRightContacts && _isGrounded) ||
                    (inputHor < 0 && !_contactsPoller.HasLeftContacts && _isGrounded) ||
                    (inputHor != 0 && !_isGrounded))
                    newVelocity = Time.fixedDeltaTime * _currentSpeed * (inputHor < 0 ? -1 : 1);

                if (!_isMoving && _isGrounded)
                {
                    if (!_isCrouching)
                        _currentForm.StartRunAnimation();
                    else
                        _currentForm.StartCrawlAnimation();
                }

                if (_currentForm.CanWallCling && !_isGrounded &&
                    ((inputHor > 0 && _contactsPoller.HasRightContacts) ||
                    (inputHor < 0 && _contactsPoller.HasLeftContacts))
                    && !_isWallClinging)
                {
                    StartgWallCling();
                }

                    _isMoving = true;
            }
            else
            {
                if (_currentForm.CanWallCling && _isWallClinging)
                    StopWallCling();

                if (_isMoving && _isGrounded)
                {
                    if (!_isCrouching)
                        _currentForm.StartIdleAnimation();
                    else
                        _currentForm.StartCrouchAnimation();
                }

                _isMoving = false;
            }

            _player.velocity = _player.velocity.Change(x: newVelocity + (_contactsPoller.IsGrounded ? _contactsPoller.GroundVelocity.x : 0));
        }
    }

    private void VerticalCheck()
    {
        if (_isReady)
        {
            if (_player.velocity.y < _fallThreshold && ((!_isFalling) ||
                (_isGrounded && !_contactsPoller.IsGrounded)))
            {
                Fall();
            }

            if (_isFalling && _contactsPoller.IsGrounded)
            {
                Land();
            }
        }
    }

    private void StartgWallCling()
    {
        _isWallClinging = true;
        _currentForm.StartWallClingAnimation();
    }

    private void StopWallCling()
    {
        _isWallClinging = false;
        if (_isGrounded)
            Land();
        else
            Fall();
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
                    _colliderStand.enabled = false;
                    _currentSpeed = _defaultSpeed / 3;
                    if (!_isMoving)
                        _currentForm.StartCrouchAnimation();
                    else
                        _currentForm.StartCrawlAnimation();
                }
            }
            else if (_isCrouching && !CheckForCeiling())
            {
                _isCrouching = false;
                _colliderStand.enabled = true;
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
        _isGrounded = true;
        _isFalling = false;
        if (_isMoving)
            _currentForm.StartRunAnimation();
        else
            _currentForm.StartIdleAnimation();
    }

    public void Jump()
    {
        if (_isReady)
        {
            if (_contactsPoller.IsGrounded)
            {
                _isGrounded = false;
                _isJumping = true;
                _player.AddForce(Vector3.up * _jumpForce);
                _currentForm.StartJumpAnimation();
                ResetState();
            }
            else if (_currentForm.CanWallCling && (_isWallClinging || _contactsPoller.HasLeftContacts || _contactsPoller.HasRightContacts))
            {
                _isWallClinging = false;
                _player.AddForce((Vector3.up * _jumpForce) + (Vector3.right * _jumpForce * 3 * -_player.transform.localScale.x));
                _currentForm.StartJumpAnimation();
            }
        }
    }

    private void ResetState()
    {
        _isMoving = false;
        _isCrouching = false;
        _currentSpeed = _defaultSpeed;
        _player.velocity = _player.velocity.Change(x: 0.0f, y: 0.0f);
    }

    public Transform FindInteractableTeleporter()
    {
        if (_isReady && _currentForm.CanTeleport)
        {
            if (_contactsPoller.IsGrounded)
            {
                System.Array.Clear(_contacts, 0, _contacts.Length);
                _player.GetContacts(_contacts);
                for (int i = 0; i < _contacts.Length; i++)
                    if (_contacts[i].collider != null && _contacts[i].collider.CompareTag("Teleporter"))
                        return _contacts[i].collider.transform;
            }
        }
        return null;
    }

    public void TeleportToPosition(Vector3 position)
    {
        _destination = position;
        ResetState();
        _isReady = false;
        _isTeleportingOut = true;
        _currentForm.StartTeleportOutAnimation();
    }

    private void TakeHit()
    {
        if (_isReady)
        {
            _isHurt = true;
            _isReady = false;
            _isShooting = false;
            if (_currentForm.CanCrouch && _isCrouching)
            {
                _currentForm.StartHurtCrouchAnimation();
                _player.velocity = _player.velocity.Change(x: 0.0f);
                _isMoving = false;
            }
            else
            {
                _currentForm.StartHurtAnimation();
                ResetState();
            }
        }
    }

    private void Die()
    {
        if (_isReady)
        {
            _isReady = false;
            _isDead = true;
            ResetState();
            StartAtPosition(_lastCheckPoint);
        }
    }

    private void Recover()
    {
        _isReady = true;
        _isHurt = false;
        if (_isCrouching)
            _currentForm.StartCrouchAnimation();
        else if (!_isGrounded)
            _currentForm.StartFallAnimation();
        else
            _currentForm.StartIdleAnimation();
    }

    public void Shoot()
    {
        if (_isReady && _currentForm.CanShoot && !_isShooting && !_isWallClinging)
        {
            if (!_isGrounded)
                _currentForm.StartShootJumpAnimation();
            else if(_isMoving)
                _currentForm.StartShootRunAnimation();
            else
                _currentForm.StartShootStandAnimation();

            _weapon.Shoot(_player.transform.localScale.x);

            _isShooting = true;
            _currentShootingCD = _shootingCD;
        }
    }

    public void RegisterHazards(HazardController hazardController, CannonController cannonController, AIController AIController)
    {
        hazardController.RegisterTarget(_health);
        cannonController.RegisterTarget(_health);
        AIController.RegisterTarget(_health);
    }

    public void Finish()
    {
        ResetState();
        _isReady = false;
        _isTeleportingOut = true;
        _currentForm.StartTeleportOutAnimation();
        _isAtFinish = true;
    }
}