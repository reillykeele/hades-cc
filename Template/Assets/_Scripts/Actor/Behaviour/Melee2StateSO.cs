using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "Melee2State", menuName = "State Machine/States/Melee 2 State")]
    public class Melee2StateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new Melee2State(stateMachine);
        }
    }

    public class Melee2State : State
    {
        private IsometricCharacterController _playerController;

        private const float _duration = 0.75f;
        private float _timeSinceEntering = 0.0f;

        public Melee2State(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("Melee2State Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;

            _playerController._anim.SetTrigger("melee2");
            Debug.Log("Melee2State Enter");
        }

        public override void Update()
        {           
            _timeSinceEntering += Time.deltaTime;

            if (_timeSinceEntering >= _duration ) 
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
