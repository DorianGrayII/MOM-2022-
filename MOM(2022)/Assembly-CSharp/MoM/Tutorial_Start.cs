using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine.UI;

namespace MOM
{
    public class Tutorial_Start : ScreenBase
    {
        public Button btYes;

        public Button btNo;

        public TextMeshProUGUI heading;

        public TextMeshProUGUI body;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        private void Localize()
        {
            if (GameManager.GetHumanWizard() != null)
            {
                this.heading.text = Localization.Get("UI_WELCOME", true, GameManager.GetHumanWizard().GetName()) + "!";
                this.body.text = Localization.Get("UI_WELCOME_DES", true, GameManager.GetHumanWizard().GetName());
            }
        }
    }
}
