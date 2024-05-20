using UnityEditor;
using UnityEngine;

namespace PathfindingDemo.GridManagement
{
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        private SerializedProperty gridWidthProp;
        private SerializedProperty gridHeightProp;

        private void OnEnable()
        {
            gridWidthProp = serializedObject.FindProperty("gridWidth");
            gridHeightProp = serializedObject.FindProperty("gridHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Regenerate grid..."))
            {
                ApplyGridSizeChanges();
            }
        }

        private void ApplyGridSizeChanges()
        {
            serializedObject.Update();

            int newWidth = gridWidthProp.intValue;
            int newHeight = gridHeightProp.intValue;

            GridManager gridManager = (GridManager)target;
            gridManager.UpdateGridSize(newWidth, newHeight);

            EditorUtility.SetDirty(gridManager);
        }
    }
}
