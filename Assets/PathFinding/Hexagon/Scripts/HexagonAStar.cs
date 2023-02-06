using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon {
    public class Hexagon {
        public float f;
        public float g;
        public float h;
        public bool isWall;
        public Vector2Int hexpos;
        public Vector3 xypos;
        public List<Hexagon> aroundHexagons;
        public Hexagon preHexagon = null;
        public Hexagon(Vector2Int hexpos, bool isWall = false) {
            this.f = 99999;
            this.g = 0;
            this.h = 0;
            this.isWall = isWall;
            this.hexpos = hexpos;
            this.xypos = HexagonUtil.HexPos2XYPos(hexpos);
            aroundHexagons = new List<Hexagon>();
        }
    }
    public class HexagonAStarFind
    {
        Dictionary<string, Hexagon> hexagonDict;
        List<Hexagon> openList = new List<Hexagon>();
        List<Hexagon> closeList = new List<Hexagon>();
        public HexagonAStarFind(Dictionary<string, Hexagon> hexagonDict)
        {
            this.hexagonDict = hexagonDict;
            foreach (var item in hexagonDict)
            {
                Hexagon hexagon = item.Value;
                SetAroundHexagon(hexagon);
            }
        }

        void SetAroundHexagon(Hexagon hexagon) {
            Vector2Int[] aroundsHexPos = new Vector2Int[] {
                hexagon.hexpos + new Vector2Int(1, 0),  // 右上
                hexagon.hexpos + new Vector2Int(1, -1), // 右下
                hexagon.hexpos + new Vector2Int(0, -1), // 下
                hexagon.hexpos + new Vector2Int(-1, 0), // 左下
                hexagon.hexpos + new Vector2Int(-1, 1), // 左上
                hexagon.hexpos + new Vector2Int(0, 1),  // 上
            };
            for (int i = 0; i < aroundsHexPos.Length; i++)
            {
                Vector2Int hexpos = aroundsHexPos[i];
                string key = HexagonUtil.HexPos2String(hexpos);
                if (hexagonDict.ContainsKey(key)) {
                    Hexagon subHexagon = hexagonDict[key];
                    hexagon.aroundHexagons.Add(subHexagon);
                }
            }
        }

        Hexagon GetHexagonByPosition(Vector3 pos) {
            Vector2Int hexpos = HexagonUtil.XYPos2HexPos(pos);
            string key = HexagonUtil.HexPos2String(hexpos);
            Hexagon hexagon = hexagonDict[key];
            return hexagon;
        }

        public List<Hexagon> FindPath(Vector3 startPos, Vector3 endPos) {
            List<Hexagon> hexagonList = new List<Hexagon>();
            Hexagon curHexagon;

            Hexagon startHexagon = GetHexagonByPosition(startPos);
            Hexagon endHexagon = GetHexagonByPosition(endPos);
            if (startHexagon == null || endHexagon == null) {
                return hexagonList;
            }
            Debug.Log("StartHexagon => " + startHexagon.hexpos.ToString());
            Debug.Log("EndHexagon => " + endHexagon.hexpos.ToString());

            startHexagon.g = 0;
            startHexagon.h = Vector3.Distance(startHexagon.xypos, endHexagon.xypos);
            startHexagon.f = startHexagon.g + startHexagon.h;
            startHexagon.preHexagon = null;
            openList.Add(startHexagon);
            while (true)
            {
                openList.Sort((Hexagon a, Hexagon b) => {
                    if (a.f < b.f)
                    {
                        return -1;
                    }
                    else { 
                        return 1;
                    }
                });
                curHexagon = openList[0];
                openList.RemoveAt(0);
                closeList.Add(curHexagon);
                if (curHexagon == endHexagon) {
                    break;
                }
                for (int i = 0; i < curHexagon.aroundHexagons.Count; i++)
                {
                    Hexagon subHexagon = curHexagon.aroundHexagons[i];
                    if (subHexagon.isWall)
                    {
                        closeList.Add(subHexagon);
                    }
                    if (!closeList.Contains(subHexagon)) { 
                        subHexagon.g = curHexagon.g + 1;
                        subHexagon.h = Vector3.Distance(subHexagon.xypos, endHexagon.xypos);
                        subHexagon.f = subHexagon.g + subHexagon.h;
                        subHexagon.preHexagon = curHexagon;
                        openList.Add(subHexagon);
                    }
                }
            }

            Hexagon curShowHexagon = endHexagon;
            while (curShowHexagon != null)
            {
                hexagonList.Add(curShowHexagon);
                Debug.Log(curShowHexagon.hexpos.ToString());
                curShowHexagon = curShowHexagon.preHexagon;
            }
            hexagonList.Reverse();
            return hexagonList;
        }

        public void Clear() {
            hexagonDict.Clear();
            openList.Clear();
            closeList.Clear();
        }
    }
}
