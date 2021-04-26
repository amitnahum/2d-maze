using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WallPosition
{
    Up,
    Down,
    Left,
    Right
}


public class MazeGenerator : MonoBehaviour {
    
    public int totalRows = 10;

    public int totalColumns = 10;

    private Dictionary<Vector2, DungeonCell> cells = new Dictionary<Vector2, DungeonCell>();
    
    List<DungeonCell> allCells = new List<DungeonCell>();

    private List<DungeonCell> unvisited = new List<DungeonCell>();
    
    Dictionary<WallPosition, Vector2> neighborDirection = new Dictionary<WallPosition, Vector2>()
    {
        {WallPosition.Up, Vector2.up},
        {WallPosition.Down, Vector2.down},
        {WallPosition.Left, Vector2.left},
        {WallPosition.Right, Vector2.right}
    };
    
    [SerializeField]
    // our cell prefab, it should be just a cell with exterior walls
    private GameObject cellPrefab;

    [SerializeField]
    private GameObject starPrefab;

    [SerializeField]
    private GameObject lavaPrefab;
    
    [SerializeField]
    private GameObject player;
    private void Start()
    {
        DrawMaze();
        MakeExit(RandomExit());
        SpawnPlayer();
    }
    
    // Creates the grid of cells.
    private void DrawMaze()
    {
        // let's draw a complete maze
        for (int x = 1; x <= totalColumns; x++)
        {
            for (int y = 1; y <= totalRows; y++)
            {
                Vector2 position = new Vector2(x, y);
                DungeonCell cell = DrawCell(position);
                cells.Add(position, cell);
                allCells.Add(cell);
            }
        }
        // now let's run our algorithm
        GenerateMaze();
        GenerateLava(Random.Range(2,5));
        GenerateStars(3);
    }

    private void GenerateLava(int number)
    {
        List<DungeonCell> randomCells = GetRandomElements<DungeonCell>(allCells, number);
        foreach (var dungeonCell in randomCells)
        {
            MakeLava(dungeonCell);
        }
    }
    private void GenerateStars(int number)
    {
        List<DungeonCell> randomCells = GetRandomElements<DungeonCell>(allCells, number);
        foreach (var dungeonCell in randomCells)
        {
            AddStar(dungeonCell);
        }
    }

    private void AddStar(DungeonCell cell)
    {
        Debug.Log("Adding star to " + cell.gridPos);
        var star = Instantiate(starPrefab, cell.gridPos, starPrefab.transform.rotation);
        star.name = "Star,X:" + cell.gridPos.x +",Y:" + cell.gridPos.y;
    }

    private void MakeLava(DungeonCell cell)
    {
        Debug.Log("Lavafying " + cell.gridPos);
        var Lava = Instantiate(lavaPrefab, cell.gridPos, lavaPrefab.transform.rotation);
    }

    public static List<T> GetRandomElements<T>(IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }


    private void GenerateMaze()
    {
        unvisited = new List<DungeonCell>(allCells);
        DungeonCell currentCell = cells[new Vector2(1,1)];
        Stack<DungeonCell> cellStack = new Stack<DungeonCell>();
        unvisited.Remove(currentCell);
        cellStack.Push(currentCell);
        while (cellStack.Count > 0)
        {
            currentCell = cellStack.Pop();
            List<DungeonCell> neighbors = GetUnvisitedNeighbors(currentCell);
            if (neighbors.Count > 0)
            {
                cellStack.Push(currentCell);
                DungeonCell randomCell = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(currentCell, GetRelativePosition(currentCell, randomCell));
                RemoveWall(randomCell, GetRelativePosition(randomCell, currentCell));
                cellStack.Push(randomCell);
                unvisited.Remove(randomCell);
            }
        }
    }

    /**
     * returns what direction the to cell is from
     * for example from: (0,0), to: (0,1) will return RIGHT
     */
    private WallPosition GetRelativePosition(DungeonCell from, DungeonCell to)
    {
        if (from.gridPos + Vector2.up == to.gridPos)
        {
            return WallPosition.Up;
        }

        if (from.gridPos + Vector2.down == to.gridPos)
        {
            return WallPosition.Down;
        }

        if (from.gridPos + Vector2.left == to.gridPos)
        {
            return WallPosition.Left;
        }

        if (from.gridPos + Vector2.right == to.gridPos)
        {
            return WallPosition.Right;
        }

        throw new Exception("Not A Neighbor!");
    }

    private List<DungeonCell> GetUnvisitedNeighbors(DungeonCell cell)
    {
        List<DungeonCell> neighbors = new List<DungeonCell>();
        DungeonCell down = GetNeighborByPosition(cell, WallPosition.Down);
        DungeonCell up = GetNeighborByPosition(cell, WallPosition.Up);
        DungeonCell left = GetNeighborByPosition(cell, WallPosition.Left);
        DungeonCell right = GetNeighborByPosition(cell, WallPosition.Right);
        if (IsUnvisited(down)) neighbors.Add(down);
        if (IsUnvisited(up)) neighbors.Add(up);
        if (IsUnvisited(left)) neighbors.Add(left);
        if (IsUnvisited(right)) neighbors.Add(right);
        return neighbors;
    }

    private bool IsUnvisited(DungeonCell cell)
    {
        return unvisited.Contains(cell);
    }
    private DungeonCell GetNeighborByPosition(DungeonCell cell, WallPosition wallPosition)
    {
        var neighborVector = cell.gridPos + neighborDirection[wallPosition];
        if (cells.ContainsKey(neighborVector))
        {
            return cells[neighborVector];
        }
        return null;
    }

    private void SpawnPlayer()
    {
        Vector2 spawnPoint = new Vector2(Random.Range(1, 10), Random.Range(1, 10));
        player.transform.position = spawnPoint;
    } 
    
    
    private void MakeExit(DungeonCell cell)
    {
        Debug.Log("Chosen exit cell is " + cell.cellObject.name); 
        List<GameObject> possibleExits = new List<GameObject>();
        if (cell.gridPos.x == 1)
        {
            possibleExits.Add(cell.cellScript.wallL);
        }
        if (cell.gridPos.y == 1)
        {
            possibleExits.Add(cell.cellScript.wallD);
        }
        if (cell.gridPos.x == totalColumns)
        {
            possibleExits.Add(cell.cellScript.wallR);
        }
        if (cell.gridPos.y == totalRows)
        {
            possibleExits.Add(cell.cellScript.wallU);
        }

        int randomIdx = Random.Range(0, possibleExits.Count - 1);
        GameObject wall = possibleExits[randomIdx];
        wall.AddComponent<Exit>();
        wall.GetComponent<SpriteRenderer>().color = Color.red;

    }
    private DungeonCell RandomExit()
    { // let's create a list of all known edges and then choose one at random
        List<DungeonCell> edges = allCells.FindAll(cell=> cell.IsEdge(totalRows, totalColumns));
        return edges[Random.Range(0,edges.Count)];
    }
    private void RemoveWall(DungeonCell cell, WallPosition position)
    {
        CellPositionToWall(cell,position).SetActive(false);
    }

    private DungeonCell DrawCell(Vector2 position)
    {
        DungeonCell cell = new DungeonCell();
        cell.gridPos = position;
        cell.cellObject = Instantiate(cellPrefab, position, cellPrefab.transform.rotation);
        cell.cellScript = cell.cellObject.GetComponent<CellScript>();
        cell.cellObject.name = "Cell,X:" + position.x + ",Y:" + position.y;
        return cell;
    }

    private GameObject CellPositionToWall(DungeonCell cell, WallPosition position)
    {
        switch (position)
        {
            case WallPosition.Down:
                return cell.cellScript.wallD;
            case WallPosition.Up:
                return cell.cellScript.wallU;
            case WallPosition.Right:
                return cell.cellScript.wallR;
            case WallPosition.Left:
                return cell.cellScript.wallL;
        }

        throw new Exception("InvalidPosition");
    }
    
    public class DungeonCell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cellScript;
        public bool IsEdge(int rows, int columns) => gridPos.x == 0 
                                                     || gridPos.x == columns
                                                    || gridPos.y == 0 
                                                    || gridPos.y == rows;
    }
}