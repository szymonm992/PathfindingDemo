using System.Collections.Generic;
using UnityEngine;
using PathfindingDemo.Grid.Tile;
using PathfindingDemo.Providers;

namespace PathfindingDemo
{
    public class AStarPathfinding : IPathfindingProvider
    {
        private readonly int regularTileCost = 1;
        private readonly IGridProvider gridProvider;

        public AStarPathfinding(int regularTileCost, IGridProvider gridProvider)
        { 
            this.gridProvider = gridProvider;
            this.regularTileCost = regularTileCost;   
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
                    var neighborTile = gridProvider.GetTileAtPosition(neighbor.GridPosition);

                    if (neighborTile == null || !neighborTile.IsAccessible)
                    {
                        continue;
                    }

                    int temporaryGScore = gScore[currentTile] + regularTileCost;

                    if (!gScore.ContainsKey(neighborTile) || temporaryGScore < gScore[neighborTile])
                    {
                        parentTileMap[neighborTile] = currentTile;
                        gScore[neighborTile] = temporaryGScore;
                        fScore[neighborTile] = gScore[neighborTile] + HeuristicCost(neighborTile, endTile);

                        if (!openSet.Contains(neighborTile))
                        {
                            openSet.Add(neighborTile);
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
