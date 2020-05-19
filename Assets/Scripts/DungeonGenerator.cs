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
    public GameObject wallObject;
    public GameObject floorObject;
    public GameObject pathObject;
    public List<Node> openMazeNodes = new List<Node>();
    public List<Node> closedMazeNodes = new List<Node>();
    public List<Node> pathMazeNodes = new List<Node>();
    System.Random random = new System.Random();
    public bool cancarve = false;
    public Node deleteThis;

    //Game loop
    private void Update()
    {
        //Create dungeon when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Start timer
            var startTime = DateTime.Now.Millisecond;

            //Generate dungeon
            GenerateDungeon();

            //End timer
            var endTime = DateTime.Now.Millisecond;
            print($"Dungeon generated. Took { endTime - startTime } ms");
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (cancarve)
            {
                RecursiveCarve(deleteThis);
                PrintMap(map);
            }
        }
    }

    private void RecursiveCarve(Node randomNodeChosen)
    {
        if (openMazeNodes.Count == 0)
        {
            cancarve = false;
            return;
        }
        pathMazeNodes.Add(randomNodeChosen);
        openMazeNodes.Remove(randomNodeChosen);

        //Scan surrounding nodes and choose a random direction
        //Get number of possible directions
        List<Node> possibleDirections = new List<Node>();
        if (openMazeNodes.Contains(map[randomNodeChosen.x + 1, randomNodeChosen.y]))
        {
            possibleDirections.Add(map[randomNodeChosen.x + 1, randomNodeChosen.y]);
        }
        if (openMazeNodes.Contains(map[randomNodeChosen.x - 1, randomNodeChosen.y]))
        {
            possibleDirections.Add(map[randomNodeChosen.x - 1, randomNodeChosen.y]);
        }
        if (openMazeNodes.Contains(map[randomNodeChosen.x, randomNodeChosen.y - 1]))
        {
            possibleDirections.Add(map[randomNodeChosen.x, randomNodeChosen.y - 1]);
        }
        if (openMazeNodes.Contains(map[randomNodeChosen.x, randomNodeChosen.y + 1]))
        {
            possibleDirections.Add(map[randomNodeChosen.x, randomNodeChosen.y + 1]);
        }

        //We reach a dead end and the current node cannot go in any of the 4 directions
        if (possibleDirections.Count == 0)
        {
            //loop through pathnodes
            foreach (var item in pathMazeNodes)
            {
                //if a tile has 4 adjacent floors
                //Get number of possible directions
                List<Node> openPositions = new List<Node>();
                if (openMazeNodes.Contains(map[item.x + 1, item.y]))
                {
                    openPositions.Add(map[item.x + 1, item.y]);
                }
                if (openMazeNodes.Contains(map[item.x - 1, item.y]))
                {
                    openPositions.Add(map[item.x - 1, item.y]);
                }
                if (openMazeNodes.Contains(map[item.x, item.y - 1]))
                {
                    openPositions.Add(map[item.x, item.y - 1]);
                }
                if (openMazeNodes.Contains(map[item.x, item.y + 1]))
                {
                    openPositions.Add(map[item.x, item.y + 1]);
                }

                if (openPositions.Count != 0)
                {
                    //Choose random node from the possibleDirections list
                    int randomIndex = random.Next(openPositions.Count);
                    deleteThis = openPositions[randomIndex];
                    return;
                }
            }

            //Take random element from open list and start carving
            if (openMazeNodes.Count != 0)
            {
                int randomOpenMazeNodeIndex = random.Next(openMazeNodes.Count);

                deleteThis = openMazeNodes[randomOpenMazeNodeIndex];
            }

            return;
            
            //carve it
            



        }

        //Choose random node from the possibleDirections list
        int randomDirectionIndex = random.Next(possibleDirections.Count);

        //Officially set the node to carve to
        Node carveNodeChosen = possibleDirections[randomDirectionIndex];

        //Switch lists
        pathMazeNodes.Add(carveNodeChosen);
        openMazeNodes.Remove(carveNodeChosen);

        //Set the adjacent tiles to wall, to make way for the "blocky" maze
        //If the "to carve" node is to the RIGHT or LEFT of our starting point
        if (carveNodeChosen.x > randomNodeChosen.x || carveNodeChosen.x < randomNodeChosen.x)
        {
            if (!pathMazeNodes.Contains(map[randomNodeChosen.x, randomNodeChosen.y + 1]))
            {
                closedMazeNodes.Add(map[randomNodeChosen.x, randomNodeChosen.y + 1]);
                openMazeNodes.Remove(map[randomNodeChosen.x, randomNodeChosen.y + 1]);
                map[randomNodeChosen.x, randomNodeChosen.y + 1].currentState = Node.States.Wall;
            }
            if (!pathMazeNodes.Contains(map[randomNodeChosen.x, randomNodeChosen.y - 1]))
            {
                closedMazeNodes.Add(map[randomNodeChosen.x, randomNodeChosen.y - 1]);
                openMazeNodes.Remove(map[randomNodeChosen.x, randomNodeChosen.y - 1]);
                map[randomNodeChosen.x, randomNodeChosen.y - 1].currentState = Node.States.Wall;
            }
        }
        //If the "to carve" node is UP or DOWN of our starting point
        if (carveNodeChosen.y > randomNodeChosen.y || carveNodeChosen.y < randomNodeChosen.y)
        {
            //Switch lists of top and bottom nodes if they're not on path
            if (!pathMazeNodes.Contains(map[randomNodeChosen.x + 1, randomNodeChosen.y]))
            {
                closedMazeNodes.Add(map[randomNodeChosen.x + 1, randomNodeChosen.y]);
                openMazeNodes.Remove(map[randomNodeChosen.x + 1, randomNodeChosen.y]);
                map[randomNodeChosen.x + 1, randomNodeChosen.y].currentState = Node.States.Wall;
            }
            if (!pathMazeNodes.Contains(map[randomNodeChosen.x - 1, randomNodeChosen.y]))
            {
                closedMazeNodes.Add(map[randomNodeChosen.x - 1, randomNodeChosen.y]);
                openMazeNodes.Remove(map[randomNodeChosen.x - 1, randomNodeChosen.y]);
                map[randomNodeChosen.x - 1, randomNodeChosen.y].currentState = Node.States.Wall;
            }
        }
        deleteThis = carveNodeChosen;
    }

    //Generate Dungeon
    private void GenerateDungeon()
    {
        //Reset map if necessary
        ResetMap();

        //Fill map with walls
        FillMap(roomWidth, roomHeight);

        //Generate maze
        GenerateMaze();

        //Print map tiles
        PrintMap(map);
    }

    //ResetMap()
    private void ResetMap()
    {
        //Clear lists
        openMazeNodes.Clear();
        closedMazeNodes.Clear();
        pathMazeNodes.Clear();

        //Destroy all objects
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    //Generate Maze within open node list
    private void GenerateMaze()
    {
        //Choose random node from the open list
        int randomIndex = random.Next(openMazeNodes.Count);

        //Switch lists and turn into wall
        Node randomNodeChosen = openMazeNodes[randomIndex];
        closedMazeNodes.Add(openMazeNodes[randomIndex]);
        pathMazeNodes.Add(randomNodeChosen);
        openMazeNodes.Remove(randomNodeChosen);
        cancarve = true;
        deleteThis = randomNodeChosen;

        
    }

    //Fill map with walls based on width and height
    private void FillMap(int _roomWidth, int _roomHeight)
    {
        //Initialize map variable
        map = new Node[_roomWidth, _roomHeight];

        //Loop through map array
        for (int x = 0; x < roomWidth-1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
            {
                //If on the border, make a wall
                if (x == 0 || y == 0 || x == roomWidth-2 || y == roomHeight-2)
                {
                    map[x, y] = new Node(x, y, Node.States.Wall);
                    closedMazeNodes.Add(map[x, y]);
;                   continue;
                }
                else
                {
                    //Otherwise, make a floor
                    map[x, y] = new Node(x, y, Node.States.Floor);
                    openMazeNodes.Add(map[x, y]);
                }
            }
        }
    }

    //Prints map with all tiles needed
    private void PrintMap(Node[,] map)
    {
        //Destroy all objects
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Loop through map array
        for (int x = 0; x < roomWidth-1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
            {
               if (closedMazeNodes.Contains(map[x,y])) 
                //if (map[x, y].currentState == Node.States.Wall)
                {
                    GameObject gameObject = Instantiate(wallObject, new Vector2(x, y), Quaternion.identity);
                    gameObject.transform.parent = this.transform;
                }
                
                //if (map[x, y].currentState == Node.States.Floor)
                {
                    if (pathMazeNodes.Contains(map[x,y]))
                    {
                        GameObject pathObj = Instantiate(pathObject, new Vector2(x, y), Quaternion.identity);
                        pathObj.transform.parent = this.transform;
                        continue;
                    }
                    if (openMazeNodes.Contains(map[x, y]))
                    {
                        GameObject gameObject = Instantiate(floorObject, new Vector2(x, y), Quaternion.identity);
                        gameObject.transform.parent = this.transform;
                    }
                        
                }
            }
        }
    }
}
