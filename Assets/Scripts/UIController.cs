using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    //public Text startText;
    public Text timerDisplay;
    public Text levelDisplay;
    public Text episodeDisplay;
    public Text itemDisplay;
    public Text gameOverDisplay;

    private GameController gameController;
    private memoryScript lvlMemory;
    // Use this for initialization
    void Awake () {
        lvlMemory = GameObject.FindGameObjectWithTag("Memory").GetComponent<memoryScript>();
        gameController = GetComponent<GameController>();
    }

    public void displayWave()
    {
        levelDisplay.text = "Level: " + lvlMemory.getCurrentLvl();
    }

    public void displayEpisode() {
        Debug.Log(lvlMemory.getCurrentEpisode());
        episodeDisplay.text = lvlMemory.getCurrentEpisode();
    }

    public void displayItemCount()
    {
        itemDisplay.text = gameController.getScores("player1") + " / " + gameController.getItemGoal();
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
}
