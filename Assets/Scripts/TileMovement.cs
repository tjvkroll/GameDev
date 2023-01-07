using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMovement : MonoBehaviour
{
    public Vector2 tilePos;
    public float moveSpeed;
    public Tilemap map;
    public GameObject spawn;
    // Start is called before the first frame update
    void Start()
    {
        tilePos = new Vector2(-6, 5);

    }

    // Update is called once per frame
    public Grid grid; //  You can also use the Tilemap object
    void Update()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = 100;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mp);
        if (Input.GetMouseButtonDown(0))
            Instantiate(spawn, mouseWorldPos, Quaternion.identity);
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
        Debug.Log(coordinate);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(tilePos.x, transform.position.y, tilePos.y), moveSpeed * Time.deltaTime);
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     tilePos.x += 1;
        // }
        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     tilePos.x -= 1;
        // }
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     tilePos.y += 1;
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     tilePos.y -= 1;
        // }
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3 mousePosition = Input.mousePosition;
        //     mousePosition.z = transform.position.y - map.transform.position.y + 1;
        //     Vector3 worldpoint = Camera.main.ScreenToWorldPoint(mousePosition);
        //     Vector3Int gridposition = map.WorldToCell(new Vector2(worldpoint.x, worldpoint.z));
        //     tilePos = ((Vector2Int)gridposition);
        //     Debug.Log($"Mouse Pos: {mousePosition.ToString()}    tilePos: {tilePos.ToString()}     WorldPoint: {worldpoint.ToString()}");
        // }
    }
}
