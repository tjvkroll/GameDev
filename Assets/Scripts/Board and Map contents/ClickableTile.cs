using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public BoardManager board;
    public BoardObject occupant;  
    void OnMouseUp() {
        // board.GeneratePathTo(tileX,tileZ);
        board.admin.OnSelectTile(this);
    }
}
