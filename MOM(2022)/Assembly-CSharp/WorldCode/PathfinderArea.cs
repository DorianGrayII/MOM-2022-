namespace WorldCode
{
    using MHUtils;
    using System;

    public class PathfinderArea
    {
        private PathfinderNode[,] areaDataMap;

        public PathfinderArea(int w, int h)
        {
            this.areaDataMap = new PathfinderNode[w, h];
        }

        public int GetHeuristicValue(Vector3i v)
        {
            int num = (v.x < 0) ? (v.x + this.areaDataMap.GetLength(0)) : v.x;
            int num2 = (v.y < 0) ? (v.y + this.areaDataMap.GetLength(1)) : v.y;
            num = (num >= this.areaDataMap.GetLength(0)) ? (num - this.areaDataMap.GetLength(0)) : num;
            return this.areaDataMap[num, (num2 >= this.areaDataMap.GetLength(1)) ? (num2 - this.areaDataMap.GetLength(1)) : num2].heuristicCost;
        }

        public PathfinderNode GetNode(Vector3i v)
        {
            int num = (v.x < 0) ? (v.x + this.areaDataMap.GetLength(0)) : v.x;
            int num2 = (v.y < 0) ? (v.y + this.areaDataMap.GetLength(1)) : v.y;
            num = (num >= this.areaDataMap.GetLength(0)) ? (num - this.areaDataMap.GetLength(0)) : num;
            return this.areaDataMap[num, (num2 >= this.areaDataMap.GetLength(1)) ? (num2 - this.areaDataMap.GetLength(1)) : num2];
        }

        public int GetTotalValue(Vector3i v)
        {
            int num = (v.x < 0) ? (v.x + this.areaDataMap.GetLength(0)) : v.x;
            int num2 = (v.y < 0) ? (v.y + this.areaDataMap.GetLength(1)) : v.y;
            num = (num >= this.areaDataMap.GetLength(0)) ? (num - this.areaDataMap.GetLength(0)) : num;
            return this.areaDataMap[num, (num2 >= this.areaDataMap.GetLength(1)) ? (num2 - this.areaDataMap.GetLength(1)) : num2].curentCost;
        }

        public void SetHeuristicValue(Vector3i v, int value)
        {
            int num = (v.x < 0) ? (v.x + this.areaDataMap.GetLength(0)) : v.x;
            int num2 = (v.y < 0) ? (v.y + this.areaDataMap.GetLength(1)) : v.y;
            num = (num >= this.areaDataMap.GetLength(0)) ? (num - this.areaDataMap.GetLength(0)) : num;
            this.areaDataMap[num, (num2 >= this.areaDataMap.GetLength(1)) ? (num2 - this.areaDataMap.GetLength(1)) : num2].heuristicCost = value;
        }

        public void SetTotalValue(Vector3i v, int value)
        {
            int num = (v.x < 0) ? (v.x + this.areaDataMap.GetLength(0)) : v.x;
            int num2 = (v.y < 0) ? (v.y + this.areaDataMap.GetLength(1)) : v.y;
            num = (num >= this.areaDataMap.GetLength(0)) ? (num - this.areaDataMap.GetLength(0)) : num;
            this.areaDataMap[num, (num2 >= this.areaDataMap.GetLength(1)) ? (num2 - this.areaDataMap.GetLength(1)) : num2].curentCost = value;
        }
    }
}

