using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug : MonoBehaviour
{
    public GameObject dungeonGenerator;
    public DungeonGenerator dunGen;
    // Start is called before the first frame update
    void Start()
    {
        dunGen = dungeonGenerator.GetComponent<DungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        int x = 0, y = 0;
        if (dunGen.deleteThis != null)
        {
            x = dunGen.deleteThis.x; y = dunGen.deleteThis.y;
        }

        GetComponent<Text>().text =
            $"Closed Nodes List Count: {dunGen.closedMazeNodes.Count}\n" +
            $"Open Nodes List Count: {dunGen.openMazeNodes.Count}\n" +
            $"Path Nodes List Count: {dunGen.pathMazeNodes.Count}\n" +
            $"Current path node carve: {x},{y}";
    }
}
