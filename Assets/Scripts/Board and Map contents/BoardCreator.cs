using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{

    public TileNode[] tileNodes;
    public GameObject selectedUnit;
    int[,] board;
    PathNode[,] pathGraph;
    List<PathNode> currentPath = null;
    int mapSizeX = 10;
    int mapSizeY = 10;

    public BoardUI boardUI;

    void Start()
    {
        // Setup the selectedUnit's variable
        selectedUnit.GetComponent<BoardObject>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<BoardObject>().tileZ = (int)selectedUnit.transform.position.z;
        selectedUnit.GetComponent<BoardObject>().map = this;

        GenerateMapData();
        GeneratePathFindingGraph();
        boardUI.GenerateQuadMovementUI(mapSizeX, mapSizeY);
        GenerateMapVisuals();
    }

    //Builds the structure of the map. Roads, Mountains, etc.
    void GenerateMapData()
    {
        //allocate memory for board
        board = new int[mapSizeX, mapSizeY];

        //  AREA MAPPPING 
        //  Forest - 0
        //  Grassland - 1
        //  Road -2
        //  Mountain - 3
        //  Swamp - 4

        //Initialize map to forest to start
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                board[x, y] = 0;
            }
        }


        // build a smiley face
        board[3, 2] = 4;
        board[3, 3] = 4;
        board[3, 4] = 4;
        board[7, 2] = 4;
        board[7, 3] = 4;
        board[7, 4] = 4;
        board[2, 6] = 3;
        board[2, 7] = 3;
        board[8, 6] = 3;
        board[8, 7] = 3;
        board[7, 7] = 3;
        board[6, 7] = 3;
        board[5, 7] = 3;
        board[4, 7] = 3;
        board[3, 7] = 3;

    }

    //Calculates cost to enter tile 
    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {
        TileNode tt = tileNodes[board[targetX, targetY]];
        float cost = tt.movementCost;

        if (UnitCanEnterTile(targetX, targetY) == false)
        {
            return Mathf.Infinity;
        }

        // Movement Normalization for Diagonal movement option        
        // if(sourceX!=targetX && sourceY!=targetY){
        //     cost += 0.001f; 
        // }   
        return cost;
    }

    // Builds a Graph used to pathfind in the game
    void GeneratePathFindingGraph()
    {
        // initializes array
        pathGraph = new PathNode[mapSizeX, mapSizeY];

        // inistializes nodes
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                pathGraph[x, y] = new PathNode();
                pathGraph[x, y].x = x;
                pathGraph[x, y].y = y;
            }
        }

        // initializes neighbors   
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                // We have a 4 way graph but this also works with
                // 6-way hexes and 8-ways tiles, etc. 

                // filling adjacency list for path graph
                // Euclidian Distance (4 - way movement)
                if (x > 0)
                {
                    pathGraph[x, y].neighbours.Add(pathGraph[x - 1, y]);
                }
                if (x < mapSizeX - 1)
                {
                    pathGraph[x, y].neighbours.Add(pathGraph[x + 1, y]);
                }
                if (y > 0)
                {
                    pathGraph[x, y].neighbours.Add(pathGraph[x, y - 1]);
                }
                if (y < mapSizeY - 1)
                {
                    pathGraph[x, y].neighbours.Add(pathGraph[x, y + 1]);
                }

                // Optional: 8-Way movement (includes diagonal movment )
                // If wanted also uncomment movement normalization in: CostToEnterTile


                // // Try left
                // if (x > 0){
                //     pathGraph[x,y].neighbours.Add(pathGraph[x-1, y]);
                //     if (y > 0){
                //         pathGraph[x,y].neighbours.Add(pathGraph[x-1, y-1]);
                //     }
                //     if(y < mapSizeY-1){
                //         pathGraph[x,y].neighbours.Add(pathGraph[x-1, y+1]);
                //     }
                // }
                // // Try Right
                // if(x < mapSizeX-1){
                //     pathGraph[x,y].neighbours.Add(pathGraph[x+1, y]);
                //     if (y > 0){
                //         pathGraph[x,y].neighbours.Add(pathGraph[x+1, y-1]);
                //     }
                //     if(y < mapSizeY-1){
                //         pathGraph[x,y].neighbours.Add(pathGraph[x+1, y+1]);
                //     }
                // }
                // // Try straight up or down
                // if (y > 0){
                //     pathGraph[x,y].neighbours.Add(pathGraph[x, y-1]);
                // }
                // if(y < mapSizeY-1){
                //     pathGraph[x,y].neighbours.Add(pathGraph[x, y+1]);
                // }
            }
        }

    }


    // Generates Map in the game
    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                TileNode tileNode = tileNodes[board[x, y]];

                GameObject go = (GameObject)Instantiate(tileNode.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                ClickableTile ct = go.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileZ = y;
                ct.board = this;
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int z)
    {
        return new Vector3(x, 0, z);
    }


    public bool UnitCanEnterTile(int x, int z)
    {

        // Here we could test various unit's hover/fly/walk capablitiies
        // versus terain flags to see if they're allowed in the tile

        return tileNodes[board[x, z]].isWalkable;
    }

    // Generates a path from current selected unit to selected target. 
    public void GeneratePathTo(int x, int z)
    {
        // clearing old path
        selectedUnit.GetComponent<BoardObject>().currentPath = null;

        // Dont generate paths to tiles that are inacessible 
        if (UnitCanEnterTile(x, z) == false)
        {
            return;
        }

        // Djikstras for movement

        // Initializing components
        Dictionary<PathNode, float> dist = new Dictionary<PathNode, float>();
        Dictionary<PathNode, PathNode> prev = new Dictionary<PathNode, PathNode>();
        List<PathNode> unvisited = new List<PathNode>();
        PathNode source = pathGraph[
                                selectedUnit.GetComponent<BoardObject>().tileX,
                                selectedUnit.GetComponent<BoardObject>().tileZ
                                ];
        PathNode target = pathGraph[x, z];
        dist[source] = 0;
        prev[source] = null;

        // defaulting value
        foreach (PathNode v in pathGraph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }

        // Implementation of djikstras
        while (unvisited.Count > 0)
        {
            // U is an unvisited node with the smallest distance
            PathNode u = null;
            foreach (PathNode possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            // allows the loop to terminate early if our target distance has been reached
            if (u == target)
            {
                break;
            }
            unvisited.Remove(u);

            // Updating paths and distances
            foreach (PathNode v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if (prev[target] == null)
        {
            // No route between source and target
            return;
        }

        currentPath = new List<PathNode>();
        PathNode curr = target;

        // If a path exists then work back from target adding route from target to source
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();
        selectedUnit.GetComponent<BoardObject>().currentPath = currentPath;
    }
}
