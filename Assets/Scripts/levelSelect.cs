using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour {

    public Text episodeDisplay;
    public Image episode1;
    public Image episode2;
    public Image episode3;

    public Image lvl1;
    public Image lvl2;
    public Image lvl3;
    public Image lvl4;
    public Image lvl5;
    public Image lvl6;

    public Canvas lvlselection;
    public Canvas lockedMessage;
    public AudioClip clickSound;
    public AudioClip hoverSound;
    
    private memoryScript lvlMemory;
    private AudioSource source;

    // Use this for initialization
    void Start () {
        episodeDisplay.text = "";
        lvlMemory = GameObject.FindGameObjectWithTag("Memory").GetComponent<memoryScript>();
        episode1 = episode1.GetComponent<Image>();
        episode2 = episode2.GetComponent<Image>();
        episode3 = episode3.GetComponent<Image>();

        lvl1 = lvl1.GetComponent<Image>();
        lvl2 = lvl2.GetComponent<Image>();
        lvl3 = lvl3.GetComponent<Image>();
        lvl4 = lvl4.GetComponent<Image>();
        lvl5 = lvl5.GetComponent<Image>();
        lvl6 = lvl6.GetComponent<Image>();

        lvlselection = lvlselection.GetComponent<Canvas>();
        lvlselection.enabled = false;
        lockedMessage.enabled = false;

        if (lvlMemory.is1Complete())            // locking level 2
            episode2.color = Color.white;
        else
            episode2.color = Color.red;

        if (lvlMemory.is2Complete())            // locking level 2
            episode3.color = Color.white;
        else
            episode3.color = Color.red;

        source = GetComponent<AudioSource>();
        source.clip = hoverSound;
    }

    public void lvl1Press() {
        lvlMemory.setCurrentLvl(1);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl2Press() {
        if ((lvlMemory.getCurrentEpisode() == "episode1" && !lvlMemory.getEpisode1Levels(0)) ||
           (lvlMemory.getCurrentEpisode() == "episode2" && !lvlMemory.getEpisode2Levels(0)) ||
           (lvlMemory.getCurrentEpisode() == "episode3" && !lvlMemory.getEpisode3Levels(0)) )
            lockedMessage.enabled = true;
        else {
            lvlMemory.setCurrentLvl(2);
            SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
        }
    }
    public void lvl3Press() {
        if ((lvlMemory.getCurrentEpisode() == "episode1" && !lvlMemory.getEpisode1Levels(1)) ||
           (lvlMemory.getCurrentEpisode() == "episode2" && !lvlMemory.getEpisode2Levels(1)) ||
           (lvlMemory.getCurrentEpisode() == "episode3" && !lvlMemory.getEpisode3Levels(1)))
            lockedMessage.enabled = true;
        else {
            lvlMemory.setCurrentLvl(3);
            SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
        }
    }
    public void lvl4Press() {
        if ((lvlMemory.getCurrentEpisode() == "episode1" && !lvlMemory.getEpisode1Levels(2)) ||
           (lvlMemory.getCurrentEpisode() == "episode2" && !lvlMemory.getEpisode2Levels(2)) ||
           (lvlMemory.getCurrentEpisode() == "episode3" && !lvlMemory.getEpisode3Levels(2)))
            lockedMessage.enabled = true;
        else {
            lvlMemory.setCurrentLvl(4);
            SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
        }
    }
    public void lvl5Press() {
        if ((lvlMemory.getCurrentEpisode() == "episode1" && !lvlMemory.getEpisode1Levels(3)) ||
           (lvlMemory.getCurrentEpisode() == "episode2" && !lvlMemory.getEpisode2Levels(3)) ||
           (lvlMemory.getCurrentEpisode() == "episode3" && !lvlMemory.getEpisode3Levels(3)))
            lockedMessage.enabled = true;
        else {
            lvlMemory.setCurrentLvl(5);
            SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
        }
    }
    public void lvl6Press() {
        if ((lvlMemory.getCurrentEpisode() == "episode1" && !lvlMemory.getEpisode1Levels(4)) ||
           (lvlMemory.getCurrentEpisode() == "episode2" && !lvlMemory.getEpisode2Levels(4)) ||
           (lvlMemory.getCurrentEpisode() == "episode3" && !lvlMemory.getEpisode3Levels(4)))
            lockedMessage.enabled = true;
        else {
            lvlMemory.setCurrentLvl(6);
            SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
        }
    }

    public void backToMenu() {
        SceneManager.LoadScene("StartMenu");
    }

    public void episode1Press() {
        clicked();
        lvlMemory.setCurrentEpisode("episode1");
        episodeDisplay.text = "Episode 1";
        setUpButtons(1);
        lvlselection.enabled = true;
    }
    public void episode2Press() {
        clicked();
        if (lvlMemory.is1Complete()) {
            lvlMemory.setCurrentEpisode("episode2");
            episodeDisplay.text = "Episode 2";
            setUpButtons(2);
            lvlselection.enabled = true;
        }
        else 
            lockedMessage.enabled = true;
        
    }
    public void episode3Press() {
        clicked();
        if (lvlMemory.is2Complete()) {
            lvlMemory.setCurrentEpisode("episode3");
            episodeDisplay.text = "Episode 3";
            setUpButtons(3);
            lvlselection.enabled = true;
        }
        else
            lockedMessage.enabled = true;
    }

    public void setUpButtons(int x) {
        lvl1.color = Color.white;
        if (x == 1) {    // setup for episode1
            if (lvlMemory.getEpisode1Levels(0)) lvl2.color = Color.white;
            else lvl2.color = Color.red;

            if (lvlMemory.getEpisode1Levels(1)) lvl3.color = Color.white;
            else lvl3.color = Color.red;

            if (lvlMemory.getEpisode1Levels(2)) lvl4.color = Color.white;
            else lvl4.color = Color.red;

            if (lvlMemory.getEpisode1Levels(3)) lvl5.color = Color.white;
            else lvl5.color = Color.red;

            if (lvlMemory.getEpisode1Levels(4)) lvl6.color = Color.white;
            else lvl6.color = Color.red;
        }
        else if(x == 2) {    // setup for episode2
            if (lvlMemory.getEpisode2Levels(0)) lvl2.color = Color.white;
            else lvl2.color = Color.red;

            if (lvlMemory.getEpisode2Levels(1)) lvl3.color = Color.white;
            else lvl3.color = Color.red;

            if (lvlMemory.getEpisode2Levels(2)) lvl4.color = Color.white;
            else lvl4.color = Color.red;

            if (lvlMemory.getEpisode2Levels(3)) lvl5.color = Color.white;
            else lvl5.color = Color.red;

            if (lvlMemory.getEpisode2Levels(4)) lvl6.color = Color.white;
            else lvl6.color = Color.red;
        }
        else {     // setup for episode3
            if (lvlMemory.getEpisode3Levels(0)) lvl2.color = Color.white;
            else lvl2.color = Color.red;

            if (lvlMemory.getEpisode3Levels(1)) lvl3.color = Color.white;
            else lvl3.color = Color.red;

            if (lvlMemory.getEpisode3Levels(2)) lvl4.color = Color.white;
            else lvl4.color = Color.red;

            if (lvlMemory.getEpisode3Levels(3)) lvl5.color = Color.white;
            else lvl5.color = Color.red;

            if (lvlMemory.getEpisode3Levels(4)) lvl6.color = Color.white;
            else lvl6.color = Color.red;
        }
    }

    public void backPress() {
        clicked();
        lvlselection.enabled = false;
        lockedMessage.enabled = false;
    }

    public void messageBackPress() {
        clicked();
        lockedMessage.enabled = false;
    }

    public void clicked() {
        source.clip = clickSound;
        source.PlayOneShot(clickSound);
    }

    public void onHover() {
        source.clip = hoverSound;
        source.PlayOneShot(hoverSound);
    }
}
