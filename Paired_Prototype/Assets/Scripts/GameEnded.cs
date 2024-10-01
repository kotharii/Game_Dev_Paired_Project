using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameEnded : MonoBehaviour
{
    public TextMeshProUGUI textGameState;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateWinText()
    {
        textGameState.text =$"<size=200%><uppercase>You win!</uppercase>";
        textGameState.color = Color.white;
    }

    public void UpdateLostText()
    {
        textGameState.text = $"<size=200%><uppercase>You lost</uppercase>";
        textGameState.color = Color.white;

    }
}
