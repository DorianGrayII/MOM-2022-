using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("MAGIC_NODE", "")]
    public class MagicNode : Location
    {
        public new static string abbreviation = "";

        [Prototype("NodeType", true)]
        public ENodeType nodeType;

        [Prototype("CustomTerrainType", true)]
        public Terrain customTerrainType;

        [Prototype("PowerRange", true)]
        public IntRange powerRange;

        [Prototype("UnitBonus", true)]
        public new Enchantment[] unitBonus;

        public static explicit operator MagicNode(Enum e)
        {
            return DataBase.Get<MagicNode>(e);
        }

        public static explicit operator MagicNode(string e)
        {
            return DataBase.Get<MagicNode>(e, reportMissing: true);
        }

        public new void Set_unitBonus(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.unitBonus = new Enchantment[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Enchantment))
                {
                    Debug.LogError("unitBonus of type Enchantment received invalid type from array! " + list[i]);
                }
                this.unitBonus[i] = list[i] as Enchantment;
            }
        }
    }
}
