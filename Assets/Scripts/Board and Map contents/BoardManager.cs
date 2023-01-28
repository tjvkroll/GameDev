using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour{

    public TileNode[] tileNodes; 
    public BoardObject selectedUnit; 
    int[,] board; 
    public ClickableTile[,] clickableBoard; 
    PathNode[,] pathGraph;
    public BoardAdmin admin; 
    List<PathNode> currentPath = null; 
    int mapSizeX = 10;  
    int mapSizeY = 10;

    public void InitializeBoard() {
        GenerateMapData();
        GeneratePathFindingGraph();
        GenerateMapVisuals();
    }
    

    // Setting the current unit (called in BoardAdmin)
    public void OnNewSelection(BoardObject newSelection){
        selectedUnit = newSelection; 
        //Draw new movement UI for new unit
    }

    // clearing selected units
    public void OnClearSelection(){
        // Clear movement UI
        selectedUnit = null;
    }

    //Builds the structure of the map. Roads, Mountains, etc.
    void GenerateMapData() {
        //allocate memory for board
        board = new int[mapSizeX, mapSizeY]; 
        
        //  AREA MAPPPING 
        //  Forest - 0
        //  Grassland - 1
        //  Road -2
        //  Mountain - 3
        //  Swamp - 4

        //Initialize map to forest to start
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                board[x,y] = 0; 
            }
        }


        // build a smiley face
        board[3,2] = 4;
        board[3,3] = 4;
        board[3,4] = 4; 
        board[7,2] = 4;
        board[7,3] = 4;
        board[7,4] = 4;
        board[2,6] = 3;
        board[2,7] = 3;
        board[8,6] = 3;
        board[8,7] = 3;
        board[7,7] = 3;
        board[6,7] = 3;
        board[5,7] = 3;
        board[4,7] = 3;
        board[3,7] = 3; 

    }

    //Calculates cost to enter tile 
    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY){
        TileNode tt = tileNodes[ board[targetX,targetY] ];
        float cost = tt.movementCost; 

        if(UnitCanEnterTile(targetX, targetY) == false){
            return Mathf.Infinity; 
        }

        // Movement Normalization for Diagonal movement option        
        // if(sourceX!=targetX && sourceY!=targetY){
        //     cost += 0.001f; 
        // }   
        return cost;  
    }

    // Builds a Graph used to pathfind in the game
    void GeneratePathFindingGraph(){
        // initializes array
        pathGraph = new PathNode[mapSizeX, mapSizeY]; 
        
        // inistializes nodes
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                pathGraph[x,y] = new PathNode();
                pathGraph[x,y].x = x; 
                pathGraph[x,y].y = y; 
            }        
        }
        
        // initializes neighbors   
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){                
                // We have a 4 way graph but this also works with
                // 6-way hexes and 8-ways tiles, etc. 
                
                // filling adjacency list for path graph
                // Euclidian Distance (4 - way movement)
                if (x > 0){
                    pathGraph[x,y].neighbours.Add(pathGraph[x-1, y]);
                }
                if(x < mapSizeX-1){
                    pathGraph[x,y].neighbours.Add(pathGraph[x+1, y]);
                }
                if (y > 0){
                    pathGraph[x,y].neighbours.Add(pathGraph[x, y-1]);
                }
                if(y < mapSizeY-1){
                    pathGraph[x,y].neighbours.Add(pathGraph[x, y+1]);
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
    void GenerateMapVisuals(){
        clickableBoard = new ClickableTile[mapSizeX,mapSizeY];
        for(int x = 0; x < mapSizeX; x++){
            for(int y = 0; y < mapSizeY; y++){
                TileNode tileNode = tileNodes[board[x,y]]; 
                
                GameObject go = (GameObject)Instantiate( tileNode.tileVisualPrefab, new Vector3(x,0,y), Quaternion.identity, transform);
                ClickableTile ct = go.GetComponent<ClickableTile>();
                ct.tileX = x; 
                ct.tileY = y;
                ct.board = this; 
                clickableBoard[x,y] = ct; 
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y){
        return new Vector3(x,0,y); 
    }


    public bool UnitCanEnterTile(int x, int y){

        // Here we could test various unit's hover/fly/walk capablitiies
        // versus terain flags to see if they're allowed in the tile
        if(clickableBoard[x,y].occupant != null || !tileNodes[ board[x,y] ].isWalkable){ return false; }
        return true; 
    }

    // Generates a path from current selected unit to selected target. 
    public void GeneratePathTo(int x, int y){
        // clearing old path
        selectedUnit.currentPath = null;  

        // Dont generate paths to tiles that are inacessible 
        if(UnitCanEnterTile(x,y) == false){
            return; 
        }

        // Djikstras for movement
        
        // Initializing components
        Dictionary<PathNode, float> dist = new Dictionary<PathNode, float>();
        Dictionary<PathNode, PathNode> prev = new Dictionary<PathNode, PathNode>();
        List<PathNode> unvisited = new List<PathNode>(); 
        PathNode source = pathGraph[
                                selectedUnit.tileX,
                                selectedUnit.tileY
                                ];
        PathNode target = pathGraph[x, y];       
        dist[source] = 0; 
        prev[source] = null;

        // defaulting value
        foreach(PathNode v in pathGraph){
            if(v != source){
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }
        
        // Implementation of djikstras
        while(unvisited.Count > 0){
            // U is an unvisited node with the smallest distance
            PathNode u = null;
            foreach(PathNode possibleU in unvisited){
                if(u == null || dist[possibleU] < dist[u]){
                    u = possibleU; 
                }
            }

            // allows the loop to terminate early if our target distance has been reached
            if (u == target){
                break; 
            }
            unvisited.Remove(u);

            // Updating paths and distances
            foreach(PathNode v in u.neighbours){
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                if(alt < dist[v]){
                    dist[v] = alt;
                    prev[v] = u; 
                }
            }
        }

        if(prev[target] == null){
            // No route between source and target
            return; 
        }

        currentPath = new List<PathNode>(); 
        PathNode curr = target; 

        // If a path exists then work back from target adding route from target to source
        while(curr != null){
            currentPath.Add(curr); 
            curr = prev[curr];         
        }
        currentPath.Reverse();
        selectedUnit.currentPath = currentPath;  
        Debug.Log($"Set path to {x}, {y}");
    }
}
