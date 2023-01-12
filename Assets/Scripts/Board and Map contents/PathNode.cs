using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // All Components to boardpathfinding mechanics
   public class PathNode {
        public List<PathNode> neighbours;
        public int x;
        public int y; 
        public PathNode(){
            neighbours = new List<PathNode>();
        }

        public float DistanceTo(PathNode n){
            return Vector3.Distance(
                    new Vector3(x, 0, y), 
                    new Vector3(n.x, 0,n.y)
            );
        }
    }