using UnityEngine;
using System.Collections;

public class PlayerController : PlayersBase
{
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");                         // controls the movement of the player
        float moveVertical = Input.GetAxis("Vertical");
        /*
        if (Input.p(KeyCode.D))
            moveHorizontal = 1;
        else if (Input.GetKeyDown(KeyCode.A))
            moveHorizontal = -1;

        if (Input.GetKeyDown(KeyCode.W))
            moveVertical = 1;
        else if (Input.GetKeyDown(KeyCode.S))
            moveVertical = -1;
        */
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed * (Time.deltaTime * 25);
    }
}
