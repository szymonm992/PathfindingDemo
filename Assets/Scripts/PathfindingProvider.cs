using UnityEngine;

namespace PathfindingDemo
{
    public class PathfindingProvider : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;

        private Tile currentStartTile = null;

        public void FindPAth(Tile targetTile)
        {
            if (currentStartTile == null || targetTile == null)
            {
                return;
            }
        }

        private void EndPathfinding()
        {
            currentStartTile = null;
        }
    }
}
