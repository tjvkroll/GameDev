using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    SELECTION,
    MOVEMENT,
    INTERACT,     // CAN BREAK UP LATER
}

public class BoardAdmin : MonoBehaviour
{
    public BoardObject currentlySelectedObject;
    public BoardManager boardManager;
    public PieceManager pieceManager;
    public CameraController Cam;
    public TurnState currentState;

    // INITALIZERS
    void Start()
    {
        InitializeAll();
        currentState = TurnState.SELECTION;
    }

    public void InitializeAll()
    {
        boardManager.InitializeBoard();
        pieceManager.InitializePieces();
    }

    // GENERAL
    public bool OnSelectTile(ClickableTile tile)
    {
        switch (currentState)
        {
            case TurnState.SELECTION:
                OnSelectSelection(tile);
                break;
            case TurnState.MOVEMENT:
                OnSelectMovement(tile);
                break;
            case TurnState.INTERACT:
                break;
        };
        return true;
    }

    public bool OnClearSelectObject()
    {
        switch (currentState)
        {
            case TurnState.SELECTION:
                OnClearSelectSelection();
                break;
            case TurnState.MOVEMENT:
                break;
            case TurnState.INTERACT:
                break;
        };
        return true;
    }

    public void OnConfirmSelect()
    {
        switch (currentState)
        {
            case TurnState.SELECTION:
                break;
            case TurnState.MOVEMENT:
                OnConfirmMovement();
                break;
            case TurnState.INTERACT:
                break;
        }
    }

    void StartState(TurnState newstate)
    {
        EndState();
        currentState = newstate;
        switch (currentState)
        {
            case TurnState.SELECTION:
                break;
            case TurnState.MOVEMENT:
                Cam.MoveToUnit(currentlySelectedObject.transform);
                boardManager.OnNewSelection(currentlySelectedObject);
                break;
            case TurnState.INTERACT:
                break;
        }
    }

    void EndState()
    {
        switch (currentState)
        {
            case TurnState.SELECTION:
                break;
            case TurnState.MOVEMENT:
                break;
            case TurnState.INTERACT:
                break;
        }
    }

    void GotoPreviousState()
    {
        switch (currentState)
        {
            case TurnState.SELECTION:
                break;
            case TurnState.MOVEMENT:
                StartState(TurnState.SELECTION);
                break;
            case TurnState.INTERACT:
                StartState(TurnState.MOVEMENT);
                break;
        }
    }


    // SELECTION
    bool OnSelectSelection(ClickableTile tile)
    {
        //if (!selectionCandidate.belongstoplayer) return false;
        if (tile.occupant == null) { return false; }
        currentlySelectedObject = tile.occupant;
        StartState(TurnState.MOVEMENT);
        return true;
    }

    bool OnClearSelectSelection()
    {
        currentlySelectedObject = null;
        boardManager.OnClearSelection();
        return true;
    }

    // MOVEMENT
    bool OnSelectMovement(ClickableTile destination)
    {
        // is tile occupied
        boardManager.GeneratePathTo(destination.tileX, destination.tileY);
        return true;
    }

    bool OnConfirmMovement()
    {
        if (currentlySelectedObject.currentPath == null) return false;
        currentlySelectedObject.MoveNextTile();
        boardManager.ClearUITiles(); // Clear tiles after movement is confirmed.
        StartState(TurnState.INTERACT);
        return true;
    }

}
