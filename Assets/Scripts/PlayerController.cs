using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class PlayerController : MonoBehaviour
    {
        public bool IsMoving { get; private set; }

        [SerializeField] private GridManager gridManager;

        public void MoveAlongThePath(IEnumerable<Tile> path)
        {
            IsMoving = true;

            foreach (var tile in path)
            {
                Debug.Log($"Path point (X:{tile.GridPositionX}, Y:{GridManager.GRID_POSITION_Y}, Z:{tile.GridPositionY})");
            }

            IsMoving = false;
        }

        private void Awake()
        {
            gridManager.PathFoundEvent += OnPathFound;
        }

        private void OnDestroy()
        {
            gridManager.PathFoundEvent -= OnPathFound;
        }

        private void OnPathFound(IEnumerable<Tile> path)
        {
            MoveAlongThePath(path);
        }
    }
}
