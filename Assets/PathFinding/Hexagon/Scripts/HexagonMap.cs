using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon {
    public class HexagonMap
    {
        public HexagonMap Instance;
        public HexagonMap() {
            Instance = this;
        }

        Vector2Int[] GetMapPosArray() {
            Vector2Int[] posArray = new Vector2Int[] { 
                new Vector2Int(0, 0),
            };
            return posArray;
        }

        public void CreateMap(){
            
        }
    }
}
