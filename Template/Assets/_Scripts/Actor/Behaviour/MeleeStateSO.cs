using UnityEngine;
using Util.FSM;

namespace Actor.Behaviour
{
    [CreateAssetMenu(fileName = "MeleeState", menuName = "State Machine/States/Melee State")]
    public class MeleeStateSO : StateSO
    {
        public override State Initialize(StateMachine stateMachine)
        {
            return new MeleeState(stateMachine);
        }
    }

    public class MeleeState : State
    {
        private IsometricCharacterController _playerController;

        private const float _duration = 0.75f;
        private float _timeSinceEntering = 0.0f;
        private bool _shouldCombo = false;

        public MeleeState(StateMachine stateMachine) : base(stateMachine) 
        {
            _playerController = stateMachine.GetComponent<IsometricCharacterController>();

            Debug.Log("MeleeState Awake");
        }

        public override void Enter()
        {
            _timeSinceEntering = 0.0f;
            _shouldCombo = false;

            _playerController._anim.SetTrigger("melee1");
            Debug.Log("MeleeState Enter");
        }

        public override void Update()
        {
            if (_playerController.AttackInput)
            {
                _playerController.AttackInput = false;
                _shouldCombo = true;
            }

            _timeSinceEntering += Time.deltaTime;

            if (_timeSinceEntering >= _duration ) 
            {
                if (_shouldCombo)
                {
                    _stateMachine.TransitionState<Melee2State>();
                }
                else
                {
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
