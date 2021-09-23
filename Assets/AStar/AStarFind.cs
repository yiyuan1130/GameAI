using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AStar
{
    public class Grid
    {
        public Grid preGrid;
        public Vector2Int idx;
        public List<Grid> neighborGrids; // 相邻格子 (逆时针存储 上左下右)
        public List<Vector3[]> neighborSides; // 相邻格子的公用边
        public List<Vector3> vertices; // 逆时针 0左上 1左下 2右下 3右上
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

        public void GenerateNeighbors(List<Grid> neighborGrids)
        {
            this.neighborGrids = neighborGrids;
            this.neighborSides = new List<Vector3[]>();
            for (int i = 0; i < this.neighborGrids.Count; i++)
            {
                Vector3[] side = new Vector3[2];
                Grid curNeighborGrid = this.neighborGrids[i];
                Vector2Int idxOffset = curNeighborGrid.idx - this.idx;
                if (idxOffset.x == 0 && idxOffset.y == 1) {
                    // 上
                    side[0] = this.vertices[0];
                    side[1] = this.vertices[3];
                }
                else if (idxOffset.x == 1 && idxOffset.y == 0) {
                    // 右
                    side[0] = this.vertices[2];
                    side[1] = this.vertices[3];
                }
                else if (idxOffset.x == 0 && idxOffset.y == -1) {
                    // 下
                    side[0] = this.vertices[1];
                    side[1] = this.vertices[2];
                }
                else if (idxOffset.x == -1 && idxOffset.y == 0) {
                    // 左
                    side[0] = this.vertices[0];
                    side[1] = this.vertices[1];
                }
                neighborSides.Add(side);
            }
        }

        void GenerateVertices() {
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

            Find();

            pathList = new List<Vector3>();
            Grid grid = endGrid;
            int count = 0;
            while (grid != null && grid != startGrid)// && count <= 20000)
            {
                pathList.Add(grid.bounds.center);
                grid = grid.preGrid;
                count++;
            }
            pathList.Reverse();
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

        void Find()
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
            for (int i = 0; i < curGrid.neighborGrids.Count; i++)
            {
                Grid grid = curGrid.neighborGrids[i];
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
