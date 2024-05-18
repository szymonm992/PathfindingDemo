using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using PathfindingDemo.Providers;
using PathfindingDemo.GridManagement;
using PathfindingDemo.Grid.Tile;

namespace PathfindingDemo
{
    public class PlayerController : MonoBehaviour, IPlayerProvider
    {
        public const float MINIMUM_DISTANCE_THRESHOLD = 0.01f;

        public bool IsMoving { get; private set; }
        public Tile PlayerTile { get; private set; }

        [Range(1f, 25f)]
        [SerializeField] private float playerMovementSpeed = 5f;
        [SerializeField] private GridManager gridManager;

        public async UniTask MoveAlongThePathAsync(IEnumerable<Tile> path)
        {
            IsMoving = true;

            foreach (var point in path)
            {
                if (point != PlayerTile)
                {
                    await MoveTowards(point.transform.position);
                    UpdatePlayerTile();
                }
            }

            IsMoving = false;
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
            gridManager.PathSelectedEvent += OnPathFound;
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
        }

        private void OnGridSizeUpdate(int _, int __)
        {
            UpdatePlayerTile();
        }

        private void OnDestroy()
        {
            gridManager.PathSelectedEvent -= OnPathFound;
        }

        private async void OnPathFound(IEnumerable<Tile> path)
        {
            await MoveAlongThePathAsync(path);
        }

        private void UpdatePlayerTile()
        {
            PlayerTile = gridManager.RecalculatePlayerTile(transform.position);
        }
    }
}
