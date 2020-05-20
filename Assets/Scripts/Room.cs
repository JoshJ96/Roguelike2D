using System.Collections.Generic;
/// <summary>
/// Represents a room of Nodes.
/// </summary>
public class Room
{
    //Size variables
    public int xMin, xMax, yMin, yMax;
    public int width, height;

    //List of floor nodes inside the room
    public List<Node> floorNodesInsideRoom;

    //Constructor
    public Room(int _startPosX, int _startPosY, int _width, int _height)
    {
        xMin = _startPosX;
        yMin = _startPosY;
        width = _width;
        height = _height;
        xMax = _startPosX + width;
        yMax = _startPosY + height;
        floorNodesInsideRoom = new List<Node>();
    }
}
