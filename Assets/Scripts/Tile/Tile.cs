using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathfindingDemo
{
    public class Tile : MonoBehaviour
    {
        public int GridPositionX { get; private set; }
        public int GridPositionY { get; private set; }
        public bool IsTraversable { get; private set; }
        public bool IsAccessible { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsEdgeTile { get; private set; }
        public IEnumerable<NeighborConnection> Neighbors { get; private set; }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color hoverTileColor;
        [SerializeField] private Color pathTileColor;
        [SerializeField] private Color traversableTileColor;
        [SerializeField] private Color nontraversableTileColor;

        private Color originalColor;

        public void Initialize(int gridPositionX, int gridPositionY, bool isEdgeTile, bool isTraversable)
        {
            GridPositionX = gridPositionX;
            GridPositionY = gridPositionY;

            Neighbors = null;
            IsTraversable = isTraversable;
            IsAccessible = IsTraversable;
            IsEdgeTile = isEdgeTile;

            UpdateTileColor();
            originalColor = meshRenderer.material.color;  
        }

        public void SetAccessible(bool value)
        {
            IsAccessible = value;
            UpdateTileColor();
        }

        public void UpdateAccessibility()
        {
            if (IsAccessible)
            {
                IsAccessible = Neighbors.Any(neighbor => neighbor.Tile.IsTraversable);
            }
            
            UpdateTileColor();
        }

        public void SetNeighbors(IEnumerable<NeighborConnection> neighbors)
        {
            Neighbors = neighbors;
        }

        public void UpdateTileColor()
        {
            if (IsSelected)
            {
                meshRenderer.material.color = pathTileColor;
                originalColor = meshRenderer.material.color;
            }
            else
            {
                meshRenderer.material.color = IsAccessible ? traversableTileColor : nontraversableTileColor;
                originalColor = meshRenderer.material.color;
            }
        }

        public void SetHighlightState(bool value)
        {
            if (!IsSelected)
            {
                meshRenderer.material.color = value ? hoverTileColor : originalColor;
            }
        }

        public void SelectTile()
        {
            IsSelected = true;
            UpdateTileColor();
        }

        public void DeselectTile()
        {
            IsSelected = false;
            UpdateTileColor();
        }
    }
}
