using UnityEngine;
using System.Collections;

public class PlayersBase : MonoBehaviour {

    public float speed;
    public AudioSource itemSound;
    public GameObject explosion;

    protected GameController gameController;
    protected UIController uiController;
    protected Rigidbody rb;

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickUp"))               // picking up items
        {
            gameController.moveItem();
            itemSound.Play(0);                                  // playing item pickup sound
            gameController.handleScores(tag, "count");
            uiController.displayItemCount();
        }
        if (other.gameObject.CompareTag("Laser"))               // picking up items
        {
            Instantiate(explosion, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            Destroy(gameObject);
            if(tag == "player1") {
                gameController.AIPathStop();
                gameController.setCurrentState(GameController.GameState.Gameover);
            }            
        }
    }
}
