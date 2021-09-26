using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar
{
    public class Grid
    {
        public Grid preGrid;
        public Vector2Int idx;
        public List<Vector3> vertices; // 逆时针 0左上 1左下 2右下 3右上

        // 0123 上左下右
        public Dictionary<int, Grid> neighborGridDic;
        public Dictionary<int, Vector3[]> neighborSideDic;
        public Dictionary<int, Vector3> verticeDic;

        public Bounds bounds;
        public bool walkAble = false;
        public int G;
        public int H;
        public int F;
        public Grid(Vector2Int idx, Vector3 pos, Vector3 size, bool walkAble = true)
        {
            this.idx = idx;
            this.preGrid = null;
            this.walkAble = walkAble;
            this.bounds = new Bounds(pos, size);
            this.G = 0;
            this.H = 0;
            this.F = 99999;
            this.GenerateVertices();
        }


        public void GenerateNeighbors(Dictionary<int, Grid> neighborGridDic)
        {
            this.neighborGridDic = neighborGridDic;
            this.neighborSideDic = new Dictionary<int, Vector3[]>();
            foreach (var item in this.neighborGridDic)
            {
                int key = item.Key;
                Grid grid = item.Value;
                if (grid != null)
                {
                    Vector3[] side = new Vector3[2];
                    Vector2Int idxOffset = grid.idx - this.idx;
                    if (idxOffset.x == 0 && idxOffset.y == 1)
                    {
                        // 上
                        side[0] = this.vertices[0];
                        side[1] = this.vertices[3];
                    }
                    else if (idxOffset.x == -1 && idxOffset.y == 0)
                    {
                        // 左
                        side[0] = this.vertices[0];
                        side[1] = this.vertices[1];
                    }
                    else if (idxOffset.x == 0 && idxOffset.y == -1)
                    {
                        // 下
                        side[0] = this.vertices[1];
                        side[1] = this.vertices[2
                            ];
                    }
                    else if (idxOffset.x == 1 && idxOffset.y == 0)
                    {
                        // 右
                        side[0] = this.vertices[2];
                        side[1] = this.vertices[3];
                    }
                    this.neighborSideDic.Add(key, side);
                }
            }
        }

        void GenerateVertices()
        {
            float halfSize = this.bounds.size.x * 0.5f;
            vertices = new List<Vector3>();
            vertices.Add(this.bounds.center + new Vector3(-halfSize, 0, halfSize)); // 左上
            vertices.Add(this.bounds.center + new Vector3(-halfSize, 0, -halfSize)); // 左下
            vertices.Add(this.bounds.center + new Vector3(halfSize, 0, -halfSize)); // 右下
            vertices.Add(this.bounds.center + new Vector3(halfSize, 0, halfSize)); // 右上
        }

        public void Reset()
        {
            this.preGrid = null;
            this.G = 0;
            this.H = 0;
            this.F = 99999;
        }
    }

    public class AStarFind
    {
        Grid[][] grids;
        Grid startGrid;
        Grid endGrid;
        Grid curGrid;
        List<Grid> openList;
        List<Grid> closeList;
        public List<Vector3> pathList;
        int maxI;
        int maxJ;
        Vector3 startPos;
        Vector3 endPos;
        public AStarFind(Grid[][] grids)
        {
            this.grids = grids;
            this.maxI = grids.Length - 1;
            this.maxJ = grids[0].Length - 1;
        }

        public void UpdateGrids(Grid[][] grids)
        {
            this.grids = grids;
            this.maxI = grids.Length - 1;
            this.maxJ = grids[0].Length - 1;
        }

        public List<Vector3> GetPath(Vector3 startPos, Vector3 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            for (int i = 0; i < grids.Length; i++)
            {
                for (int j = 0; j < grids[0].Length; j++)
                {
                    grids[i][j].Reset();
                }
            }

            bool posValid = GetStartEndGridByPos(startPos, endPos);
            if (!posValid)
                return null;

            FindGridPath();

            return FindVerticesPath(endGrid);
        }

        // 计算拐点
        List<Vector3> FindVerticesPath(Grid lastGrid)
        {
            List<Grid> reslutGrids = new List<Grid>();
            Grid curGrid = lastGrid;
            while (curGrid != null && curGrid != startGrid)
            {
                reslutGrids.Insert(0, curGrid);
                curGrid = curGrid.preGrid;
            }
            reslutGrids.Insert(0, startGrid);

            #region 格子中心路径点
            //pathList = new List<Vector3>();
            //pathList.Add(startPos);
            //for (int i = 0; i < reslutGrids.Count; i++)
            //{
            //    pathList.Add(reslutGrids[i].bounds.center);
            //}
            //pathList.Add(endPos);
            //return pathList;
            #endregion

            GameMain.GetInstance().SetGridList(reslutGrids);
            List<Vector3[]> sides = new List<Vector3[]>();
            for (int i = 0; i < reslutGrids.Count; i++)
            {
                Grid _curGrid = reslutGrids[i];
                Grid _nextGrid = null;
                if (i + 1 <= reslutGrids.Count - 1)
                {
                    _nextGrid = reslutGrids[i + 1];
                }
                if (_nextGrid != null)
                {
                    Vector2Int idxOffset = _nextGrid.idx - _curGrid.idx;
                    float halfSize = _curGrid.bounds.size.x * 0.5f;
                    Vector3 gridCenter = curGrid.bounds.center;
                    if (idxOffset.x == 0 && idxOffset.y == 1)
                    {
                        // 上
                        sides.Add(_curGrid.neighborSideDic[0]);
                    }
                    else if (idxOffset.x == -1 && idxOffset.y == 0)
                    {
                        // 左
                        sides.Add(_curGrid.neighborSideDic[1]);
                    }
                    else if (idxOffset.x == 0 && idxOffset.y == -1)
                    {
                        // 下
                        sides.Add(_curGrid.neighborSideDic[2]);
                    }
                    else if (idxOffset.x == 1 && idxOffset.y == 0)
                    {
                        // 右
                        sides.Add(_curGrid.neighborSideDic[3]);
                    }
                }
            }
            pathList = new List<Vector3>();
            pathList.Add(startPos);
            if (sides.Count <= 1)
            {
                pathList.Add(endPos);
                return pathList;
            }

            Vector3 curPos = this.startPos;
            Vector3[] curPoints = AStarFindUtil.GetLeftAndRightPoint(curPos, sides[0]); 
            for (int i = 0; i < sides.Count; i++)
            {
                Vector3[] nextSide = null;
                if (i + 1 <= sides.Count - 1)
                {
                    nextSide = sides[i + 1];
                }
                if (nextSide != null)
                {
                    curPoints = AStarFindUtil.GetLeftAndRightPoint(curPos, curPoints);
                    Vector3[] nextPoints = AStarFindUtil.GetLeftAndRightPoint(curPos, nextSide);
                    int twoPointGroupState = AStarFindUtil.CheckPointsState(curPos, curPoints, nextPoints);
                    switch (twoPointGroupState)
                    {
                        case 1: // 两点均在开口内一侧 更新为新的两个点
                            curPoints = nextPoints;
                            break;
                        case 2: // 两点在开口外两侧 不操作
                            break;
                        case 3: // 左点在开口左侧，右点在开口内侧 左点是拐点  更新左点
                            curPos = curPoints[0];
                            pathList.Add(curPos);
                            curPoints[0] = nextPoints[0];
                            break;
                        case 4: // 左点在开口内侧，右点在开口右侧 右点是拐点  更新右点
                            curPos = curPoints[1];
                            pathList.Add(curPos);
                            curPoints[1] = nextPoints[1];
                            break;
                        case 5: // 左右点均在开口左侧 左点为拐点
                            curPos = curPoints[0];
                            pathList.Add(curPos);
                            curPoints = nextPoints;
                            i++;
                            break;
                        case 6: // 左右点均在开口右侧 右点为拐点
                            curPos = curPoints[1];
                            pathList.Add(curPos);
                            curPoints = nextPoints;
                            i++;
                            break;
                        case 0:
                            Debug.LogError("两组点的位置不确定");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    pathList.Add(curPos);
                }
            }
            pathList.Add(endPos);

            return pathList;
        }

        bool GetStartEndGridByPos(Vector3 startPos, Vector3 endPos)
        {
            bool startPosValid = false;
            bool endPosValid = false;
            Grid grid;
            for (int i = 0; i < grids.Length; i++)
            {
                for (int j = 0; j < grids[i].Length; j++)
                {
                    grid = grids[i][j];
                    if (grid.walkAble)
                    {
                        if (grid.bounds.Contains(startPos))
                        {
                            startGrid = grid;
                            startPosValid = true;
                        }
                        if (grid.bounds.Contains(endPos))
                        {
                            endGrid = grid;
                            endPosValid = true;
                        }
                    }
                }
            }
            if (startGrid == null)
            {
                Debug.LogError("StartPos is INVALID");
            }
            if (endGrid == null)
            {
                Debug.LogError("EndPos is INVALID");
            }
            return startPosValid && endPosValid;
        }

        // 找格子路径
        void FindGridPath()
        {
            openList = new List<Grid>();
            closeList = new List<Grid>();
            startGrid.preGrid = null;
            startGrid.G = 0;
            startGrid.H = Mathf.Abs(endGrid.idx.x - startGrid.idx.x) + Mathf.Abs(endGrid.idx.y - startGrid.idx.y);
            startGrid.F = startGrid.G + startGrid.H;
            openList.Add(startGrid);
            curGrid = startGrid;
            int count = 0;
            while (openList.Count > 0 && curGrid != endGrid)// && count <= 20000)
            {
                SortOpenList();
                curGrid = openList[0];
                openList.RemoveAt(0);
                closeList.Add(curGrid);
                UpdateNeighborGrids();
                count++;
            }
        }

        void SortOpenList()
        {
            openList.Sort((a, b) =>
            {
                if (a.F < b.F)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
        }

        void UpdateNeighborGrids()
        {
            //for (int i = 0; i < curGrid.neighborGrids.Count; i++)
            foreach (var item in curGrid.neighborGridDic)
            {
                int key = item.Key;
                Grid grid = item.Value;
                //Grid grid = curGrid.neighborGrids[i];
                if (grid == null)
                {
                    return;
                }
                if (!grid.walkAble)
                {
                    closeList.Add(grid);
                }
                else
                {
                    if (!closeList.Contains(grid))
                    {
                        if (grid != null)
                        {
                            openList.Add(grid);
                            int g = curGrid.G + 1;
                            int h = Mathf.Abs(endGrid.idx.x - grid.idx.x) + Mathf.Abs(endGrid.idx.y - grid.idx.y);
                            int f = g + h;
                            if (grid.F > f)
                            {
                                grid.preGrid = curGrid;
                                grid.G = g;
                                grid.H = h;
                                grid.F = f;
                            }
                        }
                    }
                }
            }
        }
    }
}

