using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the dungeon at the beginning of the scene
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    //Variables
    public Node[,] map;
    public int roomWidth;
    public int roomHeight;
    System.Random random = new System.Random();
    List<Node> openNodes = new List<Node>();
    List<Node> closedNodes = new List<Node>();
    List<Node> pivotNodes = new List<Node>();
    public GameObject wallObject;
    public GameObject floorObject;

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    //Game loop
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDungeon();
        }
    }

    //Generate Dungeon
    private void GenerateDungeon()
    {
        //Start timer
        var startTime = DateTime.Now.Millisecond;

        //Fill map with walls
        FillMap();

        //Generate maze
        GenerateMaze();

        //Print map tiles
        PrintMap();

        //End timer
        var endTime = DateTime.Now.Millisecond;
        print($"Dungeon generated. Took { endTime - startTime } ms");
    }

    //Fill map with walls based on width and height
    private void FillMap()
    {
        //Make sure room values are odd
        if (roomWidth % 2 == 0)     {roomWidth--;}
        if (roomHeight % 2 == 0)    {roomHeight--;}

        //Initialize map variable
        map = new Node[roomWidth, roomHeight];

        //Loop through map array
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                //Create the node mapped to the 2D array
                Node node = new Node(x, y, Node.States.Wall);
                map[x, y] = node;

                //Decide which list to put this node on.

                //If we're on a room border, add it to the closed nodes list
                if (x == 0 || y == 0 || x == roomWidth - 1 || y == roomHeight - 1)
                {
                    closedNodes.Add(node);
                    continue;
                }

                //Otherwise, add it to the open list
                openNodes.Add(node);            
            }
        }
    }

    //Generate Maze within open node list
    private void GenerateMaze()
    {
        //Choose a random node from open list and set it to a floor
        Node node = new Node ();

        //The node's X and Y coordinate must be an ODD number
        while (node.x % 2 == 0 || node.y % 2 == 0)
        {
            int randomIndex = random.Next(openNodes.Count);
            node = openNodes[randomIndex];
        }
        node.currentState = Node.States.Floor;

        //Change node to closed list
        pivotNodes.Add(node);
        openNodes.Remove(node);

        //Start the recursion
        RecursiveCarve(node);
    }

    private void RecursiveCarve(Node node)
    {
        //Get the possible directions from that node
        List<Directions> directions = GetPossibleDirections(node);

        //Choose one of these random directions
        int randomIndex = random.Next(directions.Count);

        //If there's no directions we can go (dead end)
        if (directions.Count == 0)
        {
            List<Node> pivotToRemove = new List<Node>();
            //Loop through the "pivot points"
            foreach (var item in pivotNodes)
            {
                //Pivot directions list
                List<Directions> pivotDirections = GetPossibleDirections(item);

                //If a pivot point has no possible directions, queue it to remove from list
                if (pivotDirections.Count == 0)
                {
                    pivotToRemove.Add(item);
                }
            }

            //Delete all pivots that can't be used
            foreach (var item in pivotToRemove)
            {
                pivotNodes.Remove(item);
            }

            //If there's still usable pivot nodes to use
            if (pivotNodes.Count != 0)
            {
                //Choose a random pivot from the remaining list
                int randomPivotIndex = random.Next(pivotNodes.Count);
                Node randomPivotNode = pivotNodes[randomPivotIndex];
                RecursiveCarve(randomPivotNode);
            }
 
            //End of the maze generation
            return;
        }

        //Get list of the 2 nodes we're carving
        List<Node> nodesToCarve = GetNodesToCarve(directions[randomIndex], node);

        //nodesToCarve[0] is the node we're GOING TO
        //nodesToCarve[1] is the middle 'even number' node. Todo: reverse these? lol

        //Add the even "middle man" to the closed list
        Node middleNode = nodesToCarve[1];
        middleNode.currentState = Node.States.Floor;
        closedNodes.Add(middleNode);
        openNodes.Remove(middleNode);

        //Add the odd "pivot point" to the pathnodes list
        Node nextNode = nodesToCarve[0];
        nextNode.currentState = Node.States.Floor;
        pivotNodes.Add(nextNode);
        openNodes.Remove(nextNode);

        //Start the recursive carve at the newly carved node
        RecursiveCarve(nextNode);
    }

    //Returns a list of possible directions the node can travel
    private List<Directions> GetPossibleDirections(Node node)
    {
        //Instantiate the list
        List<Directions> directions = new List<Directions> {Directions.Up, Directions.Down, Directions.Left, Directions.Right };

        //Check room bounds, and check for a floor node

        //Remove the possibility of moving right
        if (node.x + 2 >= roomWidth-1 || map[node.x + 2, node.y].currentState == Node.States.Floor)
        {
            directions.Remove(Directions.Right);
        }
        //Remove the possibility of moving right
        if (node.x - 2 <= 0 || map[node.x - 2, node.y].currentState == Node.States.Floor)
        {
            directions.Remove(Directions.Left);
        }
        //Remove the possibility of moving up
        if (node.y + 2 >= roomHeight-1 || map[node.x, node.y + 2].currentState == Node.States.Floor)
        {
            directions.Remove(Directions.Up);
        }
        //Remove the possibility of moving down
        if (node.y - 2 <= 0 || map[node.x, node.y - 2].currentState == Node.States.Floor)
        {
            directions.Remove(Directions.Down);
        }

        //Return the refined list of directions we indeed can move
        return directions;
    }

    //Pass in a direction to go with a node, and output the nodes we'll carve
    private List<Node> GetNodesToCarve(Directions direction, Node node)
    {
        //To save confusion, this is a list of exactly TWO nodes to carve
        List<Node> nodes = new List<Node>();

        //Get these two nodes based on direction given
        switch (direction)
        {
            case Directions.Up:
                nodes.Add(map[node.x, node.y + 2]);
                nodes.Add(map[node.x, node.y + 1]);
                break;
            case Directions.Down:
                nodes.Add(map[node.x, node.y - 2]);
                nodes.Add(map[node.x, node.y - 1]);
                break;
            case Directions.Left:
                nodes.Add(map[node.x - 2, node.y]);
                nodes.Add(map[node.x - 1, node.y]);
                break;
            case Directions.Right:
                nodes.Add(map[node.x + 2, node.y]);
                nodes.Add(map[node.x + 1, node.y]);
                break;
            default:
                break;
        }
        return nodes;
    }

    //Prints map with all tiles needed
    private void PrintMap()
    {
        //Reset the lists
        openNodes.Clear();
        closedNodes.Clear();
        pivotNodes.Clear();

        //Destroy all objects if necessary
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Loop through map array
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (map[x, y].currentState == Node.States.Wall)
                {
                    GameObject gameObject = Instantiate(wallObject, new Vector2(x, y), Quaternion.identity);
                    gameObject.transform.parent = this.transform;
                }
                
                if (map[x, y].currentState == Node.States.Floor)
                {
                    GameObject gameObject = Instantiate(floorObject, new Vector2(x, y), Quaternion.identity);
                    gameObject.transform.parent = this.transform;    
                }
            }
        }
    }
}
