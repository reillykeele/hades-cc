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

        private const float _minDuration = 0.5f;

        private float _timeSinceEntering = 0.0f;

        public Melee2AttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee2AttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;

            _playerController.AllowMovement = false;
            _playerController._anim.SetTrigger("melee2");
            Debug.Log("Melee2AttackState Enter");
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
            // ?
        }

    }
}
