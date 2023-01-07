using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMovement : MonoBehaviour
{
    public Grid grid; //  You can also use the Tilemap object
    public Vector3Int coordinate;
    public float moveSpeed;
    public GameObject spawn;
    // Start is called before the first frame update
    void Start()
    {
        coordinate = new Vector3Int(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, grid.CellToWorld(coordinate), moveSpeed * Time.deltaTime);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            // Go from world position to a cell.
            coordinate = grid.WorldToCell(mouseWorldPos);
            Debug.Log($"Mouse World Pose: {mouseWorldPos.ToString()} ::::: Coordinate: {coordinate.ToString()}");
        }
    }

    /**
    * Converts mouse position to a world coordinant, With the y position of the world coordinant clamped to 0.
    */
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Camera.main.transform.position.y;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mp);
        mouseWorldPos.y = transform.position.y;
        return mouseWorldPos;
    }
}
