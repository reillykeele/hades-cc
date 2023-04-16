using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "SpecialAttackState", menuName = "State Machine/States/Special Attack State")]
    public class SpecialAttackStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new SpecialAttackState(stateMachine);
        }
    }

    public class SpecialAttackState : State
    {
        private IsometricCharacterController _playerController;

        private const float _minDuration = 0.75f;

        private float _timeSinceEntering = 0.0f;

        public SpecialAttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("SpecialAttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;

            _playerController.AllowMovement = false;
            _playerController._anim.SetTrigger("special");
            Debug.Log("SpecialAttackState Enter");
        }

        public override void Update()
        {           
            _timeSinceEntering += Time.deltaTime;

            if (_timeSinceEntering >= _minDuration) 
            {                
                _stateMachine.TransitionState<IdleState>();                
            }
        }

        public override void Exit()
        {

        }

    }
}
