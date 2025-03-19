using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class GridRow
{
    public List<int> row = new List<int>();
}


[CreateAssetMenu(fileName = "New Map Data", menuName = "MapEditor/Map Data Storage", order = 1)]
[Serializable]
public class MapData : ScriptableObject
{
    [SerializeField] public int rows = 10;
    [SerializeField] public int cols = 10;
    [SerializeField] public List<GridRow> gridData = new List<GridRow>(); // ✅ Use wrapper class

    private void OnEnable()
    {
        EnsureGridInitialized();
    }

    public void EnsureGridInitialized()
    {
        if (gridData == null || gridData.Count != rows)
        {
            gridData = new List<GridRow>();
            for (int i = 0; i < rows; i++)
            {
                GridRow newRow = new GridRow();
                for (int j = 0; j < cols; j++)
                {
                    newRow.row.Add(0);
                }
                gridData.Add(newRow);
            }
        }
    }

    public int GetGridValue(int row, int col)
    {
        if (gridData == null || row >= gridData.Count || col >= gridData[row].row.Count)
        {
            return 0;
        }
        return Mathf.Clamp(gridData[row].row[col], 0, 3);
    }

    public void SetGridValue(int row, int col, int value)
    {
        if (gridData == null || row >= gridData.Count || col >= gridData[row].row.Count)
        {
            return;
        }
        gridData[row].row[col] = Mathf.Clamp(value, 0, 3);
        SaveMapData();
    }

    private void SaveMapData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
