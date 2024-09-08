using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorAdventures : ScreenBase
    {
        public GridItemManager grid;

        public Button buttonEdit;

        public Button buttonExit;

        public Button buttonNewAdventure;

        public Button buttonCloneAdventure;

        public Button buttonCloneToModule;

        public Button buttonRename;

        public Button buttonDelete;

        public Button buttonSave;

        public TMP_InputField inputSearch;

        public int selectedAdventureHash;

        public static Adventure selectedAdventure;

        private List<Adventure> filteredAdventures;

        protected override void Awake()
        {
            this.grid.CustomDynamicItem(GridItem, UpdateGrid);
            this.grid.onSelectionChange = SelectionChange;
            this.inputSearch.onValueChanged.AddListener(UpdateFromFilter);
            this.UseFilter();
            this.UpdateScreen();
            MHEventSystem.RegisterListener<EditorNode>(UpdateRequest, this);
            MHEventSystem.RegisterListener<EventEditorGraph>(UpdateRequest, this);
        }

        private void UpdateFromFilter(object selection = null)
        {
            this.UseFilter();
            this.UpdateScreen();
        }

        private void UseFilter()
        {
            if (EventEditorModules.GetSelectedModule().adventures == null)
            {
                this.filteredAdventures = new List<Adventure>();
            }
            else
            {
                this.filteredAdventures = new List<Adventure>(EventEditorModules.GetSelectedModule().adventures);
            }
            if (!string.IsNullOrEmpty(this.inputSearch.text))
            {
                this.filteredAdventures = this.filteredAdventures.FindAll((Adventure o) => o.name.Contains(this.inputSearch.text));
            }
        }

        private void UpdateRequest(object sender, object e)
        {
            if (sender as EditorNode == null && sender as EventEditorGraph == null)
            {
                Debug.LogError("Incorrect data send, is ti a mistake?");
            }
            if (e is string text && text == "AdventureUpdate")
            {
                this.UpdateGrid();
            }
        }

        private void GridItem(GameObject itemSource, object source, object data, int index)
        {
            Adventure a = source as Adventure;
            GIAdventureItem component = itemSource.GetComponent<GIAdventureItem>();
            component.tf.text = a.name;
            component.tf2.text = a.uniqueID.ToString();
            component.image.texture = ImageLibrary.RequestSingleImage(a.GetGraphic());
            component.toggle.onValueChanged.RemoveAllListeners();
            component.toggle.isOn = a.isAllowed;
            component.toggle.onValueChanged.AddListener(delegate(bool b)
            {
                a.isAllowed = b;
            });
        }

        private void UpdateGrid()
        {
            this.grid.UpdateGrid(this.filteredAdventures);
            this.SelectionChange(null);
        }

        private void SelectionChange(object o)
        {
            Adventure adventure = this.GetSelectedAdventure();
            this.buttonEdit.interactable = adventure != null;
        }

        private Adventure GetSelectedAdventure()
        {
            return this.grid.GetSelectedObject<Adventure>();
        }

        private void CreateAdventureOfName(object o)
        {
            string value = o as string;
            if (string.IsNullOrEmpty(value))
            {
                value = "NameUndefined";
            }
            value = EventEditorAdventures.SanitizeName(value);
            Adventure adventure = new Adventure();
            adventure.isAllowed = true;
            Module selectedModule = EventEditorModules.GetSelectedModule();
            selectedModule.nextAdventureID++;
            adventure.uniqueID = selectedModule.nextAdventureID;
            adventure.name = value;
            EventEditorModules.MarkSelectedModuleAsModified();
            adventure.AddNewNode<NodeStart>(Vector3.zero);
            if (selectedModule.adventures == null)
            {
                selectedModule.adventures = new List<Adventure>();
            }
            selectedModule.adventures.Add(adventure);
            this.UseFilter();
            this.UpdateScreen();
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonNewAdventure)
            {
                PopupName.OpenPopup((EventEditorModules.GetSelectedModule().nextAdventureID + 1).ToString(), CreateAdventureOfName, null, this);
            }
            else if (s == this.buttonCloneAdventure)
            {
                Adventure adventure = this.GetSelectedAdventure();
                if (adventure != null)
                {
                    Adventure adventure2 = adventure.Clone();
                    adventure2.name = EventEditorAdventures.SanitizeName("__" + adventure2.name);
                    this.AddAdventure(adventure2);
                    this.UseFilter();
                    this.UpdateScreen();
                }
            }
            else if (s == this.buttonCloneToModule)
            {
                EventEditorCloneToModule.OpenPopup(this, CloneToModule);
            }
            else if (s == this.buttonRename)
            {
                PopupName.OpenPopup(this.GetSelectedAdventure().name, RenameAdventureToName, null, this);
            }
            else if (s == this.buttonDelete)
            {
                PopupGeneral.OpenPopup(this, "Delete Adventure", "Are you sure?", "Delete", RemoveAdventure, "UI_CANCEL");
            }
            else if (s == this.buttonExit)
            {
                MHEventSystem.TriggerEvent(this, "Back");
                UIManager.Close(this);
                EventEditorModules.ChangesFinished();
            }
            else if (s == this.buttonEdit)
            {
                Adventure adventure3 = this.GetSelectedAdventure();
                if (adventure3 == null)
                {
                    Debug.LogWarning("Adventure selected but returned as null!");
                    return;
                }
                EventEditorAdventures.selectedAdventure = adventure3;
                adventure3.PrepareForEdit();
                MHEventSystem.TriggerEvent(this, "Advance");
            }
            else if (s == this.buttonSave)
            {
                EventEditorModules.MarkSelectedModuleAsModified();
                EventEditorModules.SaveNow();
            }
        }

        private void CloneToModule(object obj)
        {
            string moduleName = obj as string;
            if (!string.IsNullOrEmpty(moduleName))
            {
                Module module = EventEditorModules.GetAdventureLibrary().modules.Find((Module o) => o.name == moduleName);
                Adventure adventure = this.GetSelectedAdventure();
                if (adventure != null)
                {
                    Adventure adventure2 = adventure.Clone();
                    adventure2.name = EventEditorAdventures.SanitizeName(adventure2.name, null, module);
                    this.SanitizeAfterClone(adventure2);
                    this.AddAdventure(adventure2, module);
                    this.UpdateScreen();
                }
                else
                {
                    Debug.LogWarning("Nothing to clone!");
                }
            }
        }

        private void AddAdventure(Adventure a, Module m = null)
        {
            if (m == null)
            {
                m = EventEditorModules.GetSelectedModule();
            }
            if (m.adventures == null)
            {
                m.adventures = new List<Adventure>();
            }
            m.adventures.Add(a);
            m.nextAdventureID++;
            a.uniqueID = m.nextAdventureID;
            EventEditorModules.MarkModuleAsModified(m);
        }

        private void SanitizeAfterClone(Adventure a)
        {
            if (a.nodes == null)
            {
                return;
            }
            foreach (BaseNode node in a.nodes)
            {
                _ = node is NodeEnd;
            }
        }

        private void RemoveAdventure(object o)
        {
            Module selectedModule = EventEditorModules.GetSelectedModule();
            Adventure item = this.GetSelectedAdventure();
            List<Adventure> list = new List<Adventure>(selectedModule.adventures);
            if (list.Contains(item))
            {
                list.Remove(item);
                selectedModule.adventures = list;
                EventEditorModules.MarkSelectedModuleAsModified();
            }
            this.UseFilter();
            this.UpdateScreen();
        }

        private void RenameAdventureToName(object o)
        {
            string value = o as string;
            if (string.IsNullOrEmpty(value))
            {
                value = "NameUndefined";
            }
            Adventure adventure = this.GetSelectedAdventure();
            value = EventEditorAdventures.SanitizeName(value, adventure);
            _ = adventure.name;
            adventure.name = value;
            this.grid.Unselect();
            this.UseFilter();
            this.UpdateScreen();
            EventEditorModules.MarkSelectedModuleAsModified();
        }

        private void UpdateScreen()
        {
            if (this.filteredAdventures == null)
            {
                this.filteredAdventures = new List<Adventure>();
            }
            this.filteredAdventures.Sort((Adventure a, Adventure b) => a.uniqueID.CompareTo(b.uniqueID));
            this.UpdateGrid();
        }

        public static string SanitizeName(string name, Adventure adv = null, Module module = null)
        {
            if (module == null)
            {
                module = EventEditorModules.GetSelectedModule();
            }
            string text = name ?? "";
            while (module.adventures != null)
            {
                bool flag = false;
                foreach (Adventure adventure in module.adventures)
                {
                    if (adventure.name == text && adventure != adv)
                    {
                        text += "_";
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            return text;
        }
    }
}
