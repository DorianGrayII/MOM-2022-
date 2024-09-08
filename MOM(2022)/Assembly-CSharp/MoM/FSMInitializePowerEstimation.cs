// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMInitializePowerEstimation
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MOM;
using UnityEngine;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMInitializePowerEstimation : FSMStateBase
{
    public override void OnEnter()
    {
        GameManager.Get();
        try
        {
            PowerEstimate.LoadData();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Power estimation load failed " + ex);
        }
        base.OnEnter();
        base.StartCoroutine(this.PreparePowerEstimate());
    }

    public IEnumerator PreparePowerEstimate()
    {
        List<Subrace> units = new List<Subrace>(DataBase.GetType<global::DBDef.Unit>());
        units.AddRange(DataBase.GetType<Hero>());
        Dictionary<Subrace, BattleUnit> sourceToUnit = new Dictionary<Subrace, BattleUnit>();
        foreach (Subrace item in units)
        {
            BattleUnit value = BattleUnit.Create(global::MOM.Unit.CreateFrom(item, simulation: true), abstractMode: true);
            sourceToUnit.Add(item, value);
        }
        List<Subrace> list = units.FindAll((Subrace o) => DataBase.GetModOrder(o) == null);
        List<BattleUnit> baseUnits = new List<BattleUnit>();
        List<BattleUnit> trialedUnits = new List<BattleUnit>();
        foreach (Subrace item2 in list)
        {
            baseUnits.Add(sourceToUnit[item2]);
            trialedUnits.Add(sourceToUnit[item2]);
        }
        yield return PowerEstimate.SortByFight(baseUnits, trialedUnits, "powerTrainingData", null);
        if (ModManager.Get().GetActiveValidMods() != null)
        {
            foreach (ModOrder i in ModManager.Get().GetActiveValidMods())
            {
                list = units.FindAll((Subrace o) => DataBase.GetModOrder(o) == i);
                if (list.Count == 0)
                {
                    continue;
                }
                trialedUnits.Clear();
                foreach (Subrace item3 in list)
                {
                    trialedUnits.Add(sourceToUnit[item3]);
                }
                yield return PowerEstimate.SortByFight(baseUnits, trialedUnits, i.name, i);
            }
        }
        MHEventSystem.TriggerEvent<GameLoader>(this, 1f);
        foreach (Multitype<BattleUnit, int> powerValue in PowerEstimate.powerValues)
        {
            powerValue.t0.GetAttFinal(TAG.MELEE_ATTACK);
        }
        base.Finish();
    }
}
