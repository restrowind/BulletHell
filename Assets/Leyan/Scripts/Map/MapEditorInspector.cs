using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MapReader))]
public class MapEditorInspector : Editor
{
    private MapReader mapReader;
    private MapData mapData;
    private Color[] colors = { Color.gray, Color.blue, Color.yellow, Color.green };
    private const float cellSize = 30f;
    private const float padding = 2f;

    public override void OnInspectorGUI()
    {
        mapReader = (MapReader)target;

        EditorGUILayout.LabelField("Map Reader", EditorStyles.boldLabel);
        mapReader.mapData = (MapData)EditorGUILayout.ObjectField("Map Data", mapReader.mapData, typeof(MapData), false);

        if (mapReader.mapData == null)
        {
            EditorGUILayout.HelpBox("No Map Data assigned! Drag a MapData asset into the slot above.", MessageType.Warning);
            return;
        }

        mapData = mapReader.mapData;
        EditorGUILayout.Space();
        DrawGridEditor();

        if (GUILayout.Button("Save Map Data"))
        {
            mapReader.SaveMapData();
        }

        EditorUtility.SetDirty(mapReader);
        serializedObject.ApplyModifiedProperties();
        Repaint();
    }

    private void DrawGridEditor()
    {
        if (mapData.gridData == null || mapData.gridData.Count != mapData.rows)
        {
            mapData.EnsureGridInitialized();
        }

        for (int y = 0; y < mapData.rows; y++)
        {
            EditorGUILayout.BeginHorizontal();
            if (y % 2 == 1)
            {
                GUILayout.Space(cellSize / 2);
            }

            for (int x = 0; x < mapData.cols; x++)
            {
                int tileValue = mapData.gridData[y].row[x]; // ✅ Adjusted to work with the GridRow wrapper
                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize + padding), GUILayout.Height(cellSize + padding));
                rect.width = cellSize;
                rect.height = cellSize;

                EditorGUI.DrawRect(rect, colors[tileValue]);

                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    Undo.RecordObject(mapData, "Change Tile Color");
                    mapData.gridData[y].row[x] = (tileValue + 1) % colors.Length;
                    EditorUtility.SetDirty(mapData);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
