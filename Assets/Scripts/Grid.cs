using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake(){   // Awake to generate the map before the Unit's Start method
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    public void CreateGrid(){
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x=0; x < gridSizeX; x++){
            for(int y=0; y<gridSizeY; y++){
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); // getting each point our node is going to occupy in the world
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); // true if we don't colide with anything in our walkable path // checsphere returns true if there is a collusion
                grid[x, y] = new Node(walkable, worldPoint, x, y); // creating new node
            }
        }
    }

    public List<Node> GetNeighbours(Node node) {    // returns a list of all neighboring nodes of given node
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {             // checking 3x3 nodes around the given node
            for(int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)               // skipping the given node
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                // check to see if this point is actually inside of a grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {   // figuring out the node that player is currently standing on // converts given world position into nodes
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;       // how far along in the grid it is: far left = 0 center = .5 far right = 1
        float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);     // make sure that array index is valid
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);     // getting x and y indicies of the array
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }


    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
       
        if (grid != null && displayGridGizmos) {
            foreach (Node n in grid) {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;  // if no collision, draw white.  if there is a collision, daw red
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f)); // draw each node
            }
        }
    }
}
