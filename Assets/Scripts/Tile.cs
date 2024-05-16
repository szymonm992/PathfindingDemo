using UnityEngine;

namespace PathfindingDemo
{
    public class Tile : MonoBehaviour
    {
        public int GridPositionX { get; private set; }
        public int GridPositionY { get; private set; }
        public bool IsTraversable { get; private set; }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color hoverTileColor;
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
            meshRenderer.material.color = currentTileState == TileState.Traversable ? traversableTileColor : nontraversableTileColor;
            originalColor = meshRenderer.material.color; 
        }

        private void OnMouseEnter()
        {
            meshRenderer.material.color = hoverTileColor;
        }

        private void OnMouseExit()
        {
            meshRenderer.material.color = originalColor;
        }
    }
}
