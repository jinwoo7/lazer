using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public Transform target;
    float speed = 3f;
    Vector3[] path;
    int targetIndex;
    float xLoc;
    float zLoc;

    void Start() {
    }

    public void makePathRequest() {
        pathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        Debug.Log(pathSuccessful);
        if (pathSuccessful) {
            StopCoroutine("FollowPath");
            targetIndex = 0;
            Debug.Log("Stopped Coroutine!");

            path = newPath;
            Debug.Log("Path Length: " + path.Length);
            for (int i = 0; i < path.Length; i++) {
                Debug.Log(path[i]);
            }

            
            StartCoroutine("FollowPath");
            Debug.Log("Started Coroutine!");
        }
    }

    IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        while (true) {
            if(transform.position == currentWaypoint) {
                xLoc = transform.position.x;
                zLoc = transform.position.z;
                targetIndex++;                          // advance to next waypoint in the path
                if(targetIndex >= path.Length) {
                    Debug.Log("done with following path");
                    yield break;                        // exit out the Coroutine
                }
                currentWaypoint = path[targetIndex];    // set new way point
            }
            // moving our position move closer to currentWayPoint each frame;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed* Time.deltaTime);
            yield return null;  // exit to move over to the next frame
        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {    // draw the path starting from index so that you don't
                Gizmos.color = Color.black;                     // daw the path that you already traveled
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);   // line that we are moving along.
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public void setxLoc(float newX) {
        xLoc = newX;
    }

    public void setzLoc(float newZ) {
        zLoc = newZ;
    }
}
