using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "Melee2AttackState", menuName = "State Machine/States/Melee 2 Attack State")]
    public class Melee2AttackStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new Melee2AttackState(stateMachine);
        }
    }

    public class Melee2AttackState : State
    {
        private IsometricCharacterController _playerController;

        // 
        private const float _minDuration = 0.5f;
        private const float _comboDuration = 0.75f;
        
        private float _timeSinceEntering = 0.0f;
        private bool _shouldCombo = false;

        public Melee2AttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee2AttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;
            _shouldCombo = false;

            _playerController.AllowMovement = false;
            _playerController.ShowSword();
            _playerController._anim.SetTrigger("melee2");
            Debug.Log("Melee2AttackState Enter");
        }

        public override void Update()
        {
            // if we attack during 
            if (_playerController.AttackInput)
            {
                _playerController.AttackInput = false;
                _shouldCombo = true;
            }

            _timeSinceEntering += Time.deltaTime;

            // if we are no longer animation locked
            if (_timeSinceEntering >= _minDuration) 
            {               
                if (_shouldCombo)
                {
                    _stateMachine.TransitionState<Melee3AttackState>();
                    return;
                }
                else if (_playerController.SpecialInput)
                {
                    _playerController.AttackInput = false;
                    _stateMachine.TransitionState<SpecialAttackState>();
                    return;
                }
                else if (_playerController.IsDashing)
                {
                    _playerController.IsDashing = false;
                    _stateMachine.TransitionState<DashState>();
                    return;
                }
                else if (_timeSinceEntering >= _comboDuration)
                {
                    // Combo broken
                    _playerController.HideSword();
                    _stateMachine.TransitionState<IdleState>();
                    return;
                }

                //_playerController.HideSword();
                _playerController.PlayerMovement();
            }
        }

        public override void Exit()
        {
            //_playerController.HideSword();
        }

    }
}
