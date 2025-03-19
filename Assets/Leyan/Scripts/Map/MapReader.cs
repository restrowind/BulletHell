using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Map Reader", menuName = "MapEditor/Map Reader", order = 2)]
[Serializable]
public class MapReader : ScriptableObject
{
    [SerializeField] public MapData mapData; // ✅ Allow drag-and-drop in Inspector

    private void OnValidate()
    {
        if (mapData != null)
        {
            Debug.Log($"[MapReader] Successfully assigned MapData: {mapData.name}");
        }
        else
        {
            Debug.LogWarning("[MapReader] No MapData assigned! Drag a MapData asset into the Inspector.");
        }
    }

    public void LoadMapData()
    {
        if (mapData == null)
        {
            Debug.LogError("[MapReader] No MapData assigned! Drag a MapData asset into the Inspector.");
            return;
        }

        Debug.Log($"[MapReader] Loaded map: {mapData.name} with {mapData.rows} rows and {mapData.cols} columns.");
    }

    public void UpdateTile(int row, int col, int value)
    {
        if (mapData != null && row < mapData.rows && col < mapData.cols)
        {
            mapData.SetGridValue(row, col, value);
            Debug.Log($"Updated tile ({row}, {col}) to value {value}");
        }
    }

    public void SaveMapData()
    {
#if UNITY_EDITOR
        if (mapData != null)
        {
            EditorUtility.SetDirty(mapData);
            AssetDatabase.SaveAssets();
            Debug.Log("[MapReader] MapData saved successfully.");
        }
#endif
    }
}
