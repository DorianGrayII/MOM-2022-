// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMApplyBattleResult
using System.Collections;
using HutongGames.PlayMaker;
using MOM;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMApplyBattleResult : FSMStateBase
{
    private Battle battle;

    public override void OnEnter()
    {
        base.OnEnter();
        this.battle = Battle.GetBattle();
        Battle.ApplyBattleChanges(this.battle);
        if (this.battle.battleWinner == Battle.BattleWinner.ATTACKER_WINS)
        {
            Location locationHostSmart = this.battle.gDefender.GetLocationHostSmart();
            if (locationHostSmart != null)
            {
                base.StartCoroutine(this.Finishing(locationHostSmart));
            }
            else
            {
                base.Finish();
            }
        }
        else
        {
            base.Finish();
        }
    }

    private IEnumerator Finishing(Location loc)
    {
        if (this.battle.playerIsAttacker)
        {
            if (loc is TownLocation)
            {
                base.Fsm.Event("PopupTownCaptured");
                yield break;
            }
        }
        else
        {
            this.battle.gAttacker.TakeOverLocation(loc);
        }
        base.Finish();
    }
}
