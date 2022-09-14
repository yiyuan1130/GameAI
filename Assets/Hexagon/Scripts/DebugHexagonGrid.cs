using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon { 
    public class DebugHexagonGrid : MonoBehaviour
    {
        public Material curMat;
        Color baseColor;
        bool isSelect = false;
        bool isWall = false;
        Vector2Int hexpos;


        void Start()
        {
            Material normalMat = GetComponent<MeshRenderer>().material;
            curMat = Instantiate(normalMat);
            GetComponent<MeshRenderer>().material = curMat;
            baseColor = curMat.GetColor("_OutsideColor");
        }

        public void Init(Vector2Int hexpos) {
            this.hexpos = hexpos;
        }

        public Vector2Int GetHexpos() {
            return hexpos;
        }

        public void OnSelect(Color selectColor) {
            if (isWall){
                return;
            }
            isSelect = !isSelect;
            if (isSelect)
            {
                curMat.SetColor("_OutsideColor", selectColor);
            }
            else {
                curMat.SetColor("_OutsideColor", baseColor);
            }
        }

        public void SetWall() {
            if (isSelect) {
                return;
            }
            isWall = !isWall;
            if (isWall)
            {
                curMat.SetColor("_OutsideColor", Color.black);
            }
            else
            {
                curMat.SetColor("_OutsideColor", baseColor);
            }
        }

        public bool IsWall() {
            return isWall;
        }

        void Update()
        {
        
        }
    }
}

