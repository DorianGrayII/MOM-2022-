namespace WorldCode
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PathfinderNode
    {
        public int heuristicCost;
        public int curentCost;
    }
}

