using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Map", menuName = "MapEditor/Map Data", order = 1)]
[Serializable]
public class GridRow
{
    public List<int> row = new List<int>();
}

[Serializable]
public class MapEditor : ScriptableObject
{
    [SerializeField] public int rows = 10;
    [SerializeField] public int cols = 10;
    [SerializeField] private List<GridRow> gridData = new List<GridRow>();
    private bool isInitialized = false;
    private bool isLocked = false; // ✅ Prevent modifications in Play Mode

    private void OnEnable()
    {
        Debug.Log($"[OnEnable] {name} - Is Playing: {Application.isPlaying}");

        if (!Application.isPlaying)
        {
            EnsureGridInitialized();
        }
        else
        {
            if (!isLocked)
            {
                LoadGridData();
                isLocked = true;
            }
        }
    }

    public void AdjustGridSize()
    {
        if (isLocked) return;
        EnsureGridInitialized();
        SaveGridData();
    }

    public int GetGridValue(int row, int col)
    {
        if (gridData == null || row >= gridData.Count || col >= gridData[row].row.Count)
        {
            return 0;
        }
        return Mathf.Clamp(gridData[row].row[col], 0, 5);
    }

    public void SetGridValue(int row, int col, int value)
    {
        if (Application.isPlaying || isLocked)
        {
            Debug.LogWarning($"[SetGridValue] Ignoring changes in Play Mode - {name}");
            return;
        }

        if (gridData == null || row >= gridData.Count || col >= gridData[row].row.Count)
        {
            return;
        }
        if (gridData[row].row[col] != value)
        {
            gridData[row].row[col] = Mathf.Clamp(value, 0, 5);
            SaveGridData();
        }
    }

    private void EnsureGridInitialized()
    {
        if (gridData == null)
        {
            gridData = new List<GridRow>();
        }

        // Handle adding new rows
        while (gridData.Count < rows)
        {
            GridRow newRow = new GridRow();
            for (int j = 0; j < cols; j++)
            {
                newRow.row.Add(0);
            }
            gridData.Add(newRow);
        }

        // Handle removing rows
        while (gridData.Count > rows)
        {
            gridData.RemoveAt(gridData.Count - 1);
        }

        // Ensure all rows have correct column sizes
        foreach (var row in gridData)
        {
            while (row.row.Count < cols)
            {
                row.row.Add(0);
            }
            while (row.row.Count > cols)
            {
                row.row.RemoveAt(row.row.Count - 1);
            }
        }
    }

    private void LoadGridData()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"[LoadGridData] {name} is NOT linked to an asset!");
            return;
        }

        MapEditor savedData = AssetDatabase.LoadAssetAtPath<MapEditor>(path);
        if (savedData != null)
        {
            gridData = new List<GridRow>();
            foreach (var row in savedData.gridData)
            {
                gridData.Add(new GridRow { row = new List<int>(row.row) });
            }
            rows = savedData.rows;
            cols = savedData.cols;
            Debug.Log($"[LoadGridData] Successfully reloaded {name} from {path}");
        }
#endif
    }

    private void SaveGridData()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && !EditorApplication.isUpdating && !EditorApplication.isCompiling)
        {
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && !EditorApplication.isUpdating && !EditorApplication.isCompiling)
        {
            EnsureGridInitialized();
            SaveGridData();
        }
    }
}
