using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class PathfindingProvider : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;

        public IEnumerable<Tile> FindPath(Tile startTile, Tile endTile)
        {
            if (startTile == null || endTile == null || !endTile.IsTraversable)
            {
                return null;
            }

            // Initialize open and closed sets
            var openSet = new HashSet<Tile> { startTile };
            var cameFrom = new Dictionary<Tile, Tile>();
            var gScore = new Dictionary<Tile, int>();
            var fScore = new Dictionary<Tile, int>();

            // Initialize g and f scores
            foreach (var tile in gridManager.Grid)
            {
                gScore[tile] = int.MaxValue;
                fScore[tile] = int.MaxValue;
            }

            gScore[startTile] = 0;
            fScore[startTile] = HeuristicCost(startTile, endTile);

            while (openSet.Count > 0)
            {
                // Find the tile with the lowest fScore
                Tile currentTile = null;

                foreach (var tile in openSet)
                {
                    if (currentTile == null || fScore[tile] < fScore[currentTile])
                    {
                        currentTile = tile;
                    }
                }

                if (currentTile == endTile)
                {
                    // Path found, reconstruct and return it
                    return ReconstructPath(cameFrom, currentTile);
                }

                openSet.Remove(currentTile);

                foreach (var neighbor in currentTile.Neighbors)
                {
                    if (!neighbor.Tile.IsTraversable)
                    {
                        continue;
                    }

                    int currentGScore = gScore[currentTile] + 1;
                    if (currentGScore < gScore[neighbor.Tile])
                    {
                        cameFrom[neighbor.Tile] = currentTile;
                        gScore[neighbor.Tile] = currentGScore;
                        fScore[neighbor.Tile] = gScore[neighbor.Tile] + HeuristicCost(neighbor.Tile, endTile);

                        if (!openSet.Contains(neighbor.Tile))
                        {
                            openSet.Add(neighbor.Tile);
                        }
                    }
                }
            }

            return null;
        }

        // Manhattan distance heuristic calculation
        private int HeuristicCost(Tile from, Tile to)
        {
            return Mathf.Abs(from.GridPositionX - to.GridPositionX) + Mathf.Abs(from.GridPositionY - to.GridPositionY);
        }

        private IEnumerable<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile currentTile)
        {
            var path = new List<Tile>();

            while (cameFrom.ContainsKey(currentTile))
            {
                path.Add(currentTile);
                currentTile = cameFrom[currentTile];
            }

            path.Reverse();
            return path;
        }
    }
}
