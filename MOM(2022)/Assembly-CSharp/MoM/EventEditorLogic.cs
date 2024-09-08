using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorLogic : ScreenBase
    {
        public static EditorNode node;

        public DropDownFilters ddFilters;

        public DropDownFilters ddProcessors;

        public DropDownFilters ddResults;

        public Button btFilters;

        public Button btProcessors;

        public Button btResults;

        public Toggle tgMainFiltering;

        public Toggle tgMainModification;

        public GameObject goTitleFilters;

        public GameObject goTitleModification;

        public GameObject goFilters;

        public GameObject goProcessors;

        public GameObject goResults;

        public GameObject goIconFilters;

        public GameObject goIconProcessors;

        public Button btExit;

        public Scrollbar scrollVertical;

        private bool modificationMode;

        private float scrollStorage;

        public override void OnStart()
        {
            base.OnStart();
            this.UpdateScreen();
            MHEventSystem.RegisterListener<AdvLogic>(LogicEvents, this);
        }

        private void LogicEvents(object sender, object e)
        {
            string text = e as string;
            AdvLogic advLogic = sender as AdvLogic;
            this.scrollStorage = this.scrollVertical.value;
            List<AdvLogic> logic = EventEditorLogic.node.GetBaseNode().logic;
            switch (text)
            {
            case "Delete":
                if (logic.Contains(advLogic))
                {
                    logic.Remove(advLogic);
                    EventEditorLogic.node.GetBaseNode().changed = true;
                }
                break;
            case "Up":
                if (logic.Contains(advLogic))
                {
                    string j = advLogic.GetType().ToString();
                    List<AdvLogic> list2 = logic.FindAll((AdvLogic o) => o.GetType().ToString() == j);
                    int num2 = list2.IndexOf(advLogic);
                    if (num2 > 0 && num2 < list2.Count)
                    {
                        int index2 = logic.IndexOf(list2[num2 - 1]);
                        logic.Remove(advLogic);
                        logic.Insert(index2, advLogic);
                    }
                }
                break;
            case "Down":
                if (logic.Contains(advLogic))
                {
                    string i = advLogic.GetType().ToString();
                    List<AdvLogic> list = logic.FindAll((AdvLogic o) => o.GetType().ToString() == i);
                    int num = list.IndexOf(advLogic);
                    if (num > -1 && num < list.Count - 1)
                    {
                        int index = logic.IndexOf(list[num + 1]);
                        logic.Remove(advLogic);
                        logic.Insert(index, advLogic);
                    }
                }
                break;
            }
            this.UpdateScreen();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (this.scrollStorage >= 0f && this.scrollVertical.value == 1f)
            {
                this.scrollVertical.value = this.scrollStorage;
                this.scrollStorage = -1f;
            }
        }

        public void UpdateScreen()
        {
            UIComponentFill.LinkDropdownEnum<LogicEntry.LEntry>(this.ddFilters, null, acceptDefaultOption: false);
            UIComponentFill.LinkDropdownEnum<LogicProcessing.LProcessing>(this.ddProcessors);
            if (this.modificationMode)
            {
                UIComponentFill.LinkDropdownEnum<LogicModifier.LModifier>(this.ddResults);
            }
            else
            {
                UIComponentFill.LinkDropdownEnum<LogicRequirement.LRequirement>(this.ddResults);
            }
            this.goTitleFilters.SetActive(!this.modificationMode);
            this.goTitleModification.SetActive(this.modificationMode);
            this.btFilters.onClick.RemoveAllListeners();
            this.btProcessors.onClick.RemoveAllListeners();
            this.btResults.onClick.RemoveAllListeners();
            this.btFilters.onClick.AddListener(delegate
            {
                EventEditorLogic.node.GetBaseNode().AddLogic(this.ddFilters.GetSelection());
                List<AdvLogic> list4 = EventEditorLogic.node.GetBaseNode().logic;
                if (list4 == null)
                {
                    list4 = new List<AdvLogic>();
                }
                this.UpdateLane(this.goFilters, list4.FindAll((AdvLogic o) => o is LogicEntry));
            });
            this.btProcessors.onClick.AddListener(delegate
            {
                EventEditorLogic.node.GetBaseNode().AddLogic(this.ddProcessors.GetSelection());
                List<AdvLogic> list3 = EventEditorLogic.node.GetBaseNode().logic;
                if (list3 == null)
                {
                    list3 = new List<AdvLogic>();
                }
                this.UpdateLane(this.goProcessors, list3.FindAll((AdvLogic o) => o is LogicProcessing));
            });
            this.btResults.onClick.AddListener(delegate
            {
                EventEditorLogic.node.GetBaseNode().AddLogic(this.ddResults.GetSelection());
                List<AdvLogic> list2 = EventEditorLogic.node.GetBaseNode().logic;
                if (list2 == null)
                {
                    list2 = new List<AdvLogic>();
                }
                if (this.modificationMode)
                {
                    this.UpdateLane(this.goResults, list2.FindAll((AdvLogic o) => o is LogicModifier));
                }
                else
                {
                    this.UpdateLane(this.goResults, list2.FindAll((AdvLogic o) => o is LogicRequirement));
                }
            });
            List<AdvLogic> list = EventEditorLogic.node.GetBaseNode().logic;
            if (list == null)
            {
                list = new List<AdvLogic>();
            }
            this.UpdateLane(this.goFilters, list.FindAll((AdvLogic o) => o is LogicEntry));
            this.UpdateLane(this.goProcessors, list.FindAll((AdvLogic o) => o is LogicProcessing));
            if (this.modificationMode)
            {
                this.UpdateLane(this.goResults, list.FindAll((AdvLogic o) => o is LogicModifier));
            }
            else
            {
                this.UpdateLane(this.goResults, list.FindAll((AdvLogic o) => o is LogicRequirement));
            }
        }

        private void UpdateLane(GameObject go, List<AdvLogic> logic)
        {
            GameObjectUtils.RemoveChildren(go.transform);
            if (logic == null)
            {
                return;
            }
            foreach (AdvLogic item in logic)
            {
                GameObject go2 = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetLogicComponent(item.GetNodeName()), go.transform);
                item.PopulateGO(go2, EventEditorLogic.node);
            }
            EventEditorLogic.node.GetBaseNode().changed = true;
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (!(s == this.btFilters) && !(s == this.btProcessors) && !(s == this.btResults))
            {
                if (s == this.tgMainFiltering && this.tgMainFiltering.isOn)
                {
                    this.modificationMode = false;
                    this.UpdateScreen();
                }
                else if (s == this.tgMainModification && this.tgMainModification.isOn)
                {
                    this.modificationMode = true;
                    this.UpdateScreen();
                }
                else if (s == this.btExit)
                {
                    MHEventSystem.TriggerEvent(this, "Back");
                    MHEventSystem.TriggerEvent(this, "Redraw");
                    UIManager.Close(this);
                }
            }
        }
    }
}
