using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace PathfindingDemo
{
    public class PlayerController : MonoBehaviour
    {
        public const float MINIMUM_DISTANCE_THRESHOLD = 0.01f;

        public bool IsMoving { get; private set; }

        [Range(1f, 20f)]
        [SerializeField] private float playerMovementSpeed = 5f;
        [SerializeField] private GridManager gridManager;

        public delegate void PlayerPositionChangedDelegate(Vector3 position);
        public event PlayerPositionChangedDelegate OnPlayerPositionChanged;

        public async UniTask MoveAlongThePathAsync(IEnumerable<Tile> path)
        {
            IsMoving = true;

            foreach (var tile in path)
            {
                // Check if the current tile is the start tile and if it is not the player tile
                if (tile != gridManager.CurrentStartTile)
                {
                    // Move towards the tile
                    await MoveTowards(tile.transform.position);
                }
            }

            IsMoving = false;
            OnPlayerPositionChanged?.Invoke(transform.position);
        }

        private async UniTask MoveTowards(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > MINIMUM_DISTANCE_THRESHOLD)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, playerMovementSpeed * Time.deltaTime);
                await UniTask.Yield();
            }
        }

        private void Awake()
        {
            gridManager.PathFoundEvent += OnPathFound;
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
        }

        private void OnGridSizeUpdate(int _, int __)
        {
            OnPlayerPositionChanged?.Invoke(transform.position);
        }

        private void OnDestroy()
        {
            gridManager.PathFoundEvent -= OnPathFound;
            gridManager.GridSizeUpdateEvent -= OnGridSizeUpdate;
        }

        private async void OnPathFound(IEnumerable<Tile> path)
        {
            await MoveAlongThePathAsync(path);
        }
    }
}
