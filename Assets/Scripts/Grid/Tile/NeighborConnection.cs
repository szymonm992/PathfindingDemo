using UnityEngine;

namespace PathfindingDemo.Grid.Tile
{
    [System.Serializable]
    public class NeighborConnection
    {
        public Vector2Int GridPosition { get; private set; }
        public Direction Direction { get; private set; }

        public NeighborConnection(Vector2Int gridPosition, Direction direction)
        {
            GridPosition = gridPosition;
            Direction = direction;
        }
    }
}
