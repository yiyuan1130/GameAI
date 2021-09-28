using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar { 
    public static class AStarFindUtil
    {
        /// <summary>
        /// 获取左右两点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns> 0左 1右 </returns>
        public static Vector3[] GetLeftAndRightPoint(Vector3 center, Vector3 pos1, Vector3 pos2) {
            Vector3[] posArr = new Vector3[2];
            Vector3 vect1 = pos1 - center;
            Vector3 vect2 = pos2 - center;
            Vector3 vectM = (pos1 + pos2) * 0.5f - center;
            if (Vector3.Cross(vectM, vect1).y < Vector3.Cross(vectM, vect2).y)
            {
                posArr[0] = pos1;
                posArr[1] = pos2;
            }
            else if (Vector3.Cross(vectM, vect1).y > Vector3.Cross(vectM, vect2).y)
            {
                posArr[0] = pos2;
                posArr[1] = pos1;
            }
            else {
                if (Vector3.Distance(center, pos1) < Vector3.Distance(center, pos2))
                {
                    posArr[0] = pos1;
                    posArr[1] = pos2;
                }
                else 
                {
                    posArr[0] = pos2;
                    posArr[1] = pos1;
                }
            }
            return posArr;
        }
        public static Vector3[] GetLeftAndRightPoint(Vector3 center, Vector3[] points) {
            return GetLeftAndRightPoint(center, points[0], points[1]);
        }


        /// <summary>
        /// 判断两组点的位置关系
        /// </summary>
        /// <param name="center"></param>
        /// <param name="points1">近处两点 区分左右后的</param>
        /// <param name="points2">远处两点 区分左右后的</param>
        /// <returns></returns>
        public static int CheckPointsState(Vector3 center, Vector3[] points1, Vector3[] points2) {
            Vector3[] vectors1 = new Vector3[] {
                points1[0] - center,
                points1[1] - center,
            };
            Vector3[] vectors2 = new Vector3[] {
                points2[0] - center,
                points2[1] - center,
            };

            if (Vector3.Cross(vectors1[0], vectors2[0]).y >= 0 && Vector3.Cross(vectors1[1], vectors2[1]).y <= 0) {
                return 1; // 两点均在开口内一侧
            }
            if (Vector3.Cross(vectors1[0], vectors2[0]).y >= 0 && Vector3.Cross(vectors1[1], vectors2[1]).y <= 0) {
                return 2; // 两点在开口外两侧
            }
            if (Vector3.Cross(vectors1[0], vectors2[0]).y <= 0 && Vector3.Cross(vectors1[1], vectors2[1]).y < 0 && Vector3.Cross(vectors1[0], vectors2[1]).y >= 0) {
                return 3; // 左点在开口左侧，右点在开口内侧
            }
            if (Vector3.Cross(vectors1[0], vectors2[0]).y >= 0 && Vector3.Cross(vectors1[1], vectors2[1]).y > 0 && Vector3.Cross(vectors1[1], vectors2[0]).y <= 0) {
                return 4; // 左点在开口内侧，右点在开口右侧
            }
            if (Vector3.Cross(vectors1[0], vectors2[0]).y < 0 && Vector3.Cross(vectors1[0], vectors2[1]).y < 0) {
                return 5; // 左右点均在开口左侧
            }
            if (Vector3.Cross(vectors1[1], vectors2[0]).y > 0 && Vector3.Cross(vectors1[1], vectors2[1]).y > 0) {
                return 6; // 左右点均在开口右侧
            }

            return 0;
        }
    }
}
