using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class PathfindingProvider : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;

        public IEnumerable<Vector3> FindPath(Tile startTile, Tile endTile)
        {
            if (startTile == null || endTile == null)
            {
                return null;
            }

            return null;
        }
    }
}
