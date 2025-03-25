using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float cellSize;

    public Node[,] grid;
    public Dictionary<Vector2Int, GameObject> occupiedCells = new Dictionary<Vector2Int, GameObject>();

    ////////////////////
    void Start()
    {
        CreateGrid();
        SpawnLevel1();
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPos = transform.position + new Vector3(x * cellSize, 0, y * cellSize);
                grid[x, y] = new Node(worldPos, x, y, true);
            }
        }
    }

    void SpawnLevel1()
    {
        GameObject blueCarPrefab = Resources.Load<GameObject>("CarBlue");
        Vector2Int[] spawnPositions = new Vector2Int[]
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0),
            new Vector2Int(3,0)
        };

        foreach (var pos in spawnPositions)
        {
            if (pos.x < gridWidth && pos.y < gridHeight)
            {
                Vector3 spawnPos = grid[pos.x, pos.y].worldPosition;
                GameObject car = Instantiate(blueCarPrefab, spawnPos, Quaternion.identity);
                occupiedCells[pos] = car;
                CarController carController = car.GetComponent<CarController>();
                if (carController != null)
                {
                    carController.gridPosition = pos;
                }
            }
        }
    }

    public bool HasAdjacentEmpty(Vector2Int cell)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight)
            {
                if (!occupiedCells.ContainsKey(neighbor))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnCarClicked(CarController car)
    {
        if (HasAdjacentEmpty(car.gridPosition))
        {
            Debug.Log("CCCC");
            occupiedCells.Remove(car.gridPosition);
            Locator.Instance.ParkMatchAreaInstance.MoveCarToPark(car.gameObject);
        }
    }

    // Editörde grid hücrelerini görsel olarak çizdirmek için OnDrawGizmos metodunu ekliyoruz.
    void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    // Hücre sınırlarını çizelim.
                    Gizmos.color = Color.white;
                    Vector3 cellCenter = grid[x, y].worldPosition;
                    Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
        else
        {
            // Oyun çalışmadan önce de grid'in konumunu göstermek için, başlangıç pozisyonundan hesaplanan hücreleri çizebiliriz.
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Gizmos.color = Color.red;
                    Vector3 cellCenter = transform.position + new Vector3(x * cellSize, 0, y * cellSize);
                    Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
    }
}

public class Node
{
    public Vector3 worldPosition;
    public int x;
    public int y;
    public bool walkable;

    public Node(Vector3 _worldPos, int _x, int _y, bool _walkable)
    {
        worldPosition = _worldPos;
        x = _x;
        y = _y;
        walkable = _walkable;
    }
}
