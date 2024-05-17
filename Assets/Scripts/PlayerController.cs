using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class PlayerController : MonoBehaviour
    {
        public bool IsMoving { get; private set; }

        [SerializeField] private GridManager gridManager;

        public void MoveAlongThePath(IEnumerable<Vector3> path)
        {
            IsMoving = true;

            foreach (var point in path)
            {
                Debug.Log($"Path point {point}");
            }

            IsMoving = false;
        }

        private void Awake()
        {
            gridManager.PathSelectedEvent += OnPathFound;
        }

        private void OnDestroy()
        {
            gridManager.PathSelectedEvent -= OnPathFound;
        }

        private void OnPathFound(IEnumerable<Vector3> path)
        {
            MoveAlongThePath(path);
        }
    }
}
