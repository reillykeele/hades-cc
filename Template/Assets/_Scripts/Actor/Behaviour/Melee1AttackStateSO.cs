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
        private const float _minDuration = 0.35f;
        private const float _comboDuration = 2.0f;
        
        private float _timeSinceEntering = 0.0f;
        private bool _shouldCombo = false;

        public Melee1AttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee1AttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;
            _shouldCombo = false;

            _playerController.AllowMovement = false;
            _playerController._anim.SetTrigger("melee1");
            Debug.Log("Melee1AttackState Enter");
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
                _playerController.AllowMovement = true;
                if (_shouldCombo)
                {
                    _stateMachine.TransitionState<Melee2AttackState>();
                }
                else if (_playerController.SpecialInput)
                {
                    _playerController.AttackInput = false;
                    _stateMachine.TransitionState<SpecialAttackState>();
                }
                else if (_timeSinceEntering >= _comboDuration)
                {
                    // Combo broken
                    _stateMachine.TransitionState<IdleState>();
                }
            }
        }

        public override void Exit()
        {
            // ?
        }

    }
}
