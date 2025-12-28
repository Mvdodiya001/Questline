using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input Readout")]
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool SprintInput { get; private set; }
        public bool JumpTriggered { get; private set; }

        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();

                _inputActions.Player.Move.performed += i => MoveInput = i.ReadValue<Vector2>();
                _inputActions.Player.Move.canceled += i => MoveInput = Vector2.zero;

                _inputActions.Player.Look.performed += i => LookInput = i.ReadValue<Vector2>();
                _inputActions.Player.Look.canceled += i => LookInput = Vector2.zero;

                _inputActions.Player.Sprint.performed += i => SprintInput = true;
                _inputActions.Player.Sprint.canceled += i => SprintInput = false;

                _inputActions.Player.Jump.performed += i => JumpTriggered = true;
                _inputActions.Player.Jump.canceled += i => JumpTriggered = false;
            }

            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            if (_inputActions != null)
            {
                _inputActions.Player.Disable();
            }
        }

        public void UseJumpInput()
        {
            JumpTriggered = false;
        }
    }
}