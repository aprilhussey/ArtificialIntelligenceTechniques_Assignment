using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeTile mazeTilePrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;
    // Please keep <= maxWidth * maxDepth, otherwise may cause a stack overflow.
    [SerializeField] private int numCoins;
    [SerializeField] private GameObject coinPrefab;
    private List<Vector3> populatedCoinPositions;

    private MazeTile[,] mazeGrid;

    private void Start()
    {
        int totalSizeOfMaze = mazeDepth * mazeWidth;
        // Generate the number of coins needed to be added.
        populatedCoinPositions = new List<Vector3>();

        // Populate coins into maze
        for (int i = 0; i < numCoins; i++)
        {
            Vector3 coinPos = this.GenerateCoinPos();
            populatedCoinPositions.Add(coinPos);
            Instantiate(coinPrefab, coinPos, Quaternion.identity);
        }

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

    private Vector3 GenerateCoinPos()
    {
        // Recursive function for finding coin pos that is not populated yet
        // Potential for stack overflow here but odds are incredibly small unless percentageCoins > 100 or maxWidth > 1000 or maxDepth > 1000
        int x = Random.Range(0, mazeWidth);
        int y = Random.Range(0, mazeDepth);
        Vector3 coinPos = new Vector3(x, 0.5f, y);
        if (populatedCoinPositions.IndexOf(coinPos) != -1)
        {
            return GenerateCoinPos();
        }
        else
        {
            return coinPos;
        }
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
