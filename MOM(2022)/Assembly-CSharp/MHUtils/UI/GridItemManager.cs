using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MHUtils.UI
{
    public class GridItemManager : ToggleGroup, IEndDragHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public enum PageSizeController
        {
            DesignedGridSize = 0,
            ScaleGridToFitAllItems = 1,
            ScaleGridToManualMaxPageSizes = 2
        }

        [Tooltip("When enabled we will allow switch off on a page as long as items on other pages are still selected, this only works with the new listItem interface")]
        public bool expandSwitchOffAcrossPages;

        public Callback onSelectionChange;

        public Callback onUpdate;

        public Callback onPageChange;

        public CustomDynamicItemMethod customItemBehaviour;

        public bool useToggleGroup;

        public GridLayoutGroup grid;

        public GameObject itemBase;

        public Scrollbar scrollbarH;

        public Scrollbar scrollbarV;

        public PageSizeController pageSizeController;

        public bool scaleCellsToFit;

        public int dragAndDropGroup;

        public object dragAndDropManager;

        public int manualGridVSize;

        public int manualGridHSize;

        private int maxHItems;

        private int maxVItems;

        public GameObject[] itemInstances;

        private int curentPage;

        private int maxPages;

        public Dictionary<Selectable, object> toggleToDataDictionary = new Dictionary<Selectable, object>();

        public Dictionary<object, GameObject> dataToGameObjectDictionary = new Dictionary<object, GameObject>();

        public GameObject pagingAutoHide;

        public TextMeshProUGUI pageingInfo;

        public Button pageingNext;

        public Button pageingPrev;

        private List<object> listOfExtraData;

        private object commonExtraData;

        private bool initialized;

        private List<object> sourcesForScrollUpdate;

        private int scrollItemOffset;

        public bool focusGained;

        public bool pageingWithMouseWheel;

        public bool delayedInitialization;

        public bool addEnterEventTrigger;

        public bool addExitEventTrigger;

        public bool addClickEventTrigger;

        public bool forceExtraDataAsWhole;

        private List<GameObject> disabledParents;

        private List<object> listItemData;

        private object listItemGridData;

        private List<object> listItemsSelected;

        protected override void OnEnable()
        {
            if (!base.allowSwitchOff && this.expandSwitchOffAcrossPages)
            {
                _ = base.allowSwitchOff;
                base.allowSwitchOff = true;
                base.OnEnable();
                base.allowSwitchOff = false;
            }
            else
            {
                base.OnEnable();
            }
        }

        protected override void Start()
        {
            if (!base.allowSwitchOff && this.expandSwitchOffAcrossPages)
            {
                _ = base.allowSwitchOff;
                base.allowSwitchOff = true;
                base.Start();
                base.allowSwitchOff = false;
            }
            else
            {
                base.Start();
            }
            if (!this.delayedInitialization)
            {
                this.Initialize();
            }
            else
            {
                base.StartCoroutine(this.DelayedInitialization());
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.focusGained = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.focusGained = false;
        }

        private IEnumerator DelayedInitialization()
        {
            yield return null;
            this.Initialize();
        }

        public void CustomDynamicItem(CustomDynamicItemMethod callback, SimpleCallback onPageChange = null)
        {
            this.Initialize();
            this.customItemBehaviour = callback;
            this.AddCallback(onPageChange);
        }

        public void SetListItems<T>(List<T> listItemData = null, object gridData = null, object selected = null) where T : class
        {
            this.Initialize();
            this.SelectListItem(selected);
            this.SetListItemData(listItemData);
            if (listItemData == null)
            {
                this.listItemData = new List<object>();
            }
            else
            {
                this.listItemData = listItemData.Cast<object>().ToList();
            }
            this.listItemGridData = gridData;
            this.customItemBehaviour = delegate(GameObject source, object o, object data, int index)
            {
                source.GetComponent<ListItem>().Set(o, data, index);
            };
            this.AddCallback(delegate
            {
                this.UpdateGrid(this.listItemData, this.listItemGridData);
            });
            this.UpdateGrid(this.listItemData, this.listItemGridData);
        }

        private void SetListItemData<T>(List<T> listItemData)
        {
            if (listItemData == null)
            {
                this.listItemData = new List<object>();
            }
            else
            {
                this.listItemData = listItemData.Cast<object>().ToList();
            }
        }

        public void UpdateListItems<T>(List<T> listItemData, object gridData = null, object selected = null)
        {
            if (selected != null)
            {
                this.SelectListItem(selected);
            }
            this.SetListItemData(listItemData);
            this.listItemGridData = gridData;
            this.UpdateGrid(this.listItemData, this.listItemGridData);
        }

        public void Initialize()
        {
            if (this.initialized)
            {
                return;
            }
            if (!base.gameObject.activeInHierarchy)
            {
                Transform parent = base.transform;
                while (parent != null)
                {
                    if (!parent.gameObject.activeSelf)
                    {
                        if (this.disabledParents == null)
                        {
                            this.disabledParents = new List<GameObject>();
                        }
                        this.disabledParents.Add(parent.gameObject);
                        parent.gameObject.SetActive(value: true);
                    }
                    parent = parent.parent;
                }
            }
            this.initialized = true;
            RectTransform component = this.grid.GetComponent<RectTransform>();
            GridLayoutGroup component2 = component.GetComponent<GridLayoutGroup>();
            Rect rect = component.rect;
            float num = rect.width - (float)this.grid.padding.left - (float)this.grid.padding.right;
            float num2 = rect.height - (float)this.grid.padding.top - (float)this.grid.padding.bottom;
            float x = component2.cellSize.x;
            float y = component2.cellSize.y;
            float num3 = num;
            float num4 = num2;
            num3 -= x;
            num4 -= y;
            num3 /= x + component2.spacing.x;
            num4 /= y + component2.spacing.y;
            this.maxHItems = Mathf.Max(1, 1 + (int)num3);
            this.maxVItems = Mathf.Max(1, 1 + (int)num4);
            if (this.pageSizeController == PageSizeController.ScaleGridToManualMaxPageSizes)
            {
                if (this.manualGridVSize > 0)
                {
                    this.maxVItems = this.manualGridVSize;
                }
                if (this.manualGridHSize > 0)
                {
                    this.maxHItems = this.manualGridHSize;
                }
            }
            if (this.scaleCellsToFit)
            {
                float num5 = (float)this.maxHItems * x + (float)(this.maxHItems - 1) * component2.spacing.x;
                if (num - num5 > x * 0.6f)
                {
                    this.maxHItems++;
                }
                Vector2 cellSize = default(Vector2);
                cellSize.x = (num - (float)(this.maxHItems - 1) * component2.spacing.x) / (float)this.maxHItems;
                cellSize.y = y;
                component2.cellSize = cellSize;
            }
            this.InitializeItems();
            if (this.pageingNext != null)
            {
                this.pageingNext.onClick.AddListener(delegate
                {
                    this.NextPage();
                });
            }
            if (this.pageingPrev != null)
            {
                this.pageingPrev.onClick.AddListener(delegate
                {
                    this.PrevPage();
                });
            }
            if (this.disabledParents == null)
            {
                return;
            }
            foreach (GameObject disabledParent in this.disabledParents)
            {
                disabledParent.SetActive(value: false);
            }
            this.disabledParents = null;
        }

        private void Update()
        {
            if (this.pageingWithMouseWheel && this.focusGained)
            {
                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis > 0f)
                {
                    this.PrevPage();
                }
                else if (axis < 0f)
                {
                    this.NextPage();
                }
            }
        }

        public bool IsInitialized()
        {
            return this.initialized;
        }

        private void InitializeItems()
        {
            if (this.addEnterEventTrigger)
            {
                this.itemBase.GetOrAddComponent<GridItemInteractiveRollOver>().owner = this;
            }
            if (this.addExitEventTrigger)
            {
                this.itemBase.GetOrAddComponent<GridItemInteractiveRollOut>().owner = this;
            }
            if (this.addClickEventTrigger)
            {
                this.itemBase.GetOrAddComponent<GridItemInteractiveClick>().owner = this;
            }
            Button b = this.itemBase.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.AddListener(delegate
                {
                    if (this.onSelectionChange != null)
                    {
                        object objectForSelectable = this.GetObjectForSelectable<object>(b);
                        this.onSelectionChange(objectForSelectable);
                    }
                });
            }
            GridItemBase component = this.itemBase.GetComponent<GridItemBase>();
            if (component != null)
            {
                component.owner = this;
            }
            this.itemInstances = new GameObject[this.maxHItems * this.maxVItems];
            this.itemInstances[0] = this.itemBase;
            this.itemBase.SetActive(value: true);
            bool flag = false;
            if (this.itemBase.transform.parent.childCount >= this.maxHItems * this.maxVItems)
            {
                flag = true;
                for (int i = 1; i < this.itemInstances.Length; i++)
                {
                    GameObject gameObject = GameObjectUtils.FindByName(this.itemBase.transform.parent.gameObject, this.itemBase.name + i, onlyDirectChildren: true);
                    if (gameObject == null)
                    {
                        flag = false;
                        break;
                    }
                    this.itemInstances[i] = gameObject;
                }
            }
            if (!flag)
            {
                for (int j = 1; j < this.itemInstances.Length; j++)
                {
                    this.itemInstances[j] = global::UnityEngine.Object.Instantiate(this.itemBase, this.itemBase.transform.parent);
                    this.itemInstances[j].name = this.itemBase.name + j;
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.grid.transform as RectTransform);
            if (!this.useToggleGroup)
            {
                return;
            }
            GameObject[] array = this.itemInstances;
            foreach (GameObject gameObject2 in array)
            {
                Toggle t = gameObject2.GetComponent<Toggle>();
                if (t == null)
                {
                    Debug.LogWarning("Expected toggle on item " + gameObject2.name + " but none was found");
                    continue;
                }
                t.onValueChanged.RemoveAllListeners();
                t.onValueChanged.AddListener(delegate(bool x)
                {
                    if (this.listItemsSelected != null)
                    {
                        object item = this.toggleToDataDictionary[t];
                        if (x)
                        {
                            if ((bool)t.group)
                            {
                                this.listItemsSelected.Clear();
                                this.listItemsSelected.Add(item);
                            }
                            else if (!this.listItemsSelected.Contains(item))
                            {
                                this.listItemsSelected.Add(item);
                            }
                        }
                        else if (this.listItemsSelected.Contains(item))
                        {
                            this.listItemsSelected.Remove(item);
                        }
                    }
                    if (!x)
                    {
                        if (this.Active() == null)
                        {
                            this.DoOnChange(null);
                        }
                    }
                    else
                    {
                        Toggle newActive = this.Active();
                        this.DoOnChange(newActive);
                    }
                });
            }
        }

        public int GetPageSize()
        {
            if (this.itemInstances != null)
            {
                return this.itemInstances.Length;
            }
            return this.maxVItems * this.maxHItems;
        }

        public GameObject GetGameObjectForData<T>(T obj)
        {
            if (obj == null || this.dataToGameObjectDictionary == null)
            {
                return null;
            }
            if (this.dataToGameObjectDictionary.ContainsKey(obj))
            {
                return this.dataToGameObjectDictionary[obj];
            }
            return null;
        }

        public object GetDataForGameObject(GameObject go)
        {
            if (this.dataToGameObjectDictionary == null)
            {
                return null;
            }
            foreach (KeyValuePair<object, GameObject> item in this.dataToGameObjectDictionary)
            {
                if (item.Value == go)
                {
                    return item.Key;
                }
            }
            return null;
        }

        public int GetIndexGameObject(GameObject go)
        {
            if (this.dataToGameObjectDictionary == null)
            {
                return -1;
            }
            foreach (KeyValuePair<object, GameObject> item in this.dataToGameObjectDictionary)
            {
                if (item.Value == go)
                {
                    return Array.IndexOf(this.itemInstances, go);
                }
            }
            return -1;
        }

        public object GetCommonExtraData()
        {
            return this.commonExtraData;
        }

        public int GetPageIndexOffset()
        {
            return this.curentPage * this.GetPageSize();
        }

        public int GetPageNr()
        {
            return this.curentPage;
        }

        public int GetMaxPageNr()
        {
            return this.maxPages;
        }

        public void SetPageNr(int page)
        {
            if (this.curentPage < this.maxPages && this.curentPage >= 0)
            {
                this.curentPage = page;
                if (this.onPageChange != null)
                {
                    this.onPageChange(this);
                }
            }
        }

        public void NextPage()
        {
            if (this.curentPage < this.maxPages - 1)
            {
                this.curentPage++;
                if (this.onPageChange != null)
                {
                    this.onPageChange(this);
                }
                else
                {
                    Debug.LogError("[ERROR]Interface element is missing UPDATE set by AddCallback or indirectly by SetMode or CustomDynamicItem. Please contact Khash with this issue, its not A'vee's fault ;P");
                }
            }
        }

        public void PrevPage()
        {
            if (this.curentPage > 0)
            {
                this.curentPage--;
                if (this.onPageChange != null)
                {
                    this.onPageChange(this);
                }
                else
                {
                    Debug.LogError("[ERROR]Interface element is missing UPDATE set by AddCallback or indirectly by SetMode or CustomDynamicItem. Please contact Khash with this issue, its not A'vee's fault ;P");
                }
            }
        }

        public void ResetPage()
        {
            if (this.curentPage > 0)
            {
                this.curentPage = 0;
                if (this.onPageChange != null)
                {
                    this.onPageChange(this);
                }
            }
        }

        public T GetObjectForSelectable<T>(Selectable t) where T : class
        {
            if (this.toggleToDataDictionary.ContainsKey(t))
            {
                return this.toggleToDataDictionary[t] as T;
            }
            return null;
        }

        public void EnsureInstanceArraySize(int size)
        {
            if (this.pageSizeController == PageSizeController.ScaleGridToFitAllItems && this.itemInstances.Length < size)
            {
                List<GameObject> list = new List<GameObject>(this.itemInstances);
                for (int i = this.itemInstances.Length; i < size; i++)
                {
                    list.Add(global::UnityEngine.Object.Instantiate(this.itemBase, this.itemBase.transform.parent));
                }
                this.itemInstances = list.ToArray();
                int num = Mathf.RoundToInt(((float)size + 0.2f) / (float)this.maxHItems);
                this.maxVItems = num;
            }
        }

        public void UpdateGridDelayed<T>(List<T> data, bool lateUpdate = true, object extraData = null)
        {
            base.StartCoroutine(this.LateGridUpdate(data, lateUpdate, extraData));
        }

        private IEnumerator LateGridUpdate<T>(List<T> data, bool lateUpdate = true, object extraData = null)
        {
            if (lateUpdate)
            {
                yield return null;
            }
            this.UpdateGrid(data, extraData);
        }

        public void UpdateGridDelayed<T>(T[] data, bool lateUpdate = true, object extraData = null)
        {
            base.StartCoroutine(this.LateGridUpdate(data, lateUpdate, extraData));
        }

        private IEnumerator LateGridUpdate<T>(T[] data, bool lateUpdate = true, object extraData = null)
        {
            if (lateUpdate)
            {
                yield return null;
            }
            this.UpdateGrid(data, extraData);
        }

        public void AddCallback(SimpleCallback c)
        {
            this.onPageChange = delegate
            {
                if (c != null)
                {
                    c();
                }
            };
        }

        public void UpdateGrid<T>(T[] items, object extraData = null)
        {
            if (items != null && items.Length != 0)
            {
                List<T> items2 = new List<T>(items);
                this.UpdateGrid(items2, extraData);
            }
            else
            {
                this.UpdateGrid(new List<T>(), extraData);
            }
        }

        public void UpdateGrid<T>(List<T> items, object extraData = null)
        {
            if (items == null)
            {
                items = new List<T>();
            }
            this.EnsureInstanceArraySize(items.Count);
            if (extraData != null)
            {
                if (!this.forceExtraDataAsWhole && extraData is List<object>)
                {
                    this.listOfExtraData = new List<object>(extraData as List<object>);
                    this.commonExtraData = null;
                }
                else
                {
                    this.listOfExtraData = null;
                    this.commonExtraData = extraData;
                }
            }
            else
            {
                this.listOfExtraData = null;
                this.commonExtraData = null;
            }
            while (true)
            {
                if (items.Count == 0)
                {
                    this.curentPage = 0;
                    break;
                }
                if (this.GetPageIndexOffset() < items.Count || this.GetPageIndexOffset() <= 0)
                {
                    break;
                }
                this.curentPage--;
            }
            bool flag = false;
            if (items.Count == 0)
            {
                for (int i = 0; i < this.itemInstances.Length; i++)
                {
                    GameObject gameObject = this.itemInstances[i];
                    gameObject.SetActive(value: false);
                }
                if (this.pageingInfo != null)
                {
                    this.pageingInfo.text = "1/1";
                }
                if ((bool)this.pageingNext)
                {
                    this.pageingNext.interactable = false;
                }
                if ((bool)this.pageingPrev)
                {
                    this.pageingPrev.interactable = false;
                }
                if ((bool)this.pagingAutoHide)
                {
                    this.pagingAutoHide.SetActive(value: false);
                }
                return;
            }
            this.toggleToDataDictionary.Clear();
            this.dataToGameObjectDictionary.Clear();
            if (this.listItemsSelected != null)
            {
                base.SetAllTogglesOff(sendCallback: false);
                if (this.listItemsSelected.Count == 0 && !base.allowSwitchOff)
                {
                    this.listItemsSelected.Add(items[this.GetPageIndexOffset()]);
                }
            }
            int j = 0;
            for (int k = this.GetPageIndexOffset(); k < items.Count; k++)
            {
                if (this.itemInstances.Length <= j)
                {
                    break;
                }
                GameObject gameObject = this.itemInstances[j];
                gameObject.SetActive(value: true);
                object obj = null;
                obj = ((this.listOfExtraData == null) ? this.commonExtraData : this.listOfExtraData);
                if (this.customItemBehaviour != null)
                {
                    this.customItemBehaviour(gameObject, items[k], obj, k);
                }
                Selectable component = gameObject.GetComponent<Selectable>();
                if (component != null)
                {
                    this.toggleToDataDictionary[component] = items[k];
                    if (this.listItemsSelected != null && component is Toggle toggle)
                    {
                        toggle.isOn = this.listItemsSelected.Contains(items[k]);
                    }
                }
                this.dataToGameObjectDictionary[items[k]] = gameObject;
                if ((gameObject.transform as RectTransform).rect.width == 0f)
                {
                    flag = true;
                }
                if (this.addEnterEventTrigger)
                {
                    GridItemInteractiveRollOver component2 = gameObject.GetComponent<GridItemInteractiveRollOver>();
                    component2.itemIndex = k;
                    component2.itemData = obj;
                    component2.itemDisplaySource = items[k];
                }
                if (this.addExitEventTrigger)
                {
                    GridItemInteractiveRollOut component3 = gameObject.GetComponent<GridItemInteractiveRollOut>();
                    component3.itemIndex = k;
                    component3.itemData = obj;
                    component3.itemDisplaySource = items[k];
                }
                if (this.addClickEventTrigger)
                {
                    GridItemInteractiveClick component4 = gameObject.GetComponent<GridItemInteractiveClick>();
                    component4.itemIndex = k;
                    component4.itemData = obj;
                    component4.itemDisplaySource = items[k];
                }
                j++;
            }
            int num = j;
            for (; j < this.itemInstances.Length; j++)
            {
                GameObject gameObject = this.itemInstances[j];
                gameObject.SetActive(value: false);
            }
            if (num > 0 && num < this.itemInstances.Length && this.listItemsSelected != null && this.listItemsSelected.Count > 0 && this.listItemsSelected[0] is T)
            {
                int num2 = items.IndexOf((T)this.listItemsSelected[0]);
                if (num2 < 0 || num <= num2 - this.GetPageIndexOffset())
                {
                    this.SelectItem(this.GetPageIndexOffset());
                }
            }
            if (!this.expandSwitchOffAcrossPages && !base.allowSwitchOff)
            {
                base.EnsureValidState();
            }
            if (flag)
            {
                this.grid.enabled = false;
                this.grid.enabled = true;
            }
            this.maxPages = 0;
            if (items.Count > 0)
            {
                this.maxPages = items.Count / this.GetPageSize();
                if (this.maxPages * this.GetPageSize() < items.Count)
                {
                    this.maxPages++;
                }
            }
            if (this.pageingInfo != null)
            {
                this.pageingInfo.text = this.curentPage + 1 + "/" + this.maxPages;
            }
            if (this.pagingAutoHide != null)
            {
                this.pagingAutoHide.SetActive(this.maxPages > 1);
            }
            if (this.maxPages < 2)
            {
                if ((bool)this.pageingNext)
                {
                    this.pageingNext.interactable = false;
                }
                if ((bool)this.pageingPrev)
                {
                    this.pageingPrev.interactable = false;
                }
            }
            else
            {
                int num3 = this.curentPage + 1;
                if ((bool)this.pageingNext)
                {
                    this.pageingNext.interactable = num3 < this.maxPages;
                }
                if ((bool)this.pageingPrev)
                {
                    this.pageingPrev.interactable = num3 > 1;
                }
            }
            if (this.onUpdate != null)
            {
                this.onUpdate(this);
            }
            if (this.scrollbarH != null || this.scrollbarV != null)
            {
                this.sourcesForScrollUpdate = new List<object>(items.Count);
                items.ForEach(delegate(T o)
                {
                    this.sourcesForScrollUpdate.Add(o);
                });
                this.UpdateSliderFromGrid();
            }
        }

        public List<object> GetItems()
        {
            if (this.dataToGameObjectDictionary.Keys != null)
            {
                return new List<object>(this.dataToGameObjectDictionary.Keys);
            }
            return null;
        }

        public Toggle Active()
        {
            Toggle toggle = base.ActiveToggles().FirstOrDefault();
            if (toggle == null && this.itemInstances != null && this.itemInstances.Length != 0)
            {
                GameObject[] array = this.itemInstances;
                for (int i = 0; i < array.Length; i++)
                {
                    Toggle component = array[i].GetComponent<Toggle>();
                    if (component != null && component.gameObject.activeSelf)
                    {
                        return component;
                    }
                }
            }
            return toggle;
        }

        public void Unselect()
        {
            if (this.useToggleGroup && base.gameObject.activeInHierarchy)
            {
                if (this.Active() != null)
                {
                    base.SetAllTogglesOff();
                }
            }
            else
            {
                if (this.itemInstances == null)
                {
                    return;
                }
                GameObject[] array = this.itemInstances;
                for (int i = 0; i < array.Length; i++)
                {
                    Toggle component = array[i].GetComponent<Toggle>();
                    if (component != null)
                    {
                        component.isOn = false;
                    }
                }
            }
        }

        protected virtual void DoOnChange(Toggle newActive)
        {
            if (this.onSelectionChange != null && (!(newActive != null) || newActive.isOn))
            {
                this.onSelectionChange(newActive);
            }
        }

        public List<T> GetSelectedObjects<T>() where T : class
        {
            return this.listItemsSelected.Cast<T>().ToList();
        }

        public T GetSelectedObject<T>() where T : class
        {
            if (this.listItemsSelected != null)
            {
                if (this.listItemsSelected.Count <= 0)
                {
                    return null;
                }
                return this.listItemsSelected[0] as T;
            }
            Toggle toggle = this.Active();
            if (toggle != null && toggle.isOn && this.toggleToDataDictionary != null && this.toggleToDataDictionary.ContainsKey(toggle))
            {
                return this.toggleToDataDictionary[toggle] as T;
            }
            return null;
        }

        public void SelectListItem(object objectOrList)
        {
            if (objectOrList is IList source)
            {
                this.listItemsSelected = source.Cast<object>().ToList();
                return;
            }
            if (this.listItemsSelected == null)
            {
                this.listItemsSelected = new List<object>();
            }
            else
            {
                this.listItemsSelected.Clear();
            }
            if (objectOrList != null)
            {
                this.listItemsSelected.Add(objectOrList);
            }
        }

        public void SelectItem(int indexOnPage)
        {
            if (this.itemInstances != null && this.itemInstances.Length > indexOnPage)
            {
                GameObject go = this.itemInstances[indexOnPage];
                this.SelectItemByGameObject(go);
            }
        }

        public void Select<T>(T obj)
        {
            GameObject gameObjectForData = this.GetGameObjectForData(obj);
            if (gameObjectForData != null)
            {
                this.SelectItemByGameObject(gameObjectForData);
            }
        }

        public void SelectItemByGameObject(GameObject go)
        {
            if (!(go != null))
            {
                return;
            }
            Toggle component = go.GetComponent<Toggle>();
            if (component != null)
            {
                if (!component.isOn && this.useToggleGroup && this.listItemsSelected == null)
                {
                    this.Unselect();
                }
                component.isOn = true;
            }
        }

        private void UpdateSliderFromGrid()
        {
            if ((this.scrollbarH == null && this.scrollbarV == null) || this.sourcesForScrollUpdate == null || this.GetPageSize() == 0)
            {
                return;
            }
            bool flag = this.GetPageSize() <= this.sourcesForScrollUpdate.Count;
            if (this.scrollbarH != null)
            {
                this.scrollbarH.interactable = flag;
                this.scrollbarH.GetComponentInChildren<Button>().interactable = flag;
            }
            if (this.scrollbarV != null)
            {
                this.scrollbarV.interactable = flag;
                this.scrollbarV.GetComponentInChildren<Button>().interactable = flag;
            }
            if (flag)
            {
                bool num = this.sourcesForScrollUpdate.Count % this.maxVItems > 0;
                bool flag2 = this.sourcesForScrollUpdate.Count % this.maxHItems > 0;
                int num2 = (num ? 1 : (this.sourcesForScrollUpdate.Count / this.maxVItems));
                int num3 = (flag2 ? 1 : (this.sourcesForScrollUpdate.Count / this.maxHItems));
                int num4 = num2 - this.maxHItems;
                int numberOfSteps = num3 - this.maxVItems;
                if (this.scrollbarH != null)
                {
                    this.scrollbarH.numberOfSteps = num4 + 1;
                    this.scrollbarH.size = Mathf.Min(1f, (float)this.maxHItems / (float)num2);
                    if (num4 > 0)
                    {
                        float value = Mathf.Min(1f, this.curentPage * this.maxHItems / this.scrollbarH.numberOfSteps);
                        this.scrollbarH.value = value;
                    }
                    else
                    {
                        this.scrollbarH.value = 0f;
                    }
                    this.scrollbarH.onValueChanged.RemoveAllListeners();
                    this.scrollbarH.onValueChanged.AddListener(UpdateGridFromHSlider);
                }
                if (this.scrollbarV != null)
                {
                    this.scrollbarV.numberOfSteps = numberOfSteps;
                    this.scrollbarV.size = Mathf.Min(1f, (float)this.maxVItems / (float)num3);
                    if (num4 > 0)
                    {
                        float value2 = Mathf.Min(1f, this.curentPage * this.maxVItems / this.scrollbarV.numberOfSteps);
                        this.scrollbarV.value = value2;
                    }
                    else
                    {
                        this.scrollbarV.value = 0f;
                    }
                    this.scrollbarV.onValueChanged.RemoveAllListeners();
                    this.scrollbarV.onValueChanged.AddListener(UpdateGridFromVSlider);
                }
            }
            else
            {
                if ((bool)this.scrollbarH)
                {
                    this.scrollbarH.numberOfSteps = 0;
                    this.scrollbarH.size = 1f;
                    this.scrollbarH.value = 1f;
                }
                if ((bool)this.scrollbarV)
                {
                    this.scrollbarV.numberOfSteps = 0;
                    this.scrollbarV.size = 1f;
                    this.scrollbarV.value = 1f;
                }
            }
        }

        private void UpdateGridFromHSlider(float v)
        {
            this.UpdateGridFromSlider(this.scrollbarH, horizontal: true);
        }

        private void UpdateGridFromVSlider(float v)
        {
            this.UpdateGridFromSlider(this.scrollbarV, horizontal: false);
        }

        private void UpdateGridFromSlider(Scrollbar scrollbar, bool horizontal)
        {
            float value = scrollbar.value;
            if (this.sourcesForScrollUpdate.Count == 0)
            {
                this.curentPage = 0;
                this.scrollItemOffset = 0;
            }
            else
            {
                this.scrollItemOffset = Mathf.RoundToInt(value * (float)(scrollbar.numberOfSteps - 1));
                this.scrollItemOffset = Mathf.Max(0, this.scrollItemOffset);
            }
            if (horizontal)
            {
                this.scrollItemOffset *= this.maxVItems;
            }
            else
            {
                this.scrollItemOffset *= this.maxHItems;
            }
            if (this.sourcesForScrollUpdate.Count == 0)
            {
                for (int i = 0; i < this.itemInstances.Length; i++)
                {
                    GameObject gameObject = this.itemInstances[i];
                    gameObject.SetActive(value: false);
                }
                return;
            }
            this.toggleToDataDictionary.Clear();
            this.dataToGameObjectDictionary.Clear();
            int j = 0;
            for (int k = this.scrollItemOffset; k < this.scrollItemOffset + this.GetPageSize(); k++)
            {
                if (this.itemInstances.Length <= j)
                {
                    break;
                }
                if (this.sourcesForScrollUpdate.Count <= k)
                {
                    break;
                }
                GameObject gameObject = this.itemInstances[j];
                gameObject.SetActive(value: true);
                if (this.listOfExtraData != null)
                {
                    if (this.customItemBehaviour != null)
                    {
                        this.customItemBehaviour(gameObject, this.sourcesForScrollUpdate[k], this.listOfExtraData);
                    }
                }
                else if (this.customItemBehaviour != null)
                {
                    this.customItemBehaviour(gameObject, this.sourcesForScrollUpdate[k], this.commonExtraData);
                }
                Selectable component = gameObject.GetComponent<Selectable>();
                if (component != null)
                {
                    this.toggleToDataDictionary[component] = this.sourcesForScrollUpdate[k];
                    if (this.listItemsSelected != null && component is Toggle toggle)
                    {
                        toggle.isOn = this.listItemsSelected.Contains(this.sourcesForScrollUpdate[k]);
                    }
                }
                this.dataToGameObjectDictionary[this.sourcesForScrollUpdate[k]] = gameObject;
                j++;
            }
            for (; j < this.itemInstances.Length; j++)
            {
                GameObject gameObject = this.itemInstances[j];
                gameObject.SetActive(value: false);
            }
            if (this.pageingInfo != null)
            {
                this.pageingInfo.text = this.curentPage + 1 + "/" + this.maxPages;
            }
            if (this.maxPages < 2)
            {
                if ((bool)this.pageingNext)
                {
                    this.pageingNext.interactable = false;
                }
                if ((bool)this.pageingPrev)
                {
                    this.pageingPrev.interactable = false;
                }
            }
            else
            {
                int pageIndexOffset = this.GetPageIndexOffset();
                if ((bool)this.pageingNext)
                {
                    this.pageingNext.interactable = pageIndexOffset < this.maxPages;
                }
                if ((bool)this.pageingPrev)
                {
                    this.pageingPrev.interactable = pageIndexOffset > -1 && this.GetPageIndexOffset() > 0;
                }
            }
            if (this.onUpdate != null)
            {
                this.onUpdate(this);
            }
        }

        public GameObject GetGOByIndex(int onPageIndex)
        {
            if (onPageIndex < 0 || onPageIndex >= this.itemInstances.Length)
            {
                return null;
            }
            return this.itemInstances[onPageIndex];
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.LogWarning("Accepting Drop on Grid");
        }
    }
}
