using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar
{
    public class Grid
    {
        public Grid preGrid;
        public Vector2Int idx;
        public List<Vector3> vertices; // ��ʱ�� 0���� 1���� 2���� 3����

        // 0123 ��������
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
                        // ��
                        side[0] = this.vertices[0];
                        side[1] = this.vertices[3];
                    }
                    else if (idxOffset.x == -1 && idxOffset.y == 0)
                    {
                        // ��
                        side[0] = this.vertices[0];
                        side[1] = this.vertices[1];
                    }
                    else if (idxOffset.x == 0 && idxOffset.y == -1)
                    {
                        // ��
                        side[0] = this.vertices[1];
                        side[1] = this.vertices[2
                            ];
                    }
                    else if (idxOffset.x == 1 && idxOffset.y == 0)
                    {
                        // ��
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
            vertices.Add(this.bounds.center + new Vector3(-halfSize, 0, halfSize)); // ����
            vertices.Add(this.bounds.center + new Vector3(-halfSize, 0, -halfSize)); // ����
            vertices.Add(this.bounds.center + new Vector3(halfSize, 0, -halfSize)); // ����
            vertices.Add(this.bounds.center + new Vector3(halfSize, 0, halfSize)); // ����
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

        // ����յ�
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

            #region ��������·����
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
                        // ��
                        sides.Add(_curGrid.neighborSideDic[0]);
                    }
                    else if (idxOffset.x == -1 && idxOffset.y == 0)
                    {
                        // ��
                        sides.Add(_curGrid.neighborSideDic[1]);
                    }
                    else if (idxOffset.x == 0 && idxOffset.y == -1)
                    {
                        // ��
                        sides.Add(_curGrid.neighborSideDic[2]);
                    }
                    else if (idxOffset.x == 1 && idxOffset.y == 0)
                    {
                        // ��
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
                        case 1: // ������ڿ�����һ�� ����Ϊ�µ�������
                            curPoints = nextPoints;
                            break;
                        case 2: // �����ڿ��������� ������
                            break;
                        case 3: // ����ڿ�����࣬�ҵ��ڿ����ڲ� ����ǹյ�  �������
                            curPos = curPoints[0];
                            pathList.Add(curPos);
                            curPoints[0] = nextPoints[0];
                            break;
                        case 4: // ����ڿ����ڲ࣬�ҵ��ڿ����Ҳ� �ҵ��ǹյ�  �����ҵ�
                            curPos = curPoints[1];
                            pathList.Add(curPos);
                            curPoints[1] = nextPoints[1];
                            break;
                        case 5: // ���ҵ���ڿ������ ���Ϊ�յ�
                            curPos = curPoints[0];
                            pathList.Add(curPos);
                            curPoints = nextPoints;
                            i++;
                            break;
                        case 6: // ���ҵ���ڿ����Ҳ� �ҵ�Ϊ�յ�
                            curPos = curPoints[1];
                            pathList.Add(curPos);
                            curPoints = nextPoints;
                            i++;
                            break;
                        case 0:
                            Debug.LogError("������λ�ò�ȷ��");
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

        // �Ҹ���·��
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

