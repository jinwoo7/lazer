using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour {

    public Image episode1;
    public Image episode2;
    public Image episode3;
    public Canvas lvlselection;

    private memoryScript lvlMemory;

    // Use this for initialization
    void Start () {
        lvlMemory = GameObject.FindGameObjectWithTag("Memory").GetComponent<memoryScript>();
        episode1 = episode1.GetComponent<Image>();
        episode2 = episode2.GetComponent<Image>();
        episode3 = episode3.GetComponent<Image>();
        lvlselection = lvlselection.GetComponent<Canvas>();
        lvlselection.enabled = false;
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
        lvlMemory.setCurrentEpisode("episode1");
        lvlselection.enabled = true;
    }
    public void episode2Press() {
        lvlMemory.setCurrentEpisode("episode2");
        lvlselection.enabled = true;
    }
    public void episode3Press() {
        lvlMemory.setCurrentEpisode("episode3");
        lvlselection.enabled = true;
    }

    public void backPress() {
        lvlselection.enabled = false;
    }
}
