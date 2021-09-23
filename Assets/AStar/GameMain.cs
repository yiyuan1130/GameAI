using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AStar
{
    public class GameMain : MonoBehaviour
    {
        Grid[][] grids;
        Bounds[] obs;
        AStarFind aStarFind;
        GameObject actorPrefab;
        public float gridSizeRate = 1f;
        bool selectStart = false;
        private void Awake()
        {
            ActorManager.Init();
            actorPrefab = GameObject.Find("Actor");
            actorPrefab.SetActive(false);
            CreateOrUpdateAStartFind();
        }

        Vector3 clickStartPos;
        Vector3 clickEndPos;
        List<Grid> tempNeighborGrids;
        List<Vector3[]> tempNeighborSides;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    #region debug显示选中的格子相邻信息
                    for (int i = 0; i < grids.Length; i++)
                    {
                        for (int j = 0; j < grids[i].Length; j++)
                        {
                            Grid g = grids[i][j];
                            if (g.bounds.Contains(new Vector3(hit.point.x, 0, hit.point.z)))
                            {
                                tempNeighborGrids = g.neighborGrids;
                                tempNeighborSides = g.neighborSides;
                                return;
                            }
                        }
                    }
                    #endregion

                    #region 选择起点、终点
                    //if (PosIsInOB(hit.point))
                    //{
                    //    return;
                    //}
                    //if (!selectStart)
                    //{
                    //    clickStartPos = hit.point;
                    //    selectStart = true;
                    //}
                    //else
                    //{
                    //    clickEndPos = hit.point;
                    //    selectStart = false;
                    //    AddMoveActor(clickStartPos, clickEndPos);
                    //    AddMoveActor(clickEndPos, clickStartPos);
                    //}
                    #endregion
                }
            }
        }

        public void AddMoveActor(Vector3 startPos, Vector3 endPos)
        {
            GameObject actor = Instantiate(actorPrefab);
            actor.SetActive(true);
            ActorMove actorMove = actor.GetComponent<ActorMove>();
            ActorManager.CreateActorMove(actorMove);
            actorMove.DoMove(aStarFind, startPos, endPos);
        }

        void CreateOrUpdateAStartFind()
        {
            GenOBs();
            GenPlane();
            if (aStarFind == null)
            {
                aStarFind = new AStarFind(grids);
            }
            else
            {
                aStarFind.UpdateGrids(grids);
            }
        }


        void GenOBs()
        {
            Transform obsTrans = GameObject.Find("OBs").transform;
            obs = new Bounds[obsTrans.childCount];
            for (int i = 0; i < obsTrans.childCount; i++)
            {
                Transform obTrans = obsTrans.GetChild(i);
                Vector3 center = new Vector3(obTrans.position.x, 0, obTrans.position.z);
                Bounds bounds = new Bounds(center, obTrans.localScale);
                obs[i] = bounds;
            }
        }
        bool PosIsInOB(Vector3 pos)
        {
            for (int i = 0; i < obs.Length; i++)
            {
                if (obs[i].Contains(pos))
                {
                    return true;
                }
            }
            return false;
        }
        bool GridIsInOB(Grid grid)
        {
            for (int i = 0; i < obs.Length; i++)
            {
                if (obs[i].Intersects(grid.bounds))
                {
                    return true;
                }
            }
            return false;
        }

        void GenPlane()
        {
            Transform planeTrans = GameObject.Find("Plane").transform;
            Vector3 mapSize = new Vector3(planeTrans.transform.localScale.x, 0, planeTrans.transform.localScale.z);
            Vector3 gridSize = Vector3.one * gridSizeRate;
            int gridCountX = (int)(mapSize.x / gridSize.x);
            int gridCountZ = (int)(mapSize.z / gridSize.z);
            Vector3 gridStartPos = new Vector3(-mapSize.x / 2 + gridSize.x / 2, 0, -mapSize.z / 2 + gridSize.z / 2);

            // 初始化每个格子
            grids = new Grid[gridCountX][];
            for (int i = 0; i < gridCountX; i++)
            {
                grids[i] = new Grid[gridCountZ];
                for (int j = 0; j < gridCountZ; j++)
                {
                    Vector3 pos = gridStartPos + new Vector3(gridSize.x * i, 0, gridSize.z * j);
                    Vector2Int idx = new Vector2Int(i, j);
                    Grid grid = new Grid(idx, pos, gridSize);
                    grid.walkAble = !GridIsInOB(grid);
                    grids[i][j] = grid;
                }
            }

            // 计算每个各自的周边格子
            for (int i = 0; i < gridCountX; i++)
            {
                for (int j = 0; j < gridCountZ; j++)
                {
                    Grid curGrid = grids[i][j];
                    Vector2Int curGridId = curGrid.idx;
                    List<Grid> neighbors = new List<Grid>();
                    Vector2Int[] neighborsIds = new Vector2Int[] { // 逆时针 上左下右
                        curGridId + new Vector2Int(0, 1),
                        curGridId + new Vector2Int(-1, 0),
                        curGridId + new Vector2Int(0, -1),
                        curGridId + new Vector2Int(1, 0),
                    };
                    for (int ii = 0; ii < neighborsIds.Length; ii++)
                    {
                        Vector2Int _idx = neighborsIds[ii];
                        if (_idx.x >= 0 && _idx.x < gridCountX && _idx.y >= 0 && _idx.y < gridCountZ) {
                            neighbors.Add(grids[_idx.x][_idx.y]);
                        }
                    }
                    curGrid.GenerateNeighbors(neighbors);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (grids == null)
            {
                return;
            }
            for (int i = 0; i < grids.Length; i++)
            {
                for (int j = 0; j < grids[i].Length; j++)
                {
                    Grid grid = grids[i][j];
                    if (grid.walkAble)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(grid.bounds.center, grid.bounds.size);
                    }
                }
            }

            if (tempNeighborGrids != null) {
                for (int i = 0; i < tempNeighborGrids.Count; i++)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.6f);
                    Gizmos.DrawCube(tempNeighborGrids[i].bounds.center, tempNeighborGrids[i].bounds.size);
                }
                for (int i = 0; i < tempNeighborSides.Count; i++)
                {
                    float a = (float)i / tempNeighborSides.Count;
                    float b = (float)i / tempNeighborGrids.Count;
                    Gizmos.color = new Color(a, b, 0, 0.5f);
                    Vector3[] posArr = tempNeighborSides[i];
                    Gizmos.DrawSphere(posArr[0], 2f);
                    Gizmos.DrawSphere(posArr[1], 2f);
                }
            }
        }
    }
}