using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTest : MonoBehaviour
{
    public Material hexagonMaterial;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        // 创建六边形预设
        //if (GUI.Button(new Rect(0, 0, 200, 100), "创建6变形预设"))
        //{
        //    CreateHexagonMesh();
        //}
    }

    public void CreateHexagonMesh()
    {
        Vector3 center = Vector3.zero;
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            Vector3.zero,
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 0f), 0, Mathf.Sin(Mathf.Deg2Rad * 0f)),
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 60f), 0, Mathf.Sin(Mathf.Deg2Rad * -60f)),
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 120f), 0, Mathf.Sin(Mathf.Deg2Rad * -120f)),
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 180f), 0, Mathf.Sin(Mathf.Deg2Rad * -180f)),
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 240f), 0, Mathf.Sin(Mathf.Deg2Rad * -240f)),
            new Vector3(Mathf.Cos(Mathf.Deg2Rad * 300f), 0, Mathf.Sin(Mathf.Deg2Rad * -300f)),
        };
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            new GameObject("Vertices " + i).transform.position = mesh.vertices[i];
        }
        mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1,
        };
        mesh.colors = new Color[] {
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
        };
        mesh.normals = new Vector3[] {
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
        };
        mesh.uv = new Vector2[] { 
            Vector2.zero,
            Vector2.one,
            Vector2.one,
            Vector2.one,
            Vector2.one,
            Vector2.one,
            Vector2.one,
        };
        UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Hexagon/Meshs/HexagonMesh.mesh");
        GameObject obj = new GameObject("Hexagon");
        obj.transform.position = center;
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = hexagonMaterial;
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(obj, "Assets/Hexagon/Prefabs/hexagon.prefab");
    }
}
