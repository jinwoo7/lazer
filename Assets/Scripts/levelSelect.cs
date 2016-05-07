using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour {

    public Text episodeDisplay;
    public Image episode1;
    public Image episode2;
    public Image episode3;
    public Canvas lvlselection;
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
        lvlselection = lvlselection.GetComponent<Canvas>();
        lvlselection.enabled = false;
        source = GetComponent<AudioSource>();
        source.clip = hoverSound;
    }

    public void lvl1Press() {
        lvlMemory.setCurrentLvl(1);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl2Press() {
        lvlMemory.setCurrentLvl(2);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl3Press() {
        lvlMemory.setCurrentLvl(3);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl4Press() {
        lvlMemory.setCurrentLvl(4);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl5Press() {
        lvlMemory.setCurrentLvl(5);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }
    public void lvl6Press() {
        lvlMemory.setCurrentLvl(6);
        SceneManager.LoadScene(lvlMemory.getCurrentEpisode());
    }

    public void backToMenu() {
        SceneManager.LoadScene("StartMenu");
    }

    public void episode1Press() {
        clicked();
        lvlMemory.setCurrentEpisode("episode1");
        episodeDisplay.text = "Episode 1";
        lvlselection.enabled = true;
    }
    public void episode2Press() {
        clicked();
        lvlMemory.setCurrentEpisode("episode2");
        episodeDisplay.text = "Episode 2";
        lvlselection.enabled = true;
    }
    public void episode3Press() {
        clicked();
        lvlMemory.setCurrentEpisode("episode3");
        episodeDisplay.text = "Episode 3";
        lvlselection.enabled = true;
    }

    public void backPress() {
        clicked();
        lvlselection.enabled = false;
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
