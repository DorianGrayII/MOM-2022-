using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

namespace MOM.Adventures
{
    public class AdvOutput
    {
        public enum GroupHint
        {
            None = 0,
            G1 = 1,
            G2 = 2,
            G3 = 3,
            G4 = 4,
            G5 = 5,
            G6 = 6,
            G7 = 7,
            G8 = 8
        }

        [XmlAttribute]
        [DefaultValue(null)]
        public string name;

        [XmlAttribute]
        public int ownerID;

        [XmlAttribute]
        public int targetID;

        [XmlAttribute]
        [DefaultValue(GroupHint.None)]
        public GroupHint group;

        [XmlAttribute]
        public float fromX;

        [XmlAttribute]
        public float fromY;

        [XmlIgnore]
        private BaseNode owner;

        [XmlIgnore]
        private BaseNode target;

        [XmlIgnore]
        public float FromX
        {
            get
            {
                return this.fromX;
            }
            set
            {
                this.fromX = (int)value;
            }
        }

        [XmlIgnore]
        public float FromY
        {
            get
            {
                return this.fromY;
            }
            set
            {
                this.fromY = (int)value;
            }
        }

        public BaseNode GetTarget(Adventure adventure)
        {
            if (this.targetID <= 0)
            {
                this.target = null;
                return null;
            }
            if (this.target != null && this.targetID == this.target.ID)
            {
                return this.target;
            }
            if (adventure != null && adventure.nodes != null)
            {
                this.target = adventure.nodes.Find((BaseNode o) => o.ID == this.targetID);
            }
            return this.target;
        }

        public BaseNode GetOwner(Adventure adventure)
        {
            if (this.ownerID <= 0)
            {
                this.owner = null;
                return null;
            }
            if (this.owner != null && this.ownerID == this.owner.ID)
            {
                return this.owner;
            }
            if (adventure != null && adventure.nodes != null)
            {
                this.owner = adventure.nodes.Find((BaseNode o) => o.ID == this.ownerID);
            }
            return this.owner;
        }

        public void UpdateCache(Adventure adventure)
        {
            if ((this.ownerID == 0 && this.owner == null) || (this.owner != null && this.owner.ID == this.ownerID))
            {
                return;
            }
            if (this.ownerID < 1)
            {
                this.owner = null;
                this.ownerID = 0;
            }
            else
            {
                BaseNode node = adventure.GetNode(this.ownerID);
                if (node == null)
                {
                    this.ownerID = 0;
                }
                else
                {
                    this.owner = node;
                }
            }
            if ((this.targetID == 0 && this.target == null) || (this.target != null && this.target.ID == this.targetID))
            {
                return;
            }
            if (this.targetID < 1)
            {
                this.target = null;
                this.targetID = 0;
                return;
            }
            BaseNode node2 = adventure.GetNode(this.targetID);
            if (node2 == null)
            {
                this.targetID = 0;
            }
            else
            {
                this.target = node2;
            }
        }

        public void Connect(BaseNode target)
        {
            this.Disconnect();
            if (target != null)
            {
                target.GetInputs().Add(this.owner);
                this.targetID = target.ID;
                this.target = target;
                this.UpdateCache(EventEditorAdventures.selectedAdventure);
            }
        }

        public void Disconnect()
        {
            Adventure selectedAdventure = EventEditorAdventures.selectedAdventure;
            if (this.owner == null)
            {
                this.UpdateCache(selectedAdventure);
            }
            if (this.target != null)
            {
                if (this.target.GetInputs().Contains(this.owner))
                {
                    this.target.GetInputs().Remove(this.owner);
                }
                this.targetID = 0;
                this.target = null;
                this.UpdateCache(selectedAdventure);
            }
        }

        public override string ToString()
        {
            if (this.owner != null)
            {
                return "OUTPUT (" + this.ownerID + "->" + this.targetID + ") " + this.name + "/ " + this.owner;
            }
            return "OUTPUT (" + this.ownerID + "->" + this.targetID + ") " + this.name;
        }

        internal int Test(BaseNode owner)
        {
            Adventure parentEvent = owner.parentEvent;
            BaseNode baseNode = this.GetTarget(parentEvent);
            int num = 0;
            if (baseNode == null)
            {
                Debug.LogWarning("[EDITOR] Node " + owner.ID + " in event (" + parentEvent.uniqueID + ")" + parentEvent.name + " in module " + parentEvent.module.name + " have Output which lacks target!");
                num++;
            }
            if (owner is NodeStory && string.IsNullOrEmpty(this.name))
            {
                Debug.LogWarning("[EDITOR] Node " + owner.ID + " in event (" + parentEvent.uniqueID + ")" + parentEvent.name + " in module " + parentEvent.module.name + " have Output which lacks text!");
                num++;
            }
            return num;
        }
    }
}
