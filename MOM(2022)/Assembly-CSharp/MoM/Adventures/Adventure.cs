// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Adventures.Adventure
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MOM;
using MOM.Adventures;
using UnityEngine;

public class Adventure
{
    public enum AdventureTriggerType
    {
        None = 0,
        PerPlayer = 1
    }

    public enum AdventurePositiveType
    {
        Neutral = 0,
        Positive = 1,
        Negative = 2
    }

    [XmlAttribute]
    public bool isAllowed;

    [XmlAttribute]
    public string name;

    [XmlAttribute]
    public int uniqueID;

    [XmlAttribute]
    public int nextNodeID;

    [XmlElement]
    public List<BaseNode> nodes;

    [XmlIgnore]
    public Module module;

    public BaseNode AddNewNode(BaseNode bn, Vector3 pos)
    {
        if (this.nodes == null)
        {
            this.nodes = new List<BaseNode>();
        }
        bn.PositionX = (int)pos.x;
        bn.PositionY = (int)pos.y;
        bn.ID = ++this.nextNodeID;
        this.nodes.Add(bn);
        bn.parentEvent = this;
        return bn;
    }

    public BaseNode AddNewNode(Type t, Vector3 pos)
    {
        if (this.nodes == null)
        {
            this.nodes = new List<BaseNode>();
        }
        BaseNode baseNode = Activator.CreateInstance(t) as BaseNode;
        baseNode.PositionX = (int)pos.x;
        baseNode.PositionY = (int)pos.y;
        baseNode.ID = ++this.nextNodeID;
        this.nodes.Add(baseNode);
        baseNode.InitializeOutputs();
        baseNode.parentEvent = this;
        return baseNode;
    }

    public BaseNode AddNewNode<T>(Vector3 pos) where T : BaseNode, new()
    {
        return this.AddNewNode(typeof(T), pos);
    }

    public void PrepareForEdit()
    {
        if (this.nodes == null)
        {
            this.nodes = new List<BaseNode>();
        }
        this.nodes.ForEach(delegate(BaseNode o)
        {
            o.PrepareForEdit(this);
        });
    }

    public Adventure Clone()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Adventure));
        using (MemoryStream memoryStream = new MemoryStream())
        {
            xmlSerializer.Serialize(memoryStream, this);
            memoryStream.Position = 0L;
            Adventure result = xmlSerializer.Deserialize(memoryStream) as Adventure;
            EventEditorModules.GetSelectedModule();
            return result;
        }
    }

    public string GetGraphic()
    {
        if (this.nodes == null || this.nodes.Count < 1)
        {
            return null;
        }
        return this.nodes[0].image;
    }

    public BaseNode GetNode(int ID)
    {
        if (this.nodes == null)
        {
            return null;
        }
        return this.nodes.Find((BaseNode o) => o.ID == ID);
    }

    public NodeStart GetStart()
    {
        return this.nodes[0] as NodeStart;
    }

    public int Test(Module m)
    {
        this.module = m;
        int num = 0;
        if (this.uniqueID <= 0)
        {
            Debug.LogWarning("[EDITOR] Event (" + this.uniqueID + ")" + this.name + " in module " + this.module.name + " has invalid ID!");
            num++;
        }
        if (this.nodes == null || this.nodes.Count < 2)
        {
            Debug.LogWarning("[EDITOR] Event (" + this.uniqueID + ")" + this.name + " in module " + this.module.name + " lacks Nodes!");
            return num + 1;
        }
        foreach (BaseNode node in this.nodes)
        {
            node.PrepareForEdit(this);
        }
        foreach (BaseNode node2 in this.nodes)
        {
            num += node2.Test(this);
        }
        return num;
    }

    internal void PrepareForGame()
    {
        if (this.nodes != null)
        {
            this.nodes.ForEach(delegate(BaseNode o)
            {
                o.parentEvent = this;
            });
        }
    }

    internal HashSet<string> GetAvaliableListNames(BaseNode node)
    {
        HashSet<string> localAvaliableListNames = node.GetLocalAvaliableListNames(onlyPublic: false, onlyPrivate: false);
        HashSet<BaseNode> hashSet = new HashSet<BaseNode>();
        List<BaseNode> list = new List<BaseNode>();
        hashSet.Add(node);
        if (node.GetInputs() != null)
        {
            list.AddRange(node.GetInputs());
        }
        while (list.Count > 0)
        {
            BaseNode baseNode = list[list.Count - 1];
            list.Remove(baseNode);
            if (hashSet.Contains(baseNode))
            {
                continue;
            }
            hashSet.Add(baseNode);
            foreach (string localAvaliableListName in baseNode.GetLocalAvaliableListNames(onlyPublic: true, onlyPrivate: false))
            {
                localAvaliableListNames.Add(localAvaliableListName);
            }
            if (baseNode.GetInputs() != null)
            {
                list.AddRange(baseNode.GetInputs());
            }
        }
        return localAvaliableListNames;
    }

    internal void ReAcquireModule()
    {
        if (this.module == null && AdventureLibrary.currentLibrary != null && AdventureLibrary.currentLibrary.modules != null)
        {
            foreach (Module module in AdventureLibrary.currentLibrary.modules)
            {
                if (module.adventures != null && module.adventures.Find((Adventure o) => o == this) != null)
                {
                    this.module = module;
                    Debug.Log("ReAcquired module " + module.name + " for adventure " + this.name);
                    break;
                }
            }
        }
        if (this.module == null)
        {
            Debug.LogError("Cannot acquire module for adventure " + this.name);
        }
    }

    public override string ToString()
    {
        return "[" + this.name + "(id " + this.uniqueID + ") module: " + this.module.name + "]";
    }
}
