using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon {
    public static class HexagonUtil
    {
        // 六边形坐标转世界坐标
        public static Vector3 HexPos2XYPos(Vector2Int hexpos) {
            float size = 1f;

            float l = Mathf.Cos(Mathf.Deg2Rad * 30) * 2 * size;
            float x = hexpos.x * Mathf.Cos(Mathf.Deg2Rad * 330) * l;
            float y = 0;
            float z = hexpos.y * l - hexpos.x * Mathf.Sin(Mathf.Deg2Rad * 330) * l;
            return new Vector3(x, y, z);
        }

        // 世界坐标转六边形坐标
        public static Vector2Int XYPos2HexPos(Vector3 xypos) {
            float size = 1f;
            float l = Mathf.Cos(Mathf.Deg2Rad * 30) * 2 * size;
            float x = xypos.x / (Mathf.Cos(Mathf.Deg2Rad * 330) * l);
            float y = (xypos.z + x * Mathf.Sin(Mathf.Deg2Rad * 330) * l) / l;
            int intx = Mathf.FloorToInt(x + 0.5f);
            int inty = Mathf.FloorToInt(y + 0.5f);
            int z = 0 - intx - inty;
            return new Vector2Int(intx, inty);
        }

        public static string HexPos2String(Vector2Int hexpos) {
            return string.Format("({0},{1})", hexpos.x, hexpos.y);
        }
    }
}
