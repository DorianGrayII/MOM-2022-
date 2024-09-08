using System.Xml.Serialization;
using MHUtils;
using UnityEngine;

namespace MOM.Adventures
{
    [XmlInclude(typeof(LogicEntry))]
    [XmlInclude(typeof(LogicModifier))]
    [XmlInclude(typeof(LogicProcessing))]
    [XmlInclude(typeof(LogicRequirement))]
    public abstract class AdvLogic
    {
        public abstract string GetNodeName();

        public abstract void PopulateGO(GameObject go, EditorNode node);

        protected void UpdateLogicBlocks(object o)
        {
            MHEventSystem.TriggerEvent<AdvLogic>(this, "LogicUpdate");
        }

        public abstract string GetScriptName();
    }
}
