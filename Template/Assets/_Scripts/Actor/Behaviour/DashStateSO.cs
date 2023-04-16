using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "DashState", menuName = "State Machine/States/Dash State")]
    public class DashStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new DashState(stateMachine);
        }
    }

    public class DashState : State
    {
        private IsometricCharacterController _playerController;

        public DashState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("DashState Awake");
        }
        
        public override void Enter()
        {
            _playerController.StartDash();
            _playerController.StartDashParticleSystem();
        }

        public override void Update()
        {           
            _playerController.DashMovement();

            if (_playerController.IsDashing == false)
            {
                _stateMachine.TransitionState<IdleState>();
                return;
            }
        }

        public override void Exit()
        {
            _playerController.StopDashParticleSystem();
        }
    }
}
