namespace MOM.Adventures
{
    using MOM;
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using UnityEngine;

    public class AdvOutput
    {
        [XmlAttribute, DefaultValue((string) null)]
        public string name;
        [XmlAttribute]
        public int ownerID;
        [XmlAttribute]
        public int targetID;
        [XmlAttribute, DefaultValue(0)]
        public GroupHint group;
        [XmlAttribute]
        public float fromX;
        [XmlAttribute]
        public float fromY;
        [XmlIgnore]
        private BaseNode owner;
        [XmlIgnore]
        private BaseNode target;

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

        public BaseNode GetOwner(Adventure adventure)
        {
            if (this.ownerID <= 0)
            {
                this.owner = null;
                return null;
            }
            if (((this.owner == null) || (this.ownerID != this.owner.ID)) && ((adventure != null) && (adventure.nodes != null)))
            {
                this.owner = adventure.nodes.Find(o => o.ID == this.ownerID);
            }
            return this.owner;
        }

        public BaseNode GetTarget(Adventure adventure)
        {
            if (this.targetID <= 0)
            {
                this.target = null;
                return null;
            }
            if (((this.target == null) || (this.targetID != this.target.ID)) && ((adventure != null) && (adventure.nodes != null)))
            {
                this.target = adventure.nodes.Find(o => o.ID == this.targetID);
            }
            return this.target;
        }

        internal int Test(BaseNode owner)
        {
            Adventure parentEvent = owner.parentEvent;
            int num = 0;
            if (this.GetTarget(parentEvent) == null)
            {
                string[] textArray1 = new string[9];
                textArray1[0] = "[EDITOR] Node ";
                textArray1[1] = owner.ID.ToString();
                textArray1[2] = " in event (";
                textArray1[3] = parentEvent.uniqueID.ToString();
                textArray1[4] = ")";
                textArray1[5] = parentEvent.name;
                textArray1[6] = " in module ";
                textArray1[7] = parentEvent.module.name;
                textArray1[8] = " have Output which lacks target!";
                Debug.LogWarning(string.Concat(textArray1));
                num++;
            }
            if ((owner is NodeStory) && string.IsNullOrEmpty(this.name))
            {
                string[] textArray2 = new string[9];
                textArray2[0] = "[EDITOR] Node ";
                textArray2[1] = owner.ID.ToString();
                textArray2[2] = " in event (";
                textArray2[3] = parentEvent.uniqueID.ToString();
                textArray2[4] = ")";
                textArray2[5] = parentEvent.name;
                textArray2[6] = " in module ";
                textArray2[7] = parentEvent.module.name;
                textArray2[8] = " have Output which lacks text!";
                Debug.LogWarning(string.Concat(textArray2));
                num++;
            }
            return num;
        }

        public override string ToString()
        {
            string text1;
            if (this.owner == null)
            {
                string[] textArray2 = new string[] { "OUTPUT (", this.ownerID.ToString(), "->", this.targetID.ToString(), ") ", this.name };
                return string.Concat(textArray2);
            }
            string[] textArray1 = new string[8];
            textArray1[0] = "OUTPUT (";
            textArray1[1] = this.ownerID.ToString();
            textArray1[2] = "->";
            textArray1[3] = this.targetID.ToString();
            textArray1[4] = ") ";
            textArray1[5] = this.name;
            textArray1[6] = "/ ";
            string[] textArray3 = textArray1;
            if (this.owner != null)
            {
                text1 = this.owner.ToString();
            }
            else
            {
                BaseNode owner = this.owner;
                text1 = null;
            }
            textArray1[7] = text1;
            return string.Concat(textArray1);
        }

        public void UpdateCache(Adventure adventure)
        {
            if (((this.ownerID != 0) || (this.owner != null)) && ((this.owner == null) || (this.owner.ID != this.ownerID)))
            {
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
                if (((this.targetID != 0) || (this.target != null)) && ((this.target == null) || (this.target.ID != this.targetID)))
                {
                    if (this.targetID < 1)
                    {
                        this.target = null;
                        this.targetID = 0;
                    }
                    else
                    {
                        BaseNode node = adventure.GetNode(this.targetID);
                        if (node == null)
                        {
                            this.targetID = 0;
                        }
                        else
                        {
                            this.target = node;
                        }
                    }
                }
            }
        }

        [XmlIgnore]
        public float FromX
        {
            get
            {
                return this.fromX;
            }
            set
            {
                this.fromX = (int) value;
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
                this.fromY = (int) value;
            }
        }

        public enum GroupHint
        {
            None,
            G1,
            G2,
            G3,
            G4,
            G5,
            G6,
            G7,
            G8
        }
    }
}

