using Input;
using System;
using Unity.VisualScripting;
using UnityEngine;

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
    
    // Components
    private Animator _anim;
    private CharacterController _controller;
    private Camera _camera;
    
    // Inputs
    private Vector2 _moveInput;
    private bool _attackInput = false;
    private bool _dashInput = false;

    // 
    private bool _isDashing = false;
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
        _input.DashEvent += OnDashStarted;
        _input.DashCancelledEvent += OnDashCancelled;
    }

    void OnDisable()
    {
        _input.MoveEvent -= OnMove;
        _input.AttackEvent -= OnAttackStarted;
        _input.AttackCancelledEvent -= OnAttackCancelled;
        _input.DashEvent -= OnDashStarted;
        _input.DashCancelledEvent -= OnDashCancelled;
    }

    void Update()
    {
        // Project movement onto the XZ plane
        var camForward = _camera.transform.forward;
		var camRight = _camera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        var adjustedMovement = camRight.normalized * _moveInput.x + camForward.normalized * _moveInput.y;
        
        // Fix to avoid getting a Vector3.zero vector, which would result in the player turning to x:0, z:0
        var inputMagnitude = _moveInput.sqrMagnitude;
        if (inputMagnitude == 0f)
            {
                adjustedMovement = transform.forward * (adjustedMovement.magnitude + .01f);
                _anim.SetFloat ("Blend", inputMagnitude, _stopAnimTime, Time.deltaTime);
            }
            else
            {
                _anim.SetFloat ("Blend", inputMagnitude, _startAnimTime, Time.deltaTime);
            }

        if (_isDashing == false && _dashInput)
        {
            // Start dashing
            _isDashing = true;
            _dashInput = false; // consume the input
            _dashDir = inputMagnitude == 0f ? transform.forward : adjustedMovement.normalized;
            _dashGoal = transform.position + _dashDir * _dashDistance;

            transform.rotation = Quaternion.LookRotation (_dashDir);
        }
        else if (_isDashing)
        {
            // Dash move
            
            if (Vector3.Distance(transform.position, _dashGoal) < 0.5f)
                _isDashing = false;

            _controller.Move(_dashDir * Time.deltaTime * _dashSpeed);
        }
        else 
        {                        
            if (inputMagnitude == 0f)            
                _anim.SetFloat ("Blend", inputMagnitude, _stopAnimTime, Time.deltaTime);           
            else           
                _anim.SetFloat ("Blend", inputMagnitude, _startAnimTime, Time.deltaTime);           

            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (adjustedMovement), _rotationSpeed);
            _controller.Move(adjustedMovement * Time.deltaTime * _moveSpeed);
        }

    }

    #region Input Handling

    private void OnMove(Vector2 move) => _moveInput = move;

    private void OnAttackStarted() => _attackInput = true;
    private void OnAttackCancelled() => _attackInput = false;

    private void OnDashStarted() => _dashInput = true;
    private void OnDashCancelled() => _dashInput = false;

    #endregion

}
