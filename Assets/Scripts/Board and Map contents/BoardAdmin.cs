using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn
{
    PLAYER,
    NPC,
    ENEMY,
}

// A simple enum intended to contain the state machine of a piece's operations. repeated until all pieces are inactive.
public enum TurnState
{
    SELECTING, // Select the piece to move? 
    MOVING, // Display movement UI and move piece
    INTERACTION, // Display Interaction MenuUI (Items, attack, etc)
    ENDING, // Maybe not used but to signify things to do on end of turn.
}

public enum PieceOwner
{
    PLAYER,
    NPC,
    ENEMY
}
/**
*   BoardAdmin, controller component and middleman between UI and board and character.
*   
*   Note: Exactly how much should be controlled remains to be discussed.   
*
*   will handle MapGeneration Start,
*   Turn Handling
*   Piece Selection and movement
*   Telling two pieces to battle?
*   - On hover, HOVERUI changes. 
*/

public class BoardAdmin : MonoBehaviour
{
    [Header("Turn")]
    public Turn currentTurn;
    public int turncount;

    [Header("Map & MapUI Handling")]
    public BoardCreator boardCreator;
    public BoardUI boardUI;

    [Header("PieceSelection & Movement")]
    public BoardObject currentlySelectedObject;

    public bool SelectPiece(BoardObject piece)
    {
        if (piece.ownership != PieceOwner.PLAYER)
        {
            return false; // Cannot select piece that isn't your own? NOTE: This will not affect hover UI, only movement selection.
        }
        currentlySelectedObject = piece;
        return true;
    }

    public bool RequestMove(int x, int y)
    {
        // Can selected object move to that location
        return true;
    }

}
