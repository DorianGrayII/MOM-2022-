using System.Collections;
using MHUtils.UI;
using UnityEngine.UI;

namespace MOM
{
    public class PopupCapitalLost : ScreenBase
    {
        public Button btClose;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            this.familiar.Set(GameManager.GetHumanWizard().familiar);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                UIManager.Close(this);
            }
        }
    }
}
