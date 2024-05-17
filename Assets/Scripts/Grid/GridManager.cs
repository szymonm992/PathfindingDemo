using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathfindingDemo.Pooling;
using PathfindingDemo.Grid.Tile;
using PathfindingDemo.Providers;
using PathfindingDemo.Player.Input;
using UnityEngine.InputSystem;

namespace PathfindingDemo.GridManagement
{
    public class GridManager : MonoBehaviour
    {
        public delegate void GridSizeUpdateDelegate(int width, int height);
        public delegate void PathFoundDelegate(IEnumerable<Tile> path);

        public event GridSizeUpdateDelegate GridSizeUpdateEvent;
        public event PathFoundDelegate PathFoundEvent;

        public const float GRID_POSITION_Y = 0.1f;
        public const float TILE_SIZE = 1f;
        public const int MAX_GRID_SIZE = 50;
        public const int TILE_REGULAR_COST = 1;

        public Tile[,] Grid => grid;
        //public bool IsSelectingTilesPermitted => !playerProvider.IsMoving;
        public bool IsSelectingTilesPermitted => true;
        public Tile CurrentStartTile { get; private set; } = null;
        public Tile PlayerTile { get; private set; } = null;

        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridWidth = 15;
        [Range(0, MAX_GRID_SIZE)]
        [SerializeField] private int gridHeight = 15;
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private LayerMask tileMask;
        [SerializeField] private LayerMask tileObstacleMask;

        private IPathfindingProvider pathfindingProvider;
        private IEnumerable<Tile> currentPath = null;
        private IEnumerable<Tile> previousPath = null;
        private MonoObjectPool<Tile> tilePool;
        private Tile currentHoveringTile = null;
        
        private Tile currentEndTile = null;
        private Tile previousHoveringTile = null;
        private Tile[,] grid;
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
            newWidth = Mathf.Max(Mathf.Min(newWidth, MAX_GRID_SIZE), 1);
            newHeight = Mathf.Max(Mathf.Min(newHeight, MAX_GRID_SIZE), 1);
            gridWidth = newWidth;
            gridHeight = newHeight;

            GenerateGrid().Forget();
        }

        private IEnumerable<NeighborConnection> GetNeighbors(Tile tile)
        {
            var neighbors = new List<NeighborConnection>();
            int x = tile.GridPositionX;
            int y = tile.GridPositionY;

            if (x > 0)
            {
                neighbors.Add(new (grid[x - 1, y], Direction.West));
            }
            if (x < gridWidth - 1)
            {
                neighbors.Add(new(grid[x + 1, y], Direction.East));
            }
            if (y > 0)
            {
                neighbors.Add(new(grid[x, y - 1], Direction.South));
            }
            if (y < gridHeight - 1)
            {
                neighbors.Add(new(grid[x, y + 1], Direction.North));
            }

            return neighbors;
        }

        private async UniTask CreateGrid()
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
                    var newTile = await CreateTileAtPosition(x, y);
                    grid[x, y] = newTile;
                }
            }

            foreach (Tile tile in grid)
            {
                tile.SetNeighbors(GetNeighbors(tile));
            }

            previousPath = currentPath;
            DeselectPathTiles();
            DisablePreviousPathPoints();

            GridSizeUpdateEvent?.Invoke(gridWidth, gridHeight);
        }

        private void CalculateEnclosedAreas()
        {
            var visited = new bool[gridWidth, gridHeight];

            foreach (var tile in grid)
            {
                tile.SetAccessible(tile.IsTraversable);
            }

            // Perform flood-fill from a traversable tile
            foreach (var tile in grid)
            {
                if (tile.IsTraversable && !visited[tile.GridPositionX, tile.GridPositionY])
                {
                    var region = new List<Tile>();
                    bool touchesBoundary = FloodFill(tile, visited, region);

                    if (!touchesBoundary && !region.Contains(PlayerTile))
                    {
                        foreach (var regionTile in region)
                        {
                            regionTile.SetAccessible(false);
                        }
                    }
                }
            }
        }

        private bool FloodFill(Tile startTile, bool[,] visited, List<Tile> region)
        {
            var stack = new Stack<Tile>();
            stack.Push(startTile);
            bool playerInside = false;

            while (stack.Count > 0)
            {
                var tile = stack.Pop();
                int x = tile.GridPositionX;
                int y = tile.GridPositionY;

                if (visited[x, y])
                {
                    continue;
                }

                visited[x, y] = true;
                region.Add(tile);

                if (tile == PlayerTile)
                {
                    playerInside = true;
                }

                foreach (var neighbor in tile.Neighbors)
                {
                    if (neighbor.Tile.IsTraversable && !visited[neighbor.Tile.GridPositionX, neighbor.Tile.GridPositionY])
                    {
                        stack.Push(neighbor.Tile);
                    }
                }
            }

            if (!playerInside)
            {
                foreach (var regionTile in region)
                {
                    regionTile.SetAccessible(false);
                }
            }

            return playerInside;
        }

        private async UniTask<Tile> CreateTileAtPosition(int x, int y)
        {
            Tile newTile = await tilePool.GetFreeObject();
            newTile.transform.position = new Vector3(x, GRID_POSITION_Y, y);
            newTile.Initialize(x, y, IsEdgeTile(x, y), IsTraversable(x, y));
            newTile.gameObject.name = $"Tile ({x},{y})";
            newTile.transform.SetParent(transform);
            return newTile;
        }

        public Tile RecalculatePlayerTile(Vector3 playerPosition)
        {
            int x = Mathf.RoundToInt(playerPosition.x / TILE_SIZE);
            int y = Mathf.RoundToInt(playerPosition.z / TILE_SIZE);

            if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            {
                PlayerTile = grid[x, y];
                return PlayerTile;
            }

            return null;
        }

        private void Awake()
        {
            pathfindingProvider = new AStarPathfinding(TILE_REGULAR_COST);
            tilePool = new MonoObjectPool<Tile>(tilePrefab, MAX_GRID_SIZE * MAX_GRID_SIZE);
        }

        private void Start()
        {
            GenerateGrid().Forget();
            inputManager.LeftMouseButtonClickEvent += OnLeftMouseButtonClick;
        }

        private void OnDestroy()
        {
            inputManager.LeftMouseButtonClickEvent -= OnLeftMouseButtonClick;
        }

        private void OnLeftMouseButtonClick(InputAction.CallbackContext context)
        {
            if (!IsSelectingTilesPermitted)
            {
                return;
            }

            if (isDetectingTile)
            {
                if (CurrentStartTile == null)
                {
                    CurrentStartTile = currentHoveringTile;
                    SetTileSelection(CurrentStartTile, true);
                }
                else if (currentEndTile == null && currentHoveringTile != CurrentStartTile && currentHoveringTile.IsTraversable)
                {
                    currentEndTile = currentHoveringTile;
                    var path = GetPath(currentEndTile);

                    if (path != null)
                    {
                        DeselectPathTiles();
                        PathFoundEvent?.Invoke(currentPath);
                    }
                }
            }
            else
            {
                DeselectPathTiles();
            }
        }

        private async UniTask GenerateGrid()
        {
            await CreateGrid();
            CalculateEnclosedAreas();
        }

        private void Update()
        {
            isDetectingTile = IsTileSelected();

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
            else if (previousPath != null)
            {
                DisablePreviousPathPoints();
            }
        }

        private void DisablePreviousPathPoints()
        {
            if (previousPath != null)
            {
                foreach (var tile in previousPath)
                {
                    if (currentPath == null || !currentPath.Contains(tile))
                    {
                        SetTileSelection(tile, false);
                    }
                }
            }
        }

        private IEnumerable<Tile> GetPath(Tile endTile)
        {
            return pathfindingProvider?.FindPath(CurrentStartTile, endTile);
        }

        private void DeselectPathTiles()
        {
            SetTileSelection(CurrentStartTile, false);
            SetTileSelection(currentEndTile, false);

            if (currentPath != null)
            {
                foreach (var tile in currentPath)
                {
                    SetTileSelection(tile, false);
                }
            }

            CurrentStartTile = null;
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
                if (currentHoveringTile != null && currentHoveringTile.IsAccessible)
                {
                    currentHoveringTile.SetHighlightState(true);
                    return true;
                }
            }

            return false;
        }

        private bool IsTraversable(int x, int y)
        {
            return !Physics.CheckBox(new (x, GRID_POSITION_Y + (TILE_SIZE * 0.5f) + (TILE_SIZE * 0.5f), y),
                new (TILE_SIZE * 0.5f, TILE_SIZE * 0.5f, TILE_SIZE * 0.5f),
                Quaternion.identity, tileObstacleMask);
        }

        private bool IsEdgeTile(int x, int y)
        {
            return x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float offset = TILE_SIZE * 0.5f;

            var bottomLeft = new Vector3(-offset, GRID_POSITION_Y, -offset);
            var bottomRight = new Vector3(gridWidth - offset, GRID_POSITION_Y, -offset);
            Gizmos.DrawLine(bottomLeft, bottomRight);

            var topLeft = new Vector3(-offset, GRID_POSITION_Y, gridHeight - offset);
            var topRight = new Vector3(gridWidth - offset, GRID_POSITION_Y, gridHeight - offset);
            Gizmos.DrawLine(topLeft, topRight);

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(bottomRight, topRight);
        }
        #endif
    }
}
