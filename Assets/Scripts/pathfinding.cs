using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;   // for checking efficientcy 
using System;               // for reversing path

public class pathfinding : MonoBehaviour {

    pathRequestManager requestManager;
    Grid grid;

    void Awake() {
        requestManager = GetComponent<pathRequestManager>();
        grid = GetComponent<Grid>();
    }
    
    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();                         //  starts the stopwatch of efficientcy check
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {    // only find path if both start and end is walkable
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                // if a node in the openset has lower fcost then it becomes the current node
                // if a node in the openset = current node, then compare h cost to see who is closer to the end node
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {         // found our path !!
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {       // gets all neighbouring nodes of current node
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) {    // skips all non-walkables and nodes in closedSet
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    // check if new path to neighbour is shorter OR nieghbour is not in open
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;               // re-calculating the neighbour's costs
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;                             // re-parenting the neighbour's parent

                        if (!openSet.Contains(neighbour))                           // if it's not in open set add it in
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null; // wait for one frame before returning
        if (pathSuccess) {      // if the path finding was an sucess!
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;                 // tracing back words

        while (currentNode != startNode) {          // adding the path starting from the end node
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);                        // reversing the path to the right way
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) {            // Simplifiying path by removing redundancy
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero; // Vector2 used to store the directions of last two nodes
        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) {      // getting the distance between the two nodes
        // diagonal cost = 14
        // adjacent cost = 10
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
