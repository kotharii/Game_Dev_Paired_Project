using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using DG.Tweening;
//using System.Diagnostics;

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
    private GoalCounting destroyed;
    private GameEnded gameState;

    // pop up text window
    public GameObject panel;
    // blocking the whole canvas after game ended
    public GameObject blockingPanel;

    /* -------------------------MANUALLY SET LATER------------------------- */
    public int RemainingDistance = 100;
    public int AccumulateDestroyed = 0;
    public int Goal = 20;
    /* -------------------------MANUALLY SET LATER------------------------- */


    //private float TweenDuration = 0.5f;

    void Start()
    {
        // Initialize all the display
        distance = FindObjectOfType<Distance>();
        destroyed = FindObjectOfType<GoalCounting>();
        gameState = FindObjectOfType<GameEnded>();

        // don't display it in the beginning
        panel.SetActive(false);
        blockingPanel.SetActive(false);

        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];
                tile.x = x;
                tile.y = y;

                tile.Item = GetRandomItem(x, y);
                Tiles[x, y] = tile;
            }
        }
    }

    private Item GetRandomItem(int x, int y)
    {
        List<Item> possibleItems = new List<Item>(ItemDatabase.Items);
        /*
        if (x >= 2 && Tiles[x - 1, y].Item == Tiles[x - 2, y].Item)
        {
            possibleItems.Remove(Tiles[x - 1, y].Item);
        }
        if (y >= 2 && Tiles[x, y - 1].Item == Tiles[x, y - 2].Item)
        {
            possibleItems.Remove(Tiles[x, y - 1].Item);
        }
        */
        if (x >= 1)
        {
            possibleItems.Remove(Tiles[x - 1, y].Item);
        }
        if (y >= 1)
        {
            possibleItems.Remove(Tiles[x, y - 1].Item);
        }

        return possibleItems[Random.Range(0, possibleItems.Count)];
    }

    // Handle tile selection logic
    public void OnTileSelected(Tile selectedTile)
    {
        if (firstSelectedTile == null)
        {
            // Select the first tile
            firstSelectedTile = selectedTile;
        }
        else if (secondSelectedTile == null && selectedTile != firstSelectedTile)
        {
            // Select the second tile
            secondSelectedTile = selectedTile;

            // Check if valid
            if (secondSelectedTile.Item == firstSelectedTile.Item)
            {
                Debug.Log("Cannot switch the same item");
            }
            else
            {
                // Swap items between the two tiles
                SwapItems(firstSelectedTile, secondSelectedTile);

                if (CanPop())
                {
                    Pop();
                }

                // Deduct the distance
                UpdateDistance(firstSelectedTile, secondSelectedTile);
                UpdateDestroyed();


                // Check if game ended
                CheckIfGameEnded();
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
        Item tempItem = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tempItem;
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

    }


    private void UpdateDestroyed()
    {
        int destroyedCount = 0;
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (Tiles[x, y].Item.sprite == null)
                    destroyedCount++;

        AccumulateDestroyed = destroyedCount;

        destroyed.UpdateDestroyedText(AccumulateDestroyed.ToString());
    }


    private void CheckIfGameEnded()
    {
        if (RemainingDistance < 0)
        {
            gameState.UpdateLostText();
            panel.SetActive(true);
            blockingPanel.SetActive(true);
        }
        else if (RemainingDistance >= 0 && Goal < AccumulateDestroyed)
        {
            gameState.UpdateWinText();
            panel.SetActive(true);
            blockingPanel.SetActive(true);
        }


    }



    /*
    // Checking neighbours
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.A)) return;
        foreach (var connectedTile in Tiles[0, 0].GetConnectedTiles()) connectedTile.icon.transform.DOScale(1.25f, TweenDuration).Play();
    }
    */

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                    return true;
        return false;
    }

    private void Pop()
    {
        // Track columns with connected tiles and the number of tiles to remove from each column
        Dictionary<int, List<int>> columnsToShift = new Dictionary<int, List<int>>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var connectedTiles = tile.GetConnectedTiles();
                if (connectedTiles.Skip(1).Count() < 2) continue;
                
                foreach (var connectedTile in connectedTiles)
                {
                    var tileX = connectedTile.x;
                    var tileY = connectedTile.y;

                    // Add the tile's y value to the list for its column
                    if (!columnsToShift.ContainsKey(tileX))
                        columnsToShift[tileX] = new List<int>();

                    
                    // Check if tileY is already in the list to avoid duplicates
                    if (!columnsToShift[tileX].Contains(tileY))
                    {
                        columnsToShift[tileX].Add(tileY);
                    }
                }
                
            }
        }
        // Process each column where tiles were removed
        foreach (var column in columnsToShift)
        {
            int tileX = column.Key;
            Debug.Log($"tileX: {tileX}");
            // Order the Y values in ascending order (lowest to highest)
            var rowsToRemove = column.Value.OrderBy(y => y).ToList();

            foreach (var tileY in rowsToRemove)
            {
                Debug.Log($"tileY: {tileY}");
                // For each tile in this column, shift the tiles above it down by one row
                for (int aboveY = tileY; aboveY > 0; aboveY--)
                {
                    Tiles[tileX, aboveY].Item = Tiles[tileX, aboveY - 1].Item;
                    Tiles[tileX, aboveY].tileButton.interactable = Tiles[tileX, aboveY - 1].tileButton.interactable;
                }

                // After shifting, set the topmost row to empty tile
                Tiles[tileX, 0].Item = ScriptableObject.CreateInstance<Item>();

                Tiles[tileX, 0].tileButton.interactable = false;
            }
        }
        if (CanPop()) Pop();
        

    }

    

}