﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int[,] map;
    public int roomWidth;
    public int roomHeight;
    public GameObject wallObject;

    private void Update()
    {
        //Create dungeon when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDungeon();
        }
    }

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
        map = new int[_roomWidth,_roomHeight];

        //Loop through map array
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                map[x, y] = 1;
            }
        }
    }

    //Prints map with all tiles needed
    private void PrintMap(int[,] map)
    {
        //Loop through map array
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                if (map[x, y] == 1)
                {
                    Instantiate(wallObject, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }
}
