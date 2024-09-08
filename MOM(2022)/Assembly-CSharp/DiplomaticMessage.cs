using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
public class DiplomaticMessage
{
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

    public bool Accept(DiplomaticStatus recipient)
    {
        MessageType messageType = this.messageType;
        if (messageType == MessageType.TreatyOffer)
        {
            DiplomaticTreaty treaty = new DiplomaticTreaty(this.GetTreatyFromMessage());
            recipient.GetReverseStatusFromTarget().StartTreaty(treaty);
            DiplomaticMessage message = new DiplomaticMessage {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            message.keys = new string[] { "DES_LET_IT_BENEFIT_US_BOTH" };
            recipient.AddMessage(message, false);
            return true;
        }
        if (messageType == MessageType.WarDeclaration)
        {
            DiplomaticTreaty treaty2 = new DiplomaticTreaty(this.GetTreatyFromMessage());
            recipient.GetReverseStatusFromTarget().StartTreaty(treaty2);
            DiplomaticMessage message2 = new DiplomaticMessage {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            if (this.playerIdea)
            {
                message2.keys = new string[] { "DES_YOU_WILL_REGRET_GOING_AGAINST_ME" };
            }
            else
            {
                message2.keys = new string[] { this.GetKey(recipient) };
            }
            recipient.AddMessage(message2, false);
            return true;
        }
        if (messageType != MessageType.BreakTreaty)
        {
            return false;
        }
        DiplomaticTreaty t = new DiplomaticTreaty(this.GetTreatyFromMessage());
        recipient.GetReverseStatusFromTarget().BreakTreaty(t);
        DiplomaticMessage dm = new DiplomaticMessage {
            domination = Domination.AddToQueue,
            messageType = MessageType.FeedbackMessage
        };
        if (!this.GetTreatyFromMessage().agreementRequired)
        {
            dm.keys = new string[] { "DES_LET_IT_BENEFIT_US_BOTH" };
        }
        else
        {
            dm.keys = new string[] { "DES_YOU_WILL_REGRET_IT" };
        }
        recipient.AddMessage(dm, false);
        return true;
    }

    public bool AIMessageActivation(DiplomaticStatus recipient)
    {
        MessageType messageType = this.messageType;
        if ((messageType == MessageType.TreatyOffer) || (messageType == MessageType.WarDeclaration))
        {
            Treaty treaty = this.GetTreatyFromMessage();
            return ((!this.CanReject() || recipient.WillAccept(treaty)) ? this.Accept(recipient) : this.Reject(recipient));
        }
        if (messageType != MessageType.BreakTreaty)
        {
            return false;
        }
        Treaty treatyFromMessage = this.GetTreatyFromMessage();
        return ((!this.CanReject() || recipient.WillAcceptBreak(treatyFromMessage)) ? this.Accept(recipient) : this.Reject(recipient));
    }

    public bool CanAccept()
    {
        MessageType messageType = this.messageType;
        return ((messageType == MessageType.TreatyOffer) || ((messageType - MessageType.WarDeclaration) <= MessageType.GoodBye));
    }

    public bool CanClose()
    {
        return (this.messageType == MessageType.GoodBye);
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
        }
        return true;
    }

    public bool CanReject()
    {
        Treaty treatyFromMessage = this.GetTreatyFromMessage();
        MessageType messageType = this.messageType;
        return ((messageType == MessageType.TreatyOffer) ? ((treatyFromMessage != null) && treatyFromMessage.agreementRequired) : ((messageType == MessageType.BreakTreaty) ? ((treatyFromMessage != null) && !treatyFromMessage.agreementRequired) : false));
    }

    public bool CanSayBye()
    {
        MessageType messageType = this.messageType;
        return (((messageType - 1) > MessageType.GoodBye) && ((messageType != MessageType.TreatyOffer) && ((messageType - MessageType.WarDeclaration) > MessageType.GoodBye)));
    }

    public bool CanTalkTreaties()
    {
        MessageType messageType = this.messageType;
        return ((messageType != MessageType.GoodBye) && ((messageType != MessageType.TradeOffer) && ((messageType - MessageType.WontTalk) > MessageType.GoodBye)));
    }

    private string GetKey(DiplomaticStatus ds)
    {
        string str = ds.owner.Get().GetBaseWizard().dbName.Substring("WIZARD-".Length);
        string str2 = ds.owner.Get().GetPersonality().dbName.Substring("PERSONALITY-".Length);
        string name = GameManager.GetHumanWizard().GetName();
        switch (this.messageType)
        {
            case MessageType.GoodBye:
                return "DES_GOOD_BYE";

            case MessageType.TalkAboutTreaties:
            {
                if (!this.playerIdea)
                {
                    Debug.LogError("It is not expected that AI will open general treaty offer!");
                    return ("NOT_IMPLEMENTED " + this.messageType.ToString());
                }
                object[] parameters = new object[] { name };
                return DBUtils.Localization.Get("DES_WHAT_TREATY_YOU_WANT_" + str2, true, parameters);
            }
            case MessageType.ThankYouForHelp:
                return "DES_THANK_YOU_FOR_YOUR_HELP";

            case MessageType.TreatyOffer:
                if ((this.keys != null) && (this.keys.Length > 1))
                {
                    Treaty t = DataBase.Get<Treaty>(this.keys[1], false);
                    if (t != null)
                    {
                        object[] parameters = new object[] { DBUtils.Localization.Get(t.GetDescriptionInfo().GetName() + "_MID_SENTENCE", true, Array.Empty<object>()), t.length, DescriptionInfoExtension.GetDILocalizedDescription(t) };
                        return DBUtils.Localization.Get("DES_I_WANT_TO_START_TREATY", true, parameters);
                    }
                }
                return "DES_I_WANT_TO_START_TREATY";

            case MessageType.TradeOffer:
            {
                object[] parameters = new object[] { this.extraData };
                return DBUtils.Localization.Get("DES_TRADE_OFFER", true, parameters);
            }
            case MessageType.ExpansionWarning:
                return "DES_EXPANSION_WARNING";

            case MessageType.WalkingOverMyTerritory:
            {
                Relationship relationship = DataBase.Get<Relationship>(RELATIONSHIP.FRIENDLY, false);
                Relationship relationship2 = DataBase.Get<Relationship>(RELATIONSHIP.RESTLESS, false);
                int num = ds.GetRelationship();
                return (((relationship == null) || (num < relationship.minValue)) ? (((relationship2 == null) || (num >= relationship2.minValue)) ? "DES_TERRITORY_BREAK_WARNING_NEUTRAL" : "DES_TERRITORY_BREAK_WARNING_HOSTILE") : "DES_TERRITORY_BREAK_WARNING_FRIENDLY");
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
                    if ((baseWizard.associateInitialGreeting != null) && !ds.target.Get().isCustom)
                    {
                        foreach (Associate_Greeting greeting in baseWizard.associateInitialGreeting)
                        {
                            if ((ds.target != null) && ReferenceEquals(ds.target.Get().GetBaseWizard(), greeting.wizard))
                            {
                                return DBUtils.Localization.Get(greeting.greeting, true, Array.Empty<object>());
                            }
                        }
                    }
                    return (DBUtils.Localization.Get("DES_" + str + "_GREETINGS", true, Array.Empty<object>()) + "\n" + DBUtils.Localization.Get("DES_INITIAL_GREETINGS_" + str2, true, Array.Empty<object>()));
                }
                if (baseWizard.associateGreeting != null)
                {
                    foreach (Associate_Greeting greeting2 in baseWizard.associateGreeting)
                    {
                        if ((ds.target != null) && ReferenceEquals(ds.target.Get().GetBaseWizard(), greeting2.wizard))
                        {
                            return DBUtils.Localization.Get(greeting2.greeting, true, Array.Empty<object>());
                        }
                    }
                }
                Relationship relationship3 = DataBase.Get<Relationship>(RELATIONSHIP.FRIENDLY, false);
                Relationship relationship4 = DataBase.Get<Relationship>(RELATIONSHIP.RESTLESS, false);
                int num2 = ds.GetRelationship();
                if ((relationship3 != null) && (num2 >= relationship3.minValue))
                {
                    object[] objArray3 = new object[] { name };
                    return DBUtils.Localization.Get("DES_GREETINGS_FRIENDLY_" + str2, true, objArray3);
                }
                if ((relationship4 != null) && (num2 < relationship4.minValue))
                {
                    object[] objArray4 = new object[] { name };
                    return DBUtils.Localization.Get("DES_GREETINGS_HOSTILE_" + str2, true, objArray4);
                }
                object[] parameters = new object[] { name };
                return DBUtils.Localization.Get("DES_GREETINGS_NEUTRAL_" + str2, true, parameters);
            }
            case MessageType.AcceptWarEnd:
            {
                object[] parameters = new object[] { name };
                return DBUtils.Localization.Get("DES_ACCEPT_WAR_END_" + str2, true, parameters);
            }
            case MessageType.AcceptTreatyBreak:
            {
                object[] parameters = new object[] { name };
                return DBUtils.Localization.Get("DES_ACCEPT_TREATY_BREAK_" + str2, true, parameters);
            }
            case MessageType.AcceptWar:
            {
                object[] parameters = new object[] { name };
                return DBUtils.Localization.Get("DES_ACCEPT_WAR_" + str2, true, parameters);
            }
            case MessageType.AcceptTreaty:
            {
                object[] parameters = new object[] { name };
                return (DBUtils.Localization.Get("DES_ACCEPT_TREATY_" + this.extraData, true, Array.Empty<object>()) + DBUtils.Localization.Get("DES_ACCEPT_TREATY_" + str2, true, parameters));
            }
            case MessageType.SuccessfulTrade:
                return DBUtils.Localization.Get("DES_SUCCESSFUL_TRADE", true, Array.Empty<object>());

            case MessageType.ExcelentTrade:
                return DBUtils.Localization.Get("DES_EXCELENT_TRADE", true, Array.Empty<object>());

            case MessageType.PoorTrade:
                return DBUtils.Localization.Get("DES_POOR_TRADE", true, Array.Empty<object>());

            case MessageType.Rejected:
                return "DES_NO_NOT_INTERESTED";

            case MessageType.FeedbackMessage:
                int? nullable1;
                if ((this.keys != null) && (this.keys.Length != 0))
                {
                    return DBUtils.Localization.Get(this.keys[0], true, Array.Empty<object>());
                }
                if (this.keys != null)
                {
                    nullable1 = new int?(this.keys.Length);
                }
                else
                {
                    string[] keys = this.keys;
                    nullable1 = null;
                }
                return ("NOT_IMPLEMENTED " + this.messageType.ToString() + " keys " + nullable1.ToString());
        }
        return ("NOT_IMPLEMENTED " + this.messageType.ToString());
    }

    public string GetString(DiplomaticStatus ds)
    {
        return DBUtils.Localization.Get(this.GetKey(ds), true, this.keys);
    }

    public Treaty GetTreatyFromMessage()
    {
        return (((this.keys == null) || (this.keys.Length != 2)) ? null : DataBase.Get<Treaty>(this.keys[1], false));
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
        if ((this.keys == null) || (this.keys.Length < 2))
        {
            return null;
        }
        List<object> possibleTradeWares = messageTarget.GetPossibleTradeWares();
        if ((possibleTradeWares == null) || (possibleTradeWares.Count < 1))
        {
            return null;
        }
        object obj2 = null;
        if (this.keys[0] == typeof(MOM.Artefact).ToString())
        {
            obj2 = possibleTradeWares.Find(o => (o is MOM.Artefact) && ((o as MOM.Artefact).GetHash().ToString() == this.keys[1]));
        }
        else if (this.keys[0] == typeof(DBReference<Spell>).ToString())
        {
            obj2 = possibleTradeWares.Find(o => (o is DBReference<Spell>) && ((o as DBReference<Spell>).dbName == this.keys[1]));
        }
        else if ((this.keys.Length > 2) && (this.keys[0] == typeof(Multitype<NodeTrade.TradeCurrency, string, int>).ToString()))
        {
            obj2 = possibleTradeWares.Find(o => (o is Multitype<NodeTrade.TradeCurrency, string, int>) && ((o as Multitype<NodeTrade.TradeCurrency, string, int>).t0.ToString() == this.keys[1]));
        }
        return obj2;
    }

    public bool MessageStillValid(DiplomaticStatus status)
    {
        return ((this.messageType != MessageType.TradeOffer) || ((status.willToTrade >= -100) && (!status.openWar && (this.GetWareFromMessage(status.target.Get()) != null))));
    }

    public bool Reject(DiplomaticStatus recipient)
    {
        MessageType messageType = this.messageType;
        if ((messageType == MessageType.TreatyOffer) || (messageType == MessageType.WarDeclaration))
        {
            DiplomaticMessage message = new DiplomaticMessage {
                domination = Domination.AddToQueue,
                messageType = MessageType.FeedbackMessage
            };
            if (!this.GetTreatyFromMessage().agreementRequired)
            {
                Debug.LogError("not valid rejection path!");
            }
            else
            {
                message.keys = new string[] { "DES_NO_NOT_INTERESTED" };
            }
            recipient.AddMessage(message, false);
            return true;
        }
        if (messageType != MessageType.BreakTreaty)
        {
            return false;
        }
        DiplomaticMessage dm = new DiplomaticMessage {
            domination = Domination.AddToQueue,
            messageType = MessageType.FeedbackMessage
        };
        if (this.GetTreatyFromMessage().agreementRequired)
        {
            Debug.LogError("not valid rejection path!");
        }
        else
        {
            dm.keys = new string[] { "DES_NO_NOT_INTERESTED" };
        }
        recipient.AddMessage(dm, false);
        return true;
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
                return;

            case MessageType.TalkAboutTreaties:
            case MessageType.TreatyOffer:
            case MessageType.BreakTreaty:
                return;

            case MessageType.ThankYouForHelp:
                return;

            case MessageType.TradeOffer:
                return;

            case MessageType.WarDeclaration:
                status.StartTreaty(new DiplomaticTreaty((Treaty) TREATY.WAR));
                return;
        }
    }

    public enum Domination
    {
        AddToQueue,
        ClearBelow,
        ClearSameAndBelow,
        ClearWholeQueue
    }

    public enum MessageType
    {
        None,
        GoodBye,
        TalkAboutTreaties,
        ThankYouForHelp,
        TreatyOffer,
        TradeOffer,
        ExpansionWarning,
        WalkingOverMyTerritory,
        AttackedMyArmy,
        AttackedMyTown,
        WontTalk,
        WarDeclaration,
        BreakTreaty,
        Greetings,
        InitialGreetings,
        AcceptWarEnd,
        AcceptTreatyBreak,
        AcceptWar,
        AcceptTreaty,
        SuccessfulTrade,
        ExcelentTrade,
        PoorTrade,
        Rejected,
        FeedbackMessage
    }
}

