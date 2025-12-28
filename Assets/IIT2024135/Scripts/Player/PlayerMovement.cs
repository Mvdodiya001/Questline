using UnityEngine;

namespace PlayerSystem
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravityValue = -15.0f;

        [Header("Camera Reference")]
        [Tooltip("Assign the Main Camera here. If empty, it attempts to find MainCamera automatically.")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController _controller;
        private PlayerInputHandler _inputHandler;
        
        private Vector3 _playerVelocity;
        private bool _isGrounded;
        private float _rotationVelocity;

        private void Awake()
        {
            if (!TryGetComponent(out _controller))
                Debug.LogError("CharacterController component is missing on the Player object.");

            if (!TryGetComponent(out _inputHandler))
                Debug.LogError("PlayerInputHandler script is missing on the Player object.");
        }

        private void Start()
        {
            if (cameraTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    cameraTransform = mainCam.transform;
                }
                else
                {
                    Debug.LogWarning("No Main Camera found. Character movement will not be relative to camera view.");
                }
            }
        }

        private void Update()
        {
            HandleGravityAndJump();
            HandleMovement();
        }

        private void HandleGravityAndJump()
        {
            _isGrounded = _controller.isGrounded;

            if (_isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = -2f; 
            }

            if (_inputHandler.JumpTriggered && _isGrounded)
            {
                _playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
                
                _inputHandler.UseJumpInput();
            }

            _playerVelocity.y += gravityValue * Time.deltaTime;
            
            _controller.Move(_playerVelocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            Vector2 input = _inputHandler.MoveInput;
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                
                if (cameraTransform != null)
                {
                    targetAngle += cameraTransform.eulerAngles.y;
                }

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                float currentSpeed = _inputHandler.SprintInput ? sprintSpeed : walkSpeed;

                _controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            }
        }
    }
}