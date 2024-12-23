using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerLevelGeneration : MonoBehaviour
{
    public float PREFAB_SIZE = 15f;
    public GameObject[] levelPlatforms;
    public GameObject startPlatform;
    public GameObject endPlatform;
    public GameObject DojoDoor;


    public int width = 6;
    public int height = 6;
    private Cell[,] grid;

    public Corner startCorner;
    public Corner endCorner;

    private (int x, int y) start, end;

    public enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    private struct Cell
    {
        public int x;
        public int y;
        public int cost;
        public bool visited;
        public int dist;
        public (int x, int y) prev;

        public bool inPath;
    }

    void Start()
    {
        if (startCorner == endCorner)
        {
            Debug.LogError("Can't be the same?");
            return;
        }

        start = GetCorner(startCorner);
        end = GetCorner(endCorner);

        InitGrid();
        GeneratePath();
        SpawnPlatforms();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position =
            new Vector3(start.x * PREFAB_SIZE, start.y * -PREFAB_SIZE - 2.5f,
            player.transform.position.z);
    }

    private (int, int) GetCorner(Corner corner)
    {
        switch (corner)
        {
            case Corner.TopLeft:
                return (0, 0);
            case Corner.TopRight:
                return (width - 1, 0);
            case Corner.BottomLeft:
                return (0, height - 1);
            case Corner.BottomRight:
                return (width - 1, height - 1);
            default:
                return (0, 0);
        }
    }

    private void InitGrid()
    {
        grid = new Cell[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = new Cell
                {
                    x = i,
                    y = j,
                    cost = Random.Range(5, 10),
                    visited = false,
                    prev = (-1, -1),
                    dist = int.MaxValue,
                    inPath = false
                };
            }
        }
    }

    private void GeneratePath()
    {
        List<Cell> frontier = new List<Cell>(); // bad pqeueue

        grid[start.x, start.y].dist = 0;
        grid[start.x, start.y].visited = true;
        grid[start.x, start.y].inPath = true;
        frontier.Add(grid[start.x, start.y]);

        // Dijkstra's algorithm
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Description
        while (frontier.Count > 0)
        {
            Cell current = frontier[0];
            frontier.RemoveAt(0);

            if (current.x == end.x && current.y == end.y)
            {
                break;
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || (i != 0 && j != 0))
                    {
                        continue;
                    }

                    int x = current.x + i;
                    int y = current.y + j;

                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        continue;
                    }

                    if (grid[x, y].visited)
                    {
                        continue;
                    }

                    int newCost = current.cost + grid[x, y].cost;

                    if (newCost < grid[x, y].dist)
                    {
                        grid[x, y].dist = newCost;
                        grid[x, y].prev = (current.x, current.y);
                        frontier.Add(grid[x, y]);
                    }

                    grid[x, y].visited = true;
                }
            }

            frontier.Sort((a, b) => a.dist.CompareTo(b.dist)); // bad pqeueue
        }


        Cell cell = grid[end.x, end.y];
        while (!(cell.x == start.x && cell.y == start.y))
        {
            grid[cell.x, cell.y].inPath = true;
            cell = grid[cell.prev.x, cell.prev.y];
        }
    }

    private void SpawnPlatforms()
    {
        bool hasSpawnedDojoDoor = false;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // Spawn position
                Vector3 pos = new Vector3(PREFAB_SIZE * i, -PREFAB_SIZE * j, 0);

                // Prefab instance
                GameObject instance;
                if (start.x == i && start.y == j)
                {
                    instance = Instantiate(startPlatform, pos, Quaternion.identity);
                }
                else if (end.x == i && end.y == j)
                {
                    instance = Instantiate(endPlatform, pos, Quaternion.identity);
                }
                else if ((!hasSpawnedDojoDoor && Random.value <= 1f / width / height) 
                    || (!hasSpawnedDojoDoor && i == width - 2 && j == height - 1)) {
                    instance = Instantiate(DojoDoor, pos, Quaternion.identity);
                    hasSpawnedDojoDoor = true;
                }
                else
                {
                    instance = Instantiate(levelPlatforms[Random.Range(0, levelPlatforms.Length)],
                        pos, Quaternion.identity);
                }

                instance.transform.parent = transform;
                if (grid[i, j].inPath)
                {
                    instance.name = "Path";
                }

                LevelPrefab platform = instance.GetComponent<LevelPrefab>();

                // Walls needed (after checking neighbor platforms)
                if (!GetWallLeft(i, j)) platform.DisableLeft();
                if (!GetWallRight(i, j)) platform.DisableRight();
                if (!GetWallTop(i, j)) platform.DisableTop();
                if (!GetWallBottom(i, j)) platform.DisableBottom();
            }
        }
    }

    private bool GetWallLeft(int i, int j)
    {
        if (i == 0) return true;
        else if (!grid[i, j].inPath) return CoinFlip();
        else return false;
    }

    private bool GetWallRight(int i, int j)
    {
        if (i == width -1) return true;
        else if (!grid[i, j].inPath) return CoinFlip();
        else return false;
    }

    private bool GetWallBottom(int i, int j)
    {
        if (j == height - 1) return true;
        else if (!grid[i, j].inPath) return CoinFlip();
        else return false;
    }

    private bool GetWallTop(int i, int j)
    {
        if (j == 0) return true;
        else if (!grid[i, j].inPath) return CoinFlip();
        else return false;
    }

    private bool CoinFlip()
    {
        return Random.Range(0, 6) == 0;
    }
}
