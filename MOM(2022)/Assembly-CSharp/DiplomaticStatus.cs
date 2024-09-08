// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DiplomaticStatus
using System.Collections.Generic;
using System.Text;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class DiplomaticStatus
{
    public const int WAR_WILL_TO_PEACE = 20;

    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;

    [ProtoMember(2)]
    public Reference<PlayerWizard> target;

    [ProtoMember(3)]
    public bool openWar;

    [ProtoMember(4)]
    private int relationship;

    [ProtoMember(5)]
    public int willOfWar;

    [ProtoMember(6)]
    public int willTreaty;

    [ProtoMember(7)]
    public int willToTrade;

    [ProtoMember(8)]
    public int willToTalk;

    [ProtoMember(11)]
    public int love;

    [ProtoMember(12)]
    public int fear;

    [ProtoMember(13)]
    public int hate;

    [ProtoMember(14)]
    public int jelousy;

    [ProtoMember(20)]
    public List<Reference<Entity>> borderPressureReasons;

    [ProtoMember(21)]
    public int borderPressure;

    [ProtoMember(22)]
    public int targetLocationsValue;

    [ProtoMember(23)]
    public int ownerLocationsValue;

    [ProtoMember(30)]
    public List<DiplomaticMessage> messages;

    [ProtoMember(31)]
    public bool messagesDirty;

    [ProtoMember(32)]
    public int warDelta;

    [ProtoMember(33)]
    public List<DiplomaticTreaty> treaties;

    [ProtoMember(34)]
    public int nextWarning;

    [ProtoMember(35)]
    public int lastStatusHash;

    [ProtoMember(36)]
    public int updateWait;

    public Personality GetOwnPersonality()
    {
        return this.owner.Get().GetPersonality();
    }

    public int GetRelationship()
    {
        return this.relationship;
    }

    private void SetRelationship(int value)
    {
        value = Mathf.Clamp(value, -100, 100);
        this.relationship = value;
        this.messagesDirty = true;
    }

    public Relationship GetDBRelationship()
    {
        int i = this.GetRelationship();
        return DataBase.GetType<Relationship>().Find((Relationship o) => o.minValue <= i);
    }

    public List<string> GetTreatiesNames()
    {
        if (this.treaties == null)
        {
            return null;
        }
        List<string> list = new List<string>();
        foreach (DiplomaticTreaty treaty in this.treaties)
        {
            list.Add(treaty.source.Get().GetDILocalizedName());
        }
        return list;
    }

    public string GetTreatiesNamesAsString()
    {
        List<string> treatiesNames = this.GetTreatiesNames();
        if (treatiesNames == null)
        {
            return null;
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string item in treatiesNames)
        {
            stringBuilder.AppendLine(item);
        }
        return stringBuilder.ToString();
    }

    public bool CanStartTalk()
    {
        return this.willToTalk > -80;
    }

    public void Recalculate()
    {
        this.ProcessLocations();
        this.love = this.CalculateLove();
        this.fear = this.CalculateFear();
        this.jelousy = this.CalculateJelousy();
        this.UpdateWillOfWar();
        this.UpdateWillOfTreaty();
        this.UpdateWillOfTrade();
        this.UpdateWillToTalk();
    }

    private void UpdateWillOfWar()
    {
        ScriptLibrary.Call("UpdateWillOfWar", this);
    }

    private void UpdateWillOfTreaty()
    {
        ScriptLibrary.Call("UpdateWillOfTreaty", this);
    }

    private void UpdateWillOfTrade()
    {
        ScriptLibrary.Call("UpdateWillOfTrade", this);
    }

    private void UpdateWillToTalk()
    {
        this.willToTalk = this.TalkInterest();
    }

    public void ChangeRelationshipBy(int value, bool affectTreaties)
    {
        this.SetRelationship(this.relationship + value);
        DiplomaticStatus statusToward = this.target.Get().GetDiplomacy().GetStatusToward(this.owner);
        statusToward.relationship = this.GetRelationship();
        if (affectTreaties)
        {
            this.willTreaty += value;
            statusToward.willTreaty += value;
        }
    }

    private int CalculateLove()
    {
        int num = 0;
        List<Reference<PlayerWizard>> discoveredWizards = this.owner.Get().GetDiscoveredWizards();
        if (this.owner.Get().GetDiplomacy().statusses != null)
        {
            NetDictionary<int, DiplomaticStatus> statusses = this.owner.Get().GetDiplomacy().statusses;
            if (discoveredWizards != null && statusses != null)
            {
                foreach (Reference<PlayerWizard> item in discoveredWizards)
                {
                    if (item.Get().GetDiplomacy().statusses == null)
                    {
                        continue;
                    }
                    NetDictionary<int, DiplomaticStatus> statusses2 = item.Get().GetDiplomacy().statusses;
                    foreach (KeyValuePair<int, DiplomaticStatus> item2 in statusses)
                    {
                        if (item2.Value.openWar && statusses2.ContainsKey(item2.Key) && statusses2[item2.Key].openWar)
                        {
                            num += 1000;
                        }
                    }
                }
            }
        }
        return num;
    }

    private int CalculateFear()
    {
        if (this.target.Get().statHistory == null || this.owner.Get().statHistory == null || this.target.Get().statHistory.stats[StatHistory.Stats.Army].Count < 1)
        {
            return 0;
        }
        List<int> list = this.target.Get().statHistory.stats[StatHistory.Stats.Army];
        int num = list[list.Count - 1];
        List<int> list2 = this.owner.Get().statHistory.stats[StatHistory.Stats.Army];
        int num2 = list2[list2.Count - 1];
        int num3 = num - num2;
        int num4 = this.targetLocationsValue - this.ownerLocationsValue;
        return Mathf.Max(0, num3 * 2 + num4);
    }

    private int CalculateJelousy()
    {
        return this.targetLocationsValue + this.target.Get().money * 3;
    }

    private void ProcessLocations()
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            Entity value = entity.Value;
            if (!(value is TownLocation))
            {
                continue;
            }
            TownLocation townLocation = value as TownLocation;
            if (townLocation.owner == 0)
            {
                continue;
            }
            if (townLocation.owner == this.target.ID)
            {
                num += townLocation.GetStrategicValue();
                foreach (KeyValuePair<int, Entity> entity2 in EntityManager.Get().entities)
                {
                    if (!(entity2.Value is IPlanePosition planePosition) || ((!(planePosition is global::MOM.Group) || !(planePosition as global::MOM.Group).alive) && !(planePosition is global::MOM.Location)) || planePosition.GetPlane() != townLocation.GetPlane())
                    {
                        continue;
                    }
                    int num4 = planePosition.GetPlane().area.HexDistance(planePosition.GetPosition(), townLocation.GetPosition());
                    if (num4 < 10 && planePosition is TownLocation)
                    {
                        if (this.borderPressureReasons == null)
                        {
                            this.borderPressureReasons = new List<Reference<Entity>>();
                        }
                        this.borderPressureReasons.Add(planePosition as TownLocation);
                        num3 += 10 - num4;
                    }
                    else if (num4 < 6 && planePosition is global::MOM.Group && !((planePosition as global::MOM.Group).GetLocationHostSmart() is TownLocation))
                    {
                        if (this.borderPressureReasons == null)
                        {
                            this.borderPressureReasons = new List<Reference<Entity>>();
                        }
                        this.borderPressureReasons.Add(planePosition as global::MOM.Group);
                        num3 += 5;
                    }
                }
            }
            else if (townLocation.owner == this.owner.ID)
            {
                num2 += townLocation.GetStrategicValue();
            }
        }
    }

    private int TalkInterest()
    {
        Personality personality = this.owner.Get().GetPersonality();
        if (personality == null)
        {
            return 0;
        }
        int a = -100;
        if (this.IsAllied())
        {
            a = 0;
        }
        int num = this.willOfWar / 4;
        int num2 = this.relationship;
        this.LowestInterst();
        int b = -personality.hostility + this.LowestInterst() + num2 + num;
        return Mathf.Max(a, b);
    }

    private int PeaceInterest()
    {
        if (!this.openWar)
        {
            return 0;
        }
        return Mathf.Max(0, (20 - this.willOfWar) * 10);
    }

    private int TreatyInterest()
    {
        return this.willTreaty;
    }

    private int TradeInterest()
    {
        return this.willToTrade;
    }

    private int LowestInterst()
    {
        return Mathf.Min(this.PeaceInterest(), this.TreatyInterest(), this.TradeInterest());
    }

    public DiplomaticMessage DequeueMessage()
    {
        List<DiplomaticMessage> messageQueue = this.GetMessageQueue();
        if (messageQueue.Count > 0)
        {
            DiplomaticMessage diplomaticMessage = messageQueue[0];
            messageQueue.Remove(diplomaticMessage);
            return diplomaticMessage;
        }
        return null;
    }

    public List<DiplomaticMessage> GetMessageQueue()
    {
        if (this.messages == null)
        {
            this.messages = new List<DiplomaticMessage>();
        }
        int num = this.relationship ^ this.willOfWar ^ this.willTreaty ^ this.willToTrade ^ this.warDelta;
        if (num != this.lastStatusHash)
        {
            this.messagesDirty = true;
            this.lastStatusHash = num;
        }
        if (this.messagesDirty)
        {
            this.SanitizeMessageQueue();
        }
        return this.messages;
    }

    public void AddMessage(DiplomaticMessage dm, bool addOnlyIfHighest = false)
    {
        if (addOnlyIfHighest && this.GetMessageQueue().Find((DiplomaticMessage o) => o.domination >= dm.domination) != null)
        {
            return;
        }
        if (dm.domination == DiplomaticMessage.Domination.ClearWholeQueue)
        {
            this.GetMessageQueue().Clear();
        }
        else if (dm.domination == DiplomaticMessage.Domination.ClearSameAndBelow)
        {
            this.messages = this.GetMessageQueue().FindAll((DiplomaticMessage o) => o.domination > dm.domination);
        }
        else if (dm.domination == DiplomaticMessage.Domination.ClearBelow)
        {
            this.messages = this.GetMessageQueue().FindAll((DiplomaticMessage o) => o.domination >= dm.domination);
        }
        this.GetMessageQueue().Add(dm);
    }

    public DiplomaticMessage GetNextMessage()
    {
        List<DiplomaticMessage> messageQueue = this.GetMessageQueue();
        if (messageQueue.Count > 0)
        {
            DiplomaticMessage diplomaticMessage = messageQueue[0];
            {
                foreach (DiplomaticMessage item in messageQueue)
                {
                    if (item.messageType > diplomaticMessage.messageType)
                    {
                        diplomaticMessage = item;
                    }
                }
                return diplomaticMessage;
            }
        }
        return null;
    }

    public bool CanTrade()
    {
        if (this.willToTrade > -50)
        {
            return this.GetRelationship() > -50;
        }
        return false;
    }

    public bool CanTalkAboutTreaties()
    {
        return true;
    }

    public bool CanStartTreaty()
    {
        foreach (Treaty item in DataBase.GetType<Treaty>())
        {
            if (this.WillAccept(item))
            {
                return true;
            }
        }
        return false;
    }

    public void SanitizeMessageQueue()
    {
        if (this.messages == null)
        {
            return;
        }
        for (int i = 0; i < this.messages.Count; i++)
        {
            if (!this.messages[i].MessageStillValid(this))
            {
                this.messages.RemoveAt(i);
                i--;
            }
        }
    }

    public DiplomaticStatus GetReverseStatusFromTarget()
    {
        return this.target.Get().GetDiplomacy().GetStatusToward(this.owner.ID);
    }

    public DiplomacyManager GetDiplomacyManager()
    {
        return this.owner.Get().GetDiplomacy();
    }

    public DiplomacyManager GetTargetDiplomacyManager()
    {
        return this.target.Get().GetDiplomacy();
    }

    public float AcceptChance(Treaty treaty)
    {
        if (!treaty.agreementRequired)
        {
            return 1f;
        }
        if (!string.IsNullOrEmpty(treaty.treatyEvaluationScript))
        {
            return (float)(int)ScriptLibrary.Call(treaty.treatyEvaluationScript, this) / 100f;
        }
        return 1f;
    }

    public bool WillAccept(Treaty treaty)
    {
        if (new MHRandom(TurnManager.GetTurnNumber()).GetDouble01() < (double)this.AcceptChance(treaty))
        {
            return true;
        }
        return false;
    }

    public bool WillAcceptBreak(Treaty treaty)
    {
        if (treaty.agreementRequired)
        {
            return true;
        }
        if (!string.IsNullOrEmpty(treaty.treatyEvaluationScript) && (int)ScriptLibrary.Call(treaty.treatyEvaluationScript, this) < 0)
        {
            return true;
        }
        return false;
    }

    public List<DiplomaticTreaty> GetTreaties()
    {
        if (this.treaties == null)
        {
            this.treaties = new List<DiplomaticTreaty>();
        }
        return this.treaties;
    }

    public bool IsAllied()
    {
        return this.GetTreaties().Find((DiplomaticTreaty o) => o.source.Get() == (Treaty)TREATY.ALLIANCE) != null;
    }

    public void StartTreaty(DiplomaticTreaty t)
    {
        if (this.GetTreaties().Find((DiplomaticTreaty o) => o.source.Get() == t.source.Get()) == null)
        {
            DiplomaticStatus reverseStatusFromTarget = this.GetReverseStatusFromTarget();
            this.GetTreaties().Add(t);
            string treatyStartScript = t.source.Get().treatyStartScript;
            if (!string.IsNullOrEmpty(treatyStartScript))
            {
                ScriptLibrary.Call(treatyStartScript, this);
            }
            reverseStatusFromTarget.StartTreaty(t);
        }
    }

    public void EndTreaty(DiplomaticTreaty t)
    {
        DiplomaticTreaty diplomaticTreaty = this.GetTreaties().Find((DiplomaticTreaty t) => t.Equal(t));
        if (diplomaticTreaty != null)
        {
            DiplomaticStatus reverseStatusFromTarget = this.GetReverseStatusFromTarget();
            this.GetTreaties().Remove(diplomaticTreaty);
            string treatyEndScript = diplomaticTreaty.source.Get().treatyEndScript;
            if (!string.IsNullOrEmpty(treatyEndScript))
            {
                ScriptLibrary.Call(treatyEndScript, this);
            }
            reverseStatusFromTarget.EndTreaty(diplomaticTreaty);
        }
    }

    public void BreakTreaty(DiplomaticTreaty t)
    {
        DiplomaticTreaty diplomaticTreaty = this.GetTreaties().Find((DiplomaticTreaty t) => t.Equal(t));
        if (diplomaticTreaty != null)
        {
            string treatyBreakScript = diplomaticTreaty.source.Get().treatyBreakScript;
            if (!string.IsNullOrEmpty(treatyBreakScript))
            {
                ScriptLibrary.Call(treatyBreakScript, this);
            }
            this.EndTreaty(diplomaticTreaty);
        }
    }

    public void ConsiderWarningForWalking()
    {
        if (!this.openWar && Mathf.Min(30, TurnManager.GetTurnNumber() - this.nextWarning) >= Random.Range(0, 100))
        {
            DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
            diplomaticMessage.domination = DiplomaticMessage.Domination.AddToQueue;
            diplomaticMessage.messageType = DiplomaticMessage.MessageType.WalkingOverMyTerritory;
            this.AddMessage(diplomaticMessage);
            this.nextWarning = TurnManager.GetTurnNumber() + 8;
        }
    }
}
