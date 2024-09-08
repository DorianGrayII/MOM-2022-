using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("ARTEFACT_GRAPHIC", "")]
    public class ArtefactGraphic : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;

        [Prototype("RequiredPower", false)]
        public ArtefactPower[] requiredPower;

        [Prototype("RequiredPowerSet", false)]
        public ArtefactPowerSet[] requiredPowerSet;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator ArtefactGraphic(Enum e)
        {
            return DataBase.Get<ArtefactGraphic>(e);
        }

        public static explicit operator ArtefactGraphic(string e)
        {
            return DataBase.Get<ArtefactGraphic>(e, reportMissing: true);
        }

        public void Set_requiredPower(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.requiredPower = new ArtefactPower[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is ArtefactPower))
                {
                    Debug.LogError("requiredPower of type ArtefactPower received invalid type from array! " + list[i]);
                }
                this.requiredPower[i] = list[i] as ArtefactPower;
            }
        }

        public void Set_requiredPowerSet(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.requiredPowerSet = new ArtefactPowerSet[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is ArtefactPowerSet))
                {
                    Debug.LogError("requiredPowerSet of type ArtefactPowerSet received invalid type from array! " + list[i]);
                }
                this.requiredPowerSet[i] = list[i] as ArtefactPowerSet;
            }
        }
    }
}
