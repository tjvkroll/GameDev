using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    public GameObject Cursor;
    public Transform uiQuadContainer;
    public GameObject[,] QuadMovementUI;
    public GameObject uiQuadPrefab;
    int mapSizeX = 10;
    int mapSizeY = 10;
    private Color movable = new Color(0, 0, 1, .07f);
    private Color attackable = new Color(1, 0, 0, .07f);


    private Ray ray;
    private RaycastHit hit;

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

    // Maybe move this cursor stuff to a game manager that tells everything else? 
    // or maybe boardcreator
    public void UpdateCursor()
    {
        if (hit.transform.CompareTag("Tile"))
        {
            int _x = hit.transform.gameObject.GetComponent<ClickableTile>().tileX;
            int _y = hit.transform.gameObject.GetComponent<ClickableTile>().tileZ;
            Cursor.transform.position = new Vector3(_x, .56f, _y);
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
                QuadMovementUI[x, y] = Instantiate(uiQuadPrefab, new Vector3(x, .55f, y), Quaternion.Euler(90, 0, 0), uiQuadContainer);
            }
        }
        SetTileTest(0, 0);
    }

    private void SetTileColor(int _x, int _y, Color col)
    {
        QuadMovementUI[_x, _y].GetComponent<Renderer>().material.color = col;
    }

    public void SetUITiles(int _x, int _y, int moveableRangeManDist, int attackRangeManDist)
    {
        SetAttackRange(_x, _y, attackRangeManDist);
        SetMoveableRange(_x, _y, moveableRangeManDist);
    }

    public void SetMoveableRange(int _x, int _y, int manhattan_distance)
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                float dist = Mathf.Abs(_x - x) + Mathf.Abs(_y - y);
                if (dist < manhattan_distance) { SetTileColor(x, y, movable); }
            }
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