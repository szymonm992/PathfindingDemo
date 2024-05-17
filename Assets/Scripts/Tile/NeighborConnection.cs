namespace PathfindingDemo
{
    [System.Serializable]
    public class NeighborConnection
    {
        public Tile Tile { get; private set; }
        public Direction Direction { get; private set; }

        public NeighborConnection(Tile tile, Direction direction)
        {
            Tile = tile;
            Direction = direction;
        }
    }
}
