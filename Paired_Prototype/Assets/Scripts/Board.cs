using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private void Awake() => Instance = this;

    void Start()
    {
        if (rows == null || rows.Length == 0)
        {
            Debug.LogError("Rows array is null or empty!");
            return;
        }
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {
            if (rows[y] == null)
            {
                Debug.LogError($"Row at index {y} is null!");
                continue;
            }
            for (var x = 0; x < Width; x++)
            {
                if (rows[y].tiles == null || rows[y].tiles.Length == 0)
                {
                    Debug.LogError($"Tile array in row {y} is null or empty!");
                    continue;
                }
                var tile = rows[y].tiles[x];
                if (tile == null)
                {
                    Debug.LogError($"Tile at ({x}, {y}) is null!");
                    continue;
                }
                tile.x = x;
                tile.y = y;

                tile.item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                Debug.Log("Assigned item: " + tile.item + " to tile at (" + x + ", " + y + ")");
                Tiles[x, y] = tile;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
