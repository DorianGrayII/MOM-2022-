// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.HallOfFame
using System.Collections;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;

public class HallOfFame : ScreenBase
{
    public Button btClose;

    public GridItemManager gridScore;

    private static HallOfFameBlock scoreData;

    private bool inGameMode;

    private bool endGame;

    public override IEnumerator PreStart()
    {
        this.gridScore.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            ScoreListItem component = itemSource.GetComponent<ScoreListItem>();
            HofEntry hofEntry = source as HofEntry;
            string text = Localization.Get(hofEntry.wizardName, true) + " " + Localization.Get("UI_OF_THE", true) + " " + Localization.Get(hofEntry.wizardRace, true);
            Texture2D texture = AssetManager.Get<Texture2D>(hofEntry.wizardPortrait);
            component.wizardIcon.texture = texture;
            component.labelPosition.text = (index + 1).ToString();
            component.labelNameRace.text = text;
            component.labelScore.text = hofEntry.wizardScore.ToString();
            component.newEntry.SetActive(hofEntry.isNew);
        });
        this.gridScore.UpdateGrid(HallOfFame.GetData().values);
        yield return base.PreStart();
    }

    public static HallOfFame Popup(bool endGame)
    {
        HallOfFame hallOfFame = UIManager.Open<HallOfFame>(UIManager.Layer.Popup);
        hallOfFame.inGameMode = true;
        hallOfFame.endGame = endGame;
        return hallOfFame;
    }

    public static HallOfFameBlock GetData()
    {
        return HallOfFameBlock.Load();
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (!(s == this.btClose))
        {
            return;
        }
        MHEventSystem.TriggerEvent(this, "FINISHED");
        if (this.inGameMode)
        {
            if (this.endGame)
            {
                FSMGameplay.Get().HandleEvent("ExitGameplay");
            }
            UIManager.Close(this);
        }
    }
}
