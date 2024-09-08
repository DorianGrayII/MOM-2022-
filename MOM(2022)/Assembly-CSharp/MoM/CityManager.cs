using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class CityManager : ScreenBase
    {
        public Button btClose;

        public Button btGoToCity;

        public GridItemManager gridCities;

        public GridItemManager gridBuildings;

        public EnchantmentGrid gridEnchantments;

        public DropDownFilters dropdownSort;

        public TextMeshProUGUI heading;

        public RawImage minimap;

        public GameObject selectProduction;

        private int framesCount;

        private List<Location> list;

        private static CityManager instance;

        public static CityManager Get()
        {
            return CityManager.instance;
        }

        public override IEnumerator PreStart()
        {
            CityManager.instance = this;
            if (FSMSelectionManager.Get().GetSelectedGroup() is Group group && group.beforeMovingAway != null)
            {
                FSMSelectionManager.Get().Select(null, focus: false);
            }
            this.heading.text = global::DBUtils.Localization.Get("UI_THE_CITIES_OF", true, GameManager.GetHumanWizard().name);
            this.gridCities.SetListItems(new List<TownLocation>());
            this.gridCities.onSelectionChange = delegate
            {
                TownLocation selectedObject = this.gridCities.GetSelectedObject<TownLocation>();
                if (selectedObject != null)
                {
                    this.gridEnchantments.SetEnchantments(selectedObject.GetEnchantmentManager().enchantments);
                    MinimapManager.Get().FocusMinimap(selectedObject.GetPlane(), selectedObject.GetPosition());
                }
            };
            this.list = GameManager.GetTownsOfWizard(PlayerWizard.HumanID());
            this.dropdownSort.gameObject.SetActive(this.list.Count > 1);
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
                                ScriptLibrary.Call(item, this.list);
                                Settings.GetData().SetTownSortOption(option as string);
                                this.UpdateTownList();
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
            this.UpdateTownList();
            AudioLibrary.RequestSFX("OpenCitiesScreen");
            yield return base.PreStart();
            MHEventSystem.RegisterListener<RolloverBase>(ChangeProduction, this);
            MHEventSystem.RegisterListener<CraftingQueue>(UpdateAll, this);
        }

        private void UpdateAll(object sender, object e)
        {
            this.UpdateTownList();
        }

        public void UpdateTownList()
        {
            this.gridCities.UpdateListItems(this.list);
            this.btGoToCity.interactable = this.list.Count > 0;
            TownLocation selectedObject = this.gridCities.GetSelectedObject<TownLocation>();
            if (selectedObject != null)
            {
                this.gridEnchantments.SetEnchantments(selectedObject.GetEnchantmentManager().enchantments);
                MinimapManager.Get().FocusMinimap(selectedObject.GetPlane(), selectedObject.GetPosition());
            }
            else
            {
                this.gridEnchantments.SetEnchantments(new List<EnchantmentInstance>());
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btGoToCity)
            {
                TownLocation selectedObject = this.gridCities.GetSelectedObject<TownLocation>();
                this.btClose.onClick.Invoke();
                bool flag = selectedObject.IsAnOutpost();
                string e = (flag ? "OutpostScreen" : "TownScreen");
                TownScreen townScreen = TownScreen.Get();
                if (townScreen != null)
                {
                    if (flag)
                    {
                        townScreen.Close();
                        FSMMapGame.Get()?.MapGameEvents(selectedObject, e);
                    }
                    else
                    {
                        townScreen.SetTown(selectedObject);
                        townScreen.UpdateAll(ignoreTowns: true);
                        FSMSelectionManager.Get().Select(selectedObject, focus: true);
                    }
                }
                else
                {
                    FSMMapGame.Get()?.MapGameEvents(selectedObject, e);
                }
            }
            if (s.name == "ButtonClose")
            {
                UIManager.Close(this);
            }
        }

        private void ChangeProduction(object sender, object e)
        {
            RolloverBase rolloverBase = sender as RolloverBase;
            if (!(e as string == "OnPointerClick") || !(rolloverBase?.GetComponentInParent<CraftingListItem>() != null) || this.selectProduction.activeInHierarchy)
            {
                return;
            }
            TownLocation town = rolloverBase.gameObject.GetComponentInParent<TownListItem>()?.GetTown();
            if (town == null)
            {
                return;
            }
            this.selectProduction.SetActive(value: true);
            List<object> i = new List<object>(town.PossibleUnits());
            i.AddRange(town.PossibleBuildings(atThisMoment: true));
            int craftingIncome = town.CalculateProductionIncome();
            this.gridBuildings.ResetPage();
            this.gridBuildings.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                IconAndNameListItem component = itemSource.GetComponent<IconAndNameListItem>();
                global::DBDef.Unit unit = source as global::DBDef.Unit;
                Building building = source as Building;
                Button component2 = itemSource.GetComponent<Button>();
                component2.onClick.RemoveAllListeners();
                component2.onClick.AddListener(delegate
                {
                    if (unit != null)
                    {
                        town.craftingQueue.ReplaceFirstIndexItem(unit);
                    }
                    if (building != null)
                    {
                        town.craftingQueue.ReplaceFirstIndexItem(building);
                    }
                    TownScreen townScreen = TownScreen.Get();
                    if (townScreen != null)
                    {
                        townScreen.UpdateAll();
                    }
                    this.UpdateTownList();
                    this.selectProduction.SetActive(value: false);
                });
                RolloverObject orAddComponent = itemSource.GetOrAddComponent<RolloverObject>();
                CraftingItem first = town.craftingQueue.GetFirst();
                if (unit != null)
                {
                    orAddComponent.source = unit;
                    float num = (float)((unit.constructionCost * (FInt.ONE - town.GetUnitDiscount(unit))).ToInt() - first.progress) / (float)craftingIncome;
                    string text = ((num <= 0f) ? "--" : Mathf.CeilToInt(num).ToString());
                    component.icon.texture = unit.GetDescriptionInfo().GetTexture();
                    component.label.text = text;
                }
                if (building != null)
                {
                    orAddComponent.source = building;
                    float num2 = (float)((building.buildCost * (FInt.ONE - town.buildingDiscount)).ToInt() - first.progress) / (float)craftingIncome;
                    string text2 = ((num2 <= 0f) ? "--" : Mathf.CeilToInt(num2).ToString());
                    component.icon.texture = building.GetDescriptionInfo().GetTexture();
                    component.label.text = text2;
                }
            }, delegate
            {
                this.gridBuildings.UpdateGrid(i);
            });
            this.gridBuildings.UpdateGrid(i);
            this.framesCount = Time.frameCount;
        }

        private void LateUpdate()
        {
            if (!Input.GetMouseButtonUp(0) || !this.selectProduction.activeInHierarchy || Time.frameCount <= this.framesCount + 20)
            {
                return;
            }
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, list);
            foreach (RaycastResult item in list)
            {
                if (GameObjectUtils.FindByNameParent(item.gameObject, "SelectProduction") != null)
                {
                    return;
                }
            }
            this.selectProduction.SetActive(value: false);
        }

        public override IEnumerator PostClose()
        {
            yield return base.PostClose();
            MinimapManager.Get().SetPlane(World.GetActivePlane());
        }
    }
}
