using UnityEngine;
using UnityEngine.InputSystem;

namespace PathfindingDemo.Player.Input
{
    public class InputManager : MonoBehaviour
    {
        public delegate void LeftMouseButtonClickDelegate(InputAction.CallbackContext context);

        public event LeftMouseButtonClickDelegate LeftMouseButtonClickEvent;

        private PlayerControls playerControls;

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
            playerControls.Player.Click.performed += OnLeftMouseButtonClick;
        }

        private void OnLeftMouseButtonClick(InputAction.CallbackContext context)
        {
            LeftMouseButtonClickEvent?.Invoke(context);   
        }
    }
}
