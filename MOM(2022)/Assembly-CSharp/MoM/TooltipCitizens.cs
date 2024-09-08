namespace MOM
{
    using DBDef;
    using MHUtils.UI;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(RaceInfo))]
    public class TooltipCitizens : TooltipBase
    {
        private RaceInfo raceInfo;

        private void Awake()
        {
            this.raceInfo = base.GetComponent<RaceInfo>();
            ScreenBase.LocalizeTextFields(base.gameObject);
        }

        public override void Populate(object source)
        {
            Race race = source as Race;
            if (race != null)
            {
                this.raceInfo.Set(race, null);
            }
        }
    }
}

