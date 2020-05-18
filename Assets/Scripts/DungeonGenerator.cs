using System;
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

    //Game loop
    private void Update()
    {
        //Create dungeon when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDungeon();
        }
    }

    //Generate Dungeon
    private void GenerateDungeon()
    {
        //Timer
        var startTime = DateTime.Now.Millisecond;

        //Fill map with walls
        FillMap(roomWidth, roomHeight);

        //Print map tiles
        PrintMap(map);

        // Get the elapsed time as a TimeSpan value.
        var endTime = DateTime.Now.Millisecond;
        print($"Dungeon generated. Took { endTime - startTime } ms");
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
                map[x, y] = new Node(x, y, Node.States.Wall);
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
                    Instantiate(wallObject, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }
}
