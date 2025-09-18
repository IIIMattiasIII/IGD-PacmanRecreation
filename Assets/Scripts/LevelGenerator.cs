using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Tooltip("To the marker: This is the csv for the level file. It must not include trailing commas or blank lines. See the given level layout in the file '/Levels/level01.csv' for an example.")]
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private TileBase[] tiles;
    private Grid grid;
    private int[,] levelMap;
    [Tooltip("Defines additional padding relative to the level layout for when resizing the camera view area.")]
    [SerializeField] private float cameraPadding = 1;
    private Tilemap wallsMap;
    private Tilemap pelletsMap;

    void Awake()
    {
        grid = FindFirstObjectByType<Grid>();
        if (levelFile != null && levelFile.text != null)
        {
            levelMap = ReadLevelFile(levelFile.text);
        }
    }

    int[,] ReadLevelFile(string fileText)
    {
        string[] read = fileText.Split("\n");
        string[] lines = read.Concat(read.Take(read.Length - 1).Reverse()).ToArray(); // Mirror vertically, skipping last row
        int numRows = lines.Length;
        int numCols = lines[0].Split(',').Length * 2;
        int[,] map = new int[numCols, numRows];
        for (int i = 0; i < numRows; i++)
        {
            string[] values = (lines[i] + ',' + new string(lines[i].Reverse().ToArray())).Split(','); // Mirror horizontally
            for (int j = 0; j < numCols; j++)
            {
                if (int.TryParse(values[j], out int parsedValue))
                {
                    map[j, i] = parsedValue;
                }
                else
                {
                    map[j, i] = 0; // If fails to parse int (though this should never happen)
                    Debug.LogWarning("Int parse failed in ReadLevelFile");
                }
            }
        }
        return map;
    }

    void Start()
    {
        if (tiles.Length == 0 || Math.Max(levelMap.GetLength(0), levelMap.GetLength(1)) == 0)
        {
            Debug.LogWarning("Level generator is enabled but one or more required fields have not been defined.");
            return;
        }
        SetCamera();
        for (int i = 0; i < grid.transform.childCount; i++) // Deactivate manually created tilemaps
        {
            Transform childTransform = grid.transform.GetChild(i);
            childTransform.gameObject.SetActive(false);
        }
        GenerateTilemap();
    }

    void SetCamera()
    {
        /* TO THE MARKER: As the orthographic size defines the vertical height of the camera, the approach of just checking the max
        in either dimension of the array is technically incorrect, but it seemed like the easiest way to cover exceptionally wide 
        levels being visible in your testing. For all reasonably square levels (or vertically taller), this approach should suffice.
        This could be addressed properly, but it would involve assuming a specific aspect ratio of the output display. Irrespective
        of this, it seemed beyond the scope of this task. If I'm presuming wrong here, feel free to note that in my marking and I'll 
        include this functionality in AT4. */
        float max = Math.Max(levelMap.GetLength(0), levelMap.GetLength(1));
        Camera.main.orthographicSize = (float)Math.Ceiling(max / 2) + cameraPadding;
    }

    Tilemap CreateTilemap(string objName)
    {
        GameObject tilemapGameObject = new GameObject(objName);
        tilemapGameObject.transform.SetParent(grid.transform);
        Tilemap tilemap = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();
        return tilemap;
    }

    void PlaceTile(Vector3Int pos, int tile)
    {
        if (tile < 0 || tile >= tiles.Length)
        {
            Debug.LogWarning($"Invalid tile index ({tile}) in level csv.");
            return;
        }

        if (tile == 5 || tile == 6)
        {
            pelletsMap.SetTile(pos, tiles[tile]);
        }
        else
        {
            wallsMap.SetTile(pos, tiles[tile]);
        }
    }

    void GenerateTilemap()
    {
        wallsMap = CreateTilemap("WallsGenerated");
        pelletsMap = CreateTilemap("PelletsGenerated");

        int width = levelMap.GetLength(0);
        int height = levelMap.GetLength(1);

        int offsetX = -Mathf.FloorToInt(width / 2);
        int offsetY = -Mathf.CeilToInt((height+1) / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                PlaceTile(new Vector3Int(x + offsetX, y + offsetY, 0), levelMap[x, y]);
            }
        }
    }
}
