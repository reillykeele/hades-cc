using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "Melee1AttackState", menuName = "State Machine/States/Melee 1 Attack State")]
    public class Melee1AttackStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new Melee1AttackState(stateMachine);
        }
    }

    public class Melee1AttackState : State
    {
        private IsometricCharacterController _playerController;

        // 
        private const float _minDuration = 0.4f;
        private const float _comboEarlyExit = 0.35f;
        private const float _comboDuration = 0.45f;
        private const float _movementDuration = 0.4f;
        private const float _movementMagnitude = 6f;

        private float _timeSinceEntering = 0.0f;
        private bool _shouldCombo = false;
        private Vector3 _movementDirection;

        public Melee1AttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee1AttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;
            _shouldCombo = false;
            _movementDirection = _playerController.CalculateInputDirection();

            _playerController.AllowMovement = false;
            _playerController.SetLookRotation(_movementDirection);
            _playerController.ShowSword();
            _playerController._anim.SetTrigger("melee1");
            Debug.Log("Melee1AttackState Enter");
        }

        public override void Update()
        {
            // if we attack during, we want to combo after 
            if (_playerController.AttackInput)
            {
                _playerController.AttackInput = false;
                _shouldCombo = true;
            }

            _timeSinceEntering += Time.deltaTime;

            if (_timeSinceEntering < _movementDuration)
            {
                var force = Vector3.Lerp(_movementDirection, Vector3.zero, _timeSinceEntering / _minDuration);
                _playerController.AddForce(force * _movementMagnitude);
            }

            if (_timeSinceEntering >= _comboEarlyExit && _shouldCombo)
            {
                _stateMachine.TransitionState<Melee2AttackState>();
                return;
            }
                
            if (_timeSinceEntering >= _minDuration) 
            {
                // if we are no longer animation locked
                if (_playerController.SpecialInput)
                {
                    _playerController.AttackInput = false;
                    _stateMachine.TransitionState<SpecialAttackState>();
                    return;
                }

                if (_playerController.ShouldDash)
                {
                    _playerController.IsDashing = false;
                    _stateMachine.TransitionState<DashState>();
                    return;
                }

                // combo broken
                if (_timeSinceEntering >= _comboDuration)
                {
                    _playerController.HideSword();
                    _stateMachine.TransitionState<IdleState>();
                    return;
                }

                _playerController.PlayerMovement();
            }
        }

        public override void Exit()
        {
        }

    }
}
