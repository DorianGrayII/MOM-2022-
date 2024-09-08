namespace MOM
{
    using MHUtils.UI;
    using MOM.Adventures;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.UI;

    public class EventEditorCloneToModule : ScreenBase
    {
        public DropDownFilters moduleSelector;
        public Button buttonConfirm;
        public Button buttonCancel;
        private Callback cancel;
        private Callback confirm;
        private System.Type type;
        private string selectedOption;

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonCancel)
            {
                UIManager.Close(this);
                if (this.cancel != null)
                {
                    this.cancel(null);
                }
            }
            else if (s == this.buttonConfirm)
            {
                if (!string.IsNullOrEmpty(this.selectedOption))
                {
                    if (this.confirm != null)
                    {
                        this.confirm(this.selectedOption);
                    }
                    else if (this.confirm != null)
                    {
                        this.confirm(null);
                    }
                }
                UIManager.Close(this);
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            List<string> listOptions = new List<string>();
            if (this.type == typeof(Module))
            {
                if ((EventEditorModules.GetAdventureLibrary() != null) && (EventEditorModules.GetAdventureLibrary().modules != null))
                {
                    EventEditorModules.GetAdventureLibrary().modules.ForEach(o => listOptions.Add(o.name));
                }
            }
            else if ((this.type == typeof(Adventure)) && ((EventEditorModules.GetSelectedModule() != null) && (EventEditorModules.GetSelectedModule().adventures != null)))
            {
                EventEditorModules.GetSelectedModule().adventures.ForEach(o => listOptions.Add(o.name));
            }
            this.moduleSelector.SetOptions(listOptions, true, true);
            this.moduleSelector.onChange = o => this.selectedOption = o as string;
        }

        public static void OpenPopup(ScreenBase parent, Callback confirm, Callback cancel, System.Type type)
        {
            EventEditorCloneToModule local1 = UIManager.Open<EventEditorCloneToModule>(UIManager.Layer.Popup, parent);
            local1.cancel = cancel;
            local1.confirm = confirm;
            if (type == null)
            {
                type = typeof(Module);
            }
            local1.type = type;
        }
    }
}

