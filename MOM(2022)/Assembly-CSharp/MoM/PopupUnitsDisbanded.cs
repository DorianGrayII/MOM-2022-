using System.Collections.Generic;
using MHUtils.UI;
using UnityEngine.UI;

namespace MOM
{
    public class PopupUnitsDisbanded : ScreenBase
    {
        private static PopupUnitsDisbanded instance;

        public Button byClose;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        public ArmyGrid gridUnits;

        public static void OpenPopup(ScreenBase parent, List<Reference<Unit>> units)
        {
            PopupUnitsDisbanded.instance = UIManager.Open<PopupUnitsDisbanded>(UIManager.Layer.Popup, parent);
            PopupUnitsDisbanded.instance.gridUnits.SetUnits(units);
            PopupUnitsDisbanded.instance.familiar.Set(GameManager.GetHumanWizard().familiar);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.byClose)
            {
                UIManager.Close(this);
            }
        }
    }
}
