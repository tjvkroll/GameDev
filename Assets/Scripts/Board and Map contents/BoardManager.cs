using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// NOTE: at around 3000 nodes, this implementation starts to slow down..
// We can optimize and parallelize some aspects of this, and use coroutines if need by.
// But if we ever wanted to go beyond 3000 nodes (55x55 roughly) we would need to look into
// alternate pathfinding items. Its worth pointing out that 3000 nodes is incredibly big, 
// and most fire emblem games only use ~400 nodes (~20x~20)
public class BoardManager : MonoBehaviour
{
    public TileNode[] tileNodes;
    public BoardObject selectedUnit;
    int[,] board;
    public ClickableTile[,] clickableBoard;
    public BoardAdmin admin;
    int mapSizeX = 50;
    int mapSizeY = 50;

    // Pathfinding and Djikstras
    PathNode[,] pathGraph;
    List<PathNode> currentPath = null;
    Dictionary<PathNode, float> dist;
    Dictionary<PathNode, PathNode> prev;

    // BoardUI
    [Header("Board UI")]
    public GameObject Cursor;
    public Transform uiQuadContainer;
    public GameObject[,] QuadMovementUI;
    public GameObject uiQuadPrefab;
    private Color movable = new Color(0, 0, 1, .07f);
    private Color attackable = new Color(1, 0, 0, .07f);
    private Ray ray;
    private RaycastHit hit;

    public void InitializeBoard()
    {
        GenerateMapData();
        GeneratePathFindingGraph();
        GenerateQuadMovementUI(mapSizeX, mapSizeY);
        GenerateMapVisuals();
    }

    // Mostly done For BoardUI.. 
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Cursor.SetActive(true);
            UpdateCursor();
        }
        else
        {
            Cursor.SetActive(false);
        }
    }

    // Setting the current unit (called in BoardAdmin)
    public void OnNewSelection(BoardObject newSelection)
    {
        selectedUnit = newSelection;
        RunDijkstras(selectedUnit.moveSpeed);
        SetMoveableRange(selectedUnit.moveSpeed);
    }

    // clearing selected units
    public void OnClearSelection()
    {
        selectedUnit = null;
        ClearUITiles();
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
        clickableBoard = new ClickableTile[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                TileNode tileNode = tileNodes[board[x, y]];

                GameObject go = (GameObject)Instantiate(tileNode.tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                ClickableTile ct = go.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.board = this;
                clickableBoard[x, y] = ct;
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, 0, y);
    }


    public bool UnitCanEnterTile(int x, int y)
    {

        // Here we could test various unit's hover/fly/walk capablitiies
        // versus terain flags to see if they're allowed in the tile
        if (clickableBoard[x, y].occupant != null || !tileNodes[board[x, y]].isWalkable) { return false; }
        return true;
    }

    // Djikstras, Should be ran each time a character is selected pretty much only once.
    // once the dictionaries are populated, other functions can just use those member functions (prev and dist)
    // TODO: limit the graph traversal by the units max travel distance.. If the unit can only travel 10 squares,
    // no need to check anything beyond that. 
    public void RunDijkstras(int travelDistance)
    {
        // Initializing/resetting components
        dist = new Dictionary<PathNode, float>();
        prev = new Dictionary<PathNode, PathNode>();
        List<PathNode> unvisited = new List<PathNode>();
        PathNode source = pathGraph[
                                selectedUnit.tileX,
                                selectedUnit.tileY
                                ];
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
            unvisited.Remove(u);
            // Updating paths and distances
            foreach (PathNode v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v); // This is if we're doing nominal distance
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
    }

    // This should only be called after we have successfully selected a unit to generate a graph 
    // from. This pulls from the currently set PREV dictionary. 
    public void GeneratePathTo(int x, int y)
    {
        // clearing old path
        selectedUnit.currentPath = null;
        // Dont generate paths to tiles that are inacessible 
        if (UnitCanEnterTile(x, y) == false) return;
        PathNode target = pathGraph[x, y];
        if (prev[target] == null || dist[target] > selectedUnit.moveSpeed)
        {
            // No route between source and target OR the distance is further than possible.
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
        selectedUnit.currentPath = currentPath;
        Debug.Log($"Set path to {x}, {y}");
    }

    // BOARD UI
    public void UpdateCursor()
    {
        if (hit.transform.CompareTag("Tile"))
        {
            int _x = hit.transform.gameObject.GetComponent<ClickableTile>().tileX;
            int _y = hit.transform.gameObject.GetComponent<ClickableTile>().tileY;
            Cursor.transform.position = new Vector3(_x, .501f, _y);
        }
    }

    public void GenerateQuadMovementUI(int _mapSizeX, int _mapSizeY)
    {
        mapSizeX = _mapSizeX;
        mapSizeY = _mapSizeY;
        QuadMovementUI = new GameObject[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                QuadMovementUI[x, y] = Instantiate(
                    uiQuadPrefab, new Vector3(x, .55f, y), Quaternion.Euler(90, 0, 0), uiQuadContainer);
            }
        }
        ClearUITiles();
    }

    private void SetTileColor(int _x, int _y, Color col)
    {
        QuadMovementUI[_x, _y].GetComponent<Renderer>().material.color = col;
    }

    public void SetMoveableRange(int manhattan_distance)
    {
        foreach (KeyValuePair<PathNode, float> nodepair in dist)
        {
            if (nodepair.Value <= manhattan_distance) { SetTileColor(nodepair.Key.x, nodepair.Key.y, movable); }
        }
    }

    public void SetAttackRange(int _x, int _y, int manhattan_distance)
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                float dist = Mathf.Abs(_x - x) + Mathf.Abs(_y - y);
                if (dist < manhattan_distance) { SetTileColor(x, y, attackable); }
                else { SetTileColor(x, y, Color.clear); }
            }
        }
    }

    public void ClearUITiles()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                SetTileColor(x, y, Color.clear);
            }
        }
    }

    public void SetTileTest(int _x, int _y) // Test function
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                float manhattan_distance = Mathf.Abs(_x - x) + Mathf.Abs(_y - y);
                if (manhattan_distance < 3) { SetTileColor(x, y, movable); }
                else if (manhattan_distance < 4) { SetTileColor(x, y, attackable); }
                else { SetTileColor(x, y, Color.clear); }
            }
        }
    }
}
