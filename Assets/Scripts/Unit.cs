using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public Vector3 target;
    public Vector3 wonderingTarget;
    public bool goingForPoint;
    public bool goingForSpeed;
    public bool wondering;
    public bool moving;
    public float speedPercentage, lazyPercentage;
    GameController gameC;
    Vector3[] path;
    int targetIndex;

    void Start() {
        speedPercentage = 10f;//Random.Range(0f, 6.5f);
        lazyPercentage = Random.Range(0f, 6.0f);
        goingForPoint = false;
        goingForSpeed = false;
        wondering = false;
        moving = false;
        gameC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public bool goForSpeed(float randomNum) {
        if (randomNum < speedPercentage)
            return true;
        return false;
    }

    public bool goForPoint(float randomNum) {
        if (randomNum < lazyPercentage)
            return true;
        return false;
    }

    public void makeWonderingPathRequest() {
        Debug.Log(tag + " wondering request!");
        pathRequestManager.RequestPath(transform.position, wonderingTarget, OnPathFound);
    }

    public void makePathRequest() {
        pathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        Debug.Log(tag + " " + pathSuccessful);
        if (pathSuccessful) {
            StopCoroutine("FollowPath");
            targetIndex = 0;
            Debug.Log("Stopped Coroutine!");

            path = newPath;

            StartCoroutine("FollowPath");
            Debug.Log("Started Coroutine!");
        }
    }

    IEnumerator FollowPath() {
        moving = true;
        if (path.Length > 0) {
            Vector3 currentWaypoint = path[0];

            while (true) {
                if (transform.position == currentWaypoint) {
                    targetIndex++;                          // advance to next waypoint in the path
                    if (targetIndex >= path.Length) {
                        Debug.Log("done with following path");
                        goingForPoint = false;
                        goingForSpeed = false;
                        wondering = false;
                        moving = false;
                        gameC.AIPathRequests();
                        yield break;                        // exit out the Coroutine
                    }
                    currentWaypoint = path[targetIndex];    // set new way point
                }
                // moving our position move closer to currentWayPoint each frame;
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, GetComponent<AIPlayer>().speed * Time.deltaTime);
                yield return null;  // exit to move over to the next frame
            }
        }
        else {
            goingForPoint = false;
            goingForSpeed = false;
            wondering = false;
            moving = false;
            gameC.AIPathRequests();
        }
        yield break;
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {    // draw the path starting from index so that you don't
                Gizmos.color = Color.black;                     // daw the path that you already traveled
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);   // line that we are moving along.
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

}
