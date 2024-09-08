// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Adventures.AdventureManager
using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using UnityEngine;

public class AdventureManager
{
    public static bool processScripts = true;

    public int nextEvent;

    public static Dictionary<Adventure, int> lastOccurence = new Dictionary<Adventure, int>();

    public static Dictionary<Adventure, List<int>> visitedNodes = new Dictionary<Adventure, List<int>>();

    public static int incomingChoice = 0;

    public static Battle battleResultSource;

    public static NodeBattle.RESULT battleResult;

    private static BaseNode curentNode;

    public bool eventsTestMode;

    private int EVENT_TURN_TRESHOLD = 50;

    public AdventureManager()
    {
        MHEventSystem.RegisterListener<AdventureScreen>(AdventureEvents, this);
        MHEventSystem.RegisterListener<Battle>(BattleResult, this);
    }

    public void Destroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    private void AdventureEvents(object sender, object e)
    {
        if (sender == AdventureManager.curentNode && e is int)
        {
            AdventureManager.incomingChoice = (int)e;
        }
    }

    private void BattleResult(object sender, object e)
    {
        if (sender is Battle)
        {
            AdventureManager.battleResultSource = sender as Battle;
        }
    }

    private static NodeBattle.RESULT GetBattleResult(int wizardID)
    {
        if (AdventureManager.battleResultSource == null)
        {
            return NodeBattle.RESULT.Surrender;
        }
        bool flag = false;
        bool flag2 = false;
        foreach (BattleUnit attackerUnit in AdventureManager.battleResultSource.attackerUnits)
        {
            if (attackerUnit.IsAlive())
            {
                flag = true;
                break;
            }
        }
        foreach (BattleUnit defenderUnit in AdventureManager.battleResultSource.defenderUnits)
        {
            if (defenderUnit.IsAlive())
            {
                flag2 = true;
                break;
            }
        }
        if (flag && flag2)
        {
            return NodeBattle.RESULT.Surrender;
        }
        if (AdventureManager.battleResultSource.attacker.GetID() == wizardID && flag)
        {
            return NodeBattle.RESULT.Win;
        }
        return NodeBattle.RESULT.Lost;
    }

    public bool IsNextEvent(PlayerWizard w)
    {
        if (this.eventsTestMode)
        {
            return true;
        }
        if (TurnManager.GetTurnNumber() < this.EVENT_TURN_TRESHOLD)
        {
            return false;
        }
        if (TurnManager.GetTurnNumber() == this.EVENT_TURN_TRESHOLD)
        {
            w.nextEventDelay = Random.Range(0, 15);
        }
        w.nextEventDelay--;
        return w.nextEventDelay <= 0;
    }

    public IEnumerator TriggerGenericEvents(PlayerWizard w)
    {
        yield return null;
        List<Adventure> list = new List<Adventure>(AdventureLibrary.currentLibrary.GetGenericEvents());
        list.RandomSort();
        foreach (Adventure item in list)
        {
            AdventureData adventureData = AdventureManager.TryToTriggerAdventure(item, w, null, null);
            if (adventureData != null)
            {
                AdventureManager.ResolveEvent(adventureData, item);
                while (AdventureManager.IsAdventureRunning())
                {
                    yield return null;
                }
            }
        }
    }

    public IEnumerator TrigerTurnEvents(PlayerWizard w)
    {
        yield return null;
        List<Adventure> perPlayerEvents = AdventureLibrary.currentLibrary.GetPerPlayerEvents();
        Random.Range(0, perPlayerEvents.Count);
        List<Adventure> list = new List<Adventure>(perPlayerEvents);
        list.RandomSort();
        foreach (Adventure item in list)
        {
            AdventureData adventureData = AdventureManager.TryToTriggerAdventure(item, w, null, null);
            if (adventureData != null)
            {
                w.nextEventDelay = Random.Range(10, 25);
                AdventureManager.ResolveEvent(adventureData, item);
                while (AdventureManager.IsAdventureRunning())
                {
                    yield return null;
                }
                break;
            }
        }
    }

    public static AdventureData TryToTriggerAdventure(Adventure a, PlayerWizard initiatorWizard, IGroup adventureSource, IGroup initiator, bool devMode = false)
    {
        NodeStart start = a.GetStart();
        if (!devMode && start.allowOnce && AdventureManager.lastOccurence.ContainsKey(a))
        {
            return null;
        }
        if (!devMode && AdventureManager.lastOccurence.ContainsKey(a))
        {
            int turnNumber = TurnManager.GetTurnNumber();
            if (start.cooldown > turnNumber - AdventureManager.lastOccurence[a])
            {
                return null;
            }
        }
        else
        {
            int turnNumber2 = TurnManager.GetTurnNumber();
            if (!devMode && start.initialDelay > turnNumber2)
            {
                return null;
            }
        }
        Dictionary<string, AdvList> dictionary = new Dictionary<string, AdvList>();
        Dictionary<string, AdvList> dictionary2 = new Dictionary<string, AdvList>();
        AdventureData adventureData = new AdventureData();
        if (initiatorWizard != null)
        {
            adventureData.mainPlayerWizard = initiatorWizard.ID;
        }
        else
        {
            adventureData.mainPlayerWizard = 0;
        }
        if (initiator != null)
        {
            adventureData.mainPlayerGroup = initiator;
            if (initiator is global::MOM.Group)
            {
                adventureData.adventurePlane = (initiator as global::MOM.Group).GetPlane();
            }
            else if (initiator is global::MOM.Location)
            {
                adventureData.adventurePlane = (initiator as global::MOM.Location).GetPlane();
            }
        }
        adventureData.advSource = adventureSource;
        if (AdventureManager.ProcessEntryLogic(adventureData, start, dictionary, dictionary2, a))
        {
            adventureData.publicLits = dictionary;
            adventureData.temporaryLists = dictionary2;
            return adventureData;
        }
        return null;
    }

    private static bool ProcessEntryLogic(AdventureData adventureData, BaseNode node, Dictionary<string, AdvList> lists, Dictionary<string, AdvList> retLists, Adventure a)
    {
        if (!AdventureManager.processScripts)
        {
            return true;
        }
        if (node.logic != null)
        {
            for (int i = 0; i < node.logic.Count; i++)
            {
                if (!(node.logic[i] is LogicEntry))
                {
                    continue;
                }
                LogicEntry logicEntry = node.logic[i] as LogicEntry;
                if (!string.IsNullOrEmpty(logicEntry.GetScriptName()) && ScriptLibrary.Call(logicEntry.GetScriptName(), adventureData, node, logicEntry, lists, retLists) is AdvList advList)
                {
                    if (retLists == null)
                    {
                        retLists = new Dictionary<string, AdvList>();
                    }
                    if (string.IsNullOrEmpty(logicEntry.listName))
                    {
                        string message = "Adventure error! List name cannot be empty!\nScript:" + logicEntry.GetScriptName() + "\nAdventure:" + a.name + "\nNode:" + node.ID;
                        Debug.LogWarning(message);
                        PopupGeneral.OpenPopup(null, "UI_WARNING", message, "UI_OK");
                        return false;
                    }
                    advList.makePublic = logicEntry.makeListPublic;
                    retLists[logicEntry.listName] = advList;
                }
            }
            for (int j = 0; j < node.logic.Count; j++)
            {
                if (!(node.logic[j] is LogicProcessing))
                {
                    continue;
                }
                LogicProcessing logicProcessing = node.logic[j] as LogicProcessing;
                if (!string.IsNullOrEmpty(logicProcessing.GetScriptName()) && ScriptLibrary.Call(logicProcessing.GetScriptName(), adventureData, node, logicProcessing, lists, retLists) is AdvList advList2)
                {
                    if (retLists == null)
                    {
                        retLists = new Dictionary<string, AdvList>();
                    }
                    if (string.IsNullOrEmpty(logicProcessing.listName))
                    {
                        string message2 = "Adventure error! List name cannot be empty!\nScript:" + logicProcessing.GetScriptName() + "\nAdventure:" + a.name + "\nNode:" + node.ID;
                        Debug.LogWarning(message2);
                        PopupGeneral.OpenPopup(null, "UI_WARNING", message2, "UI_OK");
                        return false;
                    }
                    advList2.makePublic = logicProcessing.makeListPublic;
                    retLists[logicProcessing.listName] = advList2;
                }
            }
            Dictionary<LogicRequirement.LogicGroupHint, int> dictionary = new Dictionary<LogicRequirement.LogicGroupHint, int>();
            for (int k = 0; k < node.logic.Count; k++)
            {
                if (!(node.logic[k] is LogicRequirement))
                {
                    continue;
                }
                LogicRequirement logicRequirement = node.logic[k] as LogicRequirement;
                if ((logicRequirement.group != 0 && dictionary.ContainsKey(logicRequirement.group) && dictionary[logicRequirement.group] > 0) || string.IsNullOrEmpty(logicRequirement.GetScriptName()))
                {
                    continue;
                }
                bool flag = (bool)ScriptLibrary.Call(logicRequirement.GetScriptName(), adventureData, node, logicRequirement, lists, retLists);
                if (logicRequirement.group != 0)
                {
                    if (flag)
                    {
                        dictionary[logicRequirement.group] = 1;
                    }
                    else
                    {
                        dictionary[logicRequirement.group] = 0;
                    }
                }
                else if (!flag)
                {
                    return false;
                }
            }
            foreach (KeyValuePair<LogicRequirement.LogicGroupHint, int> item in dictionary)
            {
                if (item.Value == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static void ProcessEnteringNode(AdventureData adventureData, BaseNode node, Dictionary<string, AdvList> lists, Dictionary<string, AdvList> retLists)
    {
        if (!AdventureManager.processScripts || node.logic == null)
        {
            return;
        }
        for (int i = 0; i < node.logic.Count; i++)
        {
            if (!(node.logic[i] is LogicModifier))
            {
                continue;
            }
            LogicModifier logicModifier = node.logic[i] as LogicModifier;
            if (!string.IsNullOrEmpty(logicModifier.GetScriptName()))
            {
                ScriptLibrary.Call(out var error, logicModifier.GetScriptName(), adventureData, node, logicModifier, lists, retLists);
                if (error > 0)
                {
                    Debug.LogError("Script call returned error in Module: " + node.parentEvent.module?.ToString() + ", event: " + node.parentEvent.name + ", node: " + node.ID + ", script:" + logicModifier.GetScriptName());
                }
            }
            else
            {
                Debug.LogError("Module: " + node.parentEvent.module?.ToString() + ", event: " + node.parentEvent.name + ", node: " + node.ID + " doesn't exist");
            }
        }
    }

    private static void ProgressToNode(AdventureData ad, Adventure a, BaseNode n)
    {
        AdventureManager.ProcessEnteringNode(ad, n, ad.publicLits, ad.temporaryLists);
        ad.visitedNodes.Add(n.ID);
        foreach (KeyValuePair<string, AdvList> temporaryList in ad.temporaryLists)
        {
            if (temporaryList.Value.makePublic)
            {
                ad.publicLits[temporaryList.Key] = temporaryList.Value;
            }
        }
    }

    public static void ResolveEvent(AdventureData ad, Adventure a)
    {
        if (a.module == null)
        {
            a.ReAcquireModule();
        }
        if (ad.mainPlayerWizard != PlayerWizard.HumanID())
        {
            FSMAdventure.instance.StartCoroutine(FSMAdventure.QueueOne(AdventureManager._ResolveAIEvent(ad, a)));
        }
        else
        {
            FSMAdventure.instance.StartCoroutine(FSMAdventure.QueueOne(AdventureManager._ResolveEvent(ad, a)));
        }
    }

    public static bool IsAdventureRunning()
    {
        return FSMAdventure.IsRunning();
    }

    private static IEnumerator _ResolveAIEvent(AdventureData ad, Adventure a)
    {
        GameManager.Get().TakeFocus(ad, GameManager.FocusFlag.Adventure);
        Debug.Log("AI " + ad.mainPlayerWizard + " resolving event " + a.ToString());
        AdventureManager.lastOccurence[a] = TurnManager.GetTurnNumber();
        if (ad.visitedNodes.Count > 0)
        {
            Debug.LogError("Unexpected, event have visited nodes!");
        }
        AdventureManager.ProgressToNode(ad, a, a.GetStart());
        AdventureManager.curentNode = a.GetStart();
        bool advEnd;
        do
        {
            AdventureManager.incomingChoice = -1;
            BaseNode baseNode;
            if (AdventureManager.curentNode is NodeStory)
            {
                List<Dictionary<string, AdvList>> ret = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret);
            }
            else if (AdventureManager.curentNode is NodeWorldEnchantment)
            {
                NodeWorldEnchantment nodeWorldEnchantment = AdventureManager.curentNode as NodeWorldEnchantment;
                Enchantment enchantment = DataBase.Get<Enchantment>(nodeWorldEnchantment.enchantmentName, reportMissing: false);
                if (enchantment != null)
                {
                    Enchantment e = enchantment;
                    int num = Mathf.RoundToInt((float)ScriptLibrary.Call("UTIL_GetRandomFromStringParameter", nodeWorldEnchantment.scriptStringDuration));
                    if (num == 0)
                    {
                        num = -1;
                    }
                    string parameters = ((float)ScriptLibrary.Call("UTIL_GetRandomFromStringParameter", nodeWorldEnchantment.scriptStringParameter)).ToString();
                    string targetName = nodeWorldEnchantment.targetName;
                    AdvList listByName = ad.GetListByName(targetName, null);
                    if (listByName != null)
                    {
                        foreach (object item in listByName.list)
                        {
                            if (item is Reference<global::MOM.Unit>)
                            {
                                (item as Reference<global::MOM.Unit>).Get().AddEnchantment(e, null, num, parameters);
                            }
                            else if (item is Reference<global::MOM.Location>)
                            {
                                (item as Reference<global::MOM.Location>).Get().AddEnchantment(e, null, num, parameters);
                            }
                            else if (item is IEnchantable)
                            {
                                (item as IEnchantable).AddEnchantment(e, null, num, parameters);
                            }
                        }
                    }
                    else if (targetName == NodeWorldEnchantment.WizardCriteria.AllWizzards.ToString())
                    {
                        GameManager.GetWizards();
                        foreach (PlayerWizard wizard in GameManager.GetWizards())
                        {
                            wizard.AddEnchantment(e, null, num, parameters);
                        }
                    }
                    else if (targetName == NodeWorldEnchantment.WizardCriteria.CurrentWizzard.ToString())
                    {
                        GameManager.GetWizard(ad.mainPlayerWizard).AddEnchantment(e, null, num, parameters);
                    }
                }
                List<Dictionary<string, AdvList>> ret2 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret2);
            }
            else if (AdventureManager.curentNode is NodeSpawnLocation)
            {
                NodeSpawnLocation nodeSpawnLocation = AdventureManager.curentNode as NodeSpawnLocation;
                if (!nodeSpawnLocation.destroyOwner)
                {
                    ScriptLibrary.Call(nodeSpawnLocation.scriptName, ad, AdventureManager.curentNode);
                }
                else if (ad.advSource is global::MOM.Location)
                {
                    (ad.advSource as global::MOM.Location).Destroy();
                }
                List<Dictionary<string, AdvList>> ret3 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret3);
            }
            else if (AdventureManager.curentNode is NodeBattle)
            {
                NodeBattle nodeBattle = AdventureManager.curentNode as NodeBattle;
                global::MOM.Group group = (global::MOM.Group)ScriptLibrary.Call(nodeBattle.scriptName, ad, AdventureManager.curentNode);
                if (group == null || group.GetUnits().Count == 0)
                {
                    Debug.LogError("Missing opponent group in event: " + a.name + " module: " + a.module);
                }
                List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>();
                if (!string.IsNullOrEmpty(nodeBattle.listA))
                {
                    AdvList listByName2 = ad.GetListByName(nodeBattle.listA, null);
                    if (listByName2 != null)
                    {
                        foreach (object item2 in listByName2.list)
                        {
                            if (item2 is global::MOM.Unit)
                            {
                                list.Add(item2 as global::MOM.Unit);
                            }
                            else if (item2 is global::MOM.Group)
                            {
                                list.AddRange((item2 as global::MOM.Group).GetUnits());
                            }
                        }
                    }
                }
                if (list.Count == 0)
                {
                    list = ad.mainPlayerGroup.GetUnits();
                }
                if (list == null || list.Count < 1)
                {
                    AdventureManager.battleResult = NodeBattle.RESULT.Lost;
                }
                else if (group == null || group.GetUnits() == null || group.GetUnits().Count < 1)
                {
                    AdventureManager.battleResult = NodeBattle.RESULT.Win;
                }
                else
                {
                    Battle battle = new Battle(list, group.GetUnits(), ad.mainPlayerWizard, 0);
                    battle.DebugMode(value: true);
                    bool landBattle = group.GetPlane()?.GetHexAt(group.GetPosition())?.IsLand() ?? true;
                    battle.landBattle = landBattle;
                    battle.temperature = 0.5f;
                    battle.humidity = 0.5f;
                    battle.forest = 0.38f;
                    while (FSMMapGame.Get() == null)
                    {
                        yield return null;
                    }
                    FSMCoreGame.Get().StartBattle(battle);
                    while (true)
                    {
                        bool num2 = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
                        bool flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
                        if (!num2 && !flag)
                        {
                            break;
                        }
                        yield return null;
                    }
                    AdventureManager.battleResult = AdventureManager.GetBattleResult(ad.mainPlayerWizard);
                }
                List<Dictionary<string, AdvList>> ret4 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret4, -1, AdventureManager.battleResult.ToString());
            }
            else if (AdventureManager.curentNode is NodeLocationAward)
            {
                NodeLocationAward nodeLocationAward = AdventureManager.curentNode as NodeLocationAward;
                if (ad.advSource is global::MOM.Location)
                {
                    global::MOM.Location location = ad.advSource as global::MOM.Location;
                    nodeLocationAward.ClaimAward(GameManager.GetWizard(ad.mainPlayerWizard), location, ad.mainPlayerGroup, ad.heroes);
                }
                List<Dictionary<string, AdvList>> ret5 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret5);
            }
            else
            {
                List<Dictionary<string, AdvList>> ret6 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret6);
            }
            if (baseNode == null)
            {
                Debug.LogError("Softlock after inability to choose next step in the adventure " + a.name + " node: " + AdventureManager.curentNode.ID);
            }
            AdventureManager.ProgressToNode(ad, a, baseNode);
            AdventureManager.curentNode = baseNode;
            advEnd = false;
            if (baseNode is NodeEnd)
            {
                if ((baseNode as NodeEnd).destroyEvent && ad.advSource is global::MOM.Location location2)
                {
                    location2.RemoveAdventureTrigger();
                }
                advEnd = true;
            }
            if (ad.heroes.Count > 0)
            {
                yield return AdventureManager.TryClaimHero(ad, a);
            }
        }
        while (!advEnd);
        GameManager.Get().FreeFocus(ad);
    }

    private static IEnumerator _ResolveEvent(AdventureData ad, Adventure a)
    {
        Debug.Log("Human resolving event " + a.ToString());
        HUD.Get()?.Hide();
        GameManager.Get().TakeFocus(ad, GameManager.FocusFlag.Adventure);
        AdventureOutcomeDelta delta = new AdventureOutcomeDelta();
        delta.Store();
        AdventureManager.lastOccurence[a] = TurnManager.GetTurnNumber();
        if (ad.visitedNodes.Count > 0)
        {
            Debug.LogError("Unexpected, event have visited nodes!");
        }
        AdventureManager.ProgressToNode(ad, a, a.GetStart());
        AdventureManager.curentNode = a.GetStart();
        AdventureScreen screen = UIManager.Open<AdventureScreen>(UIManager.Layer.Standard);
        bool advEnd;
        do
        {
            AdventureManager.incomingChoice = -1;
            string image = AdventureManager.curentNode.image;
            if (!string.IsNullOrEmpty(image))
            {
                ad.imageName = image;
            }
            BaseNode baseNode;
            if (AdventureManager.curentNode is NodeStory)
            {
                List<Dictionary<string, AdvList>> ret = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                List<AdventureOutcomeDelta.Outcome> outcomes = delta.GetOutcomes();
                screen.UpdateScreenBy(ad, a, AdventureManager.curentNode, outcomes);
                while (AdventureManager.incomingChoice == -1)
                {
                    yield return null;
                }
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret, AdventureManager.incomingChoice);
                delta.Store();
            }
            else if (AdventureManager.curentNode is NodeWorldEnchantment)
            {
                NodeWorldEnchantment nodeWorldEnchantment = AdventureManager.curentNode as NodeWorldEnchantment;
                Enchantment enchantment = DataBase.Get<Enchantment>(nodeWorldEnchantment.enchantmentName, reportMissing: false);
                if (enchantment != null)
                {
                    Enchantment e = enchantment;
                    int num = Mathf.RoundToInt((float)ScriptLibrary.Call("UTIL_GetRandomFromStringParameter", nodeWorldEnchantment.scriptStringDuration));
                    if (num == 0)
                    {
                        num = -1;
                    }
                    string parameters = ((float)ScriptLibrary.Call("UTIL_GetRandomFromStringParameter", nodeWorldEnchantment.scriptStringParameter)).ToString();
                    string targetName = nodeWorldEnchantment.targetName;
                    AdvList listByName = ad.GetListByName(targetName, null);
                    if (listByName != null)
                    {
                        foreach (object item6 in listByName.list)
                        {
                            if (item6 is Reference<global::MOM.Unit>)
                            {
                                (item6 as Reference<global::MOM.Unit>).Get().AddEnchantment(e, null, num, parameters);
                            }
                            else if (item6 is Reference<global::MOM.Location>)
                            {
                                (item6 as Reference<global::MOM.Location>).Get().AddEnchantment(e, null, num, parameters);
                            }
                            else if (item6 is IEnchantable)
                            {
                                (item6 as IEnchantable).AddEnchantment(e, null, num, parameters);
                            }
                        }
                    }
                    else if (targetName == NodeWorldEnchantment.WizardCriteria.AllWizzards.ToString())
                    {
                        GameManager.GetWizards();
                        foreach (PlayerWizard wizard in GameManager.GetWizards())
                        {
                            wizard.AddEnchantment(e, null, num, parameters);
                        }
                    }
                    else if (targetName == NodeWorldEnchantment.WizardCriteria.CurrentWizzard.ToString())
                    {
                        GameManager.GetWizard(ad.mainPlayerWizard).AddEnchantment(e, null, num, parameters);
                    }
                    HUD.Get()?.UpdateHUD();
                }
                List<Dictionary<string, AdvList>> ret2 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret2);
            }
            else if (AdventureManager.curentNode is NodeSpawnLocation)
            {
                NodeSpawnLocation nodeSpawnLocation = AdventureManager.curentNode as NodeSpawnLocation;
                if (!nodeSpawnLocation.destroyOwner)
                {
                    ScriptLibrary.Call(nodeSpawnLocation.scriptName, ad, AdventureManager.curentNode);
                }
                else if (ad.advSource is global::MOM.Location)
                {
                    (ad.advSource as global::MOM.Location).Destroy();
                    AchievementManager.Progress(AchievementManager.Achievement.MasterArcheologist);
                }
                List<Dictionary<string, AdvList>> ret3 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret3);
            }
            else if (AdventureManager.curentNode is NodeBattle)
            {
                NodeBattle nodeBattle = AdventureManager.curentNode as NodeBattle;
                global::MOM.Group group = (global::MOM.Group)ScriptLibrary.Call(nodeBattle.scriptName, ad, AdventureManager.curentNode);
                if (group == null || group.GetUnits().Count == 0)
                {
                    Debug.LogError("Missing opponent group in event: " + a.name + " module: " + a.module);
                }
                List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>();
                if (!string.IsNullOrEmpty(nodeBattle.listA))
                {
                    AdvList listByName2 = ad.GetListByName(nodeBattle.listA, null);
                    if (listByName2 != null)
                    {
                        foreach (object item7 in listByName2.list)
                        {
                            if (item7 is global::MOM.Unit)
                            {
                                list.Add(item7 as global::MOM.Unit);
                            }
                            else if (item7 is global::MOM.Group)
                            {
                                list.AddRange((item7 as global::MOM.Group).GetUnits());
                            }
                        }
                    }
                }
                if (list.Count == 0)
                {
                    list = ad.mainPlayerGroup.GetUnits();
                }
                if (list == null || list.Count < 1)
                {
                    AdventureManager.battleResult = NodeBattle.RESULT.Lost;
                }
                else if (group == null || group.GetUnits() == null || group.GetUnits().Count < 1)
                {
                    AdventureManager.battleResult = NodeBattle.RESULT.Win;
                }
                else
                {
                    Battle battle = new Battle(list, group.GetUnits(), ad.mainPlayerWizard, 0);
                    battle.DebugMode(value: true);
                    bool landBattle = group.GetPlane()?.GetHexAt(group.GetPosition())?.IsLand() ?? true;
                    battle.landBattle = landBattle;
                    battle.temperature = 0.5f;
                    battle.humidity = 0.5f;
                    battle.forest = 0.38f;
                    screen.SetVisible(visible: false, animate: true);
                    FSMCoreGame.Get().StartBattle(battle);
                    while (true)
                    {
                        bool num2 = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
                        bool flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
                        if (!num2 && !flag)
                        {
                            break;
                        }
                        yield return null;
                    }
                    AdventureManager.battleResult = AdventureManager.GetBattleResult(ad.mainPlayerWizard);
                }
                HUD.Get()?.Hide();
                screen.SetVisible(visible: true, animate: true);
                List<Dictionary<string, AdvList>> ret4 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret4, -1, AdventureManager.battleResult.ToString());
            }
            else if (AdventureManager.curentNode is NodeLocationAward)
            {
                NodeLocationAward nodeLocationAward = AdventureManager.curentNode as NodeLocationAward;
                if (ad.advSource is global::MOM.Location)
                {
                    global::MOM.Location location = ad.advSource as global::MOM.Location;
                    nodeLocationAward.ClaimAward(GameManager.GetWizard(ad.mainPlayerWizard), location, ad.mainPlayerGroup, ad.heroes);
                }
                List<Dictionary<string, AdvList>> ret5 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret5);
            }
            else if (AdventureManager.curentNode is NodeTrade)
            {
                NodeTrade nodeTrade = AdventureManager.curentNode as NodeTrade;
                Globals.SetDynamicData<string>("TradeResult", null);
                Trade trade = UIManager.Open<Trade>(UIManager.Layer.Popup);
                List<object> playerWares = null;
                List<object> traderWares = null;
                int playerParameter = 0;
                int traderParameter = 0;
                if (!string.IsNullOrEmpty(nodeTrade.scriptStringParameter))
                {
                    playerParameter = Mathf.RoundToInt(((Multitype<float, float, float>)ScriptLibrary.Call("UTIL_StringParameterProcessor", nodeTrade.scriptStringParameter)).t1);
                }
                if (!string.IsNullOrEmpty(nodeTrade.scriptTraderStringParameter))
                {
                    traderParameter = Mathf.RoundToInt(((Multitype<float, float, float>)ScriptLibrary.Call("UTIL_StringParameterProcessor", nodeTrade.scriptTraderStringParameter)).t1);
                }
                switch (nodeTrade.tradeCurrency)
                {
                case NodeTrade.TradeCurrency.Both:
                {
                    Multitype<NodeTrade.TradeCurrency, string, int> item4 = new Multitype<NodeTrade.TradeCurrency, string, int>(NodeTrade.TradeCurrency.Gold, "IconGold", GameManager.GetWizard(ad.mainPlayerWizard).money);
                    Multitype<NodeTrade.TradeCurrency, string, int> item5 = new Multitype<NodeTrade.TradeCurrency, string, int>(NodeTrade.TradeCurrency.Mana, "IconMana", GameManager.GetWizard(ad.mainPlayerWizard).mana);
                    if (playerWares == null)
                    {
                        playerWares = new List<object>();
                    }
                    playerWares.Add(item4);
                    playerWares.Add(item5);
                    break;
                }
                case NodeTrade.TradeCurrency.Gold:
                {
                    Multitype<NodeTrade.TradeCurrency, string, int> item2 = new Multitype<NodeTrade.TradeCurrency, string, int>(NodeTrade.TradeCurrency.Gold, "IconGold", GameManager.GetWizard(ad.mainPlayerWizard).money);
                    if (playerWares == null)
                    {
                        playerWares = new List<object>();
                    }
                    playerWares.Add(item2);
                    break;
                }
                case NodeTrade.TradeCurrency.Mana:
                {
                    Multitype<NodeTrade.TradeCurrency, string, int> item3 = new Multitype<NodeTrade.TradeCurrency, string, int>(NodeTrade.TradeCurrency.Mana, "IconMana", GameManager.GetWizard(ad.mainPlayerWizard).mana);
                    if (playerWares == null)
                    {
                        playerWares = new List<object>();
                    }
                    playerWares.Add(item3);
                    break;
                }
                default:
                {
                    AdvList listByName3;
                    if (!string.IsNullOrEmpty(nodeTrade.playerWares))
                    {
                        listByName3 = ad.GetListByName(nodeTrade.playerWares, null);
                        if (listByName3 != null)
                        {
                            foreach (object item8 in listByName3.list)
                            {
                                if (item8 is global::MOM.Group)
                                {
                                    if (playerWares == null)
                                    {
                                        playerWares = new List<object>();
                                    }
                                    (item8 as global::MOM.Group).GetUnits().ForEach(delegate(Reference<global::MOM.Unit> o)
                                    {
                                        playerWares.Add(o.Get());
                                    });
                                    continue;
                                }
                                if (item8 is Reference<global::MOM.Unit>)
                                {
                                    if (playerWares == null)
                                    {
                                        playerWares = new List<object>();
                                    }
                                    global::MOM.Unit item = (item8 as Reference<global::MOM.Unit>).Get();
                                    playerWares.Add(item);
                                    continue;
                                }
                                if (playerWares == null)
                                {
                                    playerWares = new List<object>();
                                }
                                playerWares = listByName3.list;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(nodeTrade.traderWares))
                    {
                        break;
                    }
                    listByName3 = ad.GetListByName(nodeTrade.traderWares, null);
                    if (listByName3 == null)
                    {
                        break;
                    }
                    foreach (object item9 in listByName3.list)
                    {
                        if (item9 is global::MOM.Group)
                        {
                            if (traderWares == null)
                            {
                                traderWares = new List<object>();
                            }
                            (item9 as global::MOM.Group).GetUnits().ForEach(delegate(Reference<global::MOM.Unit> o)
                            {
                                traderWares.Add(o.Get());
                            });
                            continue;
                        }
                        traderWares = listByName3.list;
                        break;
                    }
                    break;
                }
                }
                trade.SetData(ad.mainPlayerWizard, 0, nodeTrade.tradeCurrency, playerWares, traderWares, playerParameter, traderParameter);
                while (UIManager.GetScreen<Trade>(UIManager.Layer.Popup) != null)
                {
                    yield return null;
                }
                string dynamicData = Globals.GetDynamicData<string>("TradeResult");
                List<Dictionary<string, AdvList>> ret6 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret6, -1, dynamicData);
            }
            else
            {
                List<Dictionary<string, AdvList>> ret7 = AdventureManager.PrepareOutputs(ad, a, AdventureManager.curentNode);
                baseNode = AdventureManager.SelectOutput(ad, a, AdventureManager.curentNode, ret7);
            }
            if (baseNode == null)
            {
                Debug.LogError("Softlock after inability to choose next step in the adventure " + a.name + " node: " + AdventureManager.curentNode.ID);
                while (true)
                {
                    yield return null;
                }
            }
            Debug.Log(AdventureManager.curentNode.ID + "->" + baseNode.ID + " @" + a.ToString());
            AdventureManager.ProgressToNode(ad, a, baseNode);
            AdventureManager.curentNode = baseNode;
            advEnd = false;
            if (baseNode is NodeEnd)
            {
                if ((baseNode as NodeEnd).destroyEvent && ad.advSource is global::MOM.Location location2)
                {
                    location2.RemoveAdventureTrigger();
                }
                List<AdventureOutcomeDelta.Outcome> outcomes2 = delta.GetOutcomes();
                if (screen.UpdateNodeEnd(ad, a, AdventureManager.curentNode, outcomes2))
                {
                    AdventureManager.incomingChoice = -1;
                    while (AdventureManager.incomingChoice == -1)
                    {
                        yield return null;
                    }
                    advEnd = true;
                    screen.CloseMe();
                }
                else
                {
                    advEnd = true;
                    screen.CloseMe();
                }
            }
            if (ad.heroes.Count > 0)
            {
                yield return AdventureManager.TryClaimHero(ad, a);
            }
        }
        while (!advEnd);
        while ((bool)UIManager.GetScreen<AdventureScreen>(UIManager.Layer.Standard))
        {
            yield return null;
        }
        delta?.Destroy();
        GameManager.Get().FreeFocus(ad);
        HUD.Get()?.Show();
    }

    private static List<Dictionary<string, AdvList>> PrepareOutputs(AdventureData ad, Adventure a, BaseNode curentNode)
    {
        List<Dictionary<string, AdvList>> list = AdventureManager.EvaluateOptions(ad, a, curentNode, ad.publicLits);
        ad.avaliableOutputs.Clear();
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    ad.avaliableOutputs.Add(i);
                }
            }
        }
        return list;
    }

    private static BaseNode SelectOutput(AdventureData ad, Adventure a, BaseNode curentNode, List<Dictionary<string, AdvList>> ret, int index = -1, string outputName = null)
    {
        if (index == -1)
        {
            for (int i = 0; i < ret.Count; i++)
            {
                if (ret[i] != null && (string.IsNullOrEmpty(outputName) || !(curentNode.outputs[i].name != outputName)))
                {
                    index = i;
                    break;
                }
            }
        }
        if (index == -1)
        {
            Debug.LogError("Missing choice in adventure! ");
        }
        BaseNode target = curentNode.outputs[index].GetTarget(a);
        ad.temporaryLists = ret[index];
        return target;
    }

    private static IEnumerator TryClaimHero(AdventureData ad, Adventure a)
    {
        PlayerWizard w = GameManager.GetWizard(ad.mainPlayerWizard);
        if (w.IsHuman)
        {
            while (ad.heroes.Count > 0)
            {
                global::MOM.Unit u = ad.heroes[0].Key;
                IGroup g = ad.heroes[0].Value;
                UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Standard);
                unitInfo.SetHeroJoin(u, delegate
                {
                    if (w.heroes.Count < w.GetMaxHeroCount())
                    {
                        w.ModifyUnitSkillsByTraits(u);
                        g.AddUnit(u);
                        u.UpdateMP();
                    }
                    else
                    {
                        PopupGeneral.OpenPopup(null, "UI_INFO", "UI_HERO_LIMIT", "UI_OK");
                    }
                });
                while (UIManager.IsOpen(UIManager.Layer.Standard, unitInfo))
                {
                    yield return null;
                }
                ad.heroes.RemoveAt(0);
            }
            yield break;
        }
        while (ad.heroes.Count > 0)
        {
            global::MOM.Unit key = ad.heroes[0].Key;
            IGroup value = ad.heroes[0].Value;
            if (w.heroes.Count < w.GetMaxHeroCount())
            {
                w.ModifyUnitSkillsByTraits(key);
                value.AddUnit(key);
                key.UpdateMP();
            }
            ad.heroes.RemoveAt(0);
        }
    }

    private static BaseNode PickNextNode(AdventureData advData, Adventure a, BaseNode n, List<Dictionary<string, AdvList>> ret)
    {
        if (ret == null)
        {
            return null;
        }
        for (int i = 0; i < ret.Count; i++)
        {
            if (ret[i] != null)
            {
                advData.temporaryLists = ret[i];
                return a.GetNode(n.outputs[i].targetID);
            }
        }
        return null;
    }

    private static List<Dictionary<string, AdvList>> EvaluateOptions(AdventureData advData, Adventure a, BaseNode n, Dictionary<string, AdvList> lists)
    {
        List<Dictionary<string, AdvList>> list = new List<Dictionary<string, AdvList>>();
        if (n == null)
        {
            Debug.LogError("Base node is null");
        }
        if (n.outputs == null)
        {
            return list;
        }
        List<AdvOutput.GroupHint> list2 = new List<AdvOutput.GroupHint>();
        for (int i = 0; i < n.outputs.Count; i++)
        {
            if (n.outputs[i] == null)
            {
                Debug.LogError("Output " + i + " is null");
            }
            AdvOutput.GroupHint group = n.outputs[i].group;
            if (group != 0 && list2.Contains(group))
            {
                list.Add(null);
                continue;
            }
            BaseNode target = n.outputs[i].GetTarget(a);
            if (target == null)
            {
                Debug.LogError("Output " + i + " from node " + n.ID + " not connected anywhere!");
            }
            if (target.allowOnce && AdventureManager.visitedNodes.ContainsKey(a) && AdventureManager.visitedNodes[a].Contains(target.ID))
            {
                list.Add(null);
                continue;
            }
            Dictionary<string, AdvList> dictionary = new Dictionary<string, AdvList>();
            if (AdventureManager.ProcessEntryLogic(advData, n.outputs[i].GetTarget(a), lists, dictionary, a))
            {
                list.Add(dictionary);
                if (group != 0)
                {
                    list2.Add(group);
                }
            }
            else
            {
                list.Add(null);
            }
        }
        return list;
    }

    public void EnableEventsTestMode(bool isTestMode)
    {
        this.eventsTestMode = isTestMode;
    }

    public static void Clear()
    {
        if (AdventureManager.lastOccurence != null)
        {
            AdventureManager.lastOccurence.Clear();
        }
        if (AdventureManager.visitedNodes != null)
        {
            AdventureManager.visitedNodes.Clear();
        }
        AdventureLibrary.currentLibrary.Clear();
    }
}
