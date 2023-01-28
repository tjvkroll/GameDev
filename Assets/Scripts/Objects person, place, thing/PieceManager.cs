using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public List<BoardObject> playerUnits;
    public List<BoardObject> enemyUnits;  
    public List<BoardObject> npcUnits;
    public BoardManager map;
    public GameObject playerPrefab; // MAKEDYNAMIC


    // initializes unit lists and sets players starting squad
    public void PieceInit(List<BoardObject> s_playerUnits, List<BoardObject> s_enemyUnits, List<BoardObject> s_npcUnits){
        playerUnits = s_playerUnits;
        enemyUnits = s_enemyUnits; 
        npcUnits = s_npcUnits;  
    }

    public void InitializePieces() {
        playerUnits = new List<BoardObject>();
        enemyUnits = new List<BoardObject>();
        npcUnits = new List<BoardObject>();
        SpawnPiece(0,0, playerPrefab);
        SpawnPiece(0,1, playerPrefab);
    }
    
    public bool SpawnPiece(int tileX, int tileY, GameObject piece) {
        // If something is there, we cant spawn a piece.
        if (map.clickableBoard[tileX, tileY].occupant != null) return false;
        Vector3 worldCoord = map.TileCoordToWorldCoord(tileX, tileY);
        GameObject go = Instantiate(piece, worldCoord, Quaternion.identity);
        BoardObject gobo = go.GetComponent<BoardObject>();
        gobo.map = map;
        map.clickableBoard[tileX, tileY].occupant = gobo;
        gobo.tileX = tileX;
        gobo.tileY = tileY;
        playerUnits.Add(gobo); // CHANGE TO BE DIFFERENT TYPES
        return true;
    }
}
