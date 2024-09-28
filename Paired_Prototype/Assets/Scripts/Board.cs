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

    private Tile firstSelectedTile = null; // Track the first selected tile
    private Tile secondSelectedTile = null; // Track the second selected tile


    private Distance distance;

    /* -------------------------MANUALLY SET LATER------------------------- */
    public int RemainingDistance = 100;


    void Start()
    {
        // Initialize distance board
        distance = FindObjectOfType<Distance>();

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

                tile.item = GetRandomItem(x, y);
                //Debug.Log("Assigned item: " + tile.item + " to tile at (" + x + ", " + y + ")");
                Tiles[x, y] = tile;
            }
        }
    }


    // Handle tile selection logic
    public void OnTileSelected(Tile selectedTile)
    {
        if (firstSelectedTile == null)
        {
            // Select the first tile
            firstSelectedTile = selectedTile;
            Debug.Log("First tile selected at (" + selectedTile.x + ", " + selectedTile.y + ")");
        }
        else if (secondSelectedTile == null && selectedTile != firstSelectedTile)
        {
            // Select the second tile
            secondSelectedTile = selectedTile;
            Debug.Log("Second tile selected at (" + selectedTile.x + ", " + selectedTile.y + ")");


            // Check if valid
            if (secondSelectedTile.item == firstSelectedTile.item)
            {
                Debug.Log("Cannot switch the same item");
            }
            else
            {
                // Swap items between the two tiles
                SwapItems(firstSelectedTile, secondSelectedTile);

                // Deduct the distance
                UpdateDistance(firstSelectedTile, secondSelectedTile);
            }


            // Reset the color of both tiles
            firstSelectedTile.ChangeButtonColor(Color.white);
            secondSelectedTile.ChangeButtonColor(Color.white);

            firstSelectedTile.selected = !firstSelectedTile.selected;
            secondSelectedTile.selected = !secondSelectedTile.selected;


            // Reset the selection
            firstSelectedTile = null;
            secondSelectedTile = null;


        }
    }

    // Swap items between two tiles
    private void SwapItems(Tile tile1, Tile tile2)
    {
        Item tempItem = tile1.item;
        tile1.item = tile2.item;
        tile2.item = tempItem;

        Debug.Log($"Swapped items between tile ({tile1.x}, {tile1.y}) and tile ({tile2.x}, {tile2.y})");
    }


    public void OnTileDeselected(Tile deselectedTile)
    {
        if (firstSelectedTile == null || firstSelectedTile != deselectedTile)
        {
            Debug.LogError("something went wrong with the first selected tile");
            return;
        }

        firstSelectedTile = null;

    }


    private void UpdateDistance(Tile firstTile, Tile secondTile)
    {
        int cost = Mathf.Abs(firstTile.x - secondTile.x) + Mathf.Abs(firstTile.y - secondTile.y);

        RemainingDistance -= cost;
        Debug.Log($"Remaining distance: {RemainingDistance}");

        distance.UpdateDistanceText(RemainingDistance.ToString());

        CheckIfDistanceRunout();
    }

    private void CheckIfDistanceRunout()
    {
        if (RemainingDistance < 0)
        {
            Debug.Log("Ran out of distance. Game over");
        }

        // end the game
    }


    private Item GetRandomItem(int x, int y)
    {
        List<Item> possibleItems = new List<Item>(ItemDatabase.Items);

        if (x >= 2 && Tiles[x - 1, y].item == Tiles[x - 2, y].item)
        {
            possibleItems.Remove(Tiles[x - 1, y].item);
        }
        if (y >= 2 && Tiles[x, y - 1].item == Tiles[x, y - 2].item)
        {
            possibleItems.Remove(Tiles[x, y - 1].item);
        }

        return possibleItems[Random.Range(0, possibleItems.Count)];
    }
    // ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
    // Update is called once per frame
    void Update()
    {
        
    }
}
