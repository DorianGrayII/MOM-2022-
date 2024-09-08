namespace MOM.Adventures
{
    using MHUtils;
    using System;
    using System.Xml.Serialization;
    using UnityEngine;

    [XmlInclude(typeof(LogicEntry)), XmlInclude(typeof(LogicModifier)), XmlInclude(typeof(LogicProcessing)), XmlInclude(typeof(LogicRequirement))]
    public abstract class AdvLogic
    {
        protected AdvLogic()
        {
        }

        public abstract string GetNodeName();
        public abstract string GetScriptName();
        public abstract void PopulateGO(GameObject go, EditorNode node);
        protected void UpdateLogicBlocks(object o)
        {
            MHEventSystem.TriggerEvent<AdvLogic>(this, "LogicUpdate");
        }
    }
}

