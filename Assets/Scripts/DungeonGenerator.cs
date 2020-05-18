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
    public List<Node> openMazeNodes = new List<Node>();
    public List<Node> closedMazeNodes = new List<Node>();

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

        //Destroy all objects
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    //Generate Maze. Keeps 1 node outline on map
    private void GenerateMaze()
    {
        //Choose random node from the open list
        System.Random random = new System.Random();
        int randomIndex = random.Next(openMazeNodes.Count);

        openMazeNodes[randomIndex].currentState = Node.States.Wall;
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
        //Loop through map array
        for (int x = 0; x < roomWidth-1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
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
