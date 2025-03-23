using UnityEngine;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    public MapEditor mapData; // 选择的 ScriptableObject 地图数据
    public GameObject[] tilePrefabs; // 0-4 对应的地块预制体
    public Vector2 basePosition = Vector2.zero; // 地图基准位置
    public float hexSize = 1.0f; // 六边形单元宽度

    private float hexWidth;
    private float hexHeight;
    public Vector3 bornPos;

    private void Start()
    {
        if (mapData != null)
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        hexWidth = hexSize; // 水平间距 = 单元宽度
        hexHeight = Mathf.Sqrt(3) * 0.5f * hexSize; // 垂直间距 = (√3 / 2) * 单元宽度

        for (int x = -1; x < mapData.cols+1; x++)
        {
            for (int y = -1; y < mapData.rows+1; y++)
            {
                if (x == -1 || y == -1|| y== mapData.rows + 1 || x == mapData.cols + 1)
                {
                    Vector3 position = CalculateHexPosition(x, y);
                    Instantiate(tilePrefabs[0], position, Quaternion.identity, transform);

                }
                else
                {
                    int tileType = mapData.GetGridValue(y, x);
                    if (tileType >= 0 && tileType < tilePrefabs.Length)
                    {
                        Vector3 position = CalculateHexPosition(x, y);
                        Instantiate(tilePrefabs[tileType], position, Quaternion.identity, transform);
                    }
                    else
                    {
                        Vector3 position = CalculateHexPosition(x, y);
                        var tmp=Instantiate(tilePrefabs[4], position, Quaternion.identity, transform);
                        bornPos = tmp.transform.position;
                    }
                }
            }
        }
    }

    private Vector3 CalculateHexPosition(int x, int y)
    {
        float xPos = basePosition.x + x * hexWidth; // 列偏移 1 倍单元宽度
        float yPos = basePosition.y - y * hexHeight; // 六边形方向调整
        if (y % 2 == 1)
        {
            xPos += hexWidth/2; // 奇数列向下偏移 √3/2 倍单元宽度
        }
        return new Vector3(xPos, yPos, 0f);
    }
}
