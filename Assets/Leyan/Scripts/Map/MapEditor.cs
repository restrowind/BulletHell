using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Map", menuName = "MapEditor/Map Data", order = 1)]
[Serializable]
public class MapEditor : ScriptableObject
{
    [SerializeField] public int rows = 10;
    [SerializeField] public int cols = 10;
    [SerializeField] private List<List<int>> gridData = new List<List<int>>();
    private bool isInitialized = false;
    private bool isLocked = false; // ✅ New variable to lock data during Play Mode

    private void OnEnable()
    {
        Debug.Log($"[OnEnable] {name} - Is Playing: {Application.isPlaying}");

        if (!Application.isPlaying)
        {
            EnsureGridInitialized();
        }
        else
        {
            if (!isLocked) // ✅ Prevent re-initialization when Play Mode starts
            {
                LoadGridData();
                isLocked = true;
            }
        }
        Debug.Log($"[OnEnable] {name} - Grid Data Count: {gridData?.Count}");
    }

    public void AdjustGridSize()
    {
        if (isLocked) return; // ✅ Prevent modifications while locked
        EnsureGridInitialized();
        SaveGridData();
    }

    public int GetGridValue(int row, int col)
    {
        if (gridData == null || row >= gridData.Count || col >= gridData[row].Count)
        {
            return 0;
        }
        return Mathf.Clamp(gridData[row][col], 0, 3);
    }

    public void SetGridValue(int row, int col, int value)
    {
        if (Application.isPlaying || isLocked) // ✅ Prevent modifications in Play Mode
        {
            Debug.LogWarning($"[SetGridValue] Ignoring changes in Play Mode - {name}");
            return;
        }

        if (gridData == null || row >= gridData.Count || col >= gridData[row].Count)
        {
            return;
        }
        if (gridData[row][col] != value)
        {
            gridData[row][col] = Mathf.Clamp(value, 0, 3);
            SaveGridData();
        }
    }

    private void EnsureGridInitialized()
    {
        if (gridData == null || gridData.Count == 0)
        {
            gridData = new List<List<int>>();
            for (int i = 0; i < rows; i++)
            {
                gridData.Add(new List<int>());
                for (int j = 0; j < cols; j++)
                {
                    gridData[i].Add(0);
                }
            }
        }
    }

    private void LoadGridData()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"[LoadGridData] {name} is NOT linked to an asset! Attempting manual reload.");
            return;
        }

        MapEditor savedData = AssetDatabase.LoadAssetAtPath<MapEditor>(path);
        if (savedData != null)
        {
            gridData = savedData.gridData;
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


//我觉得可能的原因是ScriptableObject和编辑器继承时非运行状态下更新产生的错乱，我现在想要使用一种新的模式，我使用一个包含原先可视化地图编辑器的非SO脚本
