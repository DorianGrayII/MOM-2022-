using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class CraftingItem
    {
        [ProtoMember(1)]
        public DBReference<global::DBDef.Unit> craftedUnit;

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

        public CraftingItem(global::DBDef.Unit u, FInt unitDiscount)
        {
            this.craftedUnit = u;
            this.requirementValue = (u.constructionCost * (FInt.ONE - unitDiscount)).ToInt();
        }

        public DescriptionInfo GetDI()
        {
            if (this.craftedUnit != null)
            {
                return this.craftedUnit.Get().GetDescriptionInfo();
            }
            return this.craftedBuilding.Get().GetDescriptionInfo();
        }

        public int BuyCost()
        {
            int result = 0;
            if (this.progress == 0 && this.requirementValue >= 0)
            {
                result = this.requirementValue * 4;
            }
            else if (this.progress != 0 && this.progress <= this.requirementValue / 3)
            {
                result = (this.requirementValue - this.progress) * 3;
            }
            else if (this.progress > this.requirementValue / 3)
            {
                result = (this.requirementValue - this.progress) * 2;
            }
            return result;
        }

        public float Progress()
        {
            if (this.requirementValue == 0)
            {
                return 0f;
            }
            return Mathf.Clamp01((float)this.progress / (float)this.requirementValue);
        }
    }
}
