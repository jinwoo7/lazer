using UnityEngine;
using System.Collections;

public class PlayerController : PlayersBase
{
    Vector3 movement;
    void FixedUpdate() {
        if (gameController.playerMovement) {
            float moveHorizontal = Input.GetAxis("Horizontal");                         // controls the movement of the player
            float moveVertical = Input.GetAxis("Vertical");
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.velocity = movement * speed * (Time.deltaTime * 25);
        }
        else {
            movement = new Vector3(0.0f, 0.0f, 0.0f);
            rb.velocity = movement * speed * (Time.deltaTime * 25);
        }
    }
}
