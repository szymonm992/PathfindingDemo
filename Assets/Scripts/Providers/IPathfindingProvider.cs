using System.Collections.Generic;
using PathfindingDemo.Grid.Tile;

namespace PathfindingDemo
{
    public interface IPathfindingProvider 
    {
        IEnumerable<Tile> FindPath(Tile startTile, Tile endTile);
    }
}
