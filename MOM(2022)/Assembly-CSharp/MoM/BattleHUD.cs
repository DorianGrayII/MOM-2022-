using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class BattleHUD : ScreenBase
    {
        public class CombatSummary
        {
            public string summary;

            public List<CombatLogDetail> details;

            public bool collectingData;

            public CombatSummary(string text = null)
            {
                this.summary = text;
            }
        }

        public class CombatLogDetail
        {
            public enum Arrow
            {
                None = 0,
                Left = 1,
                Right = 2
            }

            public int initiative;

            public string left;

            public string right;

            public Arrow arrow;
        }

        private class UnitInfo
        {
            public int id;

            public int figures;

            public int hp;

            public int maxhp;

            public BattleUnit bu;

            public UnitInfo(BattleUnit u)
            {
                this.id = u.GetID();
                this.figures = u.FigureCount();
                this.hp = u.currentFigureHP;
                this.maxhp = u.GetBaseFigure().maxHitPoints;
                this.bu = u;
            }

            public string GetDelta(BattleUnit u, out int damage)
            {
                damage = 0;
                int num = this.figures - u.FigureCount();
                int num2 = 0;
                string text = null;
                if (num == 0)
                {
                    num2 = this.hp - u.currentFigureHP;
                    if (num2 == 0)
                    {
                        return null;
                    }
                    text = global::DBUtils.Localization.Get("UI_COMBAT_LOG_NO_FIGURES", true, BattleHUD.GetHPGainedOrLost(num2));
                }
                else if (num > 0)
                {
                    num2 = this.hp + (this.maxhp - u.currentFigureHP) + (num - 1) * this.maxhp;
                    text = ((num != 1) ? global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURES", true, BattleHUD.GetHPGainedOrLost(num2), num) : global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURE", true, BattleHUD.GetHPGainedOrLost(num2)));
                }
                else
                {
                    num2 = -(this.maxhp - this.hp + u.currentFigureHP + (-num - 1) * this.maxhp);
                    text = ((num != -1) ? global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURES_RESURRECTED", true, BattleHUD.GetHPGainedOrLost(num2), -num, this.bu.GetName()) : global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURE_RESURRECTED", true, BattleHUD.GetHPGainedOrLost(num2), this.bu.GetName()));
                }
                damage = -num2;
                if (num2 != 0)
                {
                    new CombatEffect(u, num2, num);
                }
                return text;
            }
        }

        public static BattleHUD instance;

        public BattleHUDInfo attackerInfo;

        public BattleHUDInfo defenderInfo;

        public GridItemManager castersGrid;

        public TextMeshProUGUI turn;

        public TextMeshProUGUI castingSkill;

        public TextMeshProUGUI castingMana;

        public TextMeshProUGUI castingRange;

        public TextMeshProUGUI spellName;

        public TextMeshProUGUI spellTarget;

        public TextMeshProUGUI spellTargetDetails;

        public TextMeshProUGUI globalSpellName;

        public TextMeshProUGUI globalSpellCaster;

        public RawImage spellImage;

        public RawImage globalSpellImage;

        public GridItemManager globalEnchantmentsGrid;

        public GameObject goCasting;

        public GameObject goCastingGlobal;

        public GameObject goAutoCombatAnim;

        public GameObject goCombatLog;

        public GameObject goCombatLogContent;

        public GameObject p_CombatLogText;

        public GameObject goMirror;

        public GameObject p_CombatLogDetails;

        public GameObject p_CastingSkillParticle;

        public GameObject CastingSkillParticleTarget;

        public ScrollRect scrollCombatLog;

        public Button buttonSpellBook;

        public Button buttonAutoPlay;

        public Button buttonSurrender;

        public Button buttonBugReport;

        public Button buttonBattleHelp;

        public Toggle tgCombatLog;

        public Toggle tgCombatSpeed1;

        public Toggle tgCombatSpeed2;

        public Toggle tgCombatSpeed4;

        public Toggle tgCameraFollow;

        public Toggle tgAutoEndTurn;

        public Toggle tgAutocombatSpellcasting;

        public Animator animator;

        public Animator CastingSkillAnimator;

        private BattleUnit selectedUnit;

        private BattleUnit rolloverUnit;

        public BattleUnit activeCaster;

        private bool weAreAlsoReady;

        private GameObject selectedHuman;

        private bool m_moving;

        private bool dirty;

        private static List<CombatSummary> combatLog;

        private static int[] figures = new int[2];

        private static int[] hp = new int[2];

        private static int[] figures2 = new int[2];

        private static int[] hp2 = new int[2];

        private static int[] hpLost = new int[2];

        private static int[] maxhp = new int[2];

        private static BattleUnit[] battleUnits = new BattleUnit[2];

        private static int[] totalDamage = new int[2];

        private static int[] totalFiguresLost = new int[2];

        private static BattleUnit[] totalBattleUnits = new BattleUnit[2];

        private static BattleUnit logBattleUnit;

        private static Spell logSpell;

        private static object logTarget;

        private static FInt curInitiative;

        private static int initiative;

        private static Skill logSkill;

        private static ISpellCaster logCaster;

        private static CombatSummary summary;

        private Coroutine m_scrollCoroutine;

        private static Dictionary<int, UnitInfo> logStoredUnitInfo = new Dictionary<int, UnitInfo>();

        private static Dictionary<int, int> logSpellDamage = new Dictionary<int, int>();

        public static BattleHUD Get()
        {
            return BattleHUD.instance;
        }

        protected override void Awake()
        {
            this.goMirror.SetActive(value: true);
            base.cGroup.alpha = 0f;
            base.Awake();
            BattleHUD.instance = this;
            this.UpdateGeneralInfo();
            this.attackerInfo.UpdateUnitInfoDisplay(null, showButtons: true);
            this.defenderInfo.UpdateUnitInfoDisplay(null, showButtons: true);
            this.castersGrid.CustomDynamicItem(SpellCasterGridItem);
            CanvasGroup component = this.castersGrid.GetComponent<CanvasGroup>();
            if (component.alpha > 0f)
            {
                component.alpha = 0f;
                component.blocksRaycasts = false;
            }
            GameObject original = AssetManager.Get<GameObject>("Marker_CurrentUnit_Ally");
            this.selectedHuman = Object.Instantiate(original, base.transform);
            this.selectedHuman.gameObject.SetActive(value: false);
            base.StartCoroutine(this.GetReady());
            MHEventSystem.RegisterListener<CharacterActor>(OnMovementChange, this);
            MHEventSystem.RegisterListener<RollOverOut>(CasterRollover, this);
        }

        private void OnDestroy()
        {
            Object.Destroy(this.selectedHuman);
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public IEnumerator GetReady()
        {
            bool animReady = false;
            while (!this.weAreAlsoReady || !animReady)
            {
                AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
                animReady = currentAnimatorStateInfo.IsTag("Ready") && currentAnimatorStateInfo.normalizedTime >= 1f;
                yield return null;
            }
            yield return FSMPreBattle.LoadMap(null);
            Battle battle = Battle.GetBattle();
            this.attackerInfo.SetFromBattlePlayer(battle.attacker);
            this.defenderInfo.SetFromBattlePlayer(battle.defender);
            this.globalEnchantmentsGrid.CustomDynamicItem(EnchantmentItem);
            this.SetCasting(null);
            BattleHUD.GetSelectedUnit();
            this.goAutoCombatAnim.SetActive(value: false);
            this.animator.SetBool("LoadingFinished", value: true);
            PlayMakerFSM.BroadcastEvent("BattleReady");
        }

        public override IEnumerator PreStart()
        {
            this.weAreAlsoReady = true;
            this.tgCombatLog.onValueChanged.AddListener(delegate(bool active)
            {
                this.goCombatLog.SetActive(active);
            });
            switch (Settings.GetData().GetBattleAnimationSpeed())
            {
            case 1:
                this.tgCombatSpeed1.isOn = true;
                break;
            case 2:
                this.tgCombatSpeed2.isOn = true;
                break;
            case 4:
                this.tgCombatSpeed4.isOn = true;
                break;
            }
            this.tgAutoEndTurn.isOn = Settings.GetData().GetAutoEndTurn();
            this.tgCameraFollow.isOn = Settings.GetData().GetBattleCameraFollow();
            this.tgAutocombatSpellcasting.isOn = GameManager.Get().useManaInAutoresolves;
            return base.PreStart();
        }

        private void SpellCasterGridItem(GameObject itemSource, object source, object data, int index)
        {
            CasterListItem component = itemSource.GetComponent<CasterListItem>();
            GameObjectUtils.FindByName(itemSource.gameObject, "CasterIconUsed").SetActive(value: false);
            Button component2 = itemSource.GetComponent<Button>();
            if (source is BattlePlayer && (source as BattlePlayer).wizard != null)
            {
                BattlePlayer battlePlayer = source as BattlePlayer;
                PlayerWizard wizard = battlePlayer.wizard;
                component.goCasterUsed.SetActive(value: false);
                component.riCasterIcon.texture = wizard.Graphic;
                component.riCasterIconUsed.texture = wizard.Graphic;
                component.labelMana.text = battlePlayer.mana.ToString();
                if ((source as BattlePlayer).spellCasted || (source as BattlePlayer).castingBlock)
                {
                    component.goCasterUsed.SetActive(value: true);
                }
            }
            else if (source is BattleUnit)
            {
                BattleUnit battleUnit = source as BattleUnit;
                component.goCasterUsed.SetActive(value: false);
                component.riCasterIcon.texture = battleUnit.GetDescriptionInfo().GetTexture();
                component.riCasterIconUsed.texture = battleUnit.GetDescriptionInfo().GetTexture();
                component.labelMana.text = battleUnit.mana.ToString();
                if ((battleUnit.canCastSpells && battleUnit.spellCasted) || battleUnit.Mp <= 0)
                {
                    component.goCasterUsed.SetActive(value: true);
                }
            }
            component2.onClick.RemoveAllListeners();
            component2.onClick.AddListener(delegate
            {
                BattlePlayer battlePlayer2 = source as BattlePlayer;
                BattleUnit battleUnit2 = source as BattleUnit;
                if (battlePlayer2?.wizard != null && battlePlayer2.wizard.banishedTurn > 0)
                {
                    PopupGeneral.OpenPopup(this, "UI_YOU_ARE_BANISHED", "UI_BANISHED_CANNOT_CAST", "UI_OKAY");
                }
                else if (battlePlayer2 != null && battlePlayer2.spellCasted)
                {
                    PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_YOU_CAN_CAST_ONLY_ONCE_PER_TURN", "UI_OK");
                }
                else if (battlePlayer2 != null && battlePlayer2.castingBlock)
                {
                    PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_SPELLCASTING_BLOCKED", "UI_OK");
                }
                else if (battleUnit2 != null && (battleUnit2.spellCasted || battleUnit2.Mp <= 0))
                {
                    PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_UNIT_YOU_CAN_CAST_ONLY_ONCE_PER_TURN", "UI_OK");
                }
                else
                {
                    CastSpells.battleCaster = source;
                    MHEventSystem.TriggerEvent(this, "OpenBook");
                    CanvasGroup component3 = this.castersGrid.GetComponent<CanvasGroup>();
                    component3.alpha = 0f;
                    component3.blocksRaycasts = false;
                }
            });
            itemSource.GetOrAddComponent<RollOverOut>().data = source;
        }

        private void EnchantmentItem(GameObject itemSource, object source, object data, int index)
        {
            EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
            RawImage rawImage = GameObjectUtils.FindByNameGetComponentInChildren<RawImage>(itemSource, "E1Image");
            Image image = GameObjectUtils.FindByNameGetComponentInChildren<Image>(itemSource, "Frame");
            rawImage.texture = enchantmentInstance.source.Get().GetDescriptionInfo().GetTexture();
            Color color = WizardColors.GetColor((enchantmentInstance.owner?.GetEntity() is PlayerWizard playerWizard) ? playerWizard.color : PlayerWizard.Color.None);
            image.color = color;
            itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = enchantmentInstance.source.Get().dbName;
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.attackerInfo.buttonWait || s == this.defenderInfo.buttonWait)
            {
                MHEventSystem.TriggerEvent<BattleHUD>(this, "EndTurn");
            }
            else if (s == this.attackerInfo.buttonNextUnit || s == this.defenderInfo.buttonNextUnit)
            {
                MHEventSystem.TriggerEvent<BattleHUD>(this, "Skip");
            }
            else if (s == this.buttonSurrender)
            {
                MHEventSystem.TriggerEvent<BattleHUD>(this, "Surrender");
            }
            else if (s == this.buttonAutoPlay)
            {
                Battle battle = Battle.GetBattle();
                bool autoPlayByAI = battle.GetHumanPlayer().autoPlayByAI;
                autoPlayByAI = !autoPlayByAI;
                battle.GetHumanPlayer().autoPlayByAI = autoPlayByAI;
                if (battle.activeTurn == null)
                {
                    return;
                }
                bool isAttackerTurn = battle.activeTurn.isAttackerTurn;
                if ((isAttackerTurn && battle.attacker.playerOwner) || (!isAttackerTurn && battle.defender.playerOwner))
                {
                    if (autoPlayByAI)
                    {
                        battle.activeTurn.StartAI(isAttackerTurn);
                    }
                    else
                    {
                        battle.activeTurn.StopAI();
                    }
                }
                if (autoPlayByAI)
                {
                    this.goAutoCombatAnim.SetActive(value: true);
                }
                else
                {
                    this.goAutoCombatAnim.SetActive(value: false);
                }
            }
            else if (s == this.tgAutocombatSpellcasting)
            {
                GameManager.Get().useManaInAutoresolves = this.tgAutocombatSpellcasting.isOn;
            }
            else if (s == this.buttonSpellBook)
            {
                CanvasGroup component = this.castersGrid.GetComponent<CanvasGroup>();
                if (component.alpha > 0f)
                {
                    component.alpha = 0f;
                    component.blocksRaycasts = false;
                    return;
                }
                Battle battle2 = Battle.GetBattle();
                BattlePlayer battlePlayer = (battle2.defender.playerOwner ? battle2.defender : battle2.attacker);
                List<BattleUnit> list = (battle2.defender.playerOwner ? battle2.defenderUnits : battle2.attackerUnits);
                List<object> list2 = new List<object>();
                if (list != null)
                {
                    foreach (BattleUnit item in list)
                    {
                        if (item.canCastSpells)
                        {
                            SpellManager spellManager = item.GetSpellManager();
                            if (spellManager.GetSpells() != null && spellManager.GetSpells().Count > 0 && item.IsAlive())
                            {
                                list2.Add(item);
                            }
                        }
                    }
                }
                bool flag = battlePlayer.wizard.GetSpells().Find((DBReference<Spell> o) => !string.IsNullOrEmpty(o.Get().battleScript)) != null;
                if (list2.Count < 1)
                {
                    if (battlePlayer.wizard.banishedTurn > 0)
                    {
                        PopupGeneral.OpenPopup(this, "UI_YOU_ARE_BANISHED", "UI_BANISHED_CANNOT_CAST", "UI_OKAY");
                        return;
                    }
                    if (battlePlayer.spellCasted)
                    {
                        PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_YOU_CAN_CAST_ONLY_ONCE_PER_TURN", "UI_OK");
                        return;
                    }
                    if (battlePlayer.castingBlock)
                    {
                        PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_SPELLCASTING_BLOCKED", "UI_OK");
                        return;
                    }
                    if (!flag)
                    {
                        PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_YOU_HAVE_NO_BATTLE_SPELLS", "UI_OK");
                        return;
                    }
                    CastSpells.battleCaster = battlePlayer;
                    MHEventSystem.TriggerEvent(this, "OpenBook");
                }
                else
                {
                    if (flag)
                    {
                        list2.Add(battlePlayer);
                    }
                    this.castersGrid.UpdateGrid(list2);
                    component.alpha = 1f;
                    component.blocksRaycasts = true;
                }
            }
            else if (s == this.buttonBugReport)
            {
                base.StartCoroutine(BugReportCatcher.OpenBugCatcher());
            }
            else if (s == this.buttonBattleHelp)
            {
                UIManager.Open<BattleHelp>(UIManager.Layer.Popup, this);
            }
            else if (s == this.tgCombatSpeed1 && this.tgCombatSpeed1.isOn)
            {
                Settings.GetData().SetBattleAnimationSpeed(1);
            }
            else if (s == this.tgCombatSpeed2 && this.tgCombatSpeed2.isOn)
            {
                Settings.GetData().SetBattleAnimationSpeed(2);
            }
            else if (s == this.tgCombatSpeed4 && this.tgCombatSpeed4.isOn)
            {
                Settings.GetData().SetBattleAnimationSpeed(4);
            }
            else if (s == this.tgAutoEndTurn)
            {
                Settings.GetData().SetAutoEndTurn(this.tgAutoEndTurn.isOn);
            }
            else if (s == this.tgCameraFollow)
            {
                Settings.GetData().SetBattleCameraFollow(this.tgCameraFollow.isOn);
            }
        }

        public override IEnumerator PostClose()
        {
            yield return base.PostClose();
            BattleHUD.instance = null;
        }

        public void Dirty()
        {
            this.dirty = true;
        }

        public void BaseUpdate()
        {
            Battle battle = Battle.GetBattle();
            battle.SortIfNeeded();
            this.attackerInfo.BaseUpdate();
            this.defenderInfo.BaseUpdate();
            if (this.globalEnchantmentsGrid.IsInitialized())
            {
                this.globalEnchantmentsGrid.UpdateGrid(battle.GetEnchantments().FindAll((EnchantmentInstance o) => !o.source.Get().hideEnch));
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (this.dirty)
            {
                this.BaseUpdate();
                this.dirty = false;
            }
        }

        public void UpdateGeneralInfo()
        {
            Battle battle = Battle.GetBattle();
            this.turn.text = global::DBUtils.Localization.Get("UI_TURN", true) + " " + battle.turn + " " + global::DBUtils.Localization.Get("UI_OF", true) + " " + battle.lastTurn;
            BattlePlayer humanPlayer = battle.GetHumanPlayer();
            this.castingRange.text = "x" + humanPlayer.towerDistanceCost.ToString("#0.0");
            this.castingMana.text = humanPlayer.mana.ToString();
            this.castingSkill.text = humanPlayer.castingSkill.ToString();
        }

        public void CasterRollover(object sender, object data)
        {
            string text = data as string;
            RollOverOut rollOverOut = sender as RollOverOut;
            if (text == "Enter" && Battle.Get() != null)
            {
                if (rollOverOut?.data is BattleUnit owner)
                {
                    this.activeCaster = owner;
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(owner);
                }
            }
            else if (text == "Exit")
            {
                this.activeCaster = null;
                if (rollOverOut?.data is BattleUnit owner2)
                {
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(owner2);
                }
            }
        }

        public void OnMovementChange(object sender, object data)
        {
            if ((string)data == "Start")
            {
                this.m_moving = true;
                this.selectedHuman.gameObject.SetActive(value: false);
            }
            else if ((string)data == "End")
            {
                this.m_moving = false;
                this.UpdateSelectedHuman();
            }
        }

        private void UpdateSelectedHuman()
        {
            if (this.selectedUnit != null)
            {
                PlayerWizard wizardOwner = this.selectedUnit.GetWizardOwner();
                if (wizardOwner != null && wizardOwner.IsHuman && this.selectedUnit.battleFormation != null)
                {
                    this.selectedHuman.gameObject.SetActive(value: true);
                    this.selectedHuman.transform.parent = this.selectedUnit.battleFormation.transform.parent;
                    this.selectedHuman.transform.localScale = Vector3.one;
                    this.selectedHuman.transform.position = HexCoordinates.HexToWorld3D(this.selectedUnit.GetPosition());
                    return;
                }
            }
            this.selectedHuman.gameObject.SetActive(value: false);
        }

        public static void RefreshSelection()
        {
            if (!(BattleHUD.Get() == null) && BattleHUD.Get().selectedUnit != null)
            {
                BattleHUD.Get().attackerInfo.Reselect();
                BattleHUD.Get().defenderInfo.Reselect();
                MHEventSystem.TriggerEvent<BattleHUD>(BattleHUD.Get(), null);
            }
        }

        public void SelectUnit(BattleUnit bu, bool attacker, bool focus)
        {
            if (bu != null && bu.IsAlive())
            {
                this.selectedUnit = bu;
                this.UpdateSelectedHuman();
                if (attacker)
                {
                    this.attackerInfo.UpdateUnitInfoDisplay(bu, showButtons: true);
                    this.attackerInfo.HighlightSelectedUnit(bu);
                }
                else
                {
                    this.defenderInfo.UpdateUnitInfoDisplay(bu, showButtons: true);
                    this.defenderInfo.HighlightSelectedUnit(bu);
                }
                if (focus && Settings.GetData().GetBattleCameraFollow())
                {
                    CameraController.CenterAt(bu.GetPosition());
                }
                MHEventSystem.TriggerEvent<BattleHUD>(this, null);
            }
        }

        public static BattleUnit GetSelectedUnit()
        {
            if (BattleHUD.Get() == null)
            {
                return null;
            }
            if (BattleHUD.Get().selectedUnit == null)
            {
                List<BattleUnit> humanPlayerUnits = Battle.GetBattle().GetHumanPlayerUnits();
                if (humanPlayerUnits.Count > 0)
                {
                    BattleUnit battleUnit = humanPlayerUnits.Find((BattleUnit o) => o.IsAlive());
                    if (battleUnit != null)
                    {
                        BattleHUD.Get().SelectUnit(battleUnit, battleUnit.attackingSide, focus: true);
                    }
                }
            }
            return BattleHUD.Get().selectedUnit;
        }

        public static void SetMessageAnim(bool player)
        {
            Animator component = GameObjectUtils.FindByName(BattleHUD.Get().gameObject, "TurnNotification").GetComponent<Animator>();
            if (player)
            {
                component.SetTrigger("PlayerTurn");
            }
            else
            {
                component.SetTrigger("EnemyTurn");
            }
            BattleHUD.Get().BaseUpdate();
        }

        public void SetCasting(Spell s)
        {
            if (s == null)
            {
                this.goCasting.SetActive(value: false);
                Battle.Get()?.plane?.GetMarkers_().ClearHighlightHexes();
            }
            else
            {
                this.goCasting.SetActive(value: true);
                this.spellImage.texture = s.GetDescriptionInfo().GetTexture();
                this.spellName.text = s.GetDescriptionInfo().GetLocalizedName();
                this.spellTarget.text = global::DBUtils.Localization.Get("UI_TARGET", true) + " " + s.targetType.desType.GetLocalizedTargetTypeDescription();
                this.spellTargetDetails.text = s.invalidTarget.GetLocalizedTargetTypeDescription();
            }
            this.BaseUpdate();
        }

        public void OnArmyOver(BattleUnit bu)
        {
            this.rolloverUnit = bu;
            FSMBattleTurn.instance.RollOverUnit(bu, Vector3i.invalid);
        }

        public void OnArmyExit(BattleUnit bu)
        {
            this.rolloverUnit = null;
            FSMBattleTurn.instance.RollOverUnit(null, Vector3i.invalid);
        }

        public void OnArmyClick(BattleUnit bu)
        {
            FSMBattleTurn.instance.LeftClickUnit(bu);
        }

        public void UIUpdateFor(bool isAttackerTurn)
        {
            Battle battle = Battle.GetBattle();
            if (battle.playerIsAttacker == isAttackerTurn)
            {
                this.buttonSpellBook.interactable = true;
                this.buttonAutoPlay.interactable = true;
                this.buttonSurrender.interactable = battle.playerIsAttacker || !(battle.gDefender?.IsHosted() ?? false);
                return;
            }
            this.buttonSpellBook.interactable = false;
            this.buttonAutoPlay.interactable = true;
            this.buttonSurrender.interactable = false;
            CanvasGroup component = this.castersGrid.GetComponent<CanvasGroup>();
            if (component != null)
            {
                component.alpha = 0f;
                component.blocksRaycasts = false;
            }
        }

        public void SetUnitDirty(BattleUnit bu)
        {
            if (bu == this.selectedUnit)
            {
                if (bu.attackingSide)
                {
                    this.attackerInfo.SetUnitDirty(bu);
                }
                else
                {
                    this.defenderInfo.SetUnitDirty(bu);
                }
            }
            else if (bu == this.rolloverUnit)
            {
                if (bu.attackingSide)
                {
                    this.defenderInfo.SetUnitDirty(bu);
                }
                else
                {
                    this.attackerInfo.SetUnitDirty(bu);
                }
            }
        }

        public void UnselectUnit()
        {
            this.selectedUnit = null;
        }

        public void AddingCastingSkillAnimation(Vector3i origin)
        {
            GameObject gameObject = GameObjectUtils.Instantiate(this.p_CastingSkillParticle, base.transform);
            gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            base.StartCoroutine(this.CastingSkillAnimation(gameObject, origin, this.CastingSkillParticleTarget, this.CastingSkillAnimator));
        }

        private IEnumerator CastingSkillAnimation(GameObject particle, Vector3i origin, GameObject target, Animator animator)
        {
            if (particle == null || target == null || animator == null)
            {
                yield return null;
            }
            int battleAnimationSpeed = Settings.GetData().GetBattleAnimationSpeed();
            yield return new WaitForSeconds(1.5f / (float)battleAnimationSpeed);
            CanvasGroup cg = particle.gameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                float timer = 0f;
                float duration = 1f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    particle.transform.position = this.CastingParticleStartingPosition(origin);
                    cg.alpha = Mathf.Clamp01(timer / duration);
                    yield return null;
                }
            }
            Vector3 targetV = target.gameObject.transform.position;
            Vector3 velocity = Vector3.zero;
            float smoothTime = 0.2f;
            while (true)
            {
                particle.transform.position = Vector3.SmoothDamp(particle.transform.position, targetV, ref velocity, smoothTime);
                if (Vector3.Distance(particle.transform.position, targetV) < 12.5f)
                {
                    break;
                }
                yield return null;
            }
            Object.Destroy(particle);
            animator.SetTrigger("Show");
            this.UpdateGeneralInfo();
            yield return null;
        }

        private Vector3 CastingParticleStartingPosition(Vector3i origin)
        {
            Vector3 position = HexCoordinates.HexToWorld3D(origin);
            Vector3 vector = Camera.main.WorldToViewportPoint(position);
            Vector3 zero = Vector3.zero;
            zero.x = vector.x * (float)Screen.width;
            zero.y = vector.y * (float)Screen.height;
            return zero;
        }

        public static void CombatLogStartStack(BattleAttackStack stack)
        {
            BattleHUD.summary = new CombatSummary();
            BattleHUD.summary.collectingData = true;
            if (stack.attackQueue.Count < 1)
            {
                Debug.LogWarning("[REPORT PLEASE] empty attack stack tried to add to the log");
                return;
            }
            BattleHUD.curInitiative = stack.attackQueue[0].initiative;
            BattleHUD.initiative = 1;
            BattleHUD.totalDamage[0] = 0;
            BattleHUD.totalDamage[1] = 0;
            BattleHUD.totalFiguresLost[0] = 0;
            BattleHUD.totalFiguresLost[1] = 0;
            Battle battle = Battle.GetBattle();
            bool flag = stack.defender.GetWizardOwner() == battle.defender.wizard;
            CombatLogDetail item = new CombatLogDetail
            {
                arrow = CombatLogDetail.Arrow.None,
                initiative = 0,
                left = (flag ? stack.defender.GetName() : stack.attacker.GetName()),
                right = (flag ? stack.attacker.GetName() : stack.defender.GetName())
            };
            BattleHUD.summary.details = new List<CombatLogDetail>();
            BattleHUD.summary.details.Add(item);
        }

        public static void CombatLogPreApplyDamages(BattleAttack attack)
        {
            BattleHUD.figures[0] = attack.source.FigureCount();
            BattleHUD.figures[1] = attack.destination.FigureCount();
            BattleHUD.hp[0] = attack.source.currentFigureHP;
            BattleHUD.hp[1] = attack.destination.currentFigureHP;
        }

        public static void CombatLogPostApplyDamages(BattleAttack attack)
        {
            int num = 0;
            if (attack.dmg != null)
            {
                int[] dmg = attack.dmg;
                foreach (int num2 in dmg)
                {
                    num += num2;
                }
            }
            string text = ((attack.skill.descriptionScript == null) ? attack.skill.GetDescriptionInfo().GetLocalizedName() : ((string)ScriptLibrary.Call(attack.skill.descriptionScript, attack.source, attack.skill, null)));
            if (num != 0)
            {
                text += global::DBUtils.Localization.Get("UI_COMBAT_LOG_DMG", true, num);
            }
            BattleHUD.battleUnits[0] = attack.source;
            BattleHUD.battleUnits[1] = attack.destination;
            string[] array = new string[2];
            Battle battle = Battle.GetBattle();
            bool flag = attack.source.GetWizardOwner() == battle.attacker.wizard;
            for (int j = 0; j < 2; j++)
            {
                BattleHUD.figures2[j] = BattleHUD.battleUnits[j].FigureCount();
                BattleHUD.hp2[j] = BattleHUD.battleUnits[j].currentFigureHP;
                BattleHUD.maxhp[j] = BattleHUD.battleUnits[j].GetBaseFigure().maxHitPoints;
                BattleHUD.hpLost[j] = 0;
                int num3 = BattleHUD.figures[j] - BattleHUD.figures2[j];
                int num4 = (flag ? (j ^ 1) : j);
                BattleHUD.totalFiguresLost[num4] += num3;
                if (num3 == 0)
                {
                    BattleHUD.hpLost[j] = BattleHUD.hp[j] - BattleHUD.hp2[j];
                }
                else if (num3 > 0 && BattleHUD.hp2[j] > 0)
                {
                    BattleHUD.hpLost[j] = BattleHUD.hp[j] + (BattleHUD.maxhp[j] - BattleHUD.hp2[j]) + (num3 - 1) * BattleHUD.maxhp[j];
                }
                else if (num3 > 0)
                {
                    BattleHUD.hpLost[j] = BattleHUD.hp[j] + (num3 - 1) * BattleHUD.maxhp[j];
                }
                if (j == 1 || BattleHUD.hpLost[j] != 0)
                {
                    array[j] = BattleHUD.GetFigureDelta(num3, BattleHUD.hpLost[j]);
                }
                BattleHUD.totalDamage[num4] += -BattleHUD.hpLost[j];
                BattleHUD.totalBattleUnits[num4] = BattleHUD.battleUnits[j];
            }
            if (BattleHUD.curInitiative != attack.initiative)
            {
                BattleHUD.curInitiative = attack.initiative;
                BattleHUD.initiative++;
            }
            if (array[0] != null)
            {
                text = text + " " + array[0];
            }
            CombatLogDetail item = new CombatLogDetail
            {
                arrow = (flag ? CombatLogDetail.Arrow.Left : CombatLogDetail.Arrow.Right),
                initiative = BattleHUD.initiative,
                left = (flag ? array[1] : text),
                right = (flag ? text : array[1])
            };
            BattleHUD.summary.details.Add(item);
        }

        private static string GetFigureDelta(int figuresLost, int hpLost)
        {
            if (figuresLost == 0)
            {
                return global::DBUtils.Localization.Get("UI_COMBAT_LOG_NO_FIGURES", true, BattleHUD.GetHPGainedOrLost(hpLost));
            }
            if (figuresLost == 1)
            {
                return global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURE", true, BattleHUD.GetHPGainedOrLost(hpLost));
            }
            if (figuresLost > 1)
            {
                return global::DBUtils.Localization.Get("UI_COMBAT_LOG_FIGURES", true, BattleHUD.GetHPGainedOrLost(hpLost), figuresLost);
            }
            return "";
        }

        public static void CombatLogFinishStack(BattleAttackStack stack)
        {
            if (BattleHUD.summary == null || !BattleHUD.summary.collectingData)
            {
                return;
            }
            Battle battle = Battle.GetBattle();
            if (battle == null)
            {
                return;
            }
            bool flag = stack.attacker.GetWizardOwner() == battle.attacker.wizard;
            List<string> list = new List<string>();
            if (!stack.attacker.IsAlive() || !stack.defender.IsAlive())
            {
                CombatLogDetail combatLogDetail = new CombatLogDetail
                {
                    arrow = CombatLogDetail.Arrow.None,
                    initiative = -1
                };
                if (!stack.attacker.IsAlive())
                {
                    if (flag)
                    {
                        combatLogDetail.right = BattleHUD.GetHeroOrUnitDied(stack.attacker);
                    }
                    else
                    {
                        combatLogDetail.left = BattleHUD.GetHeroOrUnitDied(stack.attacker);
                    }
                    list.Add(global::DBUtils.Localization.Get("UI_COMBAT_LOG_DESTROYED", true, stack.attacker.GetName(), WizardColors.GetHex(stack.attacker.GetWizardOwner())));
                }
                if (!stack.defender.IsAlive())
                {
                    if (flag)
                    {
                        combatLogDetail.left = BattleHUD.GetHeroOrUnitDied(stack.defender);
                    }
                    else
                    {
                        combatLogDetail.right = BattleHUD.GetHeroOrUnitDied(stack.defender);
                    }
                    list.Add(global::DBUtils.Localization.Get("UI_COMBAT_LOG_DESTROYED", true, stack.defender.GetName(), WizardColors.GetHex(stack.defender.GetWizardOwner())));
                }
                BattleHUD.summary.details.Add(combatLogDetail);
            }
            BattleUnit battleUnit = (flag ? stack.defender : stack.attacker);
            BattleHUD.summary.summary = "<#" + WizardColors.GetHex(battleUnit.GetWizardOwner()) + ">" + battleUnit.GetName() + "</color>";
            CombatSummary combatSummary = BattleHUD.summary;
            combatSummary.summary = combatSummary.summary + " " + BattleHUD.GetHPGainedOrLost(-BattleHUD.totalDamage[0]);
            battleUnit = (flag ? stack.attacker : stack.defender);
            CombatSummary combatSummary2 = BattleHUD.summary;
            combatSummary2.summary = combatSummary2.summary + " vs <#" + WizardColors.GetHex(battleUnit.GetWizardOwner()) + ">" + battleUnit.GetName() + "</color>";
            CombatSummary combatSummary3 = BattleHUD.summary;
            combatSummary3.summary = combatSummary3.summary + " " + BattleHUD.GetHPGainedOrLost(-BattleHUD.totalDamage[1]);
            if (BattleHUD.instance != null)
            {
                BattleHUD.instance.AddSummaryToLog(BattleHUD.summary);
            }
            foreach (string item in list)
            {
                BattleHUD.CombatLogAdd(item);
            }
            for (int i = 0; i < 2; i++)
            {
                if (BattleHUD.totalDamage[i] != 0)
                {
                    new CombatEffect(BattleHUD.totalBattleUnits[i], -BattleHUD.totalDamage[i], BattleHUD.totalFiguresLost[i]);
                }
            }
            BattleHUD.summary.collectingData = false;
        }

        public static void CombatLogSpell(ISpellCaster caster, Spell spell, object target)
        {
            BattleHUD.logCaster = caster;
            BattleHUD.logSpell = spell;
            BattleHUD.logTarget = target;
            BattleHUD.logSkill = null;
            BattleHUD.StoreUnitInfo();
        }

        public static void CombatLogSkill(Skill skill, BattleUnit target)
        {
            BattleHUD.logCaster = target;
            BattleHUD.logSpell = null;
            BattleHUD.logSkill = skill;
            BattleHUD.logTarget = target;
            BattleHUD.StoreUnitInfo();
        }

        public static string GetSpellTitle(bool causedDamage, int totalDamage = 0)
        {
            string result = "";
            string localizedName = ((BattleHUD.logSpell != null) ? BattleHUD.logSpell.GetDescriptionInfo() : BattleHUD.logSkill.GetDescriptionInfo()).GetLocalizedName();
            string text = BattleHUD.logCaster.GetName();
            string text2 = (causedDamage ? "2" : "");
            if (BattleHUD.logTarget != null)
            {
                if (BattleHUD.logTarget is BattleUnit battleUnit)
                {
                    if (causedDamage)
                    {
                        text2 = ((battleUnit.GetWizardOwner() == BattleHUD.logCaster.GetWizardOwner()) ? "2_FRIENDLY" : "2_VS");
                    }
                    result = global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_CAST_UNIT" + text2, true, text, localizedName, battleUnit.GetName(), WizardColors.GetHex(BattleHUD.logCaster.GetWizardOwner()), WizardColors.GetHex(battleUnit.GetWizardOwner()), BattleHUD.GetHPGainedOrLost(-totalDamage));
                }
                else
                {
                    object obj = BattleHUD.logTarget;
                    if (obj is Vector3i)
                    {
                        _ = (Vector3i)obj;
                        result = global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_CAST_BATTLEFIELD" + text2, true, text, localizedName, WizardColors.GetHex(BattleHUD.logCaster.GetWizardOwner()));
                    }
                    else if (BattleHUD.logTarget is Battle)
                    {
                        result = global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_CAST_BATTLEFIELD" + text2, true, text, localizedName, WizardColors.GetHex(BattleHUD.logCaster.GetWizardOwner()));
                    }
                    else if (BattleHUD.logTarget is BattlePlayer battlePlayer && battlePlayer.wizard != null)
                    {
                        result = global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_CAST_UNIT" + text2, true, text, localizedName, battlePlayer.wizard.GetName(), WizardColors.GetHex(BattleHUD.logCaster.GetWizardOwner()), WizardColors.GetHex(battlePlayer.wizard.GetWizardOwner()), BattleHUD.GetHPGainedOrLost(-totalDamage));
                    }
                }
            }
            else
            {
                result = global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_CAST" + text2, true, text, localizedName, WizardColors.GetHex(BattleHUD.logCaster.GetWizardOwner()));
            }
            return result;
        }

        public static void CombatLogSpellAddEffect()
        {
            BattleHUD.CalcUnitDelta();
        }

        private static string GetHeroOrUnitDied(BaseUnit u)
        {
            if (u.dbSource.Get() is Hero)
            {
                return global::DBUtils.Localization.Get("UI_HERO_DIED", true);
            }
            return global::DBUtils.Localization.Get("UI_UNIT_DIED", true);
        }

        private static string GetHPGainedOrLost(int lost)
        {
            if (lost >= 0)
            {
                return global::DBUtils.Localization.Get("UI_HP_LOST", true, lost);
            }
            return global::DBUtils.Localization.Get("UI_HP_GAINED", true, -lost);
        }

        private void AddSummaryToLog(CombatSummary summary)
        {
            Object.Instantiate(this.p_CombatLogText, this.goCombatLogContent.transform).GetComponent<CombatLogText>().Set(summary);
            if (this.m_scrollCoroutine != null)
            {
                base.StopCoroutine(this.m_scrollCoroutine);
            }
            this.m_scrollCoroutine = base.StartCoroutine(ScrollToBot());
            IEnumerator ScrollToBot()
            {
                yield return null;
                this.scrollCombatLog.verticalNormalizedPosition = 0f;
                this.m_scrollCoroutine = null;
            }
        }

        public static CombatSummary CombatLogAdd(string text)
        {
            BattleHUD.summary = new CombatSummary(text);
            BattleHUD.instance?.AddSummaryToLog(BattleHUD.summary);
            return BattleHUD.summary;
        }

        public static void AddDetailToLog(CombatLogDetail detail, bool sourceOnLeft = false)
        {
            string text = " ";
            if (detail.arrow == CombatLogDetail.Arrow.Left)
            {
                if (sourceOnLeft)
                {
                    BattleHUD.CombatLogAdd(detail.right + "->" + text + detail.left);
                    return;
                }
                text = "<-";
            }
            else if (detail.arrow == CombatLogDetail.Arrow.Right)
            {
                text = "->";
            }
            else if (sourceOnLeft && detail.left.Length == 0)
            {
                text = "";
            }
            BattleHUD.CombatLogAdd(detail.left + text + detail.right);
        }

        private static void OnUnitEvent(object sender, object e)
        {
            Unit unit = sender as Unit;
            Unit.UnitEvent unitEvent = e as Unit.UnitEvent;
            if (unitEvent.damage != 0)
            {
                BattleHUD.logSpellDamage.TryGetValue(unit.ID, out var value);
                BattleHUD.logSpellDamage[unit.ID] = unitEvent.damage + value;
            }
        }

        public static void CalcUnitDelta(List<BattleUnit> old, List<BattleUnit> current, DescriptionInfo di)
        {
            List<string> list = new List<string>();
            Battle battle = Battle.GetBattle();
            bool flag = false;
            bool flag2 = true;
            int num = 0;
            if (flag2)
            {
                BattleHUD.summary = new CombatSummary();
            }
            string text = di?.GetLocalizedName();
            foreach (BattleUnit u in old)
            {
                BattleUnit battleUnit = current.Find((BattleUnit o) => o.ID == u.ID);
                int num2 = battleUnit._currentFigureHP + battleUnit.figureCount * battleUnit.GetCurentFigure().maxHitPoints;
                int num3 = u._currentFigureHP + u.figureCount * u.GetCurentFigure().maxHitPoints;
                if (num2 == num3)
                {
                    continue;
                }
                int num4 = num3 - num2;
                if (u == BattleHUD.logTarget)
                {
                    num += num4;
                }
                int num5 = ((u.GetWizardOwner() != battle.defender.wizard) ? 1 : 0);
                if (!flag)
                {
                    BattleHUD.summary.details = new List<CombatLogDetail>();
                    BattleHUD.summary.details.Add(new CombatLogDetail
                    {
                        left = global::DBUtils.Localization.Get("UI_END_TURN_EFFECTS", true),
                        initiative = 0
                    });
                    flag = true;
                }
                string text2 = text;
                if (num4 != 0 && text2 != null)
                {
                    text2 = text2 + " (" + num4 + ")";
                }
                CombatLogDetail item = new CombatLogDetail
                {
                    arrow = ((num5 == 0) ? CombatLogDetail.Arrow.Left : CombatLogDetail.Arrow.Right),
                    left = "",
                    right = ""
                };
                if (flag2)
                {
                    BattleHUD.summary.details.Add(item);
                }
                if (!u.IsAlive())
                {
                    list.Add(global::DBUtils.Localization.Get("UI_COMBAT_LOG_DESTROYED", true, u.GetName(), WizardColors.GetHex(u.GetWizardOwner())));
                    string heroOrUnitDied = BattleHUD.GetHeroOrUnitDied(u);
                    item = new CombatLogDetail
                    {
                        arrow = CombatLogDetail.Arrow.None,
                        left = ((num5 == 0) ? heroOrUnitDied : ""),
                        right = ((num5 == 0) ? "" : heroOrUnitDied)
                    };
                    if (flag2)
                    {
                        BattleHUD.summary.details.Add(item);
                    }
                }
            }
            foreach (string item2 in list)
            {
                BattleHUD.CombatLogAdd(item2);
            }
        }

        public static void CalcUnitDelta()
        {
            List<string> list = new List<string>();
            MHEventSystem.UnRegisterListener(OnUnitEvent);
            Battle battle = Battle.GetBattle();
            List<BattleUnit> allUnits = battle.GetAllUnits();
            bool flag = false;
            bool flag2 = true;
            int num = 0;
            if (flag2)
            {
                BattleHUD.summary = new CombatSummary();
            }
            string text = ((BattleHUD.logSpell != null) ? BattleHUD.logSpell.GetDescriptionInfo().GetLocalizedName() : BattleHUD.logSkill.GetDescriptionInfo().GetLocalizedName());
            foreach (BattleUnit item in allUnits)
            {
                if (!BattleHUD.logStoredUnitInfo.ContainsKey(item.GetID()))
                {
                    continue;
                }
                int damage;
                string delta = BattleHUD.logStoredUnitInfo[item.GetID()].GetDelta(item, out damage);
                if (delta == null)
                {
                    continue;
                }
                if (item == BattleHUD.logTarget)
                {
                    num += damage;
                }
                int num2 = ((item.GetWizardOwner() != battle.defender.wizard) ? 1 : 0);
                if (!flag)
                {
                    if (flag2)
                    {
                        BattleHUD.summary.details = new List<CombatLogDetail>();
                        if (BattleHUD.logTarget is BattleUnit battleUnit && battleUnit.GetWizardOwner() != BattleHUD.logCaster.GetWizardOwner())
                        {
                            BattleHUD.summary.details.Add(new CombatLogDetail
                            {
                                arrow = ((num2 == 0) ? CombatLogDetail.Arrow.Left : CombatLogDetail.Arrow.Right),
                                left = ((num2 == 0) ? battleUnit.GetName() : BattleHUD.logCaster.GetName()),
                                right = ((num2 == 1) ? battleUnit.GetName() : BattleHUD.logCaster.GetName())
                            });
                        }
                        else
                        {
                            BattleHUD.summary.details.Add(new CombatLogDetail
                            {
                                left = BattleHUD.GetSpellTitle(causedDamage: true),
                                initiative = (int)((item.GetWizardOwner() != null) ? item.GetWizardOwner().color : PlayerWizard.Color.None)
                            });
                        }
                    }
                    else
                    {
                        BattleHUD.CombatLogAdd(BattleHUD.GetSpellTitle(causedDamage: true));
                    }
                    flag = true;
                }
                BattleHUD.logSpellDamage.TryGetValue(item.GetID(), out var value);
                string text2 = text;
                if (value != 0)
                {
                    text2 = text2 + " (" + value + ")";
                }
                CombatLogDetail combatLogDetail = new CombatLogDetail
                {
                    arrow = ((num2 == 0) ? CombatLogDetail.Arrow.Left : CombatLogDetail.Arrow.Right),
                    left = ((num2 == 1) ? text2 : delta),
                    right = ((num2 == 0) ? text2 : delta)
                };
                if (flag2)
                {
                    BattleHUD.summary.details.Add(combatLogDetail);
                }
                else
                {
                    BattleHUD.AddDetailToLog(combatLogDetail, sourceOnLeft: true);
                }
                if (!item.IsAlive())
                {
                    list.Add(global::DBUtils.Localization.Get("UI_COMBAT_LOG_DESTROYED", true, item.GetName(), WizardColors.GetHex(item.GetWizardOwner())));
                    string heroOrUnitDied = BattleHUD.GetHeroOrUnitDied(item);
                    combatLogDetail = new CombatLogDetail
                    {
                        arrow = CombatLogDetail.Arrow.None,
                        left = ((num2 == 0) ? heroOrUnitDied : ""),
                        right = ((num2 == 0) ? "" : heroOrUnitDied)
                    };
                    if (flag2)
                    {
                        BattleHUD.summary.details.Add(combatLogDetail);
                    }
                    else
                    {
                        BattleHUD.AddDetailToLog(combatLogDetail, sourceOnLeft: true);
                    }
                }
            }
            if (flag2)
            {
                if (BattleHUD.logSkill == null || num != 0)
                {
                    BattleHUD.summary.summary = BattleHUD.GetSpellTitle(causedDamage: false, num);
                    BattleHUD.instance.AddSummaryToLog(BattleHUD.summary);
                }
            }
            else if (!flag)
            {
                BattleHUD.CombatLogAdd(BattleHUD.GetSpellTitle(causedDamage: false));
            }
            foreach (string item2 in list)
            {
                BattleHUD.CombatLogAdd(item2);
            }
        }

        public static void StoreUnitInfo()
        {
            BattleHUD.logSpellDamage.Clear();
            BattleHUD.logStoredUnitInfo.Clear();
            MHEventSystem.RegisterListener<Unit>(OnUnitEvent, BattleHUD.instance);
            foreach (BattleUnit allUnit in Battle.GetBattle().GetAllUnits())
            {
                UnitInfo unitInfo = new UnitInfo(allUnit);
                BattleHUD.logStoredUnitInfo.Add(unitInfo.id, unitInfo);
            }
        }
    }
}
