using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public Text startText;
    public Text timerDisplay;
    public Text levelDisplay;
    public Text itemDisplay;
    public Text gameOverDisplay;

    private GameController gameController;
    // Use this for initialization
    void Start () {
        gameController = GetComponent<GameController>();
        gameOverDisplay.text = "";
        startText.text = "Press Spacebar to start!";
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void displayWave()
    {
        levelDisplay.text = "Level: " + gameController.getWave();
    }

    public void displayItemCount()
    {
        itemDisplay.text = gameController.players[0].GetComponent<PlayersBase>().getItemNum() + " / " + gameController.getItemGoal();
    }

    public void displayTime(float gameTimer)
    {
        float seconds = gameTimer % 60;
        float minutes = gameTimer / 60;
        string timeStr;
        if (gameTimer >= 60.0f)
            timeStr = minutes.ToString("0") + ":" + seconds.ToString("00");
        else
            timeStr = seconds.ToString("F2");

        timerDisplay.text = timeStr;
    }

    public void displayGameOver()
    {
        gameOverDisplay.text = "Game Over";
        gameOverDisplay.color = Color.red;
    }

    public void setStartText(string s)
    {
        startText.text = s;
    }

}
