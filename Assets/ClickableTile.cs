using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileZ;
    public BoardCreator board; 
    void OnMouseUp(){
        Debug.Log("Click"); 
        board.MoveSelectedUnitTo(tileX,tileZ); 
    }
}
