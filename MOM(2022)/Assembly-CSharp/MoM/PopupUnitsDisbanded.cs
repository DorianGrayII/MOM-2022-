namespace MOM
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class PopupUnitsDisbanded : ScreenBase
    {
        private static PopupUnitsDisbanded instance;
        public Button byClose;
        public GameObjectEnabler<PlayerWizard.Familiar> familiar;
        public ArmyGrid gridUnits;

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.byClose)
            {
                UIManager.Close(this);
            }
        }

        public static void OpenPopup(ScreenBase parent, List<Reference<MOM.Unit>> units)
        {
            instance = UIManager.Open<PopupUnitsDisbanded>(UIManager.Layer.Popup, parent);
            instance.gridUnits.SetUnits(units);
            instance.familiar.Set(GameManager.GetHumanWizard().familiar);
        }
    }
}

