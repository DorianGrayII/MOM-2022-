using DBDef;
using MHUtils.UI;
using UnityEngine;

namespace MOM
{
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
            if (source is Race race)
            {
                this.raceInfo.Set(race);
            }
        }
    }
}
