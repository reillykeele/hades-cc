using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "State Machine/States/Idle State")]
    public class IdleStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new IdleState(stateMachine);
        }
    }

    public class IdleState : State
    {
        private IsometricCharacterController _playerController;

        public IdleState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("IdleState Awake");
        }
        
        public override void Enter()
        {
            _playerController.AllowMovement = true;
            _playerController.HideSword();
        }

        public override void Update()
        {
            if (_playerController.AttackInput)
            {
                _playerController.AttackInput = false;
                _stateMachine.TransitionState<Melee1AttackState>();
                return;
            }

            if (_playerController.SpecialInput)
            {
                _playerController.SpecialInput = false;
                _stateMachine.TransitionState<SpecialAttackState>();
                return;
            }

            if (_playerController.ShouldDash)
            {
                _playerController.DashInput = false;
                _stateMachine.TransitionState<DashState>();
                return;
            }

            _playerController.PlayerMovement();
        }
    }
}
