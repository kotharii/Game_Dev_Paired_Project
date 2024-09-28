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


    public Item item
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


}
