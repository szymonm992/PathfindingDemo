using PathfindingDemo.Grid.Tile;
using UnityEngine;

namespace PathfindingDemo.Providers
{
    public interface IGridProvider 
    {
        void SetTileSelection(Tile tile, bool value);
        void UpdateGridSize(int newWidth, int newHeight);
        Tile GetTileAtPosition(Vector2Int position);
        Tile RecalculatePlayerTile(Vector3 playerPosition);
    }
}
