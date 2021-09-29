using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    // ��ȡ����xzƽ��ֵ
    public static float GetDistanceXZSquare(Vector3 p1, Vector3 p2) {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.z - p2.z) * (p1.z - p2.z);
    }
}
