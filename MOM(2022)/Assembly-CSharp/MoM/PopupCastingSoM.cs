using System.Collections;
using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine.UI;

namespace MOM
{
    public class PopupCastingSoM : ScreenBase
    {
        public Button btClose;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        public TextMeshProUGUI labelStartedCasting;

        public TextMeshProUGUI labelIsCasting;

        private static PopupCastingSoM instance;

        public static PopupCastingSoM OpenPopup(ScreenBase parent, PlayerWizard caster)
        {
            PopupCastingSoM.instance = UIManager.Open<PopupCastingSoM>(UIManager.Layer.Popup, parent);
            string text;
            if (caster.ID == PlayerWizard.HumanID())
            {
                text = Localization.Get("UI_YOU_STARTED_SOM", true);
                PopupCastingSoM.instance.labelIsCasting.text = Localization.Get("UI_SOM_PLAYER_CASTING_INFO", true);
            }
            else
            {
                text = Localization.Get("UI_WIZARD_STARTED_SOM", true, caster.GetName());
                PopupCastingSoM.instance.labelIsCasting.text = Localization.Get("UI_SOM_ENEMY_CASTING_INFO", true, caster.GetName());
            }
            PopupCastingSoM.instance.familiar.Clear();
            if (GameManager.GetHumanWizard() != null)
            {
                PopupCastingSoM.instance.familiar.Set(GameManager.GetHumanWizard().familiar);
            }
            PopupCastingSoM.instance.labelStartedCasting.text = text;
            AudioLibrary.RequestSFX("OpenPopupCastingSoM");
            return PopupCastingSoM.instance;
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                UIManager.Close(this);
            }
        }

        public static bool IsOpen()
        {
            return PopupCastingSoM.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupCastingSoM.instance = null;
            yield return base.Closing();
        }
    }
}
