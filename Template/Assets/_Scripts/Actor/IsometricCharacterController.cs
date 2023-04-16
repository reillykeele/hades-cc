using Input;
using System;
using UnityEngine;
using Util.Helpers;

[RequireComponent(typeof(CharacterController))]
public class IsometricCharacterController : MonoBehaviour
{       
    [SerializeField] private InputReader _input = default;

    [Header("Movement")]
    [SerializeField] private float _rotationSpeed = 0.1f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private int _numDashes = 1;
    [SerializeField] private float _dashDistance = 4f;
    [SerializeField] private float _dashSpeed = 3f;
    [SerializeField] private float _dashRechargeTime = 1.0f;

    [Header("Animation")]
    [Range(0,1f)] [SerializeField]
    private float _startAnimTime = 0.3f;
    [Range(0, 1f)] [SerializeField]
    private float _stopAnimTime = 0.15f;

    [SerializeField] private Transform _swordTransform;
    
    // Components
    public Animator _anim;
    private CharacterController _controller;
    private Camera _camera;
    
    // Inputs
    public Vector2 MoveInput;
    public bool AttackInput = false;
    public bool SpecialInput = false;
    public bool DashInput = false;

    // 
    public bool AllowMovement = true;
    public int Dashes;
    public bool IsDashing = false;
    public float TimeSinceDash = 0f;
    public float CurrDashDistance = 0f;
    public Vector3 CurrDashDir = Vector3.zero;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();    
    }

    void Start()
    {
        _camera = Camera.main;
    }

    void OnEnable()
    {
        _input.MoveEvent += OnMove;
        _input.AttackEvent += OnAttackStarted;
        _input.AttackCancelledEvent += OnAttackCancelled;
        _input.SpecialEvent += OnSpecialStarted;
        _input.SpecialCancelledEvent += OnSpecialCancelled;
        _input.DashEvent += OnDashStarted;
        _input.DashCancelledEvent += OnDashCancelled;
    }

    void OnDisable()
    {
        _input.MoveEvent -= OnMove;
        _input.AttackEvent -= OnAttackStarted;
        _input.AttackCancelledEvent -= OnAttackCancelled;
        _input.SpecialEvent -= OnSpecialStarted;
        _input.SpecialCancelledEvent -= OnSpecialCancelled;
        _input.DashEvent -= OnDashStarted;
        _input.DashCancelledEvent -= OnDashCancelled;
    }

    void Update()
    {
        if (Dashes < _numDashes)
        {
            TimeSinceDash += Time.deltaTime;
            if (TimeSinceDash >= _dashRechargeTime)
            {
                Dashes++;
            }
        }
    }

    /// <summary>
    /// Projects the input movement direction onto the XZ plane. 
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateInputDirection()
    {
        // Project movement onto the XZ plane
        var camForward = _camera.transform.forward;
        var camRight = _camera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        var inputDirection = camRight.normalized * MoveInput.x + camForward.normalized * MoveInput.y;

        return inputDirection.normalized;
    }

    /// <summary>
    /// Projects the adjusted input movement onto the XZ plane. 
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateAdjustedMovement()
    {
        var inputDirection = CalculateInputDirection();

        var adjustedMovement = inputDirection = MoveInput.sqrMagnitude == 0f ? 
            transform.forward * (inputDirection.magnitude + .01f) :
            inputDirection;

        return adjustedMovement;
    }

    public void PlayerMovement()
    {   
        var adjustedMovement = CalculateAdjustedMovement();

        var inputMagnitude = MoveInput.sqrMagnitude;       
        if (inputMagnitude == 0f)            
            _anim.SetFloat ("Blend", inputMagnitude, _stopAnimTime, Time.deltaTime);           
        else           
            _anim.SetFloat ("Blend", inputMagnitude, _startAnimTime, Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(adjustedMovement), _rotationSpeed);
        _controller.Move(adjustedMovement * Time.deltaTime * _moveSpeed);
    }

    /// <summary>
    /// Sets the player's rotation to the <c>lookDirection</c>. Will not rotate the player if
    /// <c>lookRotation</c> is <c>Vector3.Zero</c>.
    /// </summary>
    /// <param name="lookDirection"></param>
    public void SetLookRotation(Vector3 lookDirection)
    {
        if (lookDirection.sqrMagnitude != 0)
            transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public bool ShouldDash => Dashes > 0 && IsDashing == false && DashInput;

    public void StartDash()
    {
        var inputMagnitude = MoveInput.sqrMagnitude;
        var adjustedMovement = CalculateAdjustedMovement();

        IsDashing = true;
        Dashes--;
        TimeSinceDash = 0.0f;
        CurrDashDir = inputMagnitude == 0f ? transform.forward : adjustedMovement.normalized;
        CurrDashDistance= 0.0f;

        transform.rotation = Quaternion.LookRotation (CurrDashDir);
    }

    public void DashMovement()
    {
        if (CurrDashDistance >= _dashDistance)
        { 
            IsDashing = false;
            return;
        }

        var dist = _dashSpeed * Time.deltaTime;
        CurrDashDistance += dist;
        
        _controller.Move(CurrDashDir * dist);
    }

    public void AddForce(Vector3 force)
    {
        _controller.Move(force * Time.deltaTime);
    }

    public void ShowSword() => _swordTransform.Enable();
    public void HideSword() => _swordTransform.Enable();

    #region Input Handling

    private void OnMove(Vector2 move) => MoveInput = move;

    private void OnAttackStarted() => AttackInput = true;
    private void OnAttackCancelled() => AttackInput = false;

    private void OnSpecialStarted() => SpecialInput = true;
    private void OnSpecialCancelled() => SpecialInput = false;

    private void OnDashStarted() => DashInput = true;
    private void OnDashCancelled() => DashInput = false;

    #endregion

}
