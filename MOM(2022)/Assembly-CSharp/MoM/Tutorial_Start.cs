namespace MOM
{
    using DBUtils;
    using MHUtils.UI;
    using System;
    using TMPro;
    using UnityEngine.UI;

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
                object[] parameters = new object[] { GameManager.GetHumanWizard().GetName() };
                this.heading.text = Localization.Get("UI_WELCOME", true, parameters) + "!";
                object[] objArray2 = new object[] { GameManager.GetHumanWizard().GetName() };
                this.body.text = Localization.Get("UI_WELCOME_DES", true, objArray2);
            }
        }
    }
}

