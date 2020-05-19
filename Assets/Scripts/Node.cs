/// <summary>
/// Represents a 2D terrain in the dungeon.
/// </summary>
public class Node
{
    //State enum
    public enum States
    {
        Wall,
        Floor
    }

    //Variables
    public int x;
    public int y;
    public States currentState;

    //Constructor
    public Node(int _x, int _y, States _state)
    {
        x = _x;
        y = _y;
        currentState = _state;
    }

    //Default constructor
    public Node()
    {
        x = 0;
        y = 0;
        currentState = States.Wall;
    }
}
