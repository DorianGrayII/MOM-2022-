namespace MOM
{
    using DBDef;
    using MHUtils;
    using ProtoBuf;
    using System;
    using UnityEngine;

    [ProtoContract]
    public class CraftingItem
    {
        [ProtoMember(1)]
        public DBReference<DBDef.Unit> craftedUnit;
        [ProtoMember(2)]
        public DBReference<Building> craftedBuilding;
        [ProtoMember(3)]
        public int progress;
        [ProtoMember(4)]
        public int requirementValue;

        public CraftingItem()
        {
        }

        public CraftingItem(Building b)
        {
            this.craftedBuilding = b;
            this.requirementValue = b.buildCost;
        }

        public CraftingItem(Building b, FInt buildingDiscount)
        {
            this.craftedBuilding = b;
            this.requirementValue = (b.buildCost * (FInt.ONE - buildingDiscount)).ToInt();
        }

        public CraftingItem(DBDef.Unit u, FInt unitDiscount)
        {
            this.craftedUnit = u;
            this.requirementValue = (u.constructionCost * (FInt.ONE - unitDiscount)).ToInt();
        }

        public int BuyCost()
        {
            int num = 0;
            if ((this.progress == 0) && (this.requirementValue >= 0))
            {
                num = this.requirementValue * 4;
            }
            else if ((this.progress != 0) && (this.progress <= (this.requirementValue / 3)))
            {
                num = (this.requirementValue - this.progress) * 3;
            }
            else if (this.progress > (this.requirementValue / 3))
            {
                num = (this.requirementValue - this.progress) * 2;
            }
            return num;
        }

        public DescriptionInfo GetDI()
        {
            return ((this.craftedUnit == null) ? this.craftedBuilding.Get().GetDescriptionInfo() : this.craftedUnit.Get().GetDescriptionInfo());
        }

        public float Progress()
        {
            return ((this.requirementValue != 0) ? Mathf.Clamp01(((float) this.progress) / ((float) this.requirementValue)) : 0f);
        }
    }
}

