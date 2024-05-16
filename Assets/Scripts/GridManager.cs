using System;
using UnityEngine;

namespace PathfindingDemo
{
    public class GridManager : MonoBehaviour
    {
        public delegate void GridSizeUpdateDelegate(int width, int height);

        public event GridSizeUpdateDelegate GridSizeUpdateEvent;

        public const float GRID_POSITION_Y = 0.1f;
        public const int MAX_GRID_SIZE = 50;

        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridWidth = 15;
        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridHeight = 15;
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private Camera mainCamera;

        private MonoObjectPool<Tile> tilePool;
        private Tile[,] grid;
        private int previousGridWidth;
        private int previousGridHeight;

        public void UpdateGridSize(int newWidth, int newHeight)
        {
            if (newWidth != previousGridWidth || newHeight != previousGridHeight)
            {
                newWidth = Mathf.Max(Mathf.Min(newWidth, MAX_GRID_SIZE), 1);
                newHeight = Mathf.Max(Mathf.Min(newHeight, MAX_GRID_SIZE), 1);

                gridWidth = newWidth;
                gridHeight = newHeight;

                CreateGrid();

                previousGridWidth = gridWidth;
                previousGridHeight = gridHeight;

                GridSizeUpdateEvent?.Invoke(newWidth, newHeight);
            }
        }
        
        private void CreateGrid()
        {
            if (grid != null)
            {
                foreach (Tile tile in grid)
                {
                    tilePool.ReturnObjectToPool(tile);
                }
            }

            grid = new Tile[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    CreateTileAtPosition(x, y, out var newTile);
                    grid[x, y] = newTile;
                }
            }

            GridSizeUpdateEvent?.Invoke(gridWidth, gridHeight);
        }

        private void CreateTileAtPosition(int x, int y, out Tile newTile)
        {
            newTile = tilePool.GetFreeObject();
            newTile.transform.position = new Vector3(x, GRID_POSITION_Y, y);
            newTile.Initialize(x, y);
            newTile.gameObject.name = $"Tile ({x},{y})";
            newTile.transform.SetParent(transform);
        }

        private void Awake()
        {
            tilePool = new MonoObjectPool<Tile>(tilePrefab, MAX_GRID_SIZE * MAX_GRID_SIZE);
        }

        private void Start()
        {
            previousGridWidth = gridWidth;
            previousGridHeight = gridHeight;
            CreateGrid();
        }
    }
}
