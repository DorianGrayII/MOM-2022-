// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupCapitalLost
using System.Collections;
using MHUtils.UI;
using MOM;
using UnityEngine.UI;

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
