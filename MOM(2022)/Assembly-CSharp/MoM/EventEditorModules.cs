using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorModules : ScreenBase
    {
        private static AdventureLibrary editorLibrary;

        private static Module selectedModule;

        private static int selectedModuleHash;

        private static int selectedModuleLength;

        public GridItemManager grid;

        public Button buttonEdit;

        public Button buttonExit;

        public Button buttonNewModule;

        public Button buttonCloneModule;

        public Button buttonRename;

        public Button buttonImport;

        public Button buttonExport;

        public Button buttonTest;

        public Button buttonDelete;

        private bool updateWaiting;

        protected override void Awake()
        {
            this.grid.CustomDynamicItem(GridItem, UpdateGrid);
            this.grid.onSelectionChange = SelectionChange;
            this.UpdateGrid();
            this.updateWaiting = true;
            AdventureLibrary.LoadModulesFromDrive(ModulesReady, ModulesLoadingFailed);
        }

        public override IEnumerator PreClose()
        {
            yield return base.PreClose();
            EventEditorModules.editorLibrary = null;
            EventEditorModules.selectedModule = null;
            ImageLibrary.Destroy();
        }

        public static AdventureLibrary GetAdventureLibrary()
        {
            return EventEditorModules.editorLibrary;
        }

        public static Module GetSelectedModule()
        {
            return EventEditorModules.selectedModule;
        }

        private void ModulesLoadingFailed(object o)
        {
            Debug.LogError("Loading modules failed! \n" + o);
        }

        private void ModulesReady(object o)
        {
            EventEditorModules.editorLibrary = o as AdventureLibrary;
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (this.updateWaiting && EventEditorModules.editorLibrary != null)
            {
                this.updateWaiting = false;
                this.UpdateScreen();
            }
        }

        private void UpdateGrid()
        {
            if (EventEditorModules.editorLibrary == null)
            {
                this.grid.UpdateGrid(new List<Module>());
            }
            else
            {
                this.grid.UpdateGrid(EventEditorModules.editorLibrary.modules);
            }
            this.SelectionChange(null);
        }

        private void SelectionChange(object o)
        {
            Module selectedObject = this.grid.GetSelectedObject<Module>();
            this.buttonEdit.interactable = selectedObject != null;
            if (selectedObject != null)
            {
                EventEditorModules.selectedModule = selectedObject;
            }
        }

        private void GridItem(GameObject itemSource, object source, object data, int index)
        {
            Module i = source as Module;
            GIToggleTField component = itemSource.GetComponent<GIToggleTField>();
            component.tf.text = i.name;
            component.toggle.onValueChanged.RemoveAllListeners();
            component.toggle.isOn = i.isAllowed;
            component.toggle.onValueChanged.AddListener(delegate(bool b)
            {
                i.isAllowed = b;
                EventEditorModules.MarkModuleAsModified(i);
            });
        }

        private static void AddModule(Module m)
        {
            EventEditorModules.editorLibrary.modules.Add(m);
        }

        public static void MarkSelectedModuleAsModified()
        {
            EventEditorModules.editorLibrary.modulesModified.Add(EventEditorModules.selectedModule.uniqueID);
        }

        public static void MarkModuleAsModified(Module m)
        {
            EventEditorModules.editorLibrary.modulesModified.Add(m.uniqueID);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (EventEditorModules.editorLibrary == null)
            {
                return;
            }
            if (s == this.buttonNewModule)
            {
                PopupName.OpenPopup("ID" + global::UnityEngine.Random.Range(0, int.MaxValue), CreateModuleOfName, null, this);
            }
            else if (s == this.buttonExit)
            {
                if (EventEditorModules.editorLibrary.modulesModified.Count > 0 || EventEditorModules.editorLibrary.modulesDeleted.Count > 0)
                {
                    PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_SAVE_OR_DISCARD", "UI_SAVE_AND_EXIT", SaveAndExit, "UI_DISCARD_AND_EXIT", UndoAndExit, "UI_STAY_AND_EDIT");
                }
                else
                {
                    this.CancelAndExit();
                }
            }
            else if (s == this.buttonCloneModule)
            {
                Module module = EventEditorModules.GetSelectedModule();
                if (module != null)
                {
                    Module module2 = module.Clone();
                    module2.name = this.SanitizeModuleName("__" + module2.name);
                    EventEditorModules.AddModule(module2);
                    this.grid.Unselect();
                    EventEditorModules.MarkModuleAsModified(module2);
                    this.UpdateScreen();
                }
                else
                {
                    Debug.LogWarning("Nothing to clone!");
                }
            }
            else if (s == this.buttonRename)
            {
                PopupName.OpenPopup(EventEditorModules.GetSelectedModule().name, RenameModuleToName, null, this);
            }
            else if (s == this.buttonImport)
            {
                PopupGeneral.OpenPopup(this, "UI_IMPORT", "UI_WHICH_FORM_OF_IMPORT", "UI_SINGLE_MODE", SingleModeImport, "UI_CANCEL", null, "UI_BUNDLE_MODE", ImportBlockOfModules);
            }
            else if (s == this.buttonExport)
            {
                PopupGeneral.OpenPopup(this, "UI_EXPORT", "UI_WHICH_FORM_OF_EXPORT", "UI_SINGLE_MODE", SingleModeExport, "UI_CANCEL", null, "UI_BUNDLE_MODE", ExportSelectedModules);
            }
            else if (s == this.buttonDelete)
            {
                PopupGeneral.OpenPopup(this, "Delete Module", "Are you sure?", "Delete", RemoveModule, "UI_CANCEL");
            }
            else if (s == this.buttonEdit)
            {
                Module module3 = EventEditorModules.GetSelectedModule();
                if (module3 == null)
                {
                    Debug.LogWarning("Module selected but returned as null!");
                    return;
                }
                MHEventSystem.TriggerEvent(this, "Advance");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Module));
                MemoryStream memoryStream = new MemoryStream();
                xmlSerializer.Serialize(memoryStream, module3);
                memoryStream.Position = 0L;
                EventEditorModules.selectedModuleHash = Convert.ToBase64String(memoryStream.GetBuffer()).GetHashCode();
                EventEditorModules.selectedModuleLength = memoryStream.GetBuffer().Length;
                memoryStream.Dispose();
            }
            else if (s == this.buttonTest)
            {
                Module module4 = EventEditorModules.GetSelectedModule();
                if (module4 == null)
                {
                    Debug.Log("--- Testing Module failed, no module selected!");
                    PopupGeneral.OpenPopup(null, "Testing", "Testing Module failed, no module selected!", "UI_OKAY");
                }
                else
                {
                    Debug.Log("--- Testing Module " + module4.name + " started");
                    module4.Test();
                    Debug.Log("--- Testing Module " + module4.name + " ended");
                }
            }
        }

        private void RemoveModule(object o)
        {
            Module module = EventEditorModules.GetSelectedModule();
            List<Module> list = new List<Module>(EventEditorModules.editorLibrary.modules);
            if (list.Contains(module))
            {
                list.Remove(module);
                EventEditorModules.editorLibrary.modules = list;
                EventEditorModules.editorLibrary.modulesDeleted.Add(module.name);
            }
            this.grid.Unselect();
            this.UpdateScreen();
        }

        private string SanitizeModuleName(string name, Module m = null)
        {
            string text = name ?? "";
            List<Module> modules = EventEditorModules.editorLibrary.modules;
            while (modules != null)
            {
                bool flag = false;
                foreach (Module item in modules)
                {
                    if (item.name == text && item != m)
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

        public static void GenerateUniqueID(Module m)
        {
            do
            {
                m.uniqueID = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                if (EventEditorModules.editorLibrary.modules == null)
                {
                    EventEditorModules.editorLibrary.modules = new List<Module>();
                }
            }
            while (EventEditorModules.editorLibrary.modules.Find((Module o) => o.uniqueID == m.uniqueID) != null);
        }

        private void CreateModuleOfName(object obj)
        {
            string value = obj as string;
            if (string.IsNullOrEmpty(value))
            {
                value = "NameUndefined";
            }
            value = this.SanitizeModuleName(value);
            Module m = new Module
            {
                name = value,
                isAllowed = true
            };
            EventEditorModules.GenerateUniqueID(m);
            EventEditorModules.AddModule(m);
            EventEditorModules.MarkModuleAsModified(m);
            this.UpdateScreen();
        }

        private void RenameModuleToName(object o)
        {
            string value = o as string;
            if (string.IsNullOrEmpty(value))
            {
                value = "NameUndefined";
            }
            Module module = EventEditorModules.GetSelectedModule();
            value = this.SanitizeModuleName(value, module);
            EventEditorModules.MarkSelectedModuleAsModified();
            string item = module.name;
            module.name = value;
            this.grid.Unselect();
            this.UpdateScreen();
            EventEditorModules.editorLibrary.modulesDeleted.Add(item);
        }

        private void UpdateScreen()
        {
            EventEditorModules.editorLibrary.modules.Sort(delegate(Module a, Module b)
            {
                if (string.IsNullOrEmpty(a.name))
                {
                    return -1;
                }
                return string.IsNullOrEmpty(b.name) ? 1 : a.name.CompareTo(b.name);
            });
            this.buttonExport.interactable = EventEditorModules.editorLibrary.modules.Count > 0;
            this.UpdateGrid();
        }

        private void SingleModeImport(object o)
        {
            ModulesImport.SingleModeImport(SingleImportReady, SingleImportFailed);
        }

        private void SingleImportReady()
        {
            PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_IMPORTED_MODULE", "UI_OK");
        }

        private void SingleImportFailed()
        {
            PopupGeneral.OpenPopup(this, "UI_ERROR", "UI_IMPORT_ERROR", "UI_OK");
        }

        private void ImportBlockOfModules(object o)
        {
            ModulesImport.ImportBlockOfModules(EventEditorModules.editorLibrary.modules, ImportBlockReady, ImportBlockFailed);
        }

        private void ImportBlockReady()
        {
            PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_IMPORTED_MODULE_BLOCK", "UI_OK");
        }

        private void ImportBlockFailed()
        {
            PopupGeneral.OpenPopup(this, "UI_ERROR", "UI_IMPORT_MODULE_BLOCK_WITH_ERRORS", "UI_OK");
        }

        private void SingleModeExport(object o)
        {
            ModulesImport.SingleModeExport(SingleExportReady, SingleExportFailed);
        }

        private void SingleExportReady()
        {
            PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_EXPORTED_MODULE", "UI_OK");
        }

        private void SingleExportFailed()
        {
            PopupGeneral.OpenPopup(this, "UI_ERROR", "UI_EXPORT_ERROR", "UI_OK");
        }

        private void ExportSelectedModules(object o)
        {
            ModulesImport.ExportSelectedModules(EventEditorModules.editorLibrary.modules, ExportSelectedReady, ExportSelectedFailed);
        }

        private void ExportSelectedReady()
        {
            PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_EXPORTED_MODULE_BLOCK", "UI_OK");
        }

        private void ExportSelectedFailed()
        {
            PopupGeneral.OpenPopup(this, "UI_ERROR", "UI_EXPORT_MODULE_BLOCK_WITH_ERRORS ", "UI_OK");
        }

        public static void ChangesFinished()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Module));
            MemoryStream memoryStream = new MemoryStream();
            xmlSerializer.Serialize(memoryStream, EventEditorModules.GetSelectedModule());
            memoryStream.Position = 0L;
            int hashCode = Convert.ToBase64String(memoryStream.GetBuffer()).GetHashCode();
            if (EventEditorModules.selectedModuleLength != memoryStream.GetBuffer().Length || EventEditorModules.selectedModuleHash != hashCode)
            {
                EventEditorModules.MarkSelectedModuleAsModified();
            }
        }

        public static void SaveNow()
        {
            EventEditorModules.editorLibrary.Save();
        }

        private void SaveAndExit(object o)
        {
            EventEditorModules.editorLibrary.Save();
            AdventureLibrary.currentLibrary = EventEditorModules.editorLibrary;
            MHEventSystem.TriggerEvent(this, "Back");
        }

        private void UndoAndExit(object o)
        {
            AdventureLibrary.LoadModulesFromDrive(ModulesReady, ModulesLoadingFailed);
            MHEventSystem.TriggerEvent(this, "Back");
        }

        private void CancelAndExit()
        {
            MHEventSystem.TriggerEvent(this, "Back");
        }
    }
}
