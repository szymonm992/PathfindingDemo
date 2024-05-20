using PathfindingDemo.GridManagement;
using PathfindingDemo.Player.Input;
using UnityEngine;

namespace PathfindingDemo.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private float cameraMovementSpeed = 10f;

        private int gridWidth;
        private int gridHeight;

        private void Awake()
        {
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
        }

        private void OnDestroy()
        {
            gridManager.GridSizeUpdateEvent -= OnGridSizeUpdate;
        }

        private void Update()
        {
            CameraMovement();
        }

        private void OnGridSizeUpdate(int gridWidth, int gridHeight)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Vector3 gridCenterPosition = new(gridWidth * 0.5f, 0f, gridHeight * 0.5f);

            float angle = 90f - mainCamera.transform.eulerAngles.x;
            float offsetY = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));
            float offsetZ = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));

            gridCenterPosition += new Vector3(0f, offsetY, -offsetZ);
            mainCamera.transform.position = gridCenterPosition;

            float gridSize = Mathf.Max(gridWidth, gridHeight);
            mainCamera.orthographicSize = (gridSize * 0.5f);
        }

        private void CameraMovement()
        {
            Vector3 movementVector = new (inputManager.SignedHorizontal, 0, inputManager.SignedVertical);
            mainCamera.transform.Translate(cameraMovementSpeed * Time.deltaTime * movementVector, Space.World);
        }
    }
}
