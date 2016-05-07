using UnityEngine;
using System.Collections;

public class PlayersBase : MonoBehaviour {

    public float speed;
    public AudioClip pointSound;
    public AudioClip speedSound;
    public AudioSource soundManager;
    public GameObject explosion;

    protected GameController gameController;
    protected UIController uiController;
    protected Rigidbody rb;

    private pathRequestManager prm;

    // Use this for initialization
    void Start () {
        GameObject mainController = GameObject.FindGameObjectWithTag("GameController");
        gameController = mainController.GetComponent<GameController>();
        uiController = mainController.GetComponent<UIController>();
        prm = mainController.GetComponent<pathRequestManager>();
        rb = GetComponent<Rigidbody>();
        soundManager = GetComponent<AudioSource>();
        soundManager.clip = pointSound;
        soundManager.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickUp"))               // picking up items
        {
            gameController.moveItem();
            soundManager.clip = pointSound;                       // playing item pickup sound
            soundManager.PlayOneShot(pointSound);
            gameController.handleScores(tag, "count");
            uiController.displayItemCount();
            if(gameController.getScores("player1") == gameController.getItemGoal()) {
                gameController.setSucessful(true);
            }
        }

        if (other.gameObject.CompareTag("speedItem")) {
            soundManager.clip = speedSound;                       // playing item special item sound
            soundManager.PlayOneShot(speedSound);
            gameController.removeItem(true);
            if (tag == "player1") {
                GetComponent<PlayerController>().speed += 3.0f;
            }
            else {
                GetComponent<AIPlayer>().speed += .5f;
            }
        }

        if (other.gameObject.CompareTag("Laser") && 
            gameController.getCurrentState() == GameController.GameState.Playing) {    // getting hit
            Instantiate(explosion, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            Destroy(gameObject);
            if (tag == "player1") {
                gameController.AIPathStop();
                gameController.setCurrentState(GameController.GameState.Gameover);
            }
        }
    }
}
