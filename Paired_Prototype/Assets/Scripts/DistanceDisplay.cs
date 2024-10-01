using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Distance : MonoBehaviour
{
    public TextMeshProUGUI textDistance;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        //Debug.Log($"Remaining distance: {board.RemainingDistance}");

        UpdateDistanceText(board.RemainingDistance.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDistanceText(string remainingDistance)
    {
        textDistance.text = "Coins Remaining:\n" + remainingDistance;
    }
}
