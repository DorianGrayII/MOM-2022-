using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class DiplomacyManager
{
    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;

    [ProtoMember(2)]
    public NetDictionary<int, DiplomaticStatus> statusses;

    public DiplomacyManager()
    {
    }

    public DiplomacyManager(PlayerWizard w)
    {
        this.owner = w;
    }

    public DiplomaticStatus GetStatusToward(PlayerWizard pw)
    {
        return this.GetStatusToward(pw.GetID());
    }

    public DiplomaticStatus GetStatusToward(int pw)
    {
        if (this.statusses == null)
        {
            this.statusses = new NetDictionary<int, DiplomaticStatus>();
        }
        if (this.owner == null)
        {
            return null;
        }
        if (!this.statusses.ContainsKey(pw))
        {
            if (this.owner.Get().GetDiscoveredWizards() == null || this.owner.Get().GetDiscoveredWizards().Find((Reference<PlayerWizard> o) => o.ID == pw) == null)
            {
                return null;
            }
            DiplomaticStatus diplomaticStatus = new DiplomaticStatus();
            diplomaticStatus.target = GameManager.GetWizard(pw);
            diplomaticStatus.owner = this.owner;
            this.statusses[pw] = diplomaticStatus;
            if (diplomaticStatus.owner.Get() is PlayerWizardAI)
            {
                diplomaticStatus.ChangeRelationshipBy(-this.owner.Get().GetPersonality().hostility, affectTreaties: true);
                int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
                diplomaticStatus.willOfWar = Mathf.Clamp((int)((float)this.owner.Get().GetPersonality().hostility * ((float)settingAsInt * 0.2f)) + settingAsInt * 5, -100, 100);
            }
            diplomaticStatus.updateWait = 1;
        }
        return this.statusses[pw];
    }

    public IEnumerator DiplomaticActivity()
    {
        if (this.statusses == null)
        {
            yield break;
        }
        foreach (KeyValuePair<int, DiplomaticStatus> v in this.statusses)
        {
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.None))
            {
                yield return null;
            }
            DiplomaticStatus reverseStatusFromTarget = v.Value.GetReverseStatusFromTarget();
            List<DiplomaticTreaty> treaties = v.Value.GetTreaties();
            List<DiplomaticTreaty> rTreaties = reverseStatusFromTarget.GetTreaties();
            while (true)
            {
                DiplomaticTreaty diplomaticTreaty = treaties.Find((DiplomaticTreaty o) => rTreaties.Find((DiplomaticTreaty t) => t.Equal(o)) == null);
                if (diplomaticTreaty == null)
                {
                    break;
                }
                v.Value.EndTreaty(diplomaticTreaty);
                Debug.Log("Removal of invalid diplomatic status " + diplomaticTreaty.source.dbName + " on " + this.owner.Get().name + " toward " + v.Value.target.Get().name);
            }
            while (true)
            {
                DiplomaticTreaty diplomaticTreaty2 = rTreaties.Find((DiplomaticTreaty o) => treaties.Find((DiplomaticTreaty t) => t.Equal(o)) == null);
                if (diplomaticTreaty2 == null)
                {
                    break;
                }
                reverseStatusFromTarget.EndTreaty(diplomaticTreaty2);
                Debug.Log("Removal of invalid diplomatic status " + diplomaticTreaty2.source.dbName + " on " + v.Value.target.Get().name + " toward " + this.owner.Get().name);
            }
            if (v.Value.updateWait == 0)
            {
                v.Value.Recalculate();
                bool flag = false;
                for (int i = 0; i < treaties.Count; i++)
                {
                    DiplomaticTreaty tr = treaties[i];
                    if (tr.length > 0 && tr.length + tr.turnStarted < TurnManager.GetTurnNumber())
                    {
                        v.Value.EndTreaty(tr);
                        i--;
                        continue;
                    }
                    if (reverseStatusFromTarget.GetTreaties().FindIndex((DiplomaticTreaty o) => o.source == tr.source) < 0)
                    {
                        v.Value.EndTreaty(tr);
                        i--;
                        continue;
                    }
                    string treatyEvaluationScript = tr.source.Get().treatyEvaluationScript;
                    if (string.IsNullOrEmpty(treatyEvaluationScript))
                    {
                        continue;
                    }
                    int num = (int)ScriptLibrary.Call(treatyEvaluationScript, v.Value);
                    if (num < 0 && Random.Range(0, 100) < -num)
                    {
                        if (Random.Range(40, 100) < v.Value.willTreaty)
                        {
                            DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
                            diplomaticMessage.domination = DiplomaticMessage.Domination.ClearBelow;
                            diplomaticMessage.messageType = DiplomaticMessage.MessageType.BreakTreaty;
                            diplomaticMessage.keys = new string[2]
                            {
                                tr.source.dbName.Substring(tr.source.dbName.IndexOf("-") + 1),
                                tr.source.dbName
                            };
                            v.Value.AddMessage(diplomaticMessage);
                            flag = true;
                            v.Value.willTreaty -= 100;
                        }
                        else
                        {
                            v.Value.willTreaty += Random.Range(0, 4);
                        }
                    }
                }
                if (v.Value.CanTrade() && v.Value.willToTrade > 0)
                {
                    int num2 = (v.Value.willToTrade - 40) / 10;
                    if (Random.Range(0, 100) < num2)
                    {
                        v.Value.willToTrade = 0;
                        PlayerWizard wizard = GameManager.GetWizard(v.Key);
                        List<object> possibleTradeWares = wizard.GetPossibleTradeWares();
                        if (possibleTradeWares != null)
                        {
                            List<object> possibleTradeWares2 = this.owner.Get().GetPossibleTradeWares();
                            int num3 = 0;
                            foreach (object item in possibleTradeWares2)
                            {
                                if (wizard.AdvantageIfAcquired(item, increasedWill: false) > 0)
                                {
                                    num3 += Trade.GetValueFor(item, v.Key, wantedItem: false);
                                }
                            }
                            object obj = null;
                            int num4 = -2;
                            foreach (object item2 in possibleTradeWares)
                            {
                                if (Trade.GetValueFor(item2, v.Key, wantedItem: false) > num3)
                                {
                                    continue;
                                }
                                int num5 = this.owner.Get().AdvantageIfAcquired(item2, increasedWill: false);
                                if (num5 > 0)
                                {
                                    if (obj == null)
                                    {
                                        obj = item2;
                                        num4 = num5;
                                    }
                                    else if (num4 < num5 || (num4 == num5 && Random.Range(0f, 1f) > 0.3f))
                                    {
                                        obj = item2;
                                        num4 = num5;
                                    }
                                }
                            }
                            if (obj != null)
                            {
                                DiplomaticMessage diplomaticMessage2 = new DiplomaticMessage();
                                diplomaticMessage2.domination = DiplomaticMessage.Domination.AddToQueue;
                                diplomaticMessage2.messageType = DiplomaticMessage.MessageType.TradeOffer;
                                if (obj is Multitype<NodeTrade.TradeCurrency, string, int>)
                                {
                                    Multitype<NodeTrade.TradeCurrency, string, int> multitype = obj as Multitype<NodeTrade.TradeCurrency, string, int>;
                                    diplomaticMessage2.keys = new string[3]
                                    {
                                        multitype.GetType().ToString(),
                                        multitype.t0.ToString(),
                                        multitype.t2.ToString()
                                    };
                                    if (multitype.t0 == NodeTrade.TradeCurrency.Gold)
                                    {
                                        diplomaticMessage2.extraData = global::DBUtils.Localization.Get("DES_GOLD", true);
                                    }
                                    else if (multitype.t0 == NodeTrade.TradeCurrency.Mana)
                                    {
                                        diplomaticMessage2.extraData = global::DBUtils.Localization.Get("DES_MANA", true);
                                    }
                                }
                                else if (obj is global::MOM.Artefact)
                                {
                                    global::MOM.Artefact artefact = obj as global::MOM.Artefact;
                                    diplomaticMessage2.keys = new string[2]
                                    {
                                        artefact.GetType().ToString(),
                                        artefact.GetHash().ToString()
                                    };
                                    diplomaticMessage2.extraData = global::DBUtils.Localization.Get("DES_ARTEFACT_NAMED", true, artefact.name);
                                }
                                else if (obj is DBReference<Spell>)
                                {
                                    DBReference<Spell> dBReference = obj as DBReference<Spell>;
                                    diplomaticMessage2.keys = new string[2]
                                    {
                                        dBReference.GetType().ToString(),
                                        dBReference.dbName
                                    };
                                    diplomaticMessage2.extraData = global::DBUtils.Localization.Get("DES_SPELL_NAMED", true, dBReference.Get().GetDILocalizedName());
                                }
                                diplomaticMessage2.playerIdea = false;
                                v.Value.AddMessage(diplomaticMessage2);
                                flag = true;
                                v.Value.willTreaty -= 75;
                            }
                        }
                    }
                }
                if (!flag && !v.Value.openWar)
                {
                    foreach (Treaty t2 in DataBase.GetType<Treaty>())
                    {
                        if ((treaties != null && treaties.Find((DiplomaticTreaty o) => o.source.Get() == t2) != null) || string.IsNullOrEmpty(t2.treatyEvaluationScript))
                        {
                            continue;
                        }
                        float num6 = Mathf.Clamp01((float)(Mathf.Clamp((int)ScriptLibrary.Call(t2.treatyEvaluationScript, v.Value), -100, 100) - 50) / 100f);
                        bool flag2 = t2 == (Treaty)TREATY.WAR;
                        if (flag2)
                        {
                            float num7 = (float)this.owner.Get().GetPersonality().hostility * 0.01f;
                            num6 += num6 * num7;
                            num7 = (flag2 ? 100 : Mathf.Clamp(v.Value.willTreaty, 0, 100));
                            num6 = num6 * num7 * 0.01f;
                        }
                        else
                        {
                            num6 = num6 * num6 * num6;
                        }
                        if (Random.Range(0f, 1f) < num6)
                        {
                            DiplomaticMessage diplomaticMessage3 = new DiplomaticMessage();
                            if (flag2)
                            {
                                diplomaticMessage3.domination = DiplomaticMessage.Domination.ClearSameAndBelow;
                                diplomaticMessage3.messageType = DiplomaticMessage.MessageType.WarDeclaration;
                            }
                            else
                            {
                                diplomaticMessage3.domination = DiplomaticMessage.Domination.AddToQueue;
                                diplomaticMessage3.messageType = DiplomaticMessage.MessageType.TreatyOffer;
                            }
                            diplomaticMessage3.keys = new string[2]
                            {
                                t2.dbName.Substring(t2.dbName.IndexOf("-") + 1),
                                t2.dbName
                            };
                            v.Value.AddMessage(diplomaticMessage3);
                            v.Value.willTreaty -= 100;
                            break;
                        }
                    }
                }
            }
            else
            {
                v.Value.updateWait = 0;
            }
            if (v.Key == PlayerWizard.HumanID())
            {
                if (v.Value.openWar)
                {
                    PlayerWizardAI playerWizardAI = this.owner.Get() as PlayerWizardAI;
                    if (playerWizardAI.GetWarefforts().Find((AIWarEffort o) => o.enemy.Get().GetID() == v.Key) == null)
                    {
                        playerWizardAI.warEfforts.Add(new AIWarEffort(this.owner.Get(), v.Value.target.Get()));
                    }
                }
                else
                {
                    PlayerWizardAI playerWizardAI2 = this.owner.Get() as PlayerWizardAI;
                    AIWarEffort aIWarEffort = playerWizardAI2.GetWarefforts().Find((AIWarEffort o) => o.enemy.Get().GetID() == v.Key);
                    if (aIWarEffort != null)
                    {
                        playerWizardAI2.warEfforts.Remove(aIWarEffort);
                    }
                }
                if (v.Value.GetMessageQueue().Count > 0)
                {
                    while (HUD.Get() == null || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure))
                    {
                        yield return null;
                    }
                    Diplomacy screen = UIManager.Open<Diplomacy>(UIManager.Layer.Standard);
                    screen.StartDialog(this.owner.Get());
                    while ((!(screen == null) && screen.stateStatus < State.StateStatus.PostClose) || !(UIManager.GetScreen<Diplomacy>(UIManager.Layer.Standard) == null))
                    {
                        yield return null;
                    }
                }
                continue;
            }
            List<DiplomaticMessage> messageQueue = v.Value.GetMessageQueue();
            int num8 = 0;
            while (messageQueue.Count > 0)
            {
                num8++;
                if (num8 > 20)
                {
                    Debug.LogError(num8 + " messages processed in single stack of diplomacy between two AI. It looks like an error");
                    break;
                }
                v.Value.DequeueMessage().ResolveWithAI(v.Value);
            }
        }
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.None))
        {
            yield return null;
        }
    }

    public bool IsAtWarWith(int wizardID)
    {
        if (this.statusses == null)
        {
            return false;
        }
        return this.GetStatusToward(wizardID)?.openWar ?? false;
    }

    public bool IsAtWar()
    {
        if (this.statusses != null)
        {
            foreach (KeyValuePair<int, DiplomaticStatus> statuss in this.statusses)
            {
                if (statuss.Value.openWar)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsAlliedWith(int wizardID)
    {
        if (this.statusses == null)
        {
            return false;
        }
        DiplomaticStatus statusToward = this.GetStatusToward(wizardID);
        if (statusToward == null)
        {
            return false;
        }
        List<DiplomaticTreaty> treaties = statusToward.GetTreaties();
        if (treaties == null || treaties.Find((DiplomaticTreaty o) => o.source == (Treaty)TREATY.ALLIANCE) == null)
        {
            return false;
        }
        return true;
    }

    public void KilledUnitOf(BaseUnit u, PlayerWizard pw)
    {
        if (pw != null)
        {
            DiplomaticStatus statusToward = this.GetStatusToward(pw);
            if (statusToward != null)
            {
                statusToward.warDelta += BaseUnit.GetUnitStrength(u.dbSource);
            }
        }
    }

    public void LostUnitBy(BaseUnit u, PlayerWizard pw)
    {
        if (pw != null)
        {
            DiplomaticStatus statusToward = this.GetStatusToward(pw);
            if (statusToward != null)
            {
                statusToward.warDelta -= BaseUnit.GetUnitStrength(u.dbSource);
            }
        }
    }

    public void AtackingLocationOf(global::MOM.Location location, PlayerWizard pw)
    {
        if (pw != null)
        {
            this.GetStatusToward(pw);
        }
    }

    public void InitialGreetings(PlayerWizard pw)
    {
        DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
        diplomaticMessage.domination = DiplomaticMessage.Domination.AddToQueue;
        diplomaticMessage.messageType = DiplomaticMessage.MessageType.InitialGreetings;
        diplomaticMessage.keys = new string[1] { pw.GetName() };
        this.GetStatusToward(pw)?.AddMessage(diplomaticMessage);
    }

    public void Attacked(PlayerWizard w, global::MOM.Group g)
    {
        if (w == null)
        {
            return;
        }
        this.owner.Get().EnsureWizardIsKnown(w);
        DiplomaticStatus statusToward = this.GetStatusToward(w);
        if (!statusToward.openWar && w is PlayerWizardAI)
        {
            statusToward.StartTreaty(new DiplomaticTreaty((Treaty)TREATY.WAR));
            DiplomaticStatus reverseStatusFromTarget = statusToward.GetReverseStatusFromTarget();
            DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
            diplomaticMessage.domination = DiplomaticMessage.Domination.AddToQueue;
            if (g.GetLocationHostSmart() != null)
            {
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.AttackedMyTown;
                global::MOM.Location locationHostSmart = g.GetLocationHostSmart();
                diplomaticMessage.keys = new string[1] { locationHostSmart.GetName() };
                reverseStatusFromTarget.AddMessage(diplomaticMessage);
            }
            else
            {
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.AttackedMyArmy;
                reverseStatusFromTarget.AddMessage(diplomaticMessage);
            }
            ScriptLibrary.Call("TreatyBreakOthersReaction", statusToward, 30);
        }
    }
}
