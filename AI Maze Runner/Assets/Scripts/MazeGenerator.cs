using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;

    private MazeTile[,] mazeGrid;

    private void Start()
    {
        mazeGrid = new MazeTile[mazeWidth, mazeDepth];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                mazeGrid[x, z] = Instantiate(mazeTilePrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        GenerateMaze(null, mazeGrid[0, 0]);
    }

    private void GenerateMaze(MazeTile previousCell, MazeTile currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        while (TryGetNextUnvisitedCell(currentCell, out MazeTile nextCell))
        {
            GenerateMaze(currentCell, nextCell);
        }
    }

    private bool TryGetNextUnvisitedCell(MazeTile currentCell, out MazeTile nextCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        nextCell = unvisitedCells.Count > 0 ? unvisitedCells[Random.Range(0, unvisitedCells.Count)] : null;
        return nextCell != null;
    }

    private List<MazeTile> GetUnvisitedCells(MazeTile currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x);
        int z = Mathf.RoundToInt(currentCell.transform.position.z);

        List<MazeTile> unvisitedCells = new List<MazeTile>();

        foreach (var offset in new[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) })
        {
            int newX = x + Mathf.RoundToInt(offset.x);
            int newZ = z + Mathf.RoundToInt(offset.y);

            if (IsWithinBounds(newX, newZ) && !mazeGrid[newX, newZ].IsVisited)
            {
                unvisitedCells.Add(mazeGrid[newX, newZ]);
            }
        }

        return unvisitedCells;
    }

    private bool IsWithinBounds(int x, int z)
    {
        return x >= 0 && x < mazeWidth && z >= 0 && z < mazeDepth;
    }

    private void ClearWalls(MazeTile previousCell, MazeTile currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        Vector3 positionDiff = currentCell.transform.position - previousCell.transform.position;

        if (positionDiff.x > 0)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (positionDiff.x < 0)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (positionDiff.z > 0)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (positionDiff.z < 0)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }
    }
}
