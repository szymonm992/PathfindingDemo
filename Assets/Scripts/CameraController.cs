using PathfindingDemo.GridManagement;
using UnityEngine;

namespace PathfindingDemo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GridManager gridManager;

        private void Awake()
        {
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
        }

        private void OnGridSizeUpdate(int gridWidth, int gridHeight)
        {
            Vector3 cameraPosition = new (gridWidth * 0.5f, 0f, gridHeight * 0.5f);

            float angle = 90f - mainCamera.transform.eulerAngles.x;
            float offsetY = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));
            float offsetZ = Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Sqrt((gridWidth * 0.5f) * (gridWidth * 0.5f) + (gridHeight * 0.5f) * (gridHeight * 0.5f));

            cameraPosition += new Vector3(0f, offsetY, -offsetZ);
            mainCamera.transform.position = cameraPosition;

            float gridSize = Mathf.Max(gridWidth, gridHeight);
            mainCamera.orthographicSize = (gridSize * 0.5f);
        }
    }
}
