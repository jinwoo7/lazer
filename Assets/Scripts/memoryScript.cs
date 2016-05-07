using UnityEngine;
using System.Collections;

public class memoryScript : MonoBehaviour {

    private int episode1Points;
    private int episode1Progress;
    private int episode2Points;
    private int episode2Progress;
    private int episode3Points;
    private int episode3Progress;
    private int currentLvl;
    private string currentEpisode;
    private bool[] episode1levels = { false, false, false, false, false, false };
    private bool[] episode2levels = { false, false, false, false, false, false };
    private bool[] episode3levels = { false, false, false, false, false, false };

    // Use this for initialization
    void Awake () {
        currentEpisode = "";
        episode1Points=0;
        episode1Progress=0;
        episode2Points=0;
        episode2Progress=0;
        episode3Points=0;
        episode3Progress=0;
        currentLvl = 1;
        DontDestroyOnLoad(transform.gameObject);
    }

    public bool is1Complete() {
        return episode1levels[5];
    }

    public bool is2Complete() {
        return episode2levels[5];
    }

    public bool is3Complete() {
        return episode3levels[5];
    }

    // getters
    public int getEpisode1Points() {return episode1Points;}

    public int getEpisode2Points() {return episode2Points;}

    public int getEpisode3Points() {return episode3Points;}

    public int getEpisode1Progress() {return episode1Progress;}

    public int getEpisode2Progress() {return episode2Progress;}

    public int getEpisode3Progress() {return episode3Progress;}

    public int getCurrentLvl() { return currentLvl; }

    public string getCurrentEpisode() { return currentEpisode; }

    public bool getEpisode1Levels(int x) {
        if (x < 0 || x > 5)
            return false;
        else
            return
                episode1levels[x];
    }

    public bool getEpisode2Levels(int x) {
        if (x < 0 || x > 5)
            return false;
        else
            return
                episode2levels[x];
    }

    public bool getEpisode3Levels(int x) {
        if (x < 0 || x > 5)
            return false;
        else
            return
                episode3levels[x];
    }

    // adder
    public void addEpisode1Points() { episode1Points++; }

    public void addEpisode2Points() { episode2Points++; }

    public void addEpisode3Points() { episode3Points++; }

    public void addEpisode1Progress() { episode1Progress++; }

    public void addEpisode2Progress() { episode2Progress++; }

    public void addEpisode3Progress() { episode3Progress++; }

    // setter
    public void setCurrentLvl(int x) {
        if (currentLvl < 1) // keeping currentLvl within the range
            currentLvl = 1;
        else if(currentLvl > 6)
            currentLvl = 6;
        currentLvl = x;
    }

    public void setCurrentEpisode(string x) {
        currentEpisode = x;
    }

    public void setLevelProgression() {
        Debug.Log("current episode: " + currentEpisode);
        Debug.Log("current level: " + (currentLvl-1) );
        if (currentEpisode == "episode1")         // setting from episode1
            episode1levels[currentLvl-1] = true;
        else if (currentEpisode == "episode2")   // setting from episode2
            episode2levels[currentLvl-1] = true;
        else                                   // setting from episode3
            episode3levels[currentLvl-1] = true;
        Debug.Log("episode1: " + episode1levels);
        Debug.Log("episode2: " + episode2levels);
        Debug.Log("episode3: " + episode3levels);
    }
}
