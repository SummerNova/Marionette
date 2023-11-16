using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int width = 7;
    [SerializeField] int height = 20;
    

    public float pathPrefabSpacing = 1.0f;

    private int[,] maze;

    public GameObject wallPrefab;
    public GameObject pathPrefab;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
       maze = new int[width*2+1, height * 2 + 1];
       for (int x = 0; x < width * 2 + 1; x++) 
       { 
            for (int y = 0; y < height * 2 + 1; y++)
            {
                if (x % 2 == 1 && y % 2 == 1) maze[x, y] = 1;
                else maze[x, y] = 0;
            } 
       }
        RecursiveBacktracking(width + 1, 1);
    }

    void DrawMaze()
    {
        for (int x = 0; x < width * 2 + 1; x++)
        {
            for (int y = 0; y < height * 2 + 1; y++)
            {

                if (maze[x, y] == 2)
                {
                    Vector3 position = new Vector3(x * pathPrefabSpacing, y * pathPrefabSpacing,0);
                    Instantiate(pathPrefab, position+new Vector3(Random.Range(-1,1), Random.Range(-1, 1),0), Quaternion.identity);
                }
            }
        }
    }

    void RecursiveBacktracking(int x, int y)
    {
        maze[x, y] = 2;

        List<Vector2Int> directions = GetRandomDirections();

        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x * 2;
            int newY = y + dir.y * 2;

            if (IsInBounds(newX, newY) && maze[newX, newY] != 2)
            {
                maze[x + dir.x, y + dir.y] = 2; // Mark the path between cells
                RecursiveBacktracking(newX, newY);
            }
        }



    }

    List<Vector2Int> GetRandomDirections()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)   // Left
        };

        for (int i = 0; i < directions.Count; i++)
        {
            Vector2Int temp = directions[i];
            int randomIndex = Random.Range(i, directions.Count);
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        return directions;
    }

    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < 2 * width + 1 && y >= 0 && y < 2 * height + 1;
    }

}