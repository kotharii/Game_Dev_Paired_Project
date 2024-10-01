using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public List<Tile> GetConnectedHorizontalTiles()
    {
        var result = new List<Tile> { this };
        var exclude = new List<Tile> { this };

        // Traverse left
        var current = this.Left;
        while (current != null && current.Item == this.Item && !exclude.Contains(current))
        {
            result.Add(current);
            exclude.Add(current);
            current = current.Left;
        }

        // Traverse right
        current = this.Right;
        while (current != null && current.Item == this.Item && !exclude.Contains(current))
        {
            result.Add(current);
            exclude.Add(current);
            current = current.Right;
        }

        return result;
    }

    public List<Tile> GetConnectedVerticalTiles()
    {
        var result = new List<Tile> { this };
        var exclude = new List<Tile> { this };

        var current = this.Top;
        while (current != null && current.Item == this.Item && !exclude.Contains(current))
        {
            result.Add(current);
            exclude.Add(current);
            current = current.Top;
        }

        current = this.Bottom;
        while (current != null && current.Item == this.Item && !exclude.Contains(current))
        {
            result.Add(current);
            exclude.Add(current);
            current = current.Bottom;
        }
        return result;
    }
}