using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileZ;
    public BoardManager board;
    public BoardObject occupant;  
    void OnMouseUp() {
        // board.GeneratePathTo(tileX,tileZ);
        board.admin.OnSelectTile(this);
    }
}
