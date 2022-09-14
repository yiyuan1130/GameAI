using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hexagon { 
    public class GameMain : MonoBehaviour
    {
        public GameObject hexagonPrefab;
        Transform mapParent;
        HexagonAStarFind hexagonAStarFind;
        bool choosedStartPos;
        Vector3 startPos;
        Vector3 endPos;
        private void Awake()
        {
            CreateMap();
            InitHexagonAStarFind();
        }
        void Start()
        {
            
        }

        public void CreateMap() {
            GameObject mapParentGO = new GameObject("MapParent");
            mapParent = mapParentGO.transform;
            mapParent.position = Vector3.zero;
            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    int hexx = i;
                    int hexy = j;
                    int hexz = 0 - i - j;
                    Vector2Int hexPos = new Vector2Int(hexx, hexy);
                    Vector3 pos = HexagonUtil.HexPos2XYPos(hexPos);
                    GameObject go = Instantiate(hexagonPrefab);
                    string name = string.Format("({0},{1})", hexx, hexy);
                    go.name = name;
                    go.transform.SetParent(mapParent.transform);
                    go.transform.position = pos;
                    go.transform.Find("text").GetComponent<TextMesh>().text = name;
                    go.AddComponent<DebugHexagonGrid>().Init(hexPos);
                }
            }
        }

        public void InitHexagonAStarFind() {
            Dictionary<string, Hexagon> hexagonDict = new Dictionary<string, Hexagon>();
            for (int i = 0; i < mapParent.childCount; i++)
            {
                Transform tr = mapParent.GetChild(i);
                DebugHexagonGrid hexagonGrid = tr.GetComponent<DebugHexagonGrid>();
                Vector2Int hexpos = hexagonGrid.GetHexpos();
                bool isWall = hexagonGrid.IsWall();
                Hexagon hexagon = new Hexagon(hexpos, isWall);
                string key = HexagonUtil.HexPos2String(hexpos);
                hexagonDict.Add(key, hexagon);
            }
            hexagonAStarFind = new HexagonAStarFind(hexagonDict);
        }

        private void Update()
        {
            // ǽ
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f);
                Vector3 pos = Camera.main.ScreenToWorldPoint(screenPos);
                Vector2Int hexpos = HexagonUtil.XYPos2HexPos(pos);
                string name = string.Format("({0},{1})", hexpos.x, hexpos.y);
                DebugHexagonGrid debugHexagonGrid = mapParent.Find(name).GetComponent<DebugHexagonGrid>();
                debugHexagonGrid.SetWall();
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f);
                Vector3 pos = Camera.main.ScreenToWorldPoint(screenPos);
                Vector2Int hexpos = HexagonUtil.XYPos2HexPos(pos);
                string name = string.Format("({0},{1})", hexpos.x, hexpos.y);
                DebugHexagonGrid debugHexagonGrid = mapParent.Find(name).GetComponent<DebugHexagonGrid>();
                //debugHexagonGrid.ResetColor();
                debugHexagonGrid.OnSelect(Color.red);
                if (!choosedStartPos)
                {
                    startPos = pos;
                    choosedStartPos = true;
                }
                else
                {
                    endPos = pos;
                    choosedStartPos = false;
                    hexagonAStarFind.FindPath(startPos, endPos);
                }
            }
        }
    }
}
