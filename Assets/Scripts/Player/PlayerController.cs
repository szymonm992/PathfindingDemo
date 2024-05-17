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
        public Tile PlayerTile;

        [Range(1f, 20f)]
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
                    PlayerTile = gridManager.RecalculatePlayerTile(transform.position);
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
            gridManager.PathFoundEvent += OnPathFound;
            gridManager.GridSizeUpdateEvent += OnGridSizeUpdate;
            
        }

        private void OnGridSizeUpdate(int _, int __)
        {
            PlayerTile = gridManager.RecalculatePlayerTile(transform.position);
        }

        private void OnDestroy()
        {
            gridManager.PathFoundEvent -= OnPathFound;
        }

        private async void OnPathFound(IEnumerable<Tile> path)
        {
            await MoveAlongThePathAsync(path);
        }
    }
}
