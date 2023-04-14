using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "Melee3AttackState", menuName = "State Machine/States/Melee 3 Attack State")]
    public class Melee3AttackStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new Melee3AttackState(stateMachine);
        }
    }

    public class Melee3AttackState : State
    {
        private IsometricCharacterController _playerController;

        private const float _minDuration = 1.25f;

        private float _timeSinceEntering = 0.0f;

        public Melee3AttackState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee3AttackState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;

            _playerController.AllowMovement = false;
            _playerController.ShowSword();
            _playerController._anim.SetTrigger("melee3");
            Debug.Log("Melee3AttackState Enter");
        }

        public override void Update()
        {           
            _timeSinceEntering += Time.deltaTime;

            if (_timeSinceEntering >= _minDuration) 
            {               
                _stateMachine.TransitionState<IdleState>();         
                return;
            }
        }

        public override void Exit()
        {
            _playerController.HideSword();
        }

    }
}
