using UnityEngine;
using System.Collections;

public class PlayerController : PlayersBase
{
    void FixedUpdate()
    {
        if(gameController.playerMovement) {
            float moveHorizontal = Input.GetAxis("Horizontal");                         // controls the movement of the player
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.velocity = movement * speed * (Time.deltaTime * 25);
        }
    }
}
