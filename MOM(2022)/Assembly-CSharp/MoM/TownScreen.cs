using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class TownScreen : ScreenBase
    {
        public Button btCloseTownScreen;

        public Button btUseEarthGate;

        public TextMeshProUGUI townName;

        public TextMeshProUGUI townPopulation;

        public TextMeshProUGUI townMaxPopulation;

        public TopStatsPanel topStatsPanel;

        public TextMeshProUGUI raceName;

        public Button btRaze;

        public Button btRename;

        public Button btInfo;

        public Button btCities;

        public Button btArmies;

        public Button btDiplomacy;

        public Button btMagic;

        public Button btGame;

        public Button buttonBugReport;

        public Button btPrevTown;

        public Button btNextTown;

        public EnchantmentGrid gridEnchantments;

        public RawImage raceImage;

        public GameObject capital;

        public GameObject summoningCircle;

        public TextMeshProUGUI taxRate;

        public TextMeshProUGUI townIncome;

        public TextMeshProUGUI townUnrest;

        public Button btIncreaseTax;

        public Button btDecreaseTax;

        public Button btGoToCityManager;

        public DropDownFilters dropdownSort;

        public GridItemManager gridAllTowns;

        public ArmyGrid gridStatoningUnits;

        public TextMeshProUGUI numberOfFarmers;

        public TextMeshProUGUI numberOfWorkers;

        public TextMeshProUGUI numberOfRebels;

        public TextMeshProUGUI numberOfPopulation;

        public TextMeshProUGUI foodIncome;

        public TextMeshProUGUI productionIncome;

        public TextMeshProUGUI unrestRate;

        public TextMeshProUGUI powerIncome;

        public TextMeshProUGUI researchIncome;

        public TextMeshProUGUI goldIncome;

        public TextMeshProUGUI currentQueueItemBuyoutCost;

        public RolloverObject rebelsTooltip;

        public RolloverObject foodTooltip;

        public RolloverObject productionTooltip;

        public RolloverObject unrestTooltip;

        public RolloverObject powerTooltip;

        public RolloverObject researchTooltip;

        public RolloverObject goldTooltip;

        public Button btBuy;

        public Button btManageQueue;

        public Button btIncreaseWorkers;

        public Button btDecreaseWorkers;

        public GridItemManager gridResources;

        public GridItemManager gridQueue;

        public GridItemManager gridTownBuildings;

        public RawImage iconFarmers;

        public RawImage iconWorkers;

        public RawImage iconRebels;

        public Toggle tgShowOnHud;

        public Toggle tgShowProduction;

        public Toggle tgShowShowBuildings;

        public Toggle tgAutomanage;

        public GameObject noResources;

        public GameObject starvation;

        public GameObject constructionManager;

        public GameObject queueTooltipPosition;

        public CraftingListItem currentConstruction;

        public Material buildingHighlightEffect;

        public GridItemManager gridBuildings;

        public GridItemManager gridUnits;

        public GridItemManager gridConstructionQueue;

        public GameObject unitDetails;

        public GameObject buildingDetails;

        public Button btClose;

        public Button btAddUnitToQueue;

        public Button btAddBuildingToQueue;

        public TextMeshProUGUI buildingName;

        public TextMeshProUGUI buildingDescription;

        public TextMeshProUGUI buildingMaintenance;

        public TextMeshProUGUI buildingMaintenanceMana;

        public GridItemManager gridBuildingRequires;

        public GridItemManager gridBuildingAllows;

        public GameObject goldMaintenance;

        public GameObject manaMaintenance;

        public GameObject noMaintenance;

        public RawImage buildingIcon;

        public TextMeshProUGUI unitName;

        public TextMeshProUGUI unitDescription;

        public TextMeshProUGUI melee;

        public TextMeshProUGUI range;

        public TextMeshProUGUI armour;

        public TextMeshProUGUI resist;

        public TextMeshProUGUI figures;

        public TextMeshProUGUI hits;

        public TextMeshProUGUI movementPoints;

        public TextMeshProUGUI goldUpkeepAmount;

        public TextMeshProUGUI foodUpkeepAmount;

        public TextMeshProUGUI manaUpkeepAmount;

        public TextMeshProUGUI ammo;

        public TextMeshProUGUI mana;

        public GridItemManager gridUnitSkills;

        public GridItemManager gridUnitRequires;

        public GameObject movementWalking;

        public GameObject movementFlying;

        public GameObject movementSwimming;

        public GameObject goldUpkeep;

        public GameObject foodUpkeep;

        public GameObject manaUpkeep;

        public GameObject meleeAttackType;

        public GameObject rangeAttackType;

        public RawImage unitIcon;

        public RawImage unitIconOverdraw;

        public GameObject townMapArcanus;

        public GameObject townMapMyrror;

        public GameObject pTownMapArcanus;

        public GameObject pTownMapMyrror;

        public GameObject mapBg;

        public Animator mapsAnimator;

        public RolloverObject[] citizenRollovers;

        private TownLocation town;

        private static TownScreen instance;

        private List<Unit> selectedUnits;

        private List<Location> townList;

        private bool canMarkDirty = true;

        private bool moneyIncomeDirty = true;

        private bool foodIncomeDirty = true;

        private bool manaIncomeDirty = true;

        public static TownScreen Get()
        {
            return TownScreen.instance;
        }

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            TownScreen.instance = this;
            this.SetTown(FSMSelectionManager.Get().GetSelectedGroup() as TownLocation);
            MHEventSystem.RegisterListener<Unit>(UnitChanged, this);
            MHEventSystem.RegisterListener<RollOverOutEvents>(CraftingQueueScreenRollOverOut, this);
            MHEventSystem.RegisterListener<Artefact>(ArtefactSmashed, this);
            MHEventSystem.RegisterListener<CraftingQueue>(UpdateAll, this);
            this.constructionManager.SetActive(value: false);
            this.unitDetails.SetActive(value: false);
            this.buildingDetails.SetActive(value: false);
            if (Settings.GetData().GetTownMap())
            {
                this.mapsAnimator.SetTrigger("OpenMap");
            }
            AudioLibrary.RequestSFX("OpenTownScreen");
            this.InitializeRightPanel();
            this.InitializeTopPanel();
            this.InitializeLeftPanel();
            this.InitializeCraftingManager();
            this.UpdateAll();
            CameraController.Get().SetForcedZoom(enable: true, this.town.GetPosition());
        }

        public void UnitChanged(object sender, object e)
        {
            this.UpdateTopPanel();
        }

        public void UpdateAll(object sender, object e)
        {
            this.UpdateAll();
        }

        public void UpdateAll(bool ignoreTowns = false)
        {
            this.UpdateLeftPanel(ignoreTowns);
            this.UpdateRightPanel();
            this.UpdateTopPanel();
            this.UpdateCraftingManager(updateRightPanelpart: false);
            this.UpdateCenterVisuals();
            this.UpdateEarthGateButton();
        }

        private void InitializeRightPanel()
        {
            this.btIncreaseWorkers.onClick.AddListener(delegate
            {
                if (this.town.GetFarmers() > this.town.MinFarmers())
                {
                    this.town.farmers--;
                    this.UpdateLeftPanel();
                    this.UpdateRightPanel();
                    this.UpdateTopPanel();
                    this.UpdateCraftingManager();
                    this.UpdateSortingIfNeeded();
                }
                else
                {
                    PopupGeneral.OpenPopup(TownScreen.instance, "UI_FARMERS", "UI_FARMERS_MIN", "UI_OKAY");
                }
            });
            this.btDecreaseWorkers.onClick.AddListener(delegate
            {
                if (this.town.GetWorkers() > 0)
                {
                    this.town.farmers++;
                    this.UpdateLeftPanel();
                    this.UpdateRightPanel();
                    this.UpdateTopPanel();
                    this.UpdateCraftingManager();
                    this.UpdateSortingIfNeeded();
                }
            });
            this.gridResources.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                Multitype<Resource, bool> multitype = source as Multitype<Resource, bool>;
                Resource t = multitype.t0;
                RawImage rawImage = GameObjectUtils.FindByNameGetComponentInChildren<RawImage>(itemSource, "ResourceIcon");
                rawImage.texture = AssetManager.Get<Texture2D>(t.GetDescriptionInfo().graphic);
                rawImage.material = (multitype.t1 ? null : UIReferences.GetGrayscale());
                itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = t.dbName;
            });
            this.gridQueue.CustomDynamicItem(CraftingItem);
            PlayerWizard w = GameManager.GetHumanWizard();
            bool isOn = false;
            if (w.summaryInfoRegistration != null)
            {
                isOn = w.summaryInfoRegistration.Find((SummaryInfo o) => o.summaryType == SummaryInfo.SummaryType.eBuildingProgress && o.location.Get() == this.town) != null;
            }
            this.tgShowOnHud.onValueChanged.RemoveAllListeners();
            this.tgShowOnHud.isOn = isOn;
            this.tgShowOnHud.onValueChanged.AddListener(delegate(bool b)
            {
                if (w.summaryInfoRegistration == null)
                {
                    w.summaryInfoRegistration = new List<SummaryInfo>();
                }
                SummaryInfo summaryInfo = w.summaryInfoRegistration.Find((SummaryInfo o) => o.summaryType == SummaryInfo.SummaryType.eBuildingProgress && o.location.Get() == this.town);
                if (b && summaryInfo == null)
                {
                    SummaryInfo item = new SummaryInfo
                    {
                        location = this.town,
                        name = this.town.GetName(),
                        summaryType = SummaryInfo.SummaryType.eBuildingProgress
                    };
                    w.summaryInfoRegistration.Add(item);
                }
                else if (!b && summaryInfo != null)
                {
                    w.summaryInfoRegistration.Remove(summaryInfo);
                }
            });
            this.gridTownBuildings.SetListItems<Building>();
        }

        public void UpdateRightPanel()
        {
            StatDetails statDetails = new StatDetails();
            StatDetails statDetails2 = new StatDetails();
            StatDetails statDetails3 = new StatDetails();
            StatDetails statDetails4 = new StatDetails();
            StatDetails statDetails5 = new StatDetails();
            StatDetails statDetails6 = new StatDetails();
            StatDetails statDetails7 = new StatDetails();
            int num = this.town.CalculateFoodIncome(statDetails);
            int popUnits = this.town.GetPopUnits();
            int craftingIncome = this.town.CalculateProductionIncome(statDetails2);
            this.numberOfFarmers.text = this.town.GetFarmers().ToString();
            this.numberOfWorkers.text = this.town.GetWorkers().ToString();
            this.numberOfRebels.text = this.town.GetRebels(statDetails7).ToString();
            this.numberOfPopulation.text = global::DBUtils.Localization.SimpleGet("UI_ALL_CITIZENS") + this.town.GetPopUnits();
            this.productionIncome.text = craftingIncome.ToString();
            this.unrestRate.text = this.town.GetUnrest(statDetails3).Percent();
            this.researchIncome.text = this.town.CalculateResearchIncomeLimited(statDetails6).ToString();
            string text = this.town.CalculateMoneyIncome(includeUpkeep: true, statDetails4).ToString();
            string text2 = num + "/" + popUnits;
            string text3 = this.town.CalculatePowerIncome(statDetails5).ToString();
            if (this.goldIncome.text != text)
            {
                if (this.canMarkDirty)
                {
                    this.moneyIncomeDirty = true;
                }
                this.goldIncome.text = text;
            }
            if (this.foodIncome.text != text2)
            {
                if (this.canMarkDirty)
                {
                    this.foodIncomeDirty = true;
                }
                this.foodIncome.text = text2;
            }
            if (this.powerIncome.text != text3)
            {
                if (this.canMarkDirty)
                {
                    this.manaIncomeDirty = true;
                }
                this.powerIncome.text = text3;
            }
            this.foodTooltip.source = statDetails;
            this.productionTooltip.source = statDetails2;
            this.unrestTooltip.source = statDetails3;
            this.rebelsTooltip.source = statDetails7;
            this.powerTooltip.source = statDetails5;
            this.researchTooltip.source = statDetails6;
            this.goldTooltip.source = statDetails4;
            this.starvation.SetActive(num < popUnits);
            this.gridResources.UpdateGrid(this.town.GetResourceAndState());
            this.tgAutomanage.onValueChanged.RemoveAllListeners();
            this.tgAutomanage.isOn = this.town.autoManaged;
            this.tgAutomanage.onValueChanged.AddListener(delegate(bool b)
            {
                this.town.autoManaged = b;
                if (b)
                {
                    this.town.craftingQueue.SanitizeQueue();
                    this.town.AutoManageUpdate();
                    this.UpdateAll();
                }
            });
            if (this.town.GetResourceAndState().Count() <= 0)
            {
                this.noResources.SetActive(value: true);
            }
            else
            {
                this.noResources.SetActive(value: false);
            }
            this.UpdateRightPanelCrafringQueue(craftingIncome);
            this.UpdateBuildingsTab();
        }

        private void UpdateRightPanelCrafringQueue(int craftingIncome)
        {
            MHTimer.StartNew();
            List<CraftingItem> craftingItems = this.town.craftingQueue.craftingItems;
            if (craftingItems.Count > 0)
            {
                this.gridQueue.UpdateGrid(craftingItems.GetRange(1, craftingItems.Count - 1), craftingIncome);
            }
            else
            {
                this.gridQueue.UpdateGrid(new List<CraftingItem>(), craftingIncome);
            }
            this.currentConstruction.Set(this.town);
            CraftingItem first = this.town.craftingQueue.GetFirst();
            if (first == null)
            {
                this.currentQueueItemBuyoutCost.text = null;
                this.btBuy.interactable = false;
                return;
            }
            int num = Mathf.Max(first.BuyCost(), 0);
            this.currentQueueItemBuyoutCost.text = num.ToString();
            PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
            this.btBuy.interactable = first.progress < first.requirementValue && wizard.money >= first.BuyCost();
            RolloverObject orAddComponent = this.currentConstruction.currentItem.GetOrAddComponent<RolloverObject>();
            orAddComponent.useMouseLocation = false;
            orAddComponent.optionalPosition = this.queueTooltipPosition;
            if (first.craftedBuilding != null)
            {
                orAddComponent.source = first.craftedBuilding.Get();
            }
            else if (first.craftedUnit != null)
            {
                orAddComponent.source = first.craftedUnit.Get();
            }
        }

        public void UpdateBuildingsTab()
        {
            this.gridTownBuildings.UpdateListItems(this.town.buildings, this.town);
        }

        private void InitializeTopPanel()
        {
            this.btRename.onClick.AddListener(delegate
            {
                PopupName.OpenPopup(this.town.name, delegate(object value)
                {
                    this.town.name = value as string;
                    this.UpdateTopPanel();
                    this.UpdateLeftPanel();
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
                }, null, this);
            });
            this.btRaze.interactable = GameManager.GetHumanWizard().wizardTower != this.town;
            List<Location> cities = GameManager.GetLocationsOfWizard(GameManager.GetHumanWizard().GetID()).FindAll((Location o) => o is TownLocation && ((TownLocation)o).GetTownSize() > TownSize.Outpost);
            Button button = this.btNextTown;
            bool interactable = (this.btPrevTown.interactable = cities.Count > 1);
            button.interactable = interactable;
            this.btNextTown.onClick.AddListener(delegate
            {
                int num2 = cities.IndexOf(this.town);
                TownLocation g2 = ((num2 + 1 >= cities.Count) ? ((TownLocation)cities[0]) : ((TownLocation)cities[num2 + 1]));
                this.canMarkDirty = false;
                this.SetTown(g2);
                FSMSelectionManager.Get().Select(g2, focus: true);
                this.UpdateAll();
                this.canMarkDirty = true;
            });
            this.btPrevTown.onClick.AddListener(delegate
            {
                int num = cities.IndexOf(this.town);
                TownLocation g = ((num - 1 < 0) ? ((TownLocation)cities[cities.Count - 1]) : ((TownLocation)cities[num - 1]));
                this.canMarkDirty = false;
                this.SetTown(g);
                FSMSelectionManager.Get().Select(g, focus: true);
                this.UpdateAll();
                this.canMarkDirty = true;
            });
        }

        public void UpdateTopPanel(bool updateFame = true)
        {
            PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
            if (this.moneyIncomeDirty)
            {
                this.topStatsPanel.SetMoney(wizard);
            }
            this.moneyIncomeDirty = false;
            if (this.foodIncomeDirty)
            {
                this.topStatsPanel.SetFood(wizard);
            }
            this.foodIncomeDirty = false;
            if (this.manaIncomeDirty)
            {
                this.topStatsPanel.SetMana(wizard);
            }
            this.manaIncomeDirty = false;
            if (updateFame)
            {
                this.topStatsPanel.SetFame(wizard);
            }
            this.UpdatePopulation();
            this.townName.text = this.town.name;
            this.townMaxPopulation.text = global::DBUtils.Localization.Get("UI_MAX", true) + this.town.MaxPopulation() + "000";
            this.raceImage.texture = this.town.race.Get().GetDescriptionInfo().GetTexture();
            this.raceName.text = this.town.race.Get().GetDescriptionInfo().GetLocalizedName();
            this.capital.SetActive(this.town == wizard.wizardTower);
            this.summoningCircle.SetActive(this.town == wizard.summoningCircle);
            this.gridEnchantments.SetEnchantments(this.town.GetEnchantmentManager().GetEnchantments());
            this.btRaze.interactable = GameManager.GetHumanWizard().wizardTower != this.town;
        }

        private void UpdatePopulation()
        {
            this.townPopulation.text = this.town.Population + "(" + this.town.PopulationIncreasePerTurn().SInt() + ")";
        }

        public void UpdateBuyButton()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            CraftingItem first = this.town.craftingQueue.GetFirst();
            this.btBuy.interactable = first.progress < first.requirementValue && humanWizard.money >= first.BuyCost();
        }

        private void UpdateSortingIfNeeded()
        {
            string selection = this.dropdownSort.GetSelection();
            switch (selection)
            {
            case "UI_SORT_BY_PRODUCTION_INCOME":
            case "UI_SORT_BY_GOLD_INCOME":
            case "UI_SORT_BY_FOOD_INCOME":
            case "UI_SORT_BY_REBELS":
                this.dropdownSort.SelectOption(selection, fallbackToFirst: false, alreadyLocalized: false);
                break;
            }
        }

        private void InitializeLeftPanel()
        {
            this.townList = GameManager.GetLocationsOfWizard(PlayerWizard.HumanID()).FindAll((Location o) => o is TownLocation && ((TownLocation)o).GetTownSize() > TownSize.Outpost);
            this.gridAllTowns.SetListItems(new List<TownLocation>());
            this.gridAllTowns.onSelectionChange = delegate
            {
                TownLocation selectedObject = this.gridAllTowns.GetSelectedObject<TownLocation>();
                if (selectedObject != this.town)
                {
                    this.canMarkDirty = false;
                    this.SetTown(selectedObject);
                    FSMSelectionManager.Get().Select(this.town, focus: true);
                    this.UpdateAll(ignoreTowns: true);
                    this.canMarkDirty = true;
                }
            };
            this.dropdownSort.gameObject.SetActive(this.townList.Count > 1);
            this.dropdownSort.onChange = null;
            List<string> options = ScriptLibrary.GeDisplayNamesOfType(ScriptType.Type.UISortScript);
            List<string> scripts = ScriptLibrary.GetMetodsNamesOfType(ScriptType.Type.UISortScript);
            this.dropdownSort.SetOptions(options);
            this.dropdownSort.onChange = delegate(object option)
            {
                foreach (string item in scripts)
                {
                    bool flag = false;
                    IEnumerable<ScriptParameters> metodParameterType = ScriptLibrary.GetMetodParameterType(item);
                    if (metodParameterType != null)
                    {
                        foreach (ScriptParameters item2 in metodParameterType)
                        {
                            if (item2.displayName == option as string)
                            {
                                ScriptLibrary.Call(item, this.townList);
                                Settings.GetData().SetTownSortOption(option as string);
                                this.UpdateLeftPanel();
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            };
            string townSortOption = Settings.GetData().GetTownSortOption();
            this.dropdownSort.SelectOption(townSortOption, fallbackToFirst: false, alreadyLocalized: false);
            PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
            this.btIncreaseTax.onClick.AddListener(delegate
            {
                List<Tax> type = DataBase.GetType<Tax>();
                if (wizard.TaxRank < type.Count - 1)
                {
                    wizard.TaxRank++;
                    this.UpdateLeftPanel();
                    this.UpdateTopPanel();
                    this.UpdateRightPanel();
                    this.UpdateCraftingManager();
                    this.UpdateSortingIfNeeded();
                }
            });
            this.btDecreaseTax.onClick.AddListener(delegate
            {
                DataBase.GetType<Tax>();
                if (wizard.TaxRank > 0)
                {
                    wizard.TaxRank--;
                    this.UpdateLeftPanel();
                    this.UpdateTopPanel();
                    this.UpdateRightPanel();
                    this.UpdateCraftingManager();
                    this.UpdateSortingIfNeeded();
                }
            });
            this.gridStatoningUnits.onSelectionChange = delegate(ArmyGrid grid)
            {
                if (this.selectedUnits == null)
                {
                    this.selectedUnits = new List<Unit>();
                }
                this.selectedUnits.Clear();
                foreach (BaseUnit selected in grid.selectedList)
                {
                    this.selectedUnits.Add(selected as Unit);
                }
            };
        }

        private void UpdateLeftPanel(bool ignoreTowns = false)
        {
            if (!ignoreTowns)
            {
                this.gridAllTowns.UpdateListItems(this.townList, null, this.town);
                int num = this.townList.FindIndex((Location o) => o == this.town) / this.gridAllTowns.GetPageSize();
                if (num != this.gridAllTowns.GetPageNr())
                {
                    this.gridAllTowns.SetPageNr(num);
                }
            }
            if (this.town.GetLocalGroup() != null)
            {
                this.gridStatoningUnits.SetUnits(this.town.GetLocalGroup().GetUnits());
            }
            else
            {
                this.gridStatoningUnits.SetUnits(null);
            }
            PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
            this.taxRate.text = "x" + wizard.GetIncomeFromTax().ToString();
            this.townIncome.text = wizard.CalculateMoneyIncome(includeUpkeep: true) + "/";
            this.townUnrest.text = wizard.GetUnrestFromTax().ToIntX100() + "%";
        }

        private void InitializeCraftingManager()
        {
            CraftingItem craftingItem = this.town.craftingQueue.GetFirst();
            this.gridBuildings.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                ProductionListItem component5 = itemSource.GetComponent<ProductionListItem>();
                Building item2 = source as Building;
                bool isPossible2 = this.town.PossibleBuildings(atThisMoment: true).Contains(item2);
                int num3 = (item2.buildCost * (FInt.ONE - this.town.buildingDiscount)).ToInt();
                Button component6 = itemSource.GetComponent<Button>();
                component6.onClick.RemoveAllListeners();
                component6.onClick.AddListener(delegate
                {
                    if (this.town.craftingQueue.HaveQueueSpace() && isPossible2)
                    {
                        this.town.craftingQueue.AddItem(item2);
                        this.ByQueueLimitedUpdate();
                        RollOverOutEvents orAddComponent2 = itemSource.GetOrAddComponent<RollOverOutEvents>();
                        if (orAddComponent2 != null && orAddComponent2.isActiveAndEnabled)
                        {
                            MHEventSystem.TriggerEvent<RollOverOutEvents>(orAddComponent2, "OnPointerEnter");
                        }
                    }
                });
                itemSource.GetOrAddComponent<RollOverOutEvents>().data = item2;
                component5.icon.texture = item2.GetDescriptionInfo().GetTexture();
                component5.label.text = item2.GetDescriptionInfo().GetLocalizedName();
                Color32 color3 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color4 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 155);
                component5.label.color = (isPossible2 ? color3 : color4);
                component5.icon.material = (isPossible2 ? null : UIReferences.GetGrayscale());
                component5.productionCost.text = ((num3 > 0) ? num3.ToString() : "--");
                if (data is int num4)
                {
                    if (num4 == 0 || num3 <= 0)
                    {
                        component5.productionTime.text = "--";
                    }
                    else
                    {
                        float f2 = (float)(num3 - craftingItem.progress) / (float)num4;
                        component5.productionTime.text = Mathf.Max(1, Mathf.CeilToInt(f2)).ToString();
                    }
                }
                else
                {
                    component5.productionTime.text = "WWW";
                }
            }, UpdateCraftingManager);
            this.gridUnits.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                ProductionListItem component3 = itemSource.GetComponent<ProductionListItem>();
                global::DBDef.Unit item = source as global::DBDef.Unit;
                bool isPossible = this.town.PossibleUnits().Contains(item);
                int num = (item.constructionCost * (FInt.ONE - this.town.GetUnitDiscount(item))).ToInt();
                Button component4 = itemSource.GetComponent<Button>();
                component4.onClick.RemoveAllListeners();
                component4.onClick.AddListener(delegate
                {
                    if (this.town.craftingQueue.HaveQueueSpace() && isPossible)
                    {
                        this.town.craftingQueue.AddItem(item);
                        this.ByQueueLimitedUpdate();
                    }
                });
                itemSource.GetOrAddComponent<RollOverOutEvents>().data = item;
                component3.icon.texture = item.GetDescriptionInfo().GetTexture();
                Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                Color32 color2 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 155);
                component3.label.color = (isPossible ? color : color2);
                component3.icon.material = (isPossible ? null : UIReferences.GetGrayscale());
                component3.label.text = item.GetDescriptionInfo().GetLocalizedName();
                component3.productionCost.text = num.ToString();
                if (data is int num2)
                {
                    if (num2 == 0)
                    {
                        component3.productionTime.text = "--";
                    }
                    else
                    {
                        float f = (float)(num - craftingItem.progress) / (float)num2;
                        component3.productionTime.text = Mathf.Max(1, Mathf.CeilToInt(f)).ToString();
                    }
                }
                else
                {
                    component3.productionTime.text = "WWW";
                }
            }, UpdateCraftingManager);
            this.gridConstructionQueue.CustomDynamicItem(ConstructionCraftingItem);
            this.gridUnitSkills.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                Skill skill = source as Skill;
                GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "SkillIcon").texture = skill.GetDescriptionInfo().GetTexture();
                RolloverSimpleTooltip orAddComponent = itemSource.GetOrAddComponent<RolloverSimpleTooltip>();
                if (!string.IsNullOrEmpty(skill.descriptionScript))
                {
                    orAddComponent.title = (string)ScriptLibrary.Call(skill.descriptionScript, null, skill, data);
                }
                else
                {
                    orAddComponent.title = skill.GetDILocalizedName();
                }
                orAddComponent.sourceAsDbName = skill.dbName;
            });
            this.gridUnitRequires.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                IDescriptionInfoType descriptionInfoType3 = source as IDescriptionInfoType;
                GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "Icon").texture = descriptionInfoType3.GetDescriptionInfo().GetTexture();
                itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = (descriptionInfoType3 as Building).dbName;
            });
            this.gridBuildingRequires.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                IDescriptionInfoType descriptionInfoType2 = source as IDescriptionInfoType;
                RawImage rawImage = GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "Icon");
                GameObject gameObject = GameObjectUtils.FindByName(itemSource, "Tick");
                rawImage.texture = descriptionInfoType2.GetDescriptionInfo().GetTexture();
                Building b = source as Building;
                bool active = this.town.HaveBuilding(b);
                gameObject.SetActive(active);
                itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = (descriptionInfoType2 as Building).dbName;
            });
            this.gridBuildingAllows.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                IDescriptionInfoType descriptionInfoType = source as IDescriptionInfoType;
                GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "Icon").texture = descriptionInfoType.GetDescriptionInfo().GetTexture();
                if (source is Subrace)
                {
                    RolloverSimpleTooltip component = itemSource.GetComponent<RolloverSimpleTooltip>();
                    if (component != null)
                    {
                        global::UnityEngine.Object.Destroy(component);
                    }
                    itemSource.GetOrAddComponent<RolloverUnitTooltip>().sourceFromDb = descriptionInfoType as Subrace;
                }
                else if (source is Building)
                {
                    RolloverUnitTooltip component2 = itemSource.GetComponent<RolloverUnitTooltip>();
                    if (component2 != null)
                    {
                        global::UnityEngine.Object.Destroy(component2);
                    }
                    itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = (descriptionInfoType as Building).dbName;
                }
            });
        }

        private void UpdateManagerCetral(Building b)
        {
            this.unitDetails.SetActive(value: false);
            this.buildingDetails.SetActive(value: true);
            this.buildingName.text = b.GetDescriptionInfo().GetLocalizedName();
            this.buildingDescription.text = b.GetDescriptionInfo().GetLocalizedDescription();
            this.buildingMaintenance.text = b.upkeepCost.ToString();
            this.buildingMaintenanceMana.text = b.upkeepManaCost.ToString();
            this.goldMaintenance.SetActive(b.upkeepCost != 0);
            this.manaMaintenance.SetActive(b.upkeepManaCost != 0);
            this.noMaintenance.SetActive(!this.goldMaintenance.activeSelf && !this.manaMaintenance.activeSelf);
            this.gridBuildingRequires.UpdateGrid(b.parentBuildingRequired);
            List<global::DBDef.Unit> list = this.town.PossibleUnits(ignoreBuildingReq: true).FindAll((global::DBDef.Unit o) => o.requiredBuildings != null && Array.Find(o.requiredBuildings, (Building k) => k == b) != null);
            List<Building> list2 = this.town.PossibleBuildings(atThisMoment: false).FindAll((Building o) => o.parentBuildingRequired != null && Array.Find(o.parentBuildingRequired, (Building k) => k == b) != null);
            List<DBClass> list3 = new List<DBClass>();
            foreach (global::DBDef.Unit item in list)
            {
                list3.Add(item);
            }
            foreach (Building item2 in list2)
            {
                list3.Add(item2);
            }
            this.gridBuildingAllows.UpdateGrid(list3);
            this.buildingIcon.texture = b.GetDescriptionInfo().GetTexture();
        }

        private void UpdateManagerCetral(global::DBDef.Unit b)
        {
            this.unitDetails.SetActive(value: true);
            this.buildingDetails.SetActive(value: false);
            this.unitName.text = b.GetDescriptionInfo().GetLocalizedName();
            this.unitDescription.text = b.GetDescriptionInfo().GetLocalizedDescription();
            this.melee.text = b.GetTagSTR(TAG.MELEE_ATTACK);
            this.range.text = b.GetTagSTR(TAG.RANGE_ATTACK);
            this.armour.text = b.GetTagSTR(TAG.DEFENCE);
            this.resist.text = b.GetTagSTR(TAG.RESIST);
            this.figures.text = b.figures.ToString();
            this.hits.text = b.GetTagSTR(TAG.HIT_POINTS);
            this.movementPoints.text = b.GetTagSTR(TAG.MOVEMENT_POINTS);
            this.goldUpkeepAmount.text = b.GetTagSTR(TAG.UPKEEP_GOLD);
            this.foodUpkeepAmount.text = b.GetTagSTR(TAG.UPKEEP_FOOD);
            this.manaUpkeepAmount.text = b.GetTagSTR(TAG.UPKEEP_MANA);
            string tagSTR = b.GetTagSTR(TAG.AMMUNITION);
            if (tagSTR == "0")
            {
                this.ammo.text = "-";
            }
            else
            {
                this.ammo.text = tagSTR.ToString();
            }
            tagSTR = b.GetTagSTR(TAG.MANA_POINTS);
            if (tagSTR == "0")
            {
                this.mana.text = "-";
            }
            else
            {
                this.mana.text = tagSTR.ToString();
            }
            List<Skill> list = new List<Skill>(b.skills);
            this.town.GetWizardOwner()?.AddTraitBasedUnitSkills(b, list);
            this.gridUnitSkills.UpdateGrid(list, b);
            this.gridUnitRequires.UpdateGrid(b.requiredBuildings);
            this.movementWalking.SetActive(b.GetTag(TAG.CAN_WALK) != FInt.ZERO);
            this.movementFlying.SetActive(b.GetTag(TAG.CAN_FLY) != FInt.ZERO);
            this.movementSwimming.SetActive(b.GetTag(TAG.CAN_SWIM) != FInt.ZERO);
            this.goldUpkeep.SetActive(b.GetTag(TAG.UPKEEP_GOLD) != FInt.ZERO);
            this.foodUpkeep.SetActive(b.GetTag(TAG.UPKEEP_FOOD) != FInt.ZERO);
            this.manaUpkeep.SetActive(b.GetTag(TAG.UPKEEP_MANA) != FInt.ZERO);
            this.meleeAttackType.SetActive(value: false);
            this.rangeAttackType.SetActive(value: false);
            this.unitIcon.texture = b.GetDescriptionInfo().GetTextureLarge();
            this.unitIconOverdraw.texture = b.GetDescriptionInfo().GetTextureLarge();
        }

        public void UpdateCraftingManager()
        {
            this.UpdateCraftingManager(updateRightPanelpart: true);
        }

        public void UpdateCraftingManager(bool updateRightPanelpart)
        {
            int num = this.town.CalculateProductionIncome();
            this.gridBuildings.UpdateGrid(this.town.GetPossibleBuildingsDisplayList(), num);
            this.gridUnits.UpdateGrid(this.town.GetPossibleUnitsDisplayList(), num);
            this.gridConstructionQueue.UpdateGrid(this.town.craftingQueue.craftingItems, num);
            if (updateRightPanelpart)
            {
                this.UpdateRightPanelCrafringQueue(num);
            }
        }

        private void ConstructionCraftingItem(GameObject itemSource, object source, object data, int index)
        {
            ProductionQueueListItem component = itemSource.GetComponent<ProductionQueueListItem>();
            CraftingItem d = source as CraftingItem;
            if (d == null)
            {
                component.icon.texture = null;
                return;
            }
            component.icon.texture = d.GetDI().GetTexture();
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
            component.btRemoveFromQueue.onClick.RemoveAllListeners();
            component.btRemoveFromQueue.onClick.AddListener(delegate
            {
                this.town.craftingQueue.RemoveItem(d);
                this.ByQueueLimitedUpdate();
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
                RollOverOutEvents orAddComponent2 = itemSource.GetOrAddComponent<RollOverOutEvents>();
                if (orAddComponent2 != null && orAddComponent2.isActiveAndEnabled)
                {
                    MHEventSystem.TriggerEvent<RollOverOutEvents>(orAddComponent2, "OnPointerEnter");
                }
            });
            component.btMoveLeft.gameObject.SetActive(this.town.craftingQueue.CanMoveLeft(d));
            component.btMoveRight.gameObject.SetActive(this.town.craftingQueue.CanMoveRight(d));
            component.btMoveLeft.onClick.RemoveAllListeners();
            component.btMoveLeft.onClick.AddListener(delegate
            {
                this.town.craftingQueue.MoveLeft(d);
                this.ByQueueLimitedUpdate();
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
            });
            component.btMoveRight.onClick.RemoveAllListeners();
            component.btMoveRight.onClick.AddListener(delegate
            {
                MHTimer mHTimer = MHTimer.StartNew();
                this.town.craftingQueue.MoveRight(d);
                Debug.Log("(A)MoveRight: " + mHTimer.GetTime());
                this.ByQueueLimitedUpdate();
                Debug.Log("(B)MoveRight: " + mHTimer.GetTime());
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
                Debug.Log("(C)MoveRight: " + mHTimer.GetTime());
            });
            RollOverOutEvents orAddComponent = itemSource.GetOrAddComponent<RollOverOutEvents>();
            if (d.craftedBuilding != null)
            {
                orAddComponent.data = d.craftedBuilding.Get();
            }
            else if (d.craftedUnit != null)
            {
                orAddComponent.data = d.craftedUnit.Get();
            }
        }

        private void CraftingItem(GameObject itemSource, object source, object data, int index)
        {
            ProductionQueueListItem component = itemSource.GetComponent<ProductionQueueListItem>();
            CraftingItem d = source as CraftingItem;
            if (d == null)
            {
                component.icon.texture = null;
                return;
            }
            component.icon.texture = d.GetDI().GetTexture();
            component.btRemoveFromQueue.onClick.RemoveAllListeners();
            component.btRemoveFromQueue.onClick.AddListener(delegate
            {
                this.town.craftingQueue.RemoveItem(d);
                this.ByQueueLimitedUpdate();
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
            });
            component.btMoveLeft.gameObject.SetActive(this.town.craftingQueue.CanMoveLeft(d));
            component.btMoveRight.gameObject.SetActive(this.town.craftingQueue.CanMoveRight(d));
            component.btMoveLeft.onClick.RemoveAllListeners();
            component.btMoveLeft.onClick.AddListener(delegate
            {
                this.town.craftingQueue.MoveLeft(d);
                this.ByQueueLimitedUpdate();
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
            });
            component.btMoveRight.onClick.RemoveAllListeners();
            component.btMoveRight.onClick.AddListener(delegate
            {
                this.town.craftingQueue.MoveRight(d);
                this.ByQueueLimitedUpdate();
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.town);
            });
            RolloverObject orAddComponent = itemSource.GetOrAddComponent<RolloverObject>();
            orAddComponent.useMouseLocation = false;
            orAddComponent.optionalPosition = this.queueTooltipPosition;
            if (d.craftedBuilding != null)
            {
                orAddComponent.source = d.craftedBuilding;
            }
            else if (d.craftedUnit != null)
            {
                orAddComponent.source = d.craftedUnit;
            }
        }

        public void UpdateCenterVisuals()
        {
            bool townMap = Settings.GetData().GetTownMap();
            if (!this.tgShowShowBuildings.isOn && !townMap)
            {
                return;
            }
            bool arcanusType = this.town.GetPlane().arcanusType;
            this.townMapArcanus.SetActive(arcanusType);
            this.townMapMyrror.SetActive(!arcanusType);
            GameObject gameObject = (arcanusType ? this.townMapArcanus : this.townMapMyrror);
            if (gameObject.transform.childCount < 1)
            {
                GameObjectUtils.Instantiate(arcanusType ? this.pTownMapArcanus : this.pTownMapMyrror, gameObject.transform);
            }
            gameObject.GetComponentInChildren<TownMap>().SetTown(this.town, delegate(DBReference<Building> b)
            {
                if (b.dbName != "SUMMONING_CIRCLE")
                {
                    BuildingListItem.Sell(this.town, b, this);
                }
            });
        }

        public void UpdateEarthGateButton()
        {
            this.btUseEarthGate.gameObject.SetActive(this.town.HaveBuilding((Building)BUILDING.EARTH_GATE));
        }

        private void ByQueueLimitedUpdate()
        {
            this.UpdateRightPanel();
            if (this.moneyIncomeDirty || this.foodIncomeDirty || this.manaIncomeDirty)
            {
                this.UpdateLeftPanel(ignoreTowns: true);
                this.UpdateTopPanel();
            }
            else
            {
                this.UpdatePopulation();
            }
            this.UpdateCraftingManager(updateRightPanelpart: false);
        }

        public override IEnumerator PreClose()
        {
            yield return base.PreClose();
            Group localGroup = this.town.GetLocalGroup();
            if (localGroup != null && localGroup.GetUnits().Count > 0 && FSMSelectionManager.Get().GetSelectedGroup() == this.town)
            {
                FSMSelectionManager.Get().Select(localGroup, focus: false);
            }
            List<Location> locationsOfWizard = GameManager.GetLocationsOfWizard(PlayerWizard.HumanID());
            if (locationsOfWizard != null)
            {
                foreach (Location item in locationsOfWizard)
                {
                    if (item is TownLocation)
                    {
                        VerticalMarkerManager.Get().UpdateInfoOnMarker(item);
                    }
                }
            }
            CameraController.Get().SetForcedZoom(enable: false, Vector3i.invalid);
        }

        public void Close()
        {
            this.ButtonClick(this.btCloseTownScreen);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            bool townMap = Settings.GetData().GetTownMap();
            if (FSMCoreGame.Get().HudButton(s.name) || s == this.btGoToCityManager)
            {
                return;
            }
            if (s == this.btManageQueue)
            {
                this.constructionManager.SetActive(value: true);
            }
            else if (s == this.btCloseTownScreen)
            {
                HUD hUD = HUD.Get();
                if ((bool)hUD)
                {
                    hUD.UpdateHUD();
                }
                UIManager.Close(this);
            }
            else if (s == this.btClose)
            {
                this.unitDetails.SetActive(value: false);
                this.buildingDetails.SetActive(value: false);
                this.constructionManager.SetActive(value: false);
            }
            else if (s == this.btBuy)
            {
                if (this.town.craftingQueue.craftingItems.Count > 0)
                {
                    CraftingItem craftingItem = this.town.craftingQueue.craftingItems[0];
                    PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
                    if (craftingItem.BuyCost() <= wizard.money)
                    {
                        wizard.money -= craftingItem.BuyCost();
                        craftingItem.progress = craftingItem.requirementValue;
                        this.UpdateRightPanel();
                        this.UpdateLeftPanel();
                    }
                }
            }
            else if (s == this.btRaze)
            {
                int num = this.town.RazeFame();
                PopupGeneral.OpenPopup(this, this.town.GetName(), global::DBUtils.Localization.Get("UI_CONFIRM_RAZE_FAME_COST", true, num), "UI_RAZE", delegate
                {
                    this.town.Raze(GameManager.GetHumanWizard().GetID());
                    this.Close();
                }, "UI_CANCEL");
            }
            else if (s == this.tgShowProduction && this.tgShowProduction.isOn)
            {
                this.tgShowProduction.interactable = false;
                this.tgShowShowBuildings.interactable = true;
                if (!townMap)
                {
                    this.mapsAnimator.SetTrigger("CloseMap");
                }
            }
            else if (s == this.tgShowShowBuildings && this.tgShowShowBuildings.isOn)
            {
                this.tgShowProduction.interactable = true;
                this.tgShowShowBuildings.interactable = false;
                this.UpdateCenterVisuals();
                if (!townMap)
                {
                    this.mapsAnimator.SetTrigger("OpenMap");
                }
            }
            else if (s == this.btUseEarthGate)
            {
                PopupEarthGate.OpenPopup(this, this.town);
            }
            else if (s == this.buttonBugReport)
            {
                base.StartCoroutine(BugReportCatcher.OpenBugCatcher());
            }
        }

        public void SetTown(TownLocation tl)
        {
            this.town = tl;
            RolloverObject[] array = this.citizenRollovers;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].source = this.town.race;
            }
        }

        public TownLocation GetTown()
        {
            return this.town;
        }

        public override IEnumerator PostClose()
        {
            yield return base.PostClose();
            TownScreen.instance = null;
        }

        public List<Unit> GetSelectedUnits()
        {
            return this.selectedUnits;
        }

        private void CraftingQueueScreenRollOverOut(object sender, object e)
        {
            if (e as string == "OnPointerEnter" && this.constructionManager.activeInHierarchy)
            {
                RollOverOutEvents rollOverOutEvents = sender as RollOverOutEvents;
                if (rollOverOutEvents.data != null && rollOverOutEvents.data is Building)
                {
                    this.UpdateManagerCetral(rollOverOutEvents.data as Building);
                }
                else if (rollOverOutEvents.data != null && rollOverOutEvents.data is global::DBDef.Unit)
                {
                    this.UpdateManagerCetral(rollOverOutEvents.data as global::DBDef.Unit);
                }
            }
        }

        private void ArtefactSmashed(object sender, object e)
        {
            if (e.ToString() == "Smashed")
            {
                this.UpdateTopPanel();
            }
        }
    }
}
