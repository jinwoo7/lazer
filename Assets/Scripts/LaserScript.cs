using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
    public Rigidbody rb;
    public float speed;

    Transform thisObject;
    private string towards;
    private bool destroyIsReady;
    Vector3 newPosition;

    // Use this for initialization
    void Start () {
        destroyIsReady = false;                                     // destroy flag is off
        thisObject = gameObject.GetComponent<Transform>();          // setting its speed and direction
        if (towards == "EastWall"){
            rb.velocity = transform.right * speed;
        }
        else if (towards == "WestWall"){
            rb.velocity = transform.right * -speed;
        }
        else if (towards == "NorthWall"){
            rb.velocity = transform.forward * speed;
        }
        else if (towards == "SouthWall"){
            rb.velocity = transform.forward * -speed;
        }
    }

    void Update()
    {
        if (destroyIsReady && isCloseToEnd())                       // destroy flag is on and it is at the end
            Destroy(gameObject);
    }

	void OnTriggerEnter(Collider other)                             // gets the position of the target and raise the flag to be destroyed
    {
        if(other.tag == towards || other.tag == "Rock")
        {
            newPosition = other.GetComponent<Transform>().position;
            destroyIsReady = true;
        }
    }

    bool isCloseToEnd()                                         // calculates to see if the laser has traveled far enough
    {
        bool xIsClose = false;
        bool zIsClose = false;
        
        if (towards == "NorthWall"){
            xIsClose = true;
            if (thisObject.position.z >= newPosition.z)
                zIsClose = true;
        }

        else if (towards == "SouthWall"){
            xIsClose = true;
            if (thisObject.position.z <= newPosition.z)
                zIsClose = true;
        }

        else if (towards == "EastWall"){
            zIsClose = true;
            if (thisObject.position.x >= newPosition.x)
                xIsClose = true;
        }

        else if (towards == "WestWall"){
            zIsClose = true;
            if (thisObject.position.x <= newPosition.x)
                xIsClose = true;
        }
        return (xIsClose && zIsClose);
    }

    public string getTowards()                          // returns where it is headed
    {
        return towards;
    }

    public void setTowards(string x)                    // sets where it is headed according to the input
    {
        if (x == "NorthWall")
            towards = "SouthWall";

        else if (x == "SouthWall")
            towards = "NorthWall";

        else if (x == "EastWall")
            towards = "WestWall";

        else if (x == "WestWall")
            towards = "EastWall";
    }
}
