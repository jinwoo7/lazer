using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public Transform target;
    float speed = 20f;
    Vector3[] path;
    int targetIndex;

    void Start() {
    }

    public void makePathRequest() {
        Debug.Log("in makePathRequest");
        Debug.Log("player 2: " + transform.position);
        Debug.Log("item : " + target.position);
        pathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        Debug.Log("pathSuccessful: "+ pathSuccessful);
        if (pathSuccessful) {
            path = newPath;
            Debug.Log("target position: " + target.position);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        while (true) {
            if(transform.position == currentWaypoint) {
                targetIndex++;                          // advance to next waypoint in the path
                if(targetIndex >= path.Length) {
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
}
