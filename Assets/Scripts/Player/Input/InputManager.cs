using PathfindingDemo.Providers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PathfindingDemo.Player.Input
{
    public class InputManager : MonoBehaviour
    {
        public delegate void InputActionDelegate(InputAction.CallbackContext context);
        
        public event InputActionDelegate LeftMouseButtonClickEvent;
        public event InputActionDelegate RightMouseButtonClickEvent;
        public event InputActionDelegate ScrollMouseEvent;

        public float Horizontal => moveInput.x;
        public float Vertical => moveInput.y;
        public float SignedHorizontal => GetSignedValue(Horizontal);
        public float SignedVertical => GetSignedValue(Vertical);

        private PlayerControls playerControls;
        private Vector2 moveInput;

        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Start()
        {
            playerControls.Player.LeftMouseButtonDown.performed += OnLeftMouseButtonDown;
            playerControls.Player.RightMouseButtonDown.performed += OnRightMouseButtonDown;
            playerControls.Player.ScrollMouse.performed += OnScrollMouse;
        }

        private void OnDestroy()
        {
            playerControls.Player.LeftMouseButtonDown.performed -= OnLeftMouseButtonDown;
            playerControls.Player.RightMouseButtonDown.performed -= OnRightMouseButtonDown;
            playerControls.Player.ScrollMouse.performed -= OnScrollMouse;
        }

        private void Update()
        {
            moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        }

        private void OnScrollMouse(InputAction.CallbackContext context)
        {
            ScrollMouseEvent?.Invoke(context);
        }

        private void OnLeftMouseButtonDown(InputAction.CallbackContext context)
        {
            LeftMouseButtonClickEvent?.Invoke(context);   
        }
        
        private void OnRightMouseButtonDown(InputAction.CallbackContext context)
        {
            RightMouseButtonClickEvent?.Invoke(context);   
        }

        private float GetSignedValue(float value)
        {
            if (value != 0)
            {
                return Mathf.Sign(value);
            }

            return 0;
        }
    }
}
