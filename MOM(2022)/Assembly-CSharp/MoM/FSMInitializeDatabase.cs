// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMInitializeDatabase
using System;
using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MOM;
using UnityEngine;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMInitializeDatabase : FSMStateBase
{
    public override void OnEnter()
    {
        DataBase.InitializeDB();
        base.OnEnter();
        GameplayConfiguration gameplayConfiguration = DataBase.Get<GameplayConfiguration>(GAMEPLAY_CONFIGURATION.DEFAULT);
        if (gameplayConfiguration != null && gameplayConfiguration.option != null)
        {
            Gc gc = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Log Battle results");
            if (gc != null)
            {
                try
                {
                    BattleAttackStack.logBattle = Convert.ToBoolean(gc.setting);
                }
                catch
                {
                    Debug.Log("Log Battle results configuration contains invalid type, expected boolean");
                }
            }
        }
        MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        base.Finish();
    }
}
