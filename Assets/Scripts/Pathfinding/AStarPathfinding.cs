using System.Collections.Generic;
using UnityEngine;
using PathfindingDemo.Grid.Tile;

namespace PathfindingDemo
{
    public class AStarPathfinding : IPathfindingProvider
    {
        private int tileCost = 1;

        public AStarPathfinding(int tileCost)
        { 
            this.tileCost = tileCost;   
        }

        public IEnumerable<Tile> FindPath(Tile startTile, Tile endTile)
        {
            if (startTile == null || endTile == null || !endTile.IsAccessible)
            {
                return null;
            }

            var openSet = new HashSet<Tile> { startTile };
            var parentTileMap = new Dictionary<Tile, Tile>();
            var gScore = new Dictionary<Tile, int>
            {
                [startTile] = 0
            };
            var fScore = new Dictionary<Tile, int>
            {
                [startTile] = HeuristicCost(startTile, endTile)
            };

            while (openSet.Count > 0)
            {
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
                    return ReconstructPath(parentTileMap, currentTile);
                }

                openSet.Remove(currentTile);

                foreach (var neighbor in currentTile.Neighbors)
                {
                    if (!neighbor.Tile.IsAccessible)
                    {
                        continue;
                    }

                    int temporaryGScore = gScore[currentTile] + tileCost;

                    if (!gScore.ContainsKey(neighbor.Tile) || temporaryGScore < gScore[neighbor.Tile])
                    {
                        parentTileMap[neighbor.Tile] = currentTile;
                        gScore[neighbor.Tile] = temporaryGScore;
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

        private IEnumerable<Tile> ReconstructPath(Dictionary<Tile, Tile> parentTileMap, Tile currentTile)
        {
            var path = new List<Tile>();

            while (parentTileMap.ContainsKey(currentTile))
            {
                path.Add(currentTile);
                currentTile = parentTileMap[currentTile];
            }

            path.Reverse();
            return path;
        }
    }
}
