namespace PathfindingDemo
{
    public enum TileState 
    {
        /// <summary>
        /// State indicating tile is not traversable, due to an obstacle
        /// </summary>
        NonTraversable = 0,

        /// <summary>
        /// State indicating tile is traversable
        /// </summary>
        Traversable = 1,
    }
}
