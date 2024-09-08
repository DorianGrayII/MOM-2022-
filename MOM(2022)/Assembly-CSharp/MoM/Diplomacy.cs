using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class Diplomacy : ScreenBase
    {
        public DiplomacyInfo diplomacyInfo;

        public Button btClose;

        public RawImage wizardCurrentImage;

        public GameObject p_diplomacyText;

        public GameObject p_diplomacyAnswer;

        public GameObject paper;

        public GameObject textContainer;

        public GameObject mouseHints;

        public GameObject wizardCurrentGemGreen;

        public GameObject wizardCurrentGemBlue;

        public GameObject wizardCurrentGemRed;

        public GameObject wizardCurrentGemPurple;

        public GameObject wizardCurrentGemYellow;

        public GameObject wizardInfoL;

        public GameObject wizardInfoR;

        public GridItemManager gridWizards;

        private PlayerWizard selectedWizard;

        private Coroutine mirrorAnim;

        private bool playerInitialized;

        private bool awaitTradeClosure;

        private int prevRelationship;

        public override IEnumerator PreStart()
        {
            this.playerInitialized = TurnManager.Get().playerTurn;
            PlayerWizard wizard = GameManager.GetHumanWizard();
            this.gridWizards.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                WizardListItem component = itemSource.GetComponent<WizardListItem>();
                PlayerWizard w = (source as Reference<PlayerWizard>).Get();
                component.gemBlue.SetActive(w.color == PlayerWizard.Color.Blue);
                component.gemGreen.SetActive(w.color == PlayerWizard.Color.Green);
                component.gemPurple.SetActive(w.color == PlayerWizard.Color.Purple);
                component.gemRed.SetActive(w.color == PlayerWizard.Color.Red);
                component.gemYellow.SetActive(w.color == PlayerWizard.Color.Yellow);
                component.atWar.SetActive(w.GetDiplomacy().IsAtWarWith(PlayerWizard.HumanID()));
                component.banished.SetActive(w.banishedTurn > 0);
                RolloverSimpleTooltip orAddComponent = component.icon.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
                orAddComponent.title = w.GetName();
                orAddComponent.useMouseLocation = false;
                orAddComponent.offset.x = 5f;
                orAddComponent.offset.y = -50f;
                component.icon.texture = w.Graphic;
                MagicAndResearch magicAndResearch = w.GetMagicAndResearch();
                if (magicAndResearch != null && magicAndResearch.curentlyCastSpell != null && wizard.detectMagic)
                {
                    component.labelCurrentlyCasting.text = magicAndResearch.curentlyCastSpell.GetDILocalizedName();
                    component.currenlyCastingBg.SetActive(value: true);
                    RolloverSimpleTooltip orAddComponent2 = component.currenlyCastingBg.GetOrAddComponent<RolloverSimpleTooltip>();
                    orAddComponent2.sourceAsDbName = magicAndResearch.curentlyCastSpell.dbName;
                    orAddComponent2.useMouseLocation = false;
                    orAddComponent2.offset.x = -80f;
                    orAddComponent2.offset.y = 50f;
                }
                else
                {
                    component.labelCurrentlyCasting.text = "";
                    component.currenlyCastingBg.SetActive(value: false);
                }
                component.icon.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
                {
                    UIManager.Open<Stats>(UIManager.Layer.Standard).wizard = w;
                };
                Button component2 = component.icon.gameObject.GetComponent<Button>();
                if (component2 != null)
                {
                    component2.onClick.RemoveAllListeners();
                    component2.onClick.AddListener(delegate
                    {
                        this.StartDialog(w);
                    });
                }
            });
            if (this.playerInitialized)
            {
                this.btClose.gameObject.SetActive(value: true);
                this.paper.SetActive(value: false);
                this.mouseHints.SetActive(this.gridWizards.GetItems().Count > 0);
                this.wizardInfoL.SetActive(value: false);
                this.wizardInfoR.SetActive(value: false);
                this.ShowWizardImage(visibility: false);
                List<Reference<PlayerWizard>> items = wizard.GetDiscoveredWizards().FindAll((Reference<PlayerWizard> o) => o.Get().isAlive);
                this.gridWizards.UpdateGrid(items);
            }
            else
            {
                this.gridWizards.UpdateGrid(new List<Reference<PlayerWizard>>());
            }
            this.StartDialogDM(null);
            MHEventSystem.RegisterListener<Trade>(TradeFinished, this);
            AudioLibrary.RequestSFX("OpenDiplomacy");
            yield return base.PreStart();
        }

        private void TradeFinished(object sender, object e)
        {
            if (e as string == "Closing" && this.awaitTradeClosure)
            {
                this.awaitTradeClosure = false;
            }
            DiplomaticMessage m = null;
            DiplomaticStatus diplomaticStatus = this.selectedWizard?.GetDiplomacy()?.GetStatusToward(GameManager.GetHumanWizard());
            if (diplomaticStatus != null)
            {
                m = diplomaticStatus.DequeueMessage();
            }
            this.StartDialogDM(m);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s.name == "ButtonClose")
            {
                UIManager.Close(this);
            }
        }

        private void ShowWizardImage(bool visibility)
        {
            if (this.mirrorAnim != null)
            {
                base.StopCoroutine(this.mirrorAnim);
                this.mirrorAnim = null;
            }
            this.mirrorAnim = base.StartCoroutine(this.ShowWizardImageCoroutine(visibility));
        }

        private IEnumerator ShowWizardImageCoroutine(bool visibility)
        {
            while (true)
            {
                Color color = this.wizardCurrentImage.color;
                if (!visibility)
                {
                    if (color.a <= 0f)
                    {
                        break;
                    }
                    color.a -= 0.05f;
                }
                else
                {
                    if (color.a >= 1f)
                    {
                        break;
                    }
                    color.a += 0.05f;
                }
                this.wizardCurrentImage.color = color;
                yield return null;
            }
            this.mirrorAnim = null;
        }

        public void StartDialog(PlayerWizard w)
        {
            this.selectedWizard = w;
            this.prevRelationship = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard()).GetRelationship();
            DiplomaticStatus statusToward = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
            if (statusToward.CanStartTalk() || statusToward.GetMessageQueue().Count > 0)
            {
                DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
                diplomaticMessage.domination = DiplomaticMessage.Domination.AddToQueue;
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.Greetings;
                statusToward.AddMessage(diplomaticMessage, addOnlyIfHighest: true);
            }
            else
            {
                DiplomaticMessage diplomaticMessage2 = new DiplomaticMessage();
                diplomaticMessage2.domination = DiplomaticMessage.Domination.ClearSameAndBelow;
                diplomaticMessage2.messageType = DiplomaticMessage.MessageType.WontTalk;
                statusToward.AddMessage(diplomaticMessage2, addOnlyIfHighest: true);
            }
            DiplomaticMessage diplomaticMessage3 = statusToward.DequeueMessage();
            if (diplomaticMessage3 != null)
            {
                this.StartDialogDM(diplomaticMessage3);
            }
        }

        public void AttemptToContinueOtherwiseThis(DiplomaticMessage m)
        {
            DiplomaticStatus statusToward = this.selectedWizard.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
            if (statusToward.GetNextMessage() != null)
            {
                this.StartDialogDM(statusToward.GetNextMessage());
            }
            else
            {
                this.StartDialogDM(m);
            }
        }

        public void StartDialogDM(DiplomaticMessage m)
        {
            if (m == null)
            {
                this.btClose.gameObject.SetActive(value: true);
                this.paper.SetActive(value: false);
                this.mouseHints.SetActive(this.gridWizards.GetItems().Count > 0);
                this.wizardInfoL.SetActive(value: false);
                this.wizardInfoR.SetActive(value: false);
                this.ShowWizardImage(visibility: false);
                this.wizardCurrentGemBlue.SetActive(value: false);
                this.wizardCurrentGemGreen.SetActive(value: false);
                this.wizardCurrentGemPurple.SetActive(value: false);
                this.wizardCurrentGemRed.SetActive(value: false);
                this.wizardCurrentGemYellow.SetActive(value: false);
                this.wizardCurrentImage.texture = UIReferences.GetTransparent();
                this.diplomacyInfo.Set(null);
                List<Reference<PlayerWizard>> items = GameManager.GetHumanWizard().GetDiscoveredWizards().FindAll((Reference<PlayerWizard> o) => o.Get().isAlive);
                this.gridWizards.UpdateGrid(items);
                return;
            }
            bool flag = false;
            PlayerWizard w = this.selectedWizard;
            DiplomaticStatus aiSideStatus = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
            Treaty war = (Treaty)TREATY.WAR;
            if (aiSideStatus.GetMessageQueue() != null && aiSideStatus.GetMessageQueue().Contains(m))
            {
                aiSideStatus.GetMessageQueue().Remove(m);
            }
            else
            {
                flag = m.playerIdea;
            }
            this.wizardCurrentGemBlue.SetActive(w.color == PlayerWizard.Color.Blue);
            this.wizardCurrentGemGreen.SetActive(w.color == PlayerWizard.Color.Green);
            this.wizardCurrentGemPurple.SetActive(w.color == PlayerWizard.Color.Purple);
            this.wizardCurrentGemRed.SetActive(w.color == PlayerWizard.Color.Red);
            this.wizardCurrentGemYellow.SetActive(w.color == PlayerWizard.Color.Yellow);
            this.wizardCurrentImage.texture = w.Graphic;
            this.diplomacyInfo.Set(w);
            this.btClose.gameObject.SetActive(value: false);
            this.paper.SetActive(value: true);
            this.mouseHints.SetActive(value: false);
            this.ShowWizardImage(visibility: true);
            this.wizardInfoL.SetActive(value: true);
            this.wizardInfoR.SetActive(value: true);
            GameObjectUtils.RemoveChildren(this.textContainer.transform);
            GameObject gameObject = GameObjectUtils.Instantiate(this.p_diplomacyText, this.textContainer.transform);
            TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            string text = "";
            int relationship = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard()).GetRelationship();
            int num = relationship - this.prevRelationship;
            if (num != 0)
            {
                string text2 = w.GetPersonality().dbName.Substring("PERSONALITY-".Length);
                text = ((num >= 0) ? global::DBUtils.Localization.Get("DES_RELATIONSHIP_IMPROVED_" + text2, true) : global::DBUtils.Localization.Get("DES_RELATIONSHIP_WORSEN_" + text2, true));
                this.prevRelationship = relationship;
            }
            componentInChildren.text = text + m.GetString(aiSideStatus);
            if (flag && m.AIMessageActivation(aiSideStatus))
            {
                DiplomaticMessage diplomaticMessage = new DiplomaticMessage();
                diplomaticMessage.domination = DiplomaticMessage.Domination.AddToQueue;
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.Greetings;
                this.AttemptToContinueOtherwiseThis(diplomaticMessage);
                return;
            }
            if (m.CanContinue())
            {
                if (aiSideStatus.GetNextMessage() != null)
                {
                    gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                    componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                    componentInChildren.text = global::DBUtils.Localization.Get("UI_CONTINUE", true);
                    gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        this.StartDialogDM(aiSideStatus.GetNextMessage());
                    });
                }
                else
                {
                    if (aiSideStatus.CanTrade())
                    {
                        int num2 = -aiSideStatus.willToTrade;
                        DiplomaticMessage diplomaticMessage2 = null;
                        if (m.messageType == DiplomaticMessage.MessageType.TradeOffer)
                        {
                            diplomaticMessage2 = m;
                        }
                        object requestedItem = diplomaticMessage2?.GetWareFromMessage(GameManager.GetHumanWizard());
                        gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                        componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                        if (num2 > 0)
                        {
                            componentInChildren.text = global::DBUtils.Localization.Get("DES_UNWILLING_TRADE", true);
                        }
                        else if (requestedItem != null)
                        {
                            componentInChildren.text = global::DBUtils.Localization.Get("DES_WILLING_TRADE", true);
                        }
                        else
                        {
                            componentInChildren.text = global::DBUtils.Localization.Get("DES_TRADE2", true);
                        }
                        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            Debug.LogWarning("Open trade screen");
                            UIManager.Open<Trade>(UIManager.Layer.Standard, this).SetData(GameManager.GetHumanWizard(), w, requestedItem);
                            aiSideStatus.willToTrade = Mathf.Min(aiSideStatus.willToTrade, 0);
                            if (!m.playerIdea)
                            {
                                this.awaitTradeClosure = true;
                            }
                        });
                    }
                    if (aiSideStatus.CanTalkAboutTreaties() && m.CanTalkTreaties())
                    {
                        gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                        componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                        componentInChildren.text = global::DBUtils.Localization.Get("DES_LETS_TALK_ABOUT_TREATIES", true);
                        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            DiplomaticMessage m12 = new DiplomaticMessage
                            {
                                domination = DiplomaticMessage.Domination.AddToQueue,
                                messageType = DiplomaticMessage.MessageType.TalkAboutTreaties,
                                playerIdea = true
                            };
                            this.StartDialogDM(m12);
                        });
                    }
                    if (!aiSideStatus.openWar)
                    {
                        gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                        componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                        componentInChildren.text = global::DBUtils.Localization.Get("DES_I_WANT_TO_START_WAR", true, war.GetDescriptionInfo().GetLocalizedName());
                        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            DiplomaticStatus reverseStatusFromTarget4 = aiSideStatus.GetReverseStatusFromTarget();
                            if (aiSideStatus.WillAccept(war))
                            {
                                DiplomaticTreaty t4 = new DiplomaticTreaty(war);
                                reverseStatusFromTarget4.StartTreaty(t4);
                                DiplomaticMessage m10 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue,
                                    messageType = DiplomaticMessage.MessageType.AcceptWar
                                };
                                this.StartDialogDM(m10);
                            }
                            else
                            {
                                reverseStatusFromTarget4.ChangeRelationshipBy(-10, affectTreaties: true);
                                DiplomaticMessage m11 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue,
                                    messageType = DiplomaticMessage.MessageType.Rejected
                                };
                                this.StartDialogDM(m11);
                            }
                        });
                    }
                    else if (aiSideStatus.CanTalkAboutTreaties() && m.CanTalkTreaties())
                    {
                        DiplomaticTreaty tInstance2 = aiSideStatus.GetTreaties().Find((DiplomaticTreaty o) => o.source.Get() == war);
                        gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                        componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                        componentInChildren.text = global::DBUtils.Localization.Get("DES_I_WANT_TO_END_WAR", true, war.GetDescriptionInfo().GetLocalizedName());
                        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            DiplomaticStatus reverseStatusFromTarget3 = aiSideStatus.GetReverseStatusFromTarget();
                            if (aiSideStatus.WillAcceptBreak(war))
                            {
                                reverseStatusFromTarget3.BreakTreaty(tInstance2);
                                DiplomaticMessage m8 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue,
                                    messageType = DiplomaticMessage.MessageType.AcceptWarEnd
                                };
                                this.StartDialogDM(m8);
                            }
                            else
                            {
                                reverseStatusFromTarget3.ChangeRelationshipBy(-10, affectTreaties: true);
                                DiplomaticMessage m9 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue,
                                    messageType = DiplomaticMessage.MessageType.Rejected
                                };
                                this.StartDialogDM(m9);
                            }
                        });
                    }
                }
            }
            if (m.messageType == DiplomaticMessage.MessageType.TalkAboutTreaties)
            {
                foreach (Treaty treaty in DataBase.GetType<Treaty>())
                {
                    if (treaty == war)
                    {
                        continue;
                    }
                    DiplomaticTreaty tInstance = aiSideStatus.GetTreaties().Find((DiplomaticTreaty o) => o.source.Get() == treaty);
                    if (tInstance != null)
                    {
                        gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                        componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                        if (treaty.length > 0)
                        {
                            componentInChildren.text = global::DBUtils.Localization.Get("DES_I_WANT_TO_END_TREATY_T", true, treaty.GetDescriptionInfo().GetLocalizedName(), tInstance.TimeLeft());
                        }
                        else
                        {
                            componentInChildren.text = global::DBUtils.Localization.Get("DES_I_WANT_TO_END_WAR", true, treaty.GetDescriptionInfo().GetLocalizedName());
                        }
                        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            DiplomaticStatus reverseStatusFromTarget2 = aiSideStatus.GetReverseStatusFromTarget();
                            if (aiSideStatus.WillAcceptBreak(treaty))
                            {
                                reverseStatusFromTarget2.BreakTreaty(tInstance);
                                DiplomaticMessage diplomaticMessage5 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue
                                };
                                if (treaty == (Treaty)TREATY.WAR)
                                {
                                    diplomaticMessage5.messageType = DiplomaticMessage.MessageType.AcceptWarEnd;
                                }
                                else
                                {
                                    diplomaticMessage5.messageType = DiplomaticMessage.MessageType.AcceptTreatyBreak;
                                }
                                this.StartDialogDM(diplomaticMessage5);
                            }
                            else
                            {
                                reverseStatusFromTarget2.ChangeRelationshipBy(-10, affectTreaties: true);
                                DiplomaticMessage m7 = new DiplomaticMessage
                                {
                                    domination = DiplomaticMessage.Domination.AddToQueue,
                                    messageType = DiplomaticMessage.MessageType.Rejected
                                };
                                this.StartDialogDM(m7);
                            }
                        });
                        continue;
                    }
                    gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                    componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                    string requestMode = "NONO";
                    if (treaty.length > 0)
                    {
                        string text3 = null;
                        float num3 = aiSideStatus.AcceptChance(treaty);
                        if (num3 < 0.01f)
                        {
                            requestMode = "NONO";
                        }
                        else if (num3 < 0.3f)
                        {
                            requestMode = "UNLIKELY";
                        }
                        else if (num3 < 0.8f)
                        {
                            requestMode = "MAYBE";
                        }
                        else if (num3 < 1f)
                        {
                            requestMode = "SURELY";
                        }
                        else
                        {
                            requestMode = "YESYES";
                        }
                        text3 = "DES_" + requestMode + "_I_WANT_TO_START_TREATY_T";
                        componentInChildren.text = global::DBUtils.Localization.Get(text3, true, global::DBUtils.Localization.Get(treaty.GetDescriptionInfo().GetName() + "_MID_SENTENCE", true), treaty.length, null);
                        RolloverSimpleTooltip orAddComponent = gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
                        orAddComponent.title = global::DBUtils.Localization.Get(treaty.GetDescriptionInfo().GetName(), true);
                        orAddComponent.description = global::DBUtils.Localization.Get(treaty.GetDescriptionInfo().GetLocalizedDescription(), true);
                        orAddComponent.useMouseLocation = false;
                        orAddComponent.anchor.x = 1.2f;
                    }
                    else
                    {
                        componentInChildren.text = global::DBUtils.Localization.Get("DES_I_WANT_TO_START_WAR", true, treaty.GetDescriptionInfo().GetLocalizedName());
                    }
                    gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        DiplomaticStatus reverseStatusFromTarget = aiSideStatus.GetReverseStatusFromTarget();
                        if (aiSideStatus.WillAccept(treaty))
                        {
                            DiplomaticTreaty t3 = new DiplomaticTreaty(treaty);
                            reverseStatusFromTarget.StartTreaty(t3);
                            DiplomaticMessage diplomaticMessage4 = new DiplomaticMessage
                            {
                                domination = DiplomaticMessage.Domination.AddToQueue
                            };
                            if (treaty == (Treaty)TREATY.WAR)
                            {
                                diplomaticMessage4.messageType = DiplomaticMessage.MessageType.AcceptWar;
                            }
                            else
                            {
                                diplomaticMessage4.messageType = DiplomaticMessage.MessageType.AcceptTreaty;
                                diplomaticMessage4.extraData = requestMode;
                            }
                            this.StartDialogDM(diplomaticMessage4);
                        }
                        else
                        {
                            reverseStatusFromTarget.ChangeRelationshipBy(-10, affectTreaties: true);
                            DiplomaticMessage m6 = new DiplomaticMessage
                            {
                                domination = DiplomaticMessage.Domination.AddToQueue,
                                messageType = DiplomaticMessage.MessageType.Rejected
                            };
                            this.StartDialogDM(m6);
                        }
                    });
                }
                gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.text = global::DBUtils.Localization.Get("UI_BACK", true);
                gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    DiplomaticMessage m5 = new DiplomaticMessage
                    {
                        domination = DiplomaticMessage.Domination.AddToQueue,
                        messageType = DiplomaticMessage.MessageType.Greetings
                    };
                    this.StartDialogDM(m5);
                });
            }
            if (m.CanAccept())
            {
                gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.text = global::DBUtils.Localization.Get("DES_TRADER_WILL_AGREE", true);
                gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    Treaty treaty2 = m.GetTreatyFromMessage();
                    if (m.messageType == DiplomaticMessage.MessageType.TreatyOffer || m.messageType == DiplomaticMessage.MessageType.WarDeclaration)
                    {
                        DiplomaticTreaty t = new DiplomaticTreaty(treaty2);
                        aiSideStatus.StartTreaty(t);
                        DiplomaticMessage diplomaticMessage3 = new DiplomaticMessage
                        {
                            domination = DiplomaticMessage.Domination.AddToQueue
                        };
                        if (m.messageType == DiplomaticMessage.MessageType.WarDeclaration)
                        {
                            diplomaticMessage3.messageType = DiplomaticMessage.MessageType.GoodBye;
                        }
                        else
                        {
                            diplomaticMessage3.messageType = DiplomaticMessage.MessageType.Greetings;
                        }
                        this.AttemptToContinueOtherwiseThis(diplomaticMessage3);
                    }
                    else if (m.messageType == DiplomaticMessage.MessageType.BreakTreaty)
                    {
                        DiplomaticTreaty t2 = aiSideStatus.GetTreaties().Find((DiplomaticTreaty o) => o.source == treaty2);
                        aiSideStatus.BreakTreaty(t2);
                        DiplomaticMessage m4 = new DiplomaticMessage
                        {
                            domination = DiplomaticMessage.Domination.AddToQueue,
                            messageType = DiplomaticMessage.MessageType.GoodBye
                        };
                        this.AttemptToContinueOtherwiseThis(m4);
                    }
                });
            }
            if (m.CanReject())
            {
                gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.text = global::DBUtils.Localization.Get("DES_NO_NOT_INTERESTED", true);
                gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    aiSideStatus.willTreaty -= 150;
                    DiplomaticMessage m3 = new DiplomaticMessage
                    {
                        domination = DiplomaticMessage.Domination.AddToQueue,
                        messageType = DiplomaticMessage.MessageType.GoodBye
                    };
                    this.AttemptToContinueOtherwiseThis(m3);
                });
            }
            if (m.CanSayBye())
            {
                gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.text = global::DBUtils.Localization.Get("DES_BYE", true);
                gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    DiplomaticMessage m2 = new DiplomaticMessage
                    {
                        domination = DiplomaticMessage.Domination.AddToQueue,
                        messageType = DiplomaticMessage.MessageType.GoodBye
                    };
                    this.StartDialogDM(m2);
                });
            }
            if (m.CanClose())
            {
                gameObject = GameObjectUtils.Instantiate(this.p_diplomacyAnswer, this.textContainer.transform);
                componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.text = global::DBUtils.Localization.Get("UI_CLOSE", true);
                gameObject.GetComponentInChildren<Button>();
                if (this.playerInitialized)
                {
                    this.selectedWizard = null;
                    this.StartDialogDM(null);
                }
                else
                {
                    this.selectedWizard = null;
                    this.StartDialogDM(null);
                    UIManager.ClickFromScript(this.btClose);
                }
            }
        }
    }
}
