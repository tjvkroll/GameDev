using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{

    public TileNode[] tileNodes;
    public BoardUI boardUI;
    public GameObject selectedUnit;
    int[,] board;
    int mapSizeX = 10;
    int mapSizeY = 10;
    void Start()
    {
        GenerateMapData();
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

    public void MoveSelectedUnitTo(int x, int y)
    {
        selectedUnit.transform.position = new Vector3(x, 0, y);
        boardUI.SetAttackRange(x, y, 5);
        boardUI.SetMoveableRange(x, y, 3);
    }
}
