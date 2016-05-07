using UnityEngine;
using UnityEngine.UI; // contains all classes for UI elements
using System.Collections;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour {

    public Canvas quitMenu;
    public Canvas howToScreen;
    public Button playButton;
    public Button exitButton;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    private AudioSource source;

    // Use this for initialization
    void Start () {
        quitMenu = quitMenu.GetComponent<Canvas>();
        howToScreen = howToScreen.GetComponent<Canvas>();
        playButton = playButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();
        howToScreen.enabled = false;
        quitMenu.enabled = false;
        source = GetComponent<AudioSource>();
        source.clip = hoverSound;
    }

    public void ExitPress() {
        source.clip = clickSound;
        source.PlayOneShot(clickSound);
        quitMenu.enabled = true;
        playButton.enabled = false;
        exitButton.enabled = false;
        howToScreen.enabled = false;
    }

    public void NoPress() {
        source.clip = clickSound;
        source.PlayOneShot(clickSound);
        quitMenu.enabled = false;
        playButton.enabled = true;
        exitButton.enabled = true;
        howToScreen.enabled = false;
    }

    public void HowToPress() {
        source.clip = clickSound;
        source.PlayOneShot(clickSound);
        quitMenu.enabled = false;
        playButton.enabled = false;
        exitButton.enabled = false;
        howToScreen.enabled = true;
    }

    public void StartLevel() {
        SceneManager.LoadScene("levelSelection");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void onHover() {
        source.clip = hoverSound;
        source.PlayOneShot(hoverSound);
    }
}
