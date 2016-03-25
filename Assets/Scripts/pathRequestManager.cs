using UnityEngine;
using System.Collections;
using System.Collections.Generic;   // for using queue
using System;                       // for using "Action"

public class pathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static pathRequestManager instance;
    pathfinding pathfinding;

    bool isProcessingPath;


    void Awake() {
        instance = this;
        pathfinding = GetComponent<pathfinding>();
    }

    /* spreading out requests into number of frames */
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    /* check to see if currently processing a path.
       if not, ask the pathfinding script to process the next one*/
    void TryProcessNext() {
        if (!isProcessingPath && pathRequestQueue.Count > 0) {   // if not processing and the queue is not empty
            currentPathRequest = pathRequestQueue.Dequeue();     // gets a request from the queue
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    /* will be called by the pathfinding script once it finish finding the path*/
    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
