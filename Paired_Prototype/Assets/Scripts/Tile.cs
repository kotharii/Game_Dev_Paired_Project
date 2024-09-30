using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Net;

public sealed class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private Item _item;
    public Image icon;

    public Button tileButton;
    public bool selected = false;
    private Color normalColor;

    private Board board;



    public Item Item
    {
        get => _item;

        set
        {
            if (_item == value) return;

            _item = value;

            icon.sprite = _item.sprite;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        normalColor = tileButton.colors.normalColor;

        if (tileButton != null)
        {
            // Add a listener to the onClick event
            tileButton.onClick.AddListener(TaskOnClick);
        }
        else
        {
            Debug.LogError("button component is missing");
        }

    }


    // Update is called once per frame
    void Update()
    {

    }

    private void TaskOnClick()
    {

        if (!selected)
        {
            //Debug.Log("i:" + x + " j:" + y);

            ChangeButtonColor(Color.cyan);
            selected = !selected;

            board.OnTileSelected(this);

        }
        else
        {
            // deselect
            ChangeButtonColor(normalColor);
            selected = !selected;

            board.OnTileDeselected(this);
        }

        EventSystem.current.SetSelectedGameObject(null);

    }



    public void ChangeButtonColor(Color targetColor)
    {
        // Get the current button's ColorBlock
        ColorBlock cb = tileButton.colors;

        // Set the normal color to the target color
        cb.normalColor = targetColor;

        // Apply the ColorBlock back to the button
        tileButton.colors = cb;
    }

    public Tile Left => x > 0 ? Board.Instance.Tiles[x - 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.Tiles[x, y - 1] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.Tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.Tiles[x, y + 1] : null;
    public Tile[] Neighbours => new[]
    { 
        Left, Top, Right, Bottom,
    };
    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this, };
        if (exclude == null)
        {
            exclude = new List<Tile> { this, };
        }
        else
        {
            exclude.Add(this);
        }
        
        foreach (var neighbour in Neighbours)
        {
            //if (Neighbours == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;
            if (Neighbours == null || exclude == null || neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;
            result.AddRange(neighbour.GetConnectedTiles(exclude));
        }
        return result;
    }

    

}
