using PathfindingDemo.GridManagement;
using UnityEngine;

namespace PathfindingDemo.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private GridManager gridManager;

        private CameraMode currentCameraMode;
        private int gridWidth;
        private int gridHeight;

        private void Awake()
        {
            SetCameraMode(CameraMode.Hovering);
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
        }

        private void OnDestroy()
        {
            gridManager.GridSizeUpdateEvent -= OnGridSizeUpdate;
        }

        private void SetCameraMode(CameraMode cameraMode)
        {
            currentCameraMode = cameraMode;
             
            switch (cameraMode)
            {
                case CameraMode.Hovering:
                {
                    UpdateHoveringCamera();
                }
                break;
                case CameraMode.Free:
                {
                    
                }
                break;
            }
        }

        private void OnGridSizeUpdate(int gridWidth, int gridHeight)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;

            if (currentCameraMode == CameraMode.Hovering)
            {
                UpdateHoveringCamera();
            }
        }

        private void UpdateHoveringCamera()
        {
            Vector3 cameraPosition = new(gridWidth * 0.5f, 0f, gridHeight * 0.5f);

            float angle = 90f - mainCamera.transform.eulerAngles.x;
            float offsetY = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));
            float offsetZ = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));

            cameraPosition += new Vector3(0f, offsetY, -offsetZ);
            mainCamera.transform.position = cameraPosition;

            float gridSize = Mathf.Max(gridWidth, gridHeight);
            mainCamera.orthographicSize = (gridSize * 0.5f);
        }

        private void SetFreeCamera()
        {

        }
    }
}
