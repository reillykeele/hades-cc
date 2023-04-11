using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
	[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IMenuActions
    {
        private GameInput _gameInput;

		#region Gameplay Events
		
        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction AttackEvent = delegate { };
        public event UnityAction AttackCanceledEvent = delegate { };

		#endregion

		#region Menu Events

		#endregion

		private void OnEnable()
		{
			if (_gameInput == null)
			{
				_gameInput = new GameInput();

				_gameInput.Gameplay.SetCallbacks(this);
				_gameInput.Menu.SetCallbacks(this);

				EnableGameplayInput();
			}

		}

		private void OnDisable()
		{
			DisableAllInput();
		}

		public void DisableAllInput()
		{
			_gameInput.Gameplay.Disable();
			_gameInput.Menu.Disable();
		}

        #region Gameplay Actions

		public void EnableGameplayInput()
		{
			_gameInput.Menu.Disable();
			_gameInput.Gameplay.Enable();
		}

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
		    {
			    case InputActionPhase.Performed:
				    AttackEvent.Invoke();
				    break;
			    case InputActionPhase.Canceled:
				    AttackCanceledEvent.Invoke();
				    break;
		    }
        }

		#endregion

		#region Menu Actions

		public void EnableMenuInput()
		{
			_gameInput.Gameplay.Disable();
			_gameInput.Menu.Enable();
		}

        public void OnNewaction(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}