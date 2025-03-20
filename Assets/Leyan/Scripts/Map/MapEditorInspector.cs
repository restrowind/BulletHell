using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MapEditor))]
public class MapEditorInspector : Editor
{
    private MapEditor mapEditor;
    private Color[] colors = { Color.gray, Color.blue, Color.yellow, Color.green, Color.black }; // ✅ Added new tile type (Black)
    private const float cellSize = 30f;
    private const float padding = 2f;

    public override void OnInspectorGUI()
    {
        mapEditor = (MapEditor)target;

        if (!Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();
            mapEditor.rows = EditorGUILayout.IntField("Rows", mapEditor.rows);
            mapEditor.cols = EditorGUILayout.IntField("Cols", mapEditor.cols);
            if (EditorGUI.EndChangeCheck())
            {
                mapEditor.AdjustGridSize();
                SaveScriptableObject();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Grid Editor", EditorStyles.boldLabel);
        DrawGridEditor();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Data Array", EditorStyles.boldLabel);
        DrawGridArray();

        EditorUtility.SetDirty(mapEditor);
        serializedObject.ApplyModifiedProperties();
        Repaint();
    }

    private void DrawGridEditor()
    {
        Event e = Event.current;
        for (int y = 0; y < mapEditor.rows; y++)
        {
            EditorGUILayout.BeginHorizontal();

            if (y % 2 == 1)
            {
                GUILayout.Space(cellSize / 2);
            }

            for (int x = 0; x < mapEditor.cols; x++)
            {
                int tileValue = mapEditor.GetGridValue(y, x);
                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize + padding), GUILayout.Height(cellSize + padding));
                rect.width = cellSize;
                rect.height = cellSize;

                EditorGUI.DrawRect(rect, colors[Mathf.Clamp(tileValue, 0, colors.Length - 1)]);

                if (!Application.isPlaying && e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                {
                    Undo.RecordObject(mapEditor, "Change Tile Color");
                    mapEditor.SetGridValue(y, x, (tileValue + 1) % colors.Length);
                    SaveScriptableObject();
                    e.Use();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawGridArray()
    {
        int rows = mapEditor.rows;
        int cols = mapEditor.cols;
        for (int y = 0; y < rows; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < cols; x++)
            {
                int value = mapEditor.GetGridValue(y, x);
                int newValue = Mathf.Clamp(EditorGUILayout.IntField(value, GUILayout.Width(40)), 0, colors.Length - 1);

                if (newValue != value)
                {
                    mapEditor.SetGridValue(y, x, newValue);
                    SaveScriptableObject();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void SaveScriptableObject()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !EditorApplication.isUpdating && !EditorApplication.isCompiling)
        {
            EditorApplication.delayCall += () =>
            {
                EditorUtility.SetDirty(mapEditor);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
        }
#endif
    }
}
