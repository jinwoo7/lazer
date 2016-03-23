using UnityEngine;
using System.Collections;

public class PlayersBase : MonoBehaviour {

    public float speed;
    public AudioSource itemSound;
    public GameObject explosion;

    protected GameController gameController;
    protected UIController uiController;
    protected Rigidbody rb;

    private int itemNum;

    // Use this for initialization
    void Start () {
        itemNum = 0;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIController>();
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(gameObject.tag+"pickUp"))               // picking up items
        {
            itemSound.Play(0);                                  // playing item pickup sound
            gameController.removeItem(other.tag);                 // destroys an item
            gameController.itemCount(other.tag);
            uiController.displayItemCount();
            gameController.itemSpawn(gameObject.tag);                         // respawn an item
        }
        if (other.gameObject.CompareTag("Laser"))               // picking up items
        {
            if (tag == "player1")
                gameController.players.RemoveAt(0);
            if (tag == "player2")
                gameController.players.RemoveAt(1);
            if (tag == "player3")
                gameController.players.RemoveAt(0);
            if (tag == "player4")
                gameController.players.RemoveAt(1);
            Instantiate(explosion, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            Destroy(gameObject);
            uiController.displayGameOver();
            gameController.setCurrentState(GameController.GameState.Gameover);
        }
    }

    public void resetItemNum()
    {
        itemNum = 0;
    }

    public void inclementItemNum()
    {
        itemNum++;
    }
    
    public int getItemNum()
    {
        return itemNum;
    }
}
