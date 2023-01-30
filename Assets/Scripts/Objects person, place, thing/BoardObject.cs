using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public BoardManager map;
    //holds path for object if needed
    public List<PathNode> currentPath = null;
    //public string objName;
    public int moveSpeed = 2;

    void Update()
    {
        if (currentPath != null)
        {
            int currNode = 0;
            while (currNode < currentPath.Count - 1)
            {

                // Although we're feeding .y attributes here 
                // it's being converted to z in TileCoordToWorldCoord
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, .51f, 0);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) + new Vector3(0, .51f, 0);

                Debug.DrawLine(start, end, Color.red);
                currNode++;
            }
        }
    }

    public void MoveNextTile()
    {
        float remainingMovement = moveSpeed;
        while (remainingMovement > 0)
        {
            if (currentPath == null)
            {
                return;
            }

            // Get cost from current tile to next tile
            remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);
            // Now grab the new first node and move us to that position
            map.clickableBoard[tileX, tileY].occupant = null;
            tileX = currentPath[1].x;
            tileY = currentPath[1].y;
            map.clickableBoard[tileX, tileY].occupant = this;
            transform.position = map.TileCoordToWorldCoord(tileX, tileY);
            // Remove old current/first node from the path. 
            currentPath.RemoveAt(0);

            // We only have one tile left in the path and that tile must 
            // be our target so we clear our info. 
            if (currentPath.Count == 1)
            {
                currentPath = null;
            }
        }
    }

}
