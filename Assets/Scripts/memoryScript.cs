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
	
    // getters
    public int getEpisode1Points() {return episode1Points;}

    public int getEpisode2Points() {return episode2Points;}

    public int getEpisode3Points() {return episode3Points;}

    public int getEpisode1Progress() {return episode1Progress;}

    public int getEpisode2Progress() {return episode2Progress;}

    public int getEpisode3Progress() {return episode3Progress;}

    public int getCurrentLvl() { return currentLvl; }

    public string getCurrentEpisode() { return currentEpisode; }

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
}
