using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathfindingDemo
{
    public class GridManager : MonoBehaviour
    {
        public delegate void GridSizeUpdateDelegate(int width, int height);
        public delegate void PathFoundDelegate(IEnumerable<Tile> path);

        public event GridSizeUpdateDelegate GridSizeUpdateEvent;
        public event PathFoundDelegate PathFoundEvent;

        public const float GRID_POSITION_Y = 0.1f;
        public const int MAX_GRID_SIZE = 50;

        public Tile[,] Grid => grid;
        public bool IsSelectingTilesPermitted => !playerController.IsMoving; 

        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridWidth = 15;
        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridHeight = 15;
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PathfindingProvider pathfindingProvider;
        [SerializeField] private LayerMask tileMask;

        private IEnumerable<Tile> currentPath = null;
        private IEnumerable<Tile> previousPath = null;
        private MonoObjectPool<Tile> tilePool;
        private Tile currentHoveringTile = null;
        private Tile currentStartTile = null;
        private Tile currentEndTile = null;
        private Tile previousHoveringTile = null;
        private Tile[,] grid;
        private int previousGridWidth;
        private int previousGridHeight;
        private bool isDetectingTile;

        public void SetTileSelection(Tile tile, bool value)
        {
            if (IsSelectingTilesPermitted)
            {
                if (value)
                {
                    tile?.SelectTile();
                }
                else
                {
                    tile?.DeselectTile();
                }
            }
        }

        public void UpdateGridSize(int newWidth, int newHeight)
        {
            if (newWidth != previousGridWidth || newHeight != previousGridHeight)
            {
                newWidth = Mathf.Max(Mathf.Min(newWidth, MAX_GRID_SIZE), 1);
                newHeight = Mathf.Max(Mathf.Min(newHeight, MAX_GRID_SIZE), 1);

                previousGridWidth = gridWidth;
                previousGridHeight = gridHeight;

                gridWidth = newWidth;
                gridHeight = newHeight;

                CreateGrid();
            }
        }

        private IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            var neighbors = new List<Tile>();
            int x = tile.GridPositionX;
            int y = tile.GridPositionY;

            if (x > 0)
            {
                neighbors.Add(grid[x - 1, y]);
            }
            if (x < gridWidth - 1)
            {
                neighbors.Add(grid[x + 1, y]);
            }
            if (y > 0)
            {
                neighbors.Add(grid[x, y - 1]);
            }
            if (y < gridHeight - 1)
            {
                neighbors.Add(grid[x, y + 1]);
            }

            return neighbors;
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

            foreach (Tile tile in grid)
            {
                tile.SetNeighborList(GetNeighbors(tile));
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

        private void Update()
        {
            isDetectingTile = IsTileSelected();

            if (Input.GetMouseButtonDown(0))
            {
                if (isDetectingTile)
                {
                    if (currentStartTile == null)
                    {
                        currentStartTile = currentHoveringTile;
                        SetTileSelection(currentStartTile, true);
                    }
                    
                    else if (currentEndTile == null)
                    {
                        currentEndTile = currentHoveringTile;
                        PathFoundEvent?.Invoke(currentPath);
                        DeselectPathTiles();
                    }
                }
                else
                {
                    DeselectPathTiles();
                }
            }
            else
            {
                previousPath = currentPath;
                currentPath = GetPath(currentHoveringTile);

                if (currentPath != null)
                {
                    DisablePreviousPathPoints();

                    foreach (var pathTile in currentPath)
                    {
                        if (!pathTile.IsSelected)
                        {
                            SetTileSelection(pathTile, true);
                        }
                    }
                }
            }
        }

        private void DisablePreviousPathPoints()
        {
            if (previousPath != null && previousPath.Any())
            {
                foreach (var tile in previousPath)
                {
                    if (!currentPath.Contains(tile))
                    {
                        SetTileSelection(tile, false);
                    }
                }
            }
        }

        private IEnumerable<Tile> GetPath(Tile endTile)
        {
            return pathfindingProvider?.FindPath(currentStartTile, endTile);
        }

        private void DeselectPathTiles()
        {
            SetTileSelection(currentStartTile, false);
            SetTileSelection(currentEndTile, false);

            if (currentPath != null)
            {
                foreach (var tile in currentPath)
                {
                    SetTileSelection(tile, false);
                }
            }

            currentStartTile = null;
            currentEndTile = null;
        }

        private bool IsTileSelected()
        {
            bool isRaycastingTile = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, tileMask);

            previousHoveringTile = currentHoveringTile;

            if (previousHoveringTile != null)
            {
                previousHoveringTile.SetHighlightState(false);
            }

            if (isRaycastingTile)
            {
                currentHoveringTile = hit.collider.GetComponent<Tile>();
                if (currentHoveringTile != null && currentHoveringTile.IsTraversable)
                {
                    currentHoveringTile.SetHighlightState(true);
                    return true;
                }
            }

            return false;
        }
    }
}
