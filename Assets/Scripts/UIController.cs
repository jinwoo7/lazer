using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    //public Text startText;
    public Text timerDisplay;
    public Text levelDisplay;
    public Text episodeDisplay;
    public Text itemDisplay;

    private GameController gameController;
    private memoryScript lvlMemory;
    // Use this for initialization
    void Awake () {
        lvlMemory = GameObject.FindGameObjectWithTag("Memory").GetComponent<memoryScript>();
        gameController = GetComponent<GameController>();
    }

    public void displayWave()
    {
        levelDisplay.text = "Level " + lvlMemory.getCurrentLvl();
    }

    public void displayEpisode() {
        Debug.Log(lvlMemory.getCurrentEpisode());
        string temp = lvlMemory.getCurrentEpisode();
        if (temp == "episode1")
            temp = "Episode 1";
        else if (temp == "episode2")
            temp = "Episode 2";
        else if (temp == "episode3")
            temp = "Episode 3";
        episodeDisplay.text = temp;
    }

    public void displayItemCount()
    {
        if(gameController.getScores("player1") < gameController.getItemGoal())
            itemDisplay.color = Color.red;
        else
            itemDisplay.color = Color.blue;
        itemDisplay.text = gameController.getScores("player1") + " / " + gameController.getItemGoal() + "\ncollected";
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

        if (seconds < 5)
            timerDisplay.color = Color.red;
        else
            timerDisplay.color = Color.black;
        timerDisplay.text = timeStr;
    }
}
