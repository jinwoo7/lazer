using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
    float xLoc;
    float zLoc;

    public float getxLoc() {
        return xLoc;
    }

    public float getzLoc() {
        return zLoc;
    }

    public void setxLoc(float newX) {
        xLoc = newX;
    }

    public void setzLoc(float newZ) {
        zLoc = newZ;
    }
}
