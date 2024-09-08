using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class HUD : ScreenBase
    {
        private static HUD instance;

        public ArmyGrid armyGrid;

        public GridItemManager tasksGrid;

        public GridItemManager notificationsGrid;

        public GridItemManager globalsGrid;

        public Button btEndTurn;

        public Button btEndTurnMove;

        public Button btEndTurnCast;

        public Button btEndTurnResearch;

        public Button btMyrror;

        public Button btArcanus;

        public Button btZoomIn;

        public Button btZoomOut;

        public Button btMove;

        public Toggle tgGuard;

        public Toggle tgSkip;

        public Button btBuild;

        public Button btPlaneTravel;

        public Button btResearch;

        public Button btCast;

        public Button btNextArmy;

        public Button btPrevArmy;

        public Button buttonBugReport;

        public Button btBigMap;

        public Toggle tgSurveyor;

        public Toggle tgGlobals;

        public Toggle tgResources;

        public TextMeshProUGUI coins;

        public TextMeshProUGUI mana;

        public TextMeshProUGUI food;

        public TextMeshProUGUI fame;

        public TextMeshProUGUI turn;

        public TextMeshProUGUI researchTurnsLeft;

        public TextMeshProUGUI castingTurnsLeft;

        public TextMeshProUGUI minimapMP;

        public TextMeshProUGUI spellName;

        public TextMeshProUGUI newTurnInfo;

        public TextMeshProUGUI dateInfo;

        public TextMeshProUGUI spellTarget;

        public TextMeshProUGUI spellTargetDetails;

        public TextMeshProUGUI armySize;

        public TextMeshProUGUI loseCountdown;

        public TextMeshProUGUI globalsCount;

        public TopStatsPanel topStatsPanel;

        public Image imgResearchDone;

        public Image imgResearchAfterThisTurn;

        public Image imgCastingDone;

        public Image imgCastingAfterThisTurn;

        public RawImage riResearchIcon;

        public RawImage riCastingIcon;

        public RawImage riSpellImage;

        public GameObject movementWalking;

        public GameObject movementySwimming;

        public GameObject movementFlying;

        public GameObject armyInfo;

        public GameObject goCasting;

        public GameObject goNotifications;

        public GameObject goAllCitiesLost;

        public GameObject goRoadPlanningPrompt;

        public GameObject goGlobalsArrowLeft;

        public GameObject goGlobalsArrowRight;

        public Animator endTurnReason;

        public Animator animNewTurnNotification;

        public Animator animEnemyTurnNotification;

        public Tutorial_Generic tutorialNoDefenders;

        public Tutorial_Generic tutorialHUD2;

        public Tutorial_Generic tutorialFortressLost;

        public Tutorial_Generic tutorialAlchemy;

        public Tutorial_Generic tutorialNodes;

        private int lastGroupHash;

        private bool endingTurn;

        public RawImage minimap;

        public bool hudUpdateBreak;

        public static HUD Get()
        {
            return HUD.instance;
        }

        protected override void Awake()
        {
            base.Awake();
            TurnManager.Get();
            this.PlaneChanged();
            MHEventSystem.RegisterListener<World>(WorldEvents, this);
            MHEventSystem.RegisterListener<TurnManager>(NewTurn, this);
            MHEventSystem.RegisterListener<TerrainMarkers>(GroupChanged, null);
            MHEventSystem.RegisterListener<FSMSelectionManager>(GroupChanged, this);
            MHEventSystem.RegisterListener<Unit>(UnitChanged, this);
            MHEventSystem.RegisterListener<Group>(GroupMoved, this);
            MHEventSystem.RegisterListener<Artefact>(ArtefactSmashed, this);
            this.SetCasting(null);
        }

        protected override void Start()
        {
            base.Start();
            MinimapManager.Get().SetMinimap(this.minimap);
            bool flag = MinimapManager.Get().InitializeZoom();
            this.MinimapInMode(!flag);
            this.goNotifications.SetActive(TurnManager.Get().playerTurn);
            this.animEnemyTurnNotification.SetBool("Show", !TurnManager.Get().playerTurn);
            this.tgGlobals.isOn = GameManager.Get().showGlobalsOnHUD;
            this.tgResources.isOn = GameManager.Get().showResourcesOnHUD;
            this.goGlobalsArrowLeft.SetActive(GameManager.Get().showGlobalsOnHUD);
            this.goGlobalsArrowRight.SetActive(!GameManager.Get().showGlobalsOnHUD);
            this.globalsGrid.gameObject.SetActive(GameManager.Get().showGlobalsOnHUD);
            this.globalsGrid.CustomDynamicItem(GlobalEnchItem);
        }

        private void SummaryItem(GameObject itemSource, object source, object data, int index)
        {
            TownTaskItem component = itemSource.GetComponent<TownTaskItem>();
            SummaryInfo info = source as SummaryInfo;
            component.riCurrentTask.texture = AssetManager.Get<Texture2D>(info.graphic);
            component.labelName.text = global::DBUtils.Localization.Get(info.title, true);
            component.labelBuildTime.text = info.dataInfo.ToInt().ToString();
            Button component2 = itemSource.GetComponent<Button>();
            if (component2 != null)
            {
                component2.onClick.RemoveAllListeners();
                component2.onClick.AddListener(delegate
                {
                    info.Activate();
                });
            }
            component2.interactable = !TurnManager.Get().endTurn;
        }

        public override void UpdateState()
        {
            base.UpdateState();
            IPlanePosition selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
            if (selectedGroup != null && selectedGroup is IHashableGroup && (selectedGroup as IHashableGroup).GetHash() != this.lastGroupHash)
            {
                this.UpdateSelectedUnit();
            }
            if (SettingsBlock.IsKeyUp(Settings.KeyActions.UI_NEXT_UNIT) && UIManager.IsTopForInput(this))
            {
                this.NextPlayersArmyWithoutOrders();
            }
        }

        public void UpdateHUD()
        {
            int turnNumber = TurnManager.GetTurnNumber();
            int allCitiesLostTurn = GameManager.GetHumanWizard().allCitiesLostTurn;
            this.turn.text = Stats.CalculateDate(turnNumber) + ", " + global::DBUtils.Localization.Get("UI_TURN", true) + " " + turnNumber;
            this.goAllCitiesLost.SetActive(allCitiesLostTurn > 0 && GameManager.GetWizardTownCount(PlayerWizard.HumanID()) == 0);
            this.loseCountdown.text = (10 - (turnNumber - allCitiesLostTurn)).ToString();
            this.UpdateTopLeft();
            this.UpdateResearchButton();
            this.UpdateCastingButton();
            this.UpdateSummaryInfoGrid();
            this.UpdateNotificationGrid();
            this.UpdateGlobalsGrid();
            this.UpdateEndTurnButtons();
        }

        public void UpdateTopLeft()
        {
            this.topStatsPanel.Set(GameManager.GetHumanWizard());
        }

        public void UpdateSummaryInfoGrid()
        {
            List<SummaryInfo> summaryInfo = GameManager.GetHumanWizard().GetSummaryInfo();
            this.tasksGrid.UpdateGrid(summaryInfo);
        }

        public void UpdateNotificationGrid()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            this.notificationsGrid.UpdateListItems(humanWizard.GetNotifications());
        }

        public void UpdateGlobalsGrid()
        {
            List<EnchantmentInstance> list = new List<EnchantmentInstance>();
            List<EnchantmentInstance> enchantments = GameManager.Get().GetEnchantments();
            List<EnchantmentInstance> enchantments2 = GameManager.GetHumanWizard().GetEnchantments();
            foreach (EnchantmentInstance item in ListUtils.MultiEnumerable(enchantments, enchantments2))
            {
                if (!item.source.Get().hideEnch)
                {
                    list.Add(item);
                }
            }
            this.globalsCount.text = "(" + list.Count + ")";
            this.globalsGrid.UpdateGrid(list);
        }

        public void PulseNotifications()
        {
            ButtonWithPulse[] componentsInChildren = this.notificationsGrid.GetComponentsInChildren<ButtonWithPulse>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Pulse();
            }
        }

        private void WorldEvents(object sender, object e)
        {
            if (e is global::WorldCode.Plane)
            {
                this.PlaneChanged();
            }
        }

        private void NewTurn(object sender, object e)
        {
            if (e as string == "Turn")
            {
                this.UpdateHUD();
            }
            this.animNewTurnNotification.SetTrigger("NextTurn");
            this.animEnemyTurnNotification.SetBool("Show", value: false);
            this.newTurnInfo.text = global::DBUtils.Localization.Get("UI_TURN", true) + " " + TurnManager.GetTurnNumber();
            this.dateInfo.text = Stats.CalculateDate(TurnManager.GetTurnNumber());
            this.goNotifications.SetActive(TurnManager.Get().playerTurn);
            this.ConsiderTutorials();
        }

        private void ArtefactSmashed(object sender, object e)
        {
            if (e.ToString() == "Smashed")
            {
                this.UpdateTopLeft();
            }
        }

        public void ConsiderTutorials()
        {
            if ((bool)this.tutorialNoDefenders && this.tutorialNoDefenders.WouldShow())
            {
                List<Location> registeredLocations = GameManager.Get().registeredLocations;
                if (registeredLocations == null)
                {
                    return;
                }
                int num = PlayerWizard.HumanID();
                foreach (Location item in registeredLocations)
                {
                    if (item.owner == num && item is TownLocation townLocation && townLocation.turnsUndefended >= 15)
                    {
                        this.tutorialNoDefenders.OpenIfNotSeen(this);
                        break;
                    }
                }
            }
            if (TurnManager.GetTurnNumber() == 2)
            {
                this.tutorialHUD2.OpenIfNotSeen(this);
            }
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            if (((humanWizard.money < 20 && humanWizard.GetMana() >= 100) || (humanWizard.money >= 100 && humanWizard.GetMana() < 20)) && TurnManager.GetTurnNumber() > 20)
            {
                this.tutorialAlchemy.OpenIfNotSeen(this);
            }
        }

        private void PlaneChanged()
        {
            bool active = World.GetActivePlane() != World.GetArcanus();
            bool active2 = World.GetActivePlane() != World.GetMyrror();
            this.btArcanus.gameObject.SetActive(active);
            this.btMyrror.gameObject.SetActive(active2);
        }

        public override IEnumerator PostClose()
        {
            yield return base.PostClose();
            HUD.instance = null;
        }

        public override IEnumerator PreStart()
        {
            this.armyGrid.onSelectionChange = delegate(ArmyGrid grid)
            {
                List<Unit> selectedUnits = FSMSelectionManager.Get().selectedUnits;
                selectedUnits.Clear();
                foreach (BaseUnit selected in grid.selectedList)
                {
                    selectedUnits.Add(selected as Unit);
                }
                MHEventSystem.TriggerEvent<HUD>(this, null);
                this.UpdateSelectedUnitMP();
            };
            this.tasksGrid.CustomDynamicItem(SummaryItem);
            this.notificationsGrid.SetListItems<SummaryInfo>();
            this.UpdateSelectedUnit();
            this.UpdateHUD();
            yield return base.PreStart();
            HUD.instance = this;
        }

        public void UpdateSelectedUnit()
        {
            this.UpdateSelectedUnitMP();
            IPlanePosition selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
            IGroup group = selectedGroup as IGroup;
            if (selectedGroup == null || group == null)
            {
                this.lastGroupHash = 0;
                this.ControlButtonsOff();
                this.armyGrid.SetUnits(null);
                this.btNextArmy.interactable = false;
                this.btPrevArmy.interactable = false;
                return;
            }
            this.lastGroupHash = (selectedGroup as IHashableGroup).GetHash();
            if (selectedGroup is Group && (selectedGroup as Group).GetOwnerID() == PlayerWizard.HumanID())
            {
                this.ControlButtonsUpdate();
            }
            else
            {
                this.ControlButtonsOff();
            }
            this.armyGrid.SetUnits(group?.GetUnits()?.FindAll((Reference<Unit> o) => o.Get().GetWizardOwnerID() == PlayerWizard.HumanID() || !o.Get().IsInvisibleUnit()));
            int num = 0;
            num = group.GetOwnerID();
            bool interactable = false;
            if (num == PlayerWizard.HumanID())
            {
                interactable = (from o in GameManager.GetGroupsOfWizard(num)
                    where o.groupUnits.Count > 0
                    select o).ToList().Count > 1;
            }
            this.btNextArmy.interactable = interactable;
            this.btPrevArmy.interactable = interactable;
            this.armySize.text = (group?.GetUnits()?.Count).GetValueOrDefault() + "/9";
        }

        private void ControlButtonsOff()
        {
            this.btMove.interactable = false;
            this.tgGuard.interactable = false;
            this.tgSkip.interactable = false;
            this.btBuild.interactable = false;
            this.btPlaneTravel.interactable = false;
            this.tgGuard.SetIsOnWithoutNotify(value: false);
            this.tgSkip.SetIsOnWithoutNotify(value: false);
        }

        private void ControlButtonsUpdate()
        {
            if (!(FSMSelectionManager.Get().GetSelectedGroup() is Group group) || group.GetUnits().Count() == 0)
            {
                this.ControlButtonsOff();
                return;
            }
            this.btMove.interactable = group.CurentMP() > 0 && group.destination != Vector3i.invalid;
            this.tgGuard.interactable = true;
            this.tgSkip.interactable = group.CurentMP() > 0;
            this.btBuild.interactable = PopupSpecialActions.AnyActionPossible(group);
            this.tgGuard.SetIsOnWithoutNotify(group.Action == Group.GroupActions.Guard);
            this.tgSkip.SetIsOnWithoutNotify(group.Action == Group.GroupActions.Skip);
            Location locationAt = GameManager.Get().GetLocationAt(group.GetPosition(), group.GetPlane());
            if (locationAt != null && locationAt.locationType == ELocationType.PlaneTower)
            {
                this.btPlaneTravel.interactable = true;
            }
            else if (locationAt != null && locationAt is TownLocation && locationAt.GetOwnerID() == group.GetOwnerID() && locationAt.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.ASTRAL_GATE) != null)
            {
                this.btPlaneTravel.interactable = true;
            }
            else if (GameManager.Get().GetLocationAt(group.GetPosition(), World.GetOtherPlane(group.GetPlane())) != null)
            {
                Location locationAt2 = GameManager.Get().GetLocationAt(group.GetPosition(), World.GetOtherPlane(group.GetPlane()));
                if (locationAt2 != null && locationAt2 is TownLocation && locationAt2.GetOwnerID() == group.GetOwnerID() && (locationAt2.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.ASTRAL_GATE) != null || group.GetUnits().Exists((Reference<Unit> u) => u.Get().GetAttributes().Contains(TAG.PLANE_SHIFTER))))
                {
                    this.btPlaneTravel.interactable = true;
                }
                else
                {
                    this.btPlaneTravel.interactable = false;
                }
            }
            else if (group.GetUnits().Exists((Reference<Unit> u) => u.Get().GetAttributes().Contains(TAG.PLANE_SHIFTER)))
            {
                this.btPlaneTravel.interactable = true;
            }
            else
            {
                this.btPlaneTravel.interactable = false;
            }
        }

        public void UpdateResearchButton()
        {
            MagicAndResearch magicAndResearch = GameManager.GetHumanWizard().GetMagicAndResearch();
            magicAndResearch.GetResearchProgress(out var curentStatus, out var nextTurnStatus, out var turnsLeft);
            this.imgResearchDone.fillAmount = curentStatus;
            this.imgResearchAfterThisTurn.fillAmount = nextTurnStatus;
            RolloverSimpleTooltip orAddComponent = this.btResearch.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.sourceAsDbName = null;
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor.x = 0.4f;
            orAddComponent.anchor.y = 1.1f;
            if (magicAndResearch.curentlyResearched == null)
            {
                this.riResearchIcon.texture = UIReferences.GetTransparent();
                this.researchTurnsLeft.text = "-";
                orAddComponent.title = "UI_RESEARCH_SPELLS2";
                orAddComponent.anchor.x = 0.3f;
            }
            else
            {
                Spell spell = magicAndResearch.curentlyResearched.Get();
                if (turnsLeft > 99)
                {
                    this.researchTurnsLeft.text = "99+";
                }
                else if (GameManager.GetHumanWizard().CalculateResearchIncome() == 0)
                {
                    this.researchTurnsLeft.text = "∞";
                }
                else if (turnsLeft < 0)
                {
                    this.researchTurnsLeft.text = "1";
                }
                else
                {
                    this.researchTurnsLeft.text = turnsLeft.ToString();
                }
                this.riResearchIcon.texture = spell.GetDescriptionInfo().GetTexture();
                orAddComponent.sourceAsDbName = magicAndResearch.curentlyResearched.dbName;
            }
            this.btResearch.interactable = FSMCoreGame.Get().CanResearch();
        }

        public void UpdateCastingButton()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            MagicAndResearch magicAndResearch = humanWizard.GetMagicAndResearch();
            magicAndResearch.GetCastingProgress(out var curentStatus, out var nextTurnStatus, out var _);
            this.imgCastingDone.fillAmount = curentStatus;
            this.imgCastingAfterThisTurn.fillAmount = nextTurnStatus;
            RolloverSimpleTooltip orAddComponent = this.btCast.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor.x = 0.4f;
            orAddComponent.anchor.y = 1.1f;
            SummaryInfo castingSummaryInfo = humanWizard.GetCastingSummaryInfo();
            if (magicAndResearch.curentlyCastSpell == null)
            {
                this.btCast.interactable = true;
                this.riCastingIcon.texture = UIReferences.GetTransparent();
                this.castingTurnsLeft.text = "-";
                orAddComponent.sourceAsDbName = null;
                if (castingSummaryInfo != null && castingSummaryInfo.spell != null)
                {
                    this.btCast.interactable = false;
                    orAddComponent.title = "UI_SPELL_READY";
                    orAddComponent.description = "UI_SPELL_READY_DES";
                    orAddComponent.anchor.x = 0.35f;
                }
                else
                {
                    orAddComponent.title = "UI_CAST_SPELLS2";
                    orAddComponent.description = null;
                    orAddComponent.anchor.x = 0.3f;
                }
            }
            else
            {
                this.btCast.interactable = true;
                Spell curentlyCastSpell = magicAndResearch.curentlyCastSpell;
                this.castingTurnsLeft.text = curentlyCastSpell.GetCastingTurns(humanWizard);
                this.riCastingIcon.texture = curentlyCastSpell.GetDescriptionInfo().GetTexture();
                orAddComponent.title = null;
                orAddComponent.description = null;
                orAddComponent.sourceAsDbName = magicAndResearch.curentlyCastSpell.dbName;
            }
        }

        public void GroupChanged(object sender, object e)
        {
            this.UpdateSelectedUnit();
        }

        public void UnitChanged(object sender, object e)
        {
            if (TurnManager.Get().playerTurn)
            {
                this.UpdateTopLeft();
            }
        }

        public void UpdateSelectedUnitMP()
        {
            IPlanePosition selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
            if (selectedGroup == null || selectedGroup is Location)
            {
                this.armyInfo.SetActive(value: false);
            }
            else if (selectedGroup is Group)
            {
                this.armyInfo.SetActive(value: true);
                Group group = selectedGroup as Group;
                this.minimapMP.text = group.CurentMP().ToString(1) + "/" + group.GetMaxMP();
                this.movementWalking.SetActive(group.AllUnitsHasTag(TAG.CAN_WALK));
                this.movementySwimming.SetActive(group.AllUnitsHasTag(TAG.CAN_SWIM));
                this.movementFlying.SetActive(group.AllUnitsHasTag(TAG.CAN_FLY) || group.AllUnitsHasTag(TAG.WIND_WALKING));
            }
            else
            {
                Debug.LogError("unknown group type on hud");
            }
        }

        public void OpenResearch()
        {
            FSMCoreGame.Get().HudButton("ButtonResearch");
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (FSMCoreGame.Get().HudButton(s.name))
            {
                HUD.SwitchSurveyor();
                return;
            }
            if (s == this.tgGuard || s == this.tgSkip)
            {
                HUD.SwitchSurveyor();
                Group group = FSMSelectionManager.Get().GetSelectedGroup() as Group;
                Group.GroupActions groupActions = ((s == this.tgGuard) ? Group.GroupActions.Guard : Group.GroupActions.Skip);
                if (groupActions == group.Action)
                {
                    group.Action = Group.GroupActions.None;
                }
                else
                {
                    group.Action = groupActions;
                    group.destination = Vector3i.invalid;
                    group.engineerManager?.Destroy();
                    group.purificationManager?.Destroy();
                    this.NextPlayersArmyWithoutOrders();
                }
                this.ControlButtonsUpdate();
            }
            if (s == this.btMove)
            {
                HUD.SwitchSurveyor();
                IPlanePosition selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
                Group group2 = selectedGroup as Group;
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(group2.GetPlane(), group2.GetPosition(), group2.destination, group2);
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                FSMSelectionManager.Get().Select(selectedGroup, focus: true);
                group2.MoveViaPath(path, mergeCollidedAlliedGroups: true);
                MHEventSystem.TriggerEvent<FSMMapGame>(selectedGroup, null);
                this.ControlButtonsUpdate();
            }
            if (s == this.btNextArmy)
            {
                HUD.SwitchSurveyor();
                this.NextArmy();
            }
            else if (s == this.btPrevArmy)
            {
                HUD.SwitchSurveyor();
                this.NextArmy(forward: false);
            }
            else if (s == this.btEndTurnResearch)
            {
                HUD.SwitchSurveyor();
                this.ExecuteEndTurnResearch();
            }
            else if (s == this.btEndTurnMove)
            {
                HUD.SwitchSurveyor();
                base.StartCoroutine(this.ExecuteEndTurnMoveUnitsWrapped());
            }
            else if (s == this.btEndTurnCast)
            {
                HUD.SwitchSurveyor();
                this.ExecuteEndTurnCastSpell();
            }
            else if (s == this.btEndTurn)
            {
                HUD.SwitchSurveyor();
                this.AttemptToEndTurn();
            }
            else if (s == this.btMyrror)
            {
                World.ActivatePlane(World.GetMyrror());
            }
            else if (s == this.btArcanus)
            {
                World.ActivatePlane(World.GetArcanus());
            }
            else if (s == this.btPlaneTravel)
            {
                HUD.SwitchSurveyor();
                IPlanePosition selectedGroup2 = FSMSelectionManager.Get().GetSelectedGroup();
                Group gr = selectedGroup2 as Group;
                if (gr == null)
                {
                    return;
                }
                Debug.Log("Plane: " + gr.GetPlane().planeSource.dbName + ", position: " + gr.GetPosition().ToString());
                Location location = GameManager.GetLocationsOfThePlane(gr.GetPlane()).Find((Location o) => o.GetPosition() == gr.GetPosition());
                if (location != null && location.locationType == ELocationType.PlaneTower)
                {
                    if (gr.IsSwitchPlaneDestinationValid())
                    {
                        gr.PlaneSwitch();
                    }
                    return;
                }
                if (location != null && location is TownLocation && location.GetOwnerID() == gr.GetOwnerID() && location.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.ASTRAL_GATE) != null)
                {
                    List<Unit> selectedUnits = FSMSelectionManager.Get().selectedUnits;
                    if (selectedUnits != null && selectedUnits.Count > 0 && selectedUnits.Count < gr.GetUnits().Count)
                    {
                        if (gr.IsSwitchPlaneDestinationValid(selectedUnits))
                        {
                            gr.PlaneSwitch(selectedUnits);
                        }
                    }
                    else if (gr.IsSwitchPlaneDestinationValid())
                    {
                        gr.PlaneSwitch();
                    }
                    return;
                }
                if (GameManager.Get().GetLocationAt(gr.GetPosition(), World.GetOtherPlane(gr.GetPlane())) != null)
                {
                    Location locationAt = GameManager.Get().GetLocationAt(gr.GetPosition(), World.GetOtherPlane(gr.GetPlane()));
                    if (locationAt != null && locationAt.GetOwnerID() == gr.GetOwnerID() && (locationAt.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.ASTRAL_GATE) != null || gr.GetUnits().Exists((Reference<Unit> u) => u.Get().GetAttributes().Contains(TAG.PLANE_SHIFTER))) && gr.IsSwitchPlaneDestinationValid())
                    {
                        gr.PlaneSwitch();
                    }
                    return;
                }
                List<Reference<Unit>> list = gr.GetUnits().FindAll((Reference<Unit> o) => o.Get().GetAttributes().Contains(TAG.PLANE_SHIFTER));
                if (gr.transporter != null)
                {
                    if (gr.IsSwitchPlaneDestinationValid())
                    {
                        gr.PlaneSwitch();
                    }
                    return;
                }
                List<Unit> i = new List<Unit>();
                list.ForEach(delegate(Reference<Unit> o)
                {
                    i.Add(o.Get());
                });
                if (gr.IsSwitchPlaneDestinationValid(i))
                {
                    gr.PlaneSwitch(i);
                }
            }
            else if (s == this.btBuild)
            {
                HUD.SwitchSurveyor();
                if (FSMSelectionManager.Get().GetSelectedGroup() is Group g)
                {
                    PopupSpecialActions.OpenPopup(this, g);
                }
            }
            else if (s == this.tgSurveyor)
            {
                if (!this.tgSurveyor.isOn)
                {
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                }
            }
            else if (s == this.tgGlobals)
            {
                this.globalsGrid.gameObject.SetActive(this.tgGlobals.isOn);
                this.goGlobalsArrowLeft.gameObject.SetActive(this.tgGlobals.isOn);
                this.goGlobalsArrowRight.gameObject.SetActive(!this.tgGlobals.isOn);
                GameManager.Get().showGlobalsOnHUD = this.tgGlobals.isOn;
            }
            else if (s == this.tgResources)
            {
                GameManager.Get().showResourcesOnHUD = this.tgResources.isOn;
            }
            else if (s == this.btBigMap)
            {
                UIManager.Open<BigMap>(UIManager.Layer.Standard);
            }
            else if (s == this.btZoomIn)
            {
                MinimapManager.Get().ChangeZoom(2.5f);
            }
            else if (s == this.btZoomOut)
            {
                MinimapManager.Get().ChangeZoom(0.9f);
            }
            else if (s == this.buttonBugReport)
            {
                base.StartCoroutine(BugReportCatcher.OpenBugCatcher());
            }
        }

        public void MinimapInMode(bool zoomedIn)
        {
            this.btZoomIn.gameObject.SetActive(!zoomedIn);
            this.btZoomOut.gameObject.SetActive(zoomedIn);
        }

        private void GroupMoved(object sender, object e)
        {
            if (sender is Group group && group.GetOwnerID() == PlayerWizard.HumanID() && e != null && e is List<Vector3i>)
            {
                this.UpdateEndTurnButtons();
            }
        }

        public void UpdateEndTurnButtons()
        {
            this.btEndTurnResearch.gameObject.SetActive(value: false);
            this.btEndTurnMove.gameObject.SetActive(value: false);
            this.btEndTurnCast.gameObject.SetActive(value: false);
            this.btEndTurn.gameObject.SetActive(value: false);
            if (TurnManager.Get().playerTurn)
            {
                if (this.GetStatusEndTurnResearch())
                {
                    this.btEndTurnResearch.gameObject.SetActive(value: true);
                    return;
                }
                if (this.GetStatusEndTurnMoveUnits())
                {
                    this.btEndTurnMove.gameObject.SetActive(value: true);
                    return;
                }
                if (this.GetStatusEndTurnCastSpell())
                {
                    this.btEndTurnCast.gameObject.SetActive(value: true);
                    return;
                }
                this.btEndTurn.gameObject.SetActive(value: true);
                this.btEndTurn.interactable = TurnManager.Get().playerTurn;
            }
            else
            {
                this.btEndTurn.gameObject.SetActive(value: true);
                this.btEndTurn.interactable = false;
            }
        }

        private void AttemptToEndTurn()
        {
            if (!this.endingTurn)
            {
                base.StartCoroutine(this.DoAttemptEndTurn());
            }
        }

        private IEnumerator DoAttemptEndTurn()
        {
            this.endingTurn = true;
            PlayerWizard w = GameManager.GetHumanWizard();
            int num = w.money + w.CalculateMoneyIncome(includeUpkeep: true);
            int num2 = w.GetMana() + w.CalculateManaIncome(includeUpkeep: true);
            int num3 = w.CalculateFoodIncome(includeUpkeep: true);
            if (num < 0)
            {
                PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_MONEY_UPKEEP_FAILING", "UI_END_TURN", delegate
                {
                    TurnManager.EndTurn();
                }, "UI_CANCEL");
            }
            else if (num2 < 0)
            {
                PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_MANA_UPKEEP_FAILING", "UI_END_TURN", delegate
                {
                    TurnManager.EndTurn();
                }, "UI_CANCEL");
            }
            else if (num3 < 0)
            {
                PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_FOOD_UPKEEP_FAILING", "UI_END_TURN", delegate
                {
                    TurnManager.EndTurn();
                }, "UI_CANCEL");
            }
            else if (this.GetStatusEndTurnResearch())
            {
                this.ExecuteEndTurnResearch();
            }
            else if (w.ClearNotifications() != null)
            {
                SummaryInfo summaryInfo = w.ClearNotifications();
                this.EndTurnReason(summaryInfo.GetEndTurnReason());
                this.PulseNotifications();
            }
            else
            {
                yield return this.ExecuteEndTurnMoveUnits();
                if (!w.AnyGroupsWithMPLeft())
                {
                    TurnManager.EndTurn();
                    this.UpdateEndTurnButtons();
                }
            }
            this.endingTurn = false;
        }

        public bool GetStatusEndTurnCastSpell()
        {
            return GameManager.GetHumanWizard().GetCastingSummaryInfo() != null;
        }

        public void ExecuteEndTurnCastSpell()
        {
            GameManager.GetHumanWizard().GetCastingSummaryInfo().Activate();
            this.UpdateEndTurnButtons();
        }

        public bool GetStatusEndTurnResearch()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            if (humanWizard.GetMagicAndResearch().curentlyResearched == null && humanWizard.GetMagicAndResearch().curentResearchOptions.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void ExecuteEndTurnResearch()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            HUD.Get().OpenResearch();
            humanWizard.RemoveNotificationOfType(SummaryInfo.SummaryType.eResearchAvailiable);
        }

        public bool GetStatusEndTurnMoveUnits()
        {
            foreach (Group item in GameManager.GetGroupsOfWizard(PlayerWizard.HumanID()))
            {
                if (item.alive && item.CurentMP() > 0 && item.Action == Group.GroupActions.None && item.engineerManager == null && item.purificationManager == null)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator ExecuteEndTurnMoveUnitsWrapped()
        {
            if (!this.endingTurn)
            {
                this.endingTurn = true;
                yield return this.ExecuteEndTurnMoveUnits();
                this.endingTurn = false;
            }
        }

        public IEnumerator ExecuteEndTurnMoveUnits()
        {
            PlayerWizard w = GameManager.GetHumanWizard();
            yield return w.AdvanceWorldWorks();
            bool num = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
            bool flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
            if (num || flag)
            {
                FSMSelectionManager.Get().Select(null, focus: false);
                int timeout = 5;
                for (int i = 1; i <= timeout; i++)
                {
                    bool num2 = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
                    flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
                    if (!(num2 || flag))
                    {
                        break;
                    }
                    if (i == timeout)
                    {
                        yield break;
                    }
                    yield return null;
                }
            }
            yield return w.FocusOnGroupWithMPLeft();
            this.UpdateEndTurnButtons();
        }

        public void EndTurnReason(string reason)
        {
            this.endTurnReason.gameObject.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = global::DBUtils.Localization.Get(reason, true);
            this.endTurnReason.SetTrigger("Show");
        }

        public void Hide()
        {
            if (base.gameObject.activeSelf)
            {
                base.gameObject.SetActive(value: false);
            }
        }

        public void Show()
        {
            if (!base.gameObject.activeSelf)
            {
                base.gameObject.SetActive(value: true);
                this.UpdateTopLeft();
                this.UpdateSelectedUnit();
                base.cGroup.alpha = 1f;
                this.animEnemyTurnNotification.SetBool("Show", !TurnManager.Get().playerTurn);
                this.goNotifications.SetActive(TurnManager.Get().playerTurn);
            }
        }

        public void SetCasting(Spell s)
        {
            if (s == null)
            {
                this.goCasting.SetActive(value: false);
                this.UpdateEndTurnButtons();
                return;
            }
            this.goCasting.SetActive(value: true);
            this.riSpellImage.texture = s.GetDescriptionInfo().GetTexture();
            this.spellName.text = s.GetDescriptionInfo().GetLocalizedName();
            this.spellTarget.text = global::DBUtils.Localization.Get("UI_TARGET", true) + " " + s.targetType.desType.GetLocalizedTargetTypeDescription();
            this.spellTargetDetails.text = s.invalidTarget.GetLocalizedTargetTypeDescription();
        }

        public void Update()
        {
            if (GameManager.Get() == null || GameManager.Get().duringSaveReloading)
            {
                return;
            }
            if (SettingsBlock.IsKeyUp(Settings.KeyActions.UI_QUICK_SAVE) && UIManager.IsTopForInput(this))
            {
                if (GameManager.Get().IsFocusFree())
                {
                    FSMSelectionManager.Get().Select(null, focus: false);
                    string pROFILES = MHApplication.PROFILES;
                    if (!Directory.Exists(pROFILES))
                    {
                        Directory.CreateDirectory(pROFILES);
                    }
                    if (SaveManager.SaveGame(World.Get().seed, "quick_save") != null)
                    {
                        PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_QUICK_SAVE_FAILED", "UI_OK");
                    }
                    else
                    {
                        PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_QUICK_SAVE_SUCCESS", "UI_OK");
                    }
                }
                return;
            }
            if (SettingsBlock.IsKeyUp(Settings.KeyActions.UI_QUICK_LOAD) && UIManager.IsTopForInput(this))
            {
                if (!GameManager.Get().IsFocusFree() || !TurnManager.Get().playerTurn)
                {
                    return;
                }
                this.tgSurveyor.isOn = false;
                FSMSelectionManager.Get().Select(null, focus: false);
                if (Directory.Exists(MHApplication.PROFILES))
                {
                    SaveMeta saveMeta = SaveManager.GetAvaliableSaves().Find((SaveMeta o) => o.worldSeed == World.Get().seed && o.saveName == "quick_save");
                    if (saveMeta != null)
                    {
                        Debug.Log("Loading quick save");
                        GameManager.Get().duringSaveReloading = true;
                        DataBase.UpdateUse(saveMeta.dlc);
                        SaveGame.LoadSavedGame(saveMeta);
                    }
                    else
                    {
                        PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_THIS_GAME_LACK_QUICK_SAVE", "UI_OK");
                    }
                }
                return;
            }
            TooltipSurveyor.DoUpdate(this.tgSurveyor.isOn);
            if (this.tgSurveyor.isOn)
            {
                IPlanePosition selectedGroup = FSMSelectionManager.Get().GetSelectedGroup();
                if (selectedGroup != null && selectedGroup is Group group && group.GetUnits().Find((Reference<Unit> o) => o.Get().IsSettler()) != null)
                {
                    global::WorldCode.Plane plane = group.GetPlane();
                    if (plane.settlerDataTexture == null)
                    {
                        plane.settlerDataTexture = new Texture2D(plane.area.AreaWidth, plane.area.AreaHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
                        plane.settlerDataTexture.filterMode = FilterMode.Point;
                    }
                    if (!plane.isSettlerDataReady)
                    {
                        plane.isSettlerDataReady = true;
                        V3iRect area = plane.area;
                        List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(plane);
                        List<Vector3i> list = new List<Vector3i>();
                        foreach (Location item in locationsOfThePlane)
                        {
                            if (item is TownLocation)
                            {
                                list.Add(item.GetPosition());
                            }
                        }
                        Dictionary<Vector3i, Hex> hexes = plane.GetHexes();
                        foreach (Vector3i landHex in plane.GetLandHexes())
                        {
                            int x = (landHex.x + area.AreaWidth) % area.AreaWidth;
                            int y = (landHex.y + area.AreaHeight) % area.AreaHeight;
                            plane.GetHexAt(landHex);
                            int num = DataHeatMaps.Get(plane).SettlementValue(hexes, list, landHex);
                            plane.settlerDataTexture.SetPixel(x, y, new Color((float)num / 255f, 0f, 0f));
                        }
                        plane.settlerDataTexture.Apply();
                    }
                    plane.SetSettlerDataTo(active: true);
                }
                else
                {
                    World.GetActivePlane()?.SetSettlerDataTo(active: false);
                }
            }
            else
            {
                World.GetActivePlane()?.SetSettlerDataTo(active: false);
            }
        }

        public static void SwitchSurveyor(bool on = false)
        {
            HUD hUD = HUD.Get();
            if (hUD != null)
            {
                hUD.tgSurveyor.isOn = on;
            }
        }

        private void OnDisable()
        {
            TooltipSurveyor.DoUpdate(openAllowed: false);
            this.endingTurn = false;
        }

        private bool NextArmy(bool forward = true, bool withoutOrders = false)
        {
            Group curGroup = FSMSelectionManager.Get().GetSelectedGroup() as Group;
            List<Group> list = (from o in GameManager.GetGroupsOfWizard(PlayerWizard.HumanID())
                where o.alive && o.groupUnits.Count > 0
                select o).ToList();
            if (withoutOrders)
            {
                list = list.FindAll((Group o) => !o.isActivelyBuilding && o.CurentMP() > 0 && o.Action == Group.GroupActions.None);
            }
            list.Sort(delegate(Group a, Group b)
            {
                int num2 = a.GetPlane().arcanusType.CompareTo(b.GetPlane().arcanusType);
                if (num2 != 0)
                {
                    return num2;
                }
                int num3 = ((a.beforeMovingAway != null) ? a.beforeMovingAway.ID : a.GetID());
                int value = ((b.beforeMovingAway != null) ? b.beforeMovingAway.ID : b.GetID());
                return num3.CompareTo(value);
            });
            if (list.Count > 0)
            {
                int num = ((curGroup != null) ? list.FindIndex((Group o) => o == curGroup) : 0);
                num = (num + list.Count + (forward ? 1 : (-1))) % list.Count;
                FSMSelectionManager.Get().Select(list[num], focus: true);
                return true;
            }
            FSMSelectionManager.Get().Select(null, focus: false);
            return false;
        }

        public bool NextPlayersArmyWithoutOrders()
        {
            return this.NextArmy(forward: true, withoutOrders: true);
        }

        public void OpenBanishedTutorial()
        {
            this.tutorialFortressLost.OpenIfNotSeen(this);
        }

        public void OpenMagicNodeTutorial()
        {
            if (this.tutorialNodes.WouldShow())
            {
                this.tutorialNodes.OpenIfNotSeen(this);
            }
        }

        public void GlobalEnchItem(GameObject itemSource, object source, object data, int index)
        {
            if (source is EnchantmentInstance enchantmentInstance)
            {
                EnchantmentListItem component = itemSource.GetComponent<EnchantmentListItem>();
                component.image.texture = enchantmentInstance.source.Get().GetDescriptionInfo().GetTexture();
                component.Set(enchantmentInstance);
            }
        }
    }
}
