using System;
using System.Collections.Generic;
using MHUtils.UI;
using MOM.Adventures;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorCloneToModule : ScreenBase
    {
        public DropDownFilters moduleSelector;

        public Button buttonConfirm;

        public Button buttonCancel;

        private Callback cancel;

        private Callback confirm;

        private Type type;

        private string selectedOption;

        public static void OpenPopup(ScreenBase parent, Callback confirm = null, Callback cancel = null, Type type = null)
        {
            EventEditorCloneToModule eventEditorCloneToModule = UIManager.Open<EventEditorCloneToModule>(UIManager.Layer.Popup, parent);
            eventEditorCloneToModule.cancel = cancel;
            eventEditorCloneToModule.confirm = confirm;
            if (type == null)
            {
                type = typeof(Module);
            }
            eventEditorCloneToModule.type = type;
        }

        public override void OnStart()
        {
            base.OnStart();
            List<string> listOptions = new List<string>();
            if (this.type == typeof(Module))
            {
                if (EventEditorModules.GetAdventureLibrary() != null && EventEditorModules.GetAdventureLibrary().modules != null)
                {
                    EventEditorModules.GetAdventureLibrary().modules.ForEach(delegate(Module o)
                    {
                        listOptions.Add(o.name);
                    });
                }
            }
            else if (this.type == typeof(Adventure) && EventEditorModules.GetSelectedModule() != null && EventEditorModules.GetSelectedModule().adventures != null)
            {
                EventEditorModules.GetSelectedModule().adventures.ForEach(delegate(Adventure o)
                {
                    listOptions.Add(o.name);
                });
            }
            this.moduleSelector.SetOptions(listOptions);
            this.moduleSelector.onChange = delegate(object o)
            {
                this.selectedOption = o as string;
            };
        }

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
            else
            {
                if (!(s == this.buttonConfirm))
                {
                    return;
                }
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
    }
}
