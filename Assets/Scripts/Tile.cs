using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class Tile : MonoBehaviour
    {
        public int GridPositionX { get; private set; }
        public int GridPositionY { get; private set; }
        public bool IsTraversable { get; private set; }
        public bool IsSelected { get; private set; }
        public IEnumerable<NeighborConnection> Neighbors { get; private set; }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color hoverTileColor;
        [SerializeField] private Color pathTileColor;
        [SerializeField] private Color traversableTileColor;
        [SerializeField] private Color nontraversableTileColor;

        private Color originalColor;

        public void Initialize(int gridPositionX, int gridPositionY)
        {
            GridPositionX = gridPositionX;
            GridPositionY = gridPositionY;

            Neighbors = null;
            IsTraversable = false;

            UpdateTileColor();
            originalColor = meshRenderer.material.color;  
        }

        public void SetTraversable(bool value)
        {
            IsTraversable = value;
            UpdateTileColor();
        }

        public void SetNeighbors(IEnumerable<NeighborConnection> neighbors)
        {
            Neighbors = neighbors;
        }

        private void UpdateTileColor()
        {
            if (IsSelected)
            {
                meshRenderer.material.color = pathTileColor;
                originalColor = meshRenderer.material.color;
            }
            else
            {
                meshRenderer.material.color = IsTraversable ? traversableTileColor : nontraversableTileColor;
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
