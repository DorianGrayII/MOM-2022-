using System;
using System.Collections.Generic;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("ARTEFACT_SLOT", "")]
    public class ArtefactSlot : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;

        [Prototype("ETypes", false)]
        public EEquipmentType[] eTypes;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator ArtefactSlot(Enum e)
        {
            return DataBase.Get<ArtefactSlot>(e);
        }

        public static explicit operator ArtefactSlot(string e)
        {
            return DataBase.Get<ArtefactSlot>(e, reportMissing: true);
        }

        public void Set_eTypes(List<object> list)
        {
            if (list != null && list.Count != 0)
            {
                this.eTypes = new EEquipmentType[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    this.eTypes[i] = (EEquipmentType)list[i];
                }
            }
        }
    }
}
