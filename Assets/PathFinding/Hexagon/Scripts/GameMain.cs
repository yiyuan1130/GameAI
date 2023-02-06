using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            //InitHexagonAStarFind();
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

        public void CreateHexagonAStarFind() {
            if (hexagonAStarFind != null) {
                hexagonAStarFind.Clear();
            }
            Dictionary<string, Hexagon> hexagonDict = new Dictionary<string, Hexagon>();
            for (int i = 0; i < mapParent.childCount; i++)
            {
                Transform tr = mapParent.GetChild(i);
                DebugHexagonGrid hexagonGrid = tr.GetComponent<DebugHexagonGrid>();
                Vector2Int hexpos = hexagonGrid.GetHexpos();
                bool isWall = hexagonGrid.IsWall();
                if (isWall) {
                    Debug.Log("iswall" + hexpos.ToString());
                }
                Hexagon hexagon = new Hexagon(hexpos, isWall);
                string key = HexagonUtil.HexPos2String(hexpos);
                hexagonDict.Add(key, hexagon);
            }
            hexagonAStarFind = new HexagonAStarFind(hexagonDict);
        }

        DebugHexagonGrid selectStartHexagon;
        DebugHexagonGrid selectEndHexagon;
        private void Update()
        {
            if (IsClickUI()) {
                return;
            }

            // «Ω
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
                if (!choosedStartPos)
                {
                    startPos = pos;
                    choosedStartPos = true;
                    selectStartHexagon = debugHexagonGrid;
                    selectStartHexagon.OnSelect(Color.red);
                }
                else
                {
                    endPos = pos;
                    choosedStartPos = false;
                    selectEndHexagon = debugHexagonGrid;
                    selectEndHexagon.OnSelect(Color.red);

                    DelayDo(1, OnSelectTowPositionEnd);
                    
                }
            }
        }

        void DelayDo(float delay, Action callabck) {
            StartCoroutine(_DelayDo(delay, callabck));
        }
        IEnumerator _DelayDo(float delay, Action callabck) {
            yield return new WaitForSeconds(delay);
            callabck();
        }
        void OnSelectTowPositionEnd()
        {
            selectStartHexagon.OnSelect(Color.red);
            selectEndHexagon.OnSelect(Color.red);
            CreateHexagonAStarFind();
            List<Hexagon> hexagonPathList = hexagonAStarFind.FindPath(startPos, endPos);
            ShowPathList(hexagonPathList);
        }

        void ShowPathList(List<Hexagon> hexagonPathList) {
            StartCoroutine(_ShowPath(hexagonPathList));
        }
        IEnumerator _ShowPath(List<Hexagon> hexagonPathList) {
            WaitForSeconds wait = new WaitForSeconds(2f / hexagonPathList.Count);
            for (int i = 0; i < hexagonPathList.Count; i++)
            {
                Hexagon hexagon = hexagonPathList[i];
                string hexName = HexagonUtil.HexPos2String(hexagon.hexpos);
                mapParent.Find(hexName).GetComponent<DebugHexagonGrid>().OnSelect(Color.green);
                yield return wait;
            }
        }


        List<RaycastResult> checkClickUIResult = new List<RaycastResult>();
        bool IsClickUI() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            EventSystem.current.RaycastAll(eventDataCurrentPosition, checkClickUIResult);
            bool ret = checkClickUIResult.Count > 0;
            checkClickUIResult.Clear();
            return ret;
        }

        // UIœ‡πÿ
        public void ResetMap() {
            for (int i = 0; i < mapParent.childCount; i++)
            {
                Transform tr = mapParent.GetChild(i);
                DebugHexagonGrid hexagonGrid = tr.GetComponent<DebugHexagonGrid>();
                if (hexagonGrid.IsWall()) {
                    hexagonGrid.SetWall();
                }
                if (hexagonGrid.IsSelect()) {
                    hexagonGrid.OnSelect(Color.red);
                }
            }
        }
    }
}
