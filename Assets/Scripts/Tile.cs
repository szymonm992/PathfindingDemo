using UnityEngine;

namespace PathfindingDemo
{
    public class Tile : MonoBehaviour
    {
        public int GridPositionX { get; private set; }
        public int GridPositionY { get; private set; }
        public bool IsTraversable { get; private set; }
        public bool IsSelected { get; private set; }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color hoverTileColor;
        [SerializeField] private Color pathTileColor;
        [SerializeField] private Color traversableTileColor;
        [SerializeField] private Color nontraversableTileColor;

        private TileState currentTileState;
        private Color originalColor;

        public void Initialize(int gridPositionX, int gridPositionY)
        {
            GridPositionX = gridPositionX;
            GridPositionY = gridPositionY;

            IsTraversable = DetermineWhetherTraversable();
            currentTileState = IsTraversable ? TileState.Traversable : TileState.NonTraversable;

            SetTileState(currentTileState);
            originalColor = meshRenderer.material.color;  
        }

        private bool DetermineWhetherTraversable()
        {
            return true;
        }

        private void SetTileState(TileState tileState)
        {
            currentTileState = tileState;
            UpdateTileColor();
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
                meshRenderer.material.color = currentTileState == TileState.Traversable ? traversableTileColor : nontraversableTileColor;
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
