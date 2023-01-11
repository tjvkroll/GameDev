using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
    public int tileX;
    public int tileZ;
    //holds path for object if needed
    public List<BoardCreator.PathNode> currentPath = null; 
    public string objName;

}
