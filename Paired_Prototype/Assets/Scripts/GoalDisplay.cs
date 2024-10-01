using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoalCounting : MonoBehaviour
{
    public TextMeshProUGUI textGoal;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        UpdateDestroyedText(board.AccumulateDestroyed.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDestroyedText(string AccumulateDestroyed)
    {
        textGoal.text = "Goal: " + board.Goal + "\nDestoryed: " + AccumulateDestroyed;
    }
}
