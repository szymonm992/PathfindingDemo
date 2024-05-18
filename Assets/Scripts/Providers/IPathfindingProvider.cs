using System.Collections.Generic;
using PathfindingDemo.Grid.Tile;

namespace PathfindingDemo
{
    public interface IPathfindingProvider 
    {
        /// <summary>
        /// Returns a collection of tiles between two poitns (inclusive)
        /// </summary>
        /// <param name="startTile">A tile to start looking for path from</param>
        /// <param name="endTile">An end node for pathfinding</param>
        /// <returns></returns>
        IEnumerable<Tile> FindPath(Tile startTile, Tile endTile);
    }
}
