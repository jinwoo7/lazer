using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
    }
}
