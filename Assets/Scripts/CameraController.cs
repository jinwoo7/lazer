using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject player;

    private Vector3 offset;
	// Use this for initialization
	void Start () {
        offset = transform.position - player.transform.position;
	}
	
	// LateUpdate is called once per frame just like an update but it is garenteed to run after update is processed
	void LateUpdate () {
        transform.position = player.transform.position + offset;
	}
}
