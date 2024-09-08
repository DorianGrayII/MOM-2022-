// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMCleanup
using System;
using System.Collections;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using UnityEngine;
using WorldCode;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMCleanup : FSMStateBase
{
    public bool fullCleanupForced;

    public override void OnEnter()
    {
        base.StartCoroutine(this.CleanupSequence());
    }

    private IEnumerator CleanupSequence()
    {
        World.Get().seed = 0;
        World.CleanupSequence();
        FOW.CleanupSequence();
        EntityManager.CleanupSequence();
        TurnManager.CleanupSequence();
        VerticalMarkerManager.Get().ResetSystem();
        DifficultySettingsData.Clear();
        MinimapManager.Get().Clear();
        MemoryManager.ClearGameplay();
        MemoryManager.LogStatus();
        AdventureManager.Clear();
        EnchantmentRegister.Clear();
        ProtoLibrary.GetInstance()?.ResetAfterChanges();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;
        while (HUD.Get() != null)
        {
            yield return null;
        }
        SaveGame.DoQueuedLoad();
        base.Finish();
    }
}
