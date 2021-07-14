using UnityEngine;

public class PlayerModelPhys 
{
    PlayerView _currentForm;
    PlayerView _transformedForm;
    PlayerView _untransformedForm;

    Rigidbody2D _player;
    Collider2D _collider;           //Имеем композитный коллайдер, представляющий полную высоту персонажа
    Collider2D _colliderStand;      //Верхняя часть композитного коллайдера, отключается для получения высоты сидящего персонажа

    private readonly ContactsPoller _contactsPoller;

    private float _defaultSpeed = 200f;
    private float _currentSpeed;

    private const float _inputThreshold = 0.1f;
    private float _jumpForce = 350.0f;

    private const float _spriteOffset = 0f;

    private Vector3 _leftScale = new Vector3(-1, 1, 1);
    private Vector3 _rightScale = new Vector3(1, 1, 1);

    bool _isReady;
    bool _isMoving;
    bool _isGrounded;
    bool _isJumping;
    bool _isFalling;
    bool _isCrouching;
    bool _isTeleporting;
    bool _isChangingFormPhaseOne;
    bool _isChangingFormPhaseTwo;
    bool _isWallClinging;

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

        _contactsPoller = new ContactsPoller(_collider);

        _normalMaterial = Resources.Load<PhysicsMaterial2D>("NormalMaterial");
        _wallClingMaterial = Resources.Load<PhysicsMaterial2D>("WallClingMaterial");
        SetPhysMaterial();

        _currentSpeed = _defaultSpeed;
        _isGrounded = true;
    }

    private void SetPhysMaterial()
    {
        _collider.sharedMaterial = _currentForm.CanWallCling ? _wallClingMaterial : _normalMaterial;
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

        VerticalCheck();
    }

    //Проверка на наличие потолка над персонажем, чтобы определить, хватает ли ему высоты встать в полный рост из приседания
    //Делается рейкастом, а не через проверку коллизий сверху потом, что проверка на коллизии будет работать только
    //с потолком филигранно выверенной высоты, строго по коллайдеру.
    //Проверка рейкастом позволяет персонажу заползать в туннели произвольной высоты, ниже его роста.
    private bool CheckForCeiling()
    {
        //Проверяем по тому же принципу, по которому работала проверка земли у нефизического персонажа
        //Сначала проверяем центр
        _raycastStart = new Vector2(_collider.bounds.center.x, _collider.bounds.max.y);
        _raycastEnd = new Vector2(_collider.bounds.center.x, _player.transform.position.y + _heightDifference);
        _raycastDirection = _raycastEnd - _raycastStart;
        RaycastHit2D hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
        Debug.DrawLine(_raycastStart, _raycastEnd, Color.red);

        if (hit.collider == null)
        {
            //Если центр свободен, на всякий случай проверяем левый край
            _raycastStart = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
            _raycastEnd = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y + _heightDifference);
            _raycastDirection = _raycastEnd - _raycastStart;
            hit = Physics2D.Raycast(_raycastStart, _raycastDirection, _heightDifference, LayerMask.GetMask("Ground"));
            Debug.DrawLine(_raycastStart, _raycastEnd, Color.blue);

            if (hit.collider == null)
            {
                //И правый
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

            _player.velocity = _player.velocity.Change(x: newVelocity);
        }
    }

    private void VerticalCheck()
    {
        if (_isReady)
        {
            if (_player.velocity.y < 0 && !_isFalling)
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
        _player.velocity = _player.velocity.Change(x: 0.0f);
    }

    public Transform Transform => _player.transform;
}