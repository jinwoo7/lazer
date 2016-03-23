using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {
    public GameObject Laser;
    public Material normal;
    public Material charging;

    private Renderer thisRenderer;
    private int OneNorTwoChar;
    private float timer;
    private float flashingTime;
    private bool isCharging;                                // flag for checking if the wall is charging up
    private GameController gameController;
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        thisRenderer = gameObject.GetComponent<Renderer>(); // renderer of this object
        OneNorTwoChar = 1;                                  // variable used to toggle between two materials. 1=normal 2=charging
        flashingTime = 0.2f;                                // duration of the warning
        isCharging = false;                                 // wall is currently not charging
    }

    void Update()
    {
        switch(gameController.getGameState())
        {
            case GameController.GameState.Transition:
                if(isCharging)
                {
                    thisRenderer.material = normal;             // go back to normal state
                    setIsCharging(false);                       // turn off the charging flag
                    timer = 0;                                  // reset timer
                    flashingTime = 0.2f;
                }
                break;
            case GameController.GameState.Playing:
                if (isCharging)                                      // when the wall is charging
                {
                    timer += Time.deltaTime;
                    if (timer > flashingTime)                        // toggle materials during flashing time
                    {
                        if (OneNorTwoChar == 1)
                        {
                            thisRenderer.material = charging;
                            OneNorTwoChar = 2;
                        }
                        else if (OneNorTwoChar == 2)
                        {
                            thisRenderer.material = normal;
                            OneNorTwoChar = 1;
                        }
                        flashingTime += 0.2f;                       // adds 0.2f until next toggle
                    }
                    if (timer > gameController.getWarningTime())                         // waits for (warning)time
                    {
                        instantiateLaser();                         // shoots laser
                        thisRenderer.material = normal;             // go back to normal state
                        setIsCharging(false);                       // turn off the charging flag
                        timer = 0;                                  // reset timer
                        flashingTime = 0.2f;
                    }
                }
                break;
        }
    }

    void instantiateLaser()                        // creates an instance of a laser
    {
        Vector3 newPosition = gameObject.GetComponent<Transform>().position;
        GameObject thisLaser = (GameObject)Instantiate(Laser, newPosition, Quaternion.identity);
        thisLaser.GetComponent<LaserScript>().setTowards(gameObject.tag);
    }

    public bool getIsCharging()                     // returns a flag for determining if it the wall is charging or not
    {
        return isCharging;
    }

    public void setIsCharging(bool x)               // setting the charging flag on or off
    {
        if (x)
        {
            thisRenderer.material = charging;        // when charging is on, set to charging color;
            OneNorTwoChar = 2;
        }
        isCharging = x;
    }
}
