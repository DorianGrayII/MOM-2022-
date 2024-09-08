using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using MOM.Adventures;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class DiplomaticMessage
{
    public enum MessageType
    {
        None = 0,
        GoodBye = 1,
        TalkAboutTreaties = 2,
        ThankYouForHelp = 3,
        TreatyOffer = 4,
        TradeOffer = 5,
        ExpansionWarning = 6,
        WalkingOverMyTerritory = 7,
        AttackedMyArmy = 8,
        AttackedMyTown = 9,
        WontTalk = 10,
        WarDeclaration = 11,
        BreakTreaty = 12,
        Greetings = 13,
        InitialGreetings = 14,
        AcceptWarEnd = 15,
        AcceptTreatyBreak = 16,
        AcceptWar = 17,
        AcceptTreaty = 18,
        SuccessfulTrade = 19,
        ExcelentTrade = 20,
        PoorTrade = 21,
        Rejected = 22,
        FeedbackMessage = 23
    }

    public enum Domination
    {
        AddToQueue = 0,
        ClearBelow = 1,
        ClearSameAndBelow = 2,
        ClearWholeQueue = 3
    }

    [ProtoMember(1)]
    public MessageType messageType;

    [ProtoMember(2)]
    public Domination domination;

    [ProtoMember(3)]
    public string[] keys;

    [ProtoMember(4)]
    public string extraData;

    [ProtoMember(5)]
    public DBReference<Relationship> minRelationship;

    [ProtoMember(6)]
    public bool playerIdea;

    public string GetString(DiplomaticStatus ds)
    {
        string key = this.GetKey(ds);
        object[] parameters = this.keys;
        return global::DBUtils.Localization.Get(key, symbolParse: true, parameters);
    }

    public bool CanReject()
    {
        Treaty treatyFromMessage = this.GetTreatyFromMessage();
        switch (this.messageType)
        {
        case MessageType.TreatyOffer:
            if (treatyFromMessage != null && treatyFromMessage.agreementRequired)
            {
                return true;
            }
            return false;
        case MessageType.BreakTreaty:
            if (treatyFromMessage != null && !treatyFromMessage.agreementRequired)
            {
                return true;
            }
            return false;
        default:
            return false;
        }
    }

    public bool CanAccept()
    {
        MessageType messageType = this.messageType;
        if (messageType == MessageType.TreatyOffer || (uint)(messageType - 11) <= 1u)
        {
            return true;
        }
        return false;
    }

    public bool CanSayBye()
    {
        MessageType messageType = this.messageType;
        if ((uint)(messageType - 1) <= 1u || messageType == MessageType.TreatyOffer || (uint)(messageType - 11) <= 1u)
        {
            return false;
        }
        return true;
    }

    public bool CanContinue()
    {
        switch (this.messageType)
        {
        case MessageType.GoodBye:
        case MessageType.TalkAboutTreaties:
        case MessageType.TreatyOffer:
        case MessageType.AttackedMyArmy:
        case MessageType.AttackedMyTown:
        case MessageType.WontTalk:
        case MessageType.WarDeclaration:
        case MessageType.BreakTreaty:
            return false;
        default:
            return true;
        }
    }

    public bool CanTalkTreaties()
    {
        MessageType messageType = this.messageType;
        if (messageType == MessageType.GoodBye || messageType == MessageType.TradeOffer || (uint)(messageType - 10) <= 1u)
        {
            return false;
        }
        return true;
    }

    public bool CanClose()
    {
        if (this.messageType == MessageType.GoodBye)
        {
            return true;
        }
        return false;
    }

    private string GetKey(DiplomaticStatus ds)
    {
        string dbName = ds.owner.Get().GetPersonality().dbName;
        string text = ds.owner.Get().GetBaseWizard().dbName.Substring("WIZARD-".Length);
        string text2 = dbName.Substring("PERSONALITY-".Length);
        string name = GameManager.GetHumanWizard().GetName();
        switch (this.messageType)
        {
        case MessageType.GoodBye:
            return "DES_GOOD_BYE";
        case MessageType.ThankYouForHelp:
            return "DES_THANK_YOU_FOR_YOUR_HELP";
        case MessageType.TreatyOffer:
            if (this.keys != null && this.keys.Length > 1)
            {
                Treaty treaty = DataBase.Get<Treaty>(this.keys[1], reportMissing: false);
                if (treaty != null)
                {
                    return global::DBUtils.Localization.Get("DES_I_WANT_TO_START_TREATY", true, global::DBUtils.Localization.Get(treaty.GetDescriptionInfo().GetName() + "_MID_SENTENCE", true), treaty.length, treaty.GetDILocalizedDescription());
                }
            }
            return "DES_I_WANT_TO_START_TREATY";
        case MessageType.TradeOffer:
            return global::DBUtils.Localization.Get("DES_TRADE_OFFER", true, this.extraData);
        case MessageType.ExpansionWarning:
            return "DES_EXPANSION_WARNING";
        case MessageType.WalkingOverMyTerritory:
        {
            Relationship relationship4 = DataBase.Get<Relationship>(RELATIONSHIP.FRIENDLY);
            Relationship relationship5 = DataBase.Get<Relationship>(RELATIONSHIP.RESTLESS);
            int relationship6 = ds.GetRelationship();
            if (relationship4 != null && relationship6 >= relationship4.minValue)
            {
                return "DES_TERRITORY_BREAK_WARNING_FRIENDLY";
            }
            if (relationship5 != null && relationship6 < relationship5.minValue)
            {
                return "DES_TERRITORY_BREAK_WARNING_HOSTILE";
            }
            return "DES_TERRITORY_BREAK_WARNING_NEUTRAL";
        }
        case MessageType.AttackedMyArmy:
            return "DES_WAR_FOR_ATTACKED_MY_ARMY";
        case MessageType.AttackedMyTown:
            return "DES_WAR_FOR_ATTACKED_MY_TOWN";
        case MessageType.WontTalk:
            return "DES_NOT_INTERESTED_IN_TALKING";
        case MessageType.WarDeclaration:
            return "DES_WAR_DECLARATION";
        case MessageType.BreakTreaty:
            return "DES_I_WANT_TO_END_TREATY";
        case MessageType.Greetings:
        case MessageType.InitialGreetings:
        {
            Wizard baseWizard = ds.owner.Get().GetBaseWizard();
            if (this.messageType == MessageType.InitialGreetings)
            {
                if (baseWizard.associateInitialGreeting != null && !ds.target.Get().isCustom)
                {
                    Associate_Greeting[] associateInitialGreeting = baseWizard.associateInitialGreeting;
                    foreach (Associate_Greeting associate_Greeting in associateInitialGreeting)
                    {
                        if (ds.target != null && ds.target.Get().GetBaseWizard() == associate_Greeting.wizard)
                        {
                            return global::DBUtils.Localization.Get(associate_Greeting.greeting, true);
                        }
                    }
                }
                return global::DBUtils.Localization.Get("DES_" + text + "_GREETINGS", true) + "\n" + global::DBUtils.Localization.Get("DES_INITIAL_GREETINGS_" + text2, true);
            }
            if (baseWizard.associateGreeting != null)
            {
                Associate_Greeting[] associateInitialGreeting = baseWizard.associateGreeting;
                foreach (Associate_Greeting associate_Greeting2 in associateInitialGreeting)
                {
                    if (ds.target != null && ds.target.Get().GetBaseWizard() == associate_Greeting2.wizard)
                    {
                        return global::DBUtils.Localization.Get(associate_Greeting2.greeting, true);
                    }
                }
            }
            Relationship relationship = DataBase.Get<Relationship>(RELATIONSHIP.FRIENDLY);
            Relationship relationship2 = DataBase.Get<Relationship>(RELATIONSHIP.RESTLESS);
            int relationship3 = ds.GetRelationship();
            if (relationship != null && relationship3 >= relationship.minValue)
            {
                return global::DBUtils.Localization.Get("DES_GREETINGS_FRIENDLY_" + text2, true, name);
            }
            if (relationship2 != null && relationship3 < relationship2.minValue)
            {
                return global::DBUtils.Localization.Get("DES_GREETINGS_HOSTILE_" + text2, true, name);
            }
            return global::DBUtils.Localization.Get("DES_GREETINGS_NEUTRAL_" + text2, true, name);
        }
        case MessageType.Rejected:
            return "DES_NO_NOT_INTERESTED";
        case MessageType.TalkAboutTreaties:
            if (this.playerIdea)
            {
                return global::DBUtils.Localization.Get("DES_WHAT_TREATY_YOU_WANT_" + text2, true, name);
            }
            Debug.LogError("It is not expected that AI will open general treaty offer!");
            return "NOT_IMPLEMENTED " + this.messageType;
        case MessageType.AcceptTreaty:
            return global::DBUtils.Localization.Get("DES_ACCEPT_TREATY_" + this.extraData, true) + global::DBUtils.Localization.Get("DES_ACCEPT_TREATY_" + text2, true, name);
        case MessageType.AcceptTreatyBreak:
            return global::DBUtils.Localization.Get("DES_ACCEPT_TREATY_BREAK_" + text2, true, name);
        case MessageType.AcceptWar:
            return global::DBUtils.Localization.Get("DES_ACCEPT_WAR_" + text2, true, name);
        case MessageType.AcceptWarEnd:
            return global::DBUtils.Localization.Get("DES_ACCEPT_WAR_END_" + text2, true, name);
        case MessageType.FeedbackMessage:
            if (this.keys != null && this.keys.Length != 0)
            {
                return global::DBUtils.Localization.Get(this.keys[0], true);
            }
            return "NOT_IMPLEMENTED " + this.messageType.ToString() + " keys " + this.keys?.Length;
        case MessageType.SuccessfulTrade:
            return global::DBUtils.Localization.Get("DES_SUCCESSFUL_TRADE", true);
        case MessageType.ExcelentTrade:
            return global::DBUtils.Localization.Get("DES_EXCELENT_TRADE", true);
        case MessageType.PoorTrade:
            return global::DBUtils.Localization.Get("DES_POOR_TRADE", true);
        default:
            return "NOT_IMPLEMENTED " + this.messageType;
        }
    }

    public void ResolveWithAI(DiplomaticStatus status)
    {
        status.owner.Get();
        status.target.Get();
        switch (this.messageType)
        {
        case MessageType.GoodBye:
        case MessageType.ExpansionWarning:
        case MessageType.WalkingOverMyTerritory:
        case MessageType.AttackedMyArmy:
        case MessageType.AttackedMyTown:
        case MessageType.WontTalk:
        case MessageType.Greetings:
        case MessageType.InitialGreetings:
            break;
        case MessageType.TradeOffer:
            break;
        case MessageType.ThankYouForHelp:
            break;
        case MessageType.WarDeclaration:
            status.StartTreaty(new DiplomaticTreaty((Treaty)TREATY.WAR));
            break;
        case MessageType.TalkAboutTreaties:
        case MessageType.TreatyOffer:
        case MessageType.BreakTreaty:
            break;
        }
    }

    public object GetWareFromMessage(PlayerWizard messageTarget)
    {
        if (messageTarget == null)
        {
            Debug.LogError("no message target. It is required to recreate an item");
            return null;
        }
        if (this.messageType != MessageType.TradeOffer)
        {
            Debug.LogError(this.messageType.ToString() + " message not designed to contain items");
            return null;
        }
        if (this.keys == null || this.keys.Length < 2)
        {
            return null;
        }
        List<object> possibleTradeWares = messageTarget.GetPossibleTradeWares();
        if (possibleTradeWares == null || possibleTradeWares.Count < 1)
        {
            return null;
        }
        object result = null;
        if (this.keys[0] == typeof(global::MOM.Artefact).ToString())
        {
            result = possibleTradeWares.Find((object o) => o is global::MOM.Artefact && (o as global::MOM.Artefact).GetHash().ToString() == this.keys[1]);
        }
        else if (this.keys[0] == typeof(DBReference<Spell>).ToString())
        {
            result = possibleTradeWares.Find((object o) => o is DBReference<Spell> && (o as DBReference<Spell>).dbName == this.keys[1]);
        }
        else if (this.keys.Length > 2 && this.keys[0] == typeof(Multitype<NodeTrade.TradeCurrency, string, int>).ToString())
        {
            result = possibleTradeWares.Find((object o) => o is Multitype<NodeTrade.TradeCurrency, string, int> && (o as Multitype<NodeTrade.TradeCurrency, string, int>).t0.ToString() == this.keys[1]);
        }
        return result;
    }

    public Treaty GetTreatyFromMessage()
    {
        if (this.keys != null && this.keys.Length == 2)
        {
            return DataBase.Get<Treaty>(this.keys[1], reportMissing: false);
        }
        return null;
    }

    public bool MessageStillValid(DiplomaticStatus status)
    {
        if (this.messageType == MessageType.TradeOffer)
        {
            if (status.willToTrade < -100 || status.openWar)
            {
                return false;
            }
            if (this.GetWareFromMessage(status.target.Get()) == null)
            {
                return false;
            }
            return true;
        }
        return true;
    }

    public bool Accept(DiplomaticStatus recipient)
    {
        switch (this.messageType)
        {
        case MessageType.TreatyOffer:
        {
            DiplomaticTreaty t3 = new DiplomaticTreaty(this.GetTreatyFromMessage());
            recipient.GetReverseStatusFromTarget().StartTreaty(t3);
            DiplomaticMessage diplomaticMessage3 = new DiplomaticMessage();
            diplomaticMessage3.domination = Domination.AddToQueue;
            diplomaticMessage3.messageType = MessageType.FeedbackMessage;
            diplomaticMessage3.keys = new string[1] { "DES_LET_IT_BENEFIT_US_BOTH" };
            recipient.AddMessage(diplomaticMessage3);
            return true;
        }
        case MessageType.WarDeclaration:
        {
            DiplomaticTreaty t2 = new DiplomaticTreaty(this.GetTreatyFromMessage());
            recipient.GetReverseStatusFromTarget().StartTreaty(t2);
            DiplomaticMessage diplomaticMessage2 = new DiplomaticMessage();
            diplomaticMessage2.domination = Domination.AddToQueue;
            diplomaticMessage2.messageType = MessageType.FeedbackMessage;
            if (this.playerIdea)
            {
                diplomaticMessage2.keys = new string[1] { "DES_YOU_WILL_REGRET_GOING_AGAINST_ME" };
            }
            else
            {
                diplomaticMessage2.keys = new string[1] { this.GetKey(recipient) };
            }
            recipient.AddMessage(diplomaticMessage2);
            return true;
        }
        case MessageType.BreakTreaty:
        {
            Treaty treatyFromMessage = this.GetTreatyFromMessage();
            DiplomaticTreaty t = new DiplomaticTreaty(treatyFromMessage);
            recipient.GetReverseStatusFromTarget().BreakTreaty(t);
            DiplomaticMessage diplomaticMessage = new DiplomaticMessage
            {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            if (!treatyFromMessage.agreementRequired)
            {
                diplomaticMessage.keys = new string[1] { "DES_LET_IT_BENEFIT_US_BOTH" };
            }
            else
            {
                diplomaticMessage.keys = new string[1] { "DES_YOU_WILL_REGRET_IT" };
            }
            recipient.AddMessage(diplomaticMessage);
            return true;
        }
        default:
            return false;
        }
    }

    public bool Reject(DiplomaticStatus recipient)
    {
        switch (this.messageType)
        {
        case MessageType.TreatyOffer:
        case MessageType.WarDeclaration:
        {
            Treaty treatyFromMessage2 = this.GetTreatyFromMessage();
            DiplomaticMessage diplomaticMessage2 = new DiplomaticMessage
            {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            if (treatyFromMessage2.agreementRequired)
            {
                diplomaticMessage2.keys = new string[1] { "DES_NO_NOT_INTERESTED" };
            }
            else
            {
                Debug.LogError("not valid rejection path!");
            }
            recipient.AddMessage(diplomaticMessage2);
            return true;
        }
        case MessageType.BreakTreaty:
        {
            Treaty treatyFromMessage = this.GetTreatyFromMessage();
            DiplomaticMessage diplomaticMessage = new DiplomaticMessage
            {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            if (!treatyFromMessage.agreementRequired)
            {
                diplomaticMessage.keys = new string[1] { "DES_NO_NOT_INTERESTED" };
            }
            else
            {
                Debug.LogError("not valid rejection path!");
            }
            recipient.AddMessage(diplomaticMessage);
            return true;
        }
        default:
            return false;
        }
    }

    public bool AIMessageActivation(DiplomaticStatus recipient)
    {
        switch (this.messageType)
        {
        case MessageType.TreatyOffer:
        case MessageType.WarDeclaration:
        {
            Treaty treatyFromMessage2 = this.GetTreatyFromMessage();
            if (this.CanReject() && !recipient.WillAccept(treatyFromMessage2))
            {
                return this.Reject(recipient);
            }
            return this.Accept(recipient);
        }
        case MessageType.BreakTreaty:
        {
            Treaty treatyFromMessage = this.GetTreatyFromMessage();
            if (this.CanReject() && !recipient.WillAcceptBreak(treatyFromMessage))
            {
                return this.Reject(recipient);
            }
            return this.Accept(recipient);
        }
        default:
            return false;
        }
    }
}
