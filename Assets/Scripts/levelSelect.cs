using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;

public class levelSelect : MonoBehaviour {

    public Image episode1;
    public Image episode2;
    public Image episode3;
    public Canvas lvlselection;

    private int currentlvl;

    // Use this for initialization
    void Start () {
        episode1 = episode1.GetComponent<Image>();
        episode2 = episode2.GetComponent<Image>();
        episode3 = episode3.GetComponent<Image>();
        lvlselection = lvlselection.GetComponent<Canvas>();
        lvlselection.enabled = false;
    }

    public void lvl1Press() {
        if(currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }
    public void lvl2Press() {
        if (currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }
    public void lvl3Press() {
        if (currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }
    public void lvl4Press() {
        if (currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }
    public void lvl5Press() {
        if (currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }
    public void lvl6Press() {
        if (currentlvl == 1) {
            Application.LoadLevel(2);
        }
    }

    public void backToMenu() {
        Application.LoadLevel(0);
    }

    public void episode1Press() {
        currentlvl = 1;
        lvlselection.enabled = true;
    }
    public void episode2Press() {
        currentlvl = 2;
        lvlselection.enabled = true;
    }
    public void episode3Press() {
        currentlvl = 3;
        lvlselection.enabled = true;
    }

    public void backPress() {
        currentlvl = 0;
        lvlselection.enabled = false;
    }
}
