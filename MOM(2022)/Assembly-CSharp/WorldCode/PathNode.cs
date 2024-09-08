namespace WorldCode
{
    using DBDef;
    using MHUtils;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PathNode
    {
        public Vector3i pos;
        public int index;
        public int column;
        public int row;
        public int terrainMoveCost;
        public bool water;
        public bool mountain;
        public bool hill;
        public bool forest;
        public bool tundra;
        public bool desert;
        public bool swamp;
        public int requestID;
        public FInt gCost;
        public FInt hCost;
        public FInt sumCost;
        public int totalDanger;
        public int cameFrom;
        public int stepCount;
        public int nextItem;
        public int prevItem;
        public bool isOnTheList;
        public int nextAreaItem;
        public int prevAreaItem;
        public bool isOnTheAreaList;
        public void UpdateBaseData(Vector3i pos, SearcherDataV2 owner)
        {
            this.pos = pos;
            this.column = owner.GetColumn(pos);
            this.row = owner.GetRow(pos);
            this.index = owner.GetIndex(pos);
        }

        public void UpdateTerrainData(Hex h, int mpCost)
        {
            this.terrainMoveCost = mpCost;
            this.water = !h.IsLand();
            this.mountain = h.HaveFlag(ETerrainType.Mountain);
            this.hill = h.HaveFlag(ETerrainType.Hill);
            this.forest = h.HaveFlag(ETerrainType.Forest);
            this.tundra = h.HaveFlag(ETerrainType.Tundra);
            this.desert = h.HaveFlag(ETerrainType.Desert);
            this.swamp = h.HaveFlag(ETerrainType.Swamp);
        }

        public void EnsureInitialized(RequestDataV2 rd)
        {
            if (rd.requestID != this.requestID)
            {
                this.requestID = rd.requestID;
                this.gCost = FInt.ONE * 0x2710;
                if (rd.to == Vector3i.invalid)
                {
                    this.hCost = FInt.ZERO;
                }
                else if (SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo) < 0)
                {
                    this.hCost = new FInt(HexCoordinates.HexDistance(this.pos, rd.to));
                }
                else
                {
                    this.hCost = new FInt(0.1f * HexCoordinates.HexDistance(this.pos, rd.to));
                    if (rd.data.wrapping && ((this.hCost * 10) > (rd.data.width / 2)))
                    {
                        this.hCost = new FInt(0.1f * rd.data.width) - this.hCost;
                    }
                }
                this.sumCost = FInt.ONE * 0x2710;
                this.cameFrom = -1;
                this.stepCount = 0;
                this.nextItem = -1;
                this.prevItem = -1;
                this.isOnTheList = false;
                this.nextAreaItem = -1;
                this.prevAreaItem = -1;
                this.isOnTheAreaList = false;
                this.totalDanger = (rd.dangerMap != null) ? 0xf4240 : 0;
            }
        }

        public void SetGCost(FInt value)
        {
            this.gCost = value;
            this.sumCost = value + this.hCost;
        }
    }
}

