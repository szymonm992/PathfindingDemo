using System.Collections.Generic;

namespace PathfindingDemo
{
    public interface IPathfindingProvider 
    {
        IEnumerable<Tile> FindPath(Tile startTile, Tile endTile);
    }
}
