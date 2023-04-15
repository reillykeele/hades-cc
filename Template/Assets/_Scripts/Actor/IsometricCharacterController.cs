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
    [SerializeField] private float _dashDistance = 4f;
    [SerializeField] private float _dashSpeed = 3f;
    
    [Header("Animation")]
    [Range(0,1f)]
    [SerializeField]
    private float _startAnimTime = 0.3f;
    [Range(0, 1f)]
    [SerializeField]
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
    public bool IsDashing = false;
    private Vector3 _dashGoal = Vector3.zero;
    private Vector3 _dashDir = Vector3.zero;

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
        
    }

    /// <summary>
    /// Projects the input movement onto the XZ plane. 
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateAdjustedMovement()
    {
        // Project movement onto the XZ plane
        var camForward = _camera.transform.forward;
		var camRight = _camera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        var adjustedMovement = camRight.normalized * MoveInput.x + camForward.normalized * MoveInput.y;

        adjustedMovement = MoveInput.sqrMagnitude == 0f ? 
            transform.forward * (adjustedMovement.magnitude + .01f) : 
            adjustedMovement;

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

        transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (adjustedMovement), _rotationSpeed);
        _controller.Move(adjustedMovement * Time.deltaTime * _moveSpeed);        
    }

    public bool ShouldDash => IsDashing == false && DashInput;

    public void StartDash()
    {
        var inputMagnitude = MoveInput.sqrMagnitude;
        var adjustedMovement = CalculateAdjustedMovement();

        IsDashing = true;
        _dashDir = inputMagnitude == 0f ? transform.forward : adjustedMovement.normalized;
        _dashGoal = transform.position + _dashDir * _dashDistance;

        transform.rotation = Quaternion.LookRotation (_dashDir);
    }

    public void DashMovement()
    {
        if (Vector3.Distance(transform.position, _dashGoal) < 0.5f)
        { 
            IsDashing = false;
            return;
        }

        _controller.Move(_dashDir * Time.deltaTime * _dashSpeed);
    }    

    public void ShowSword() => _swordTransform.Enable();
    public void HideSword() => _swordTransform.Disable();

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
