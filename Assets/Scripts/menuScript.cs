using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;

public class menuScript : MonoBehaviour {

    public Canvas quitMenu;
    public Canvas howToScreen;
    public Button playButton;
    public Button exitButton;


	// Use this for initialization
	void Start () {
        quitMenu = quitMenu.GetComponent<Canvas>();
        howToScreen = howToScreen.GetComponent<Canvas>();
        playButton = playButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();
        howToScreen.enabled = false;
        quitMenu.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ExitPress() {
        quitMenu.enabled = true;
        playButton.enabled = false;
        exitButton.enabled = false;
        howToScreen.enabled = false;
    }

    public void NoPress() {
        quitMenu.enabled = false;
        playButton.enabled = true;
        exitButton.enabled = true;
        howToScreen.enabled = false;
    }

    public void HowToPress() {
        quitMenu.enabled = false;
        playButton.enabled = false;
        exitButton.enabled = false;
        howToScreen.enabled = true;
    }

    public void StartLevel() {
        Application.LoadLevel(1);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
