using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM.Adventures
{
    public class EditorNode : MonoBehaviour
    {
        private BaseNode node;

        private Adventure adventure;

        public GameObject heading;

        public GridItemManager outputGrid;

        public Button btLogicEditor;

        public Button btNewOutput;

        public Button btSelectImage;

        public Button btDeleteNode;

        public Button btDeleteImage;

        public Button btCloneNode;

        public Button btCloneToAdventure;

        public RawImage image;

        private Dictionary<string, LogicEntry.LEntry> avaliableLists;

        private float listValidityForFrame;

        private void Awake()
        {
            if (this.outputGrid != null)
            {
                this.outputGrid.CustomDynamicItem(OutputItem);
            }
        }

        private void Start()
        {
            if (this.btLogicEditor != null)
            {
                this.btLogicEditor.onClick.AddListener(delegate
                {
                    EventEditorLogic.node = this;
                    MHEventSystem.TriggerEvent<EditorNode>(this, "LogicEditor");
                });
            }
            if (this.btNewOutput != null)
            {
                this.btNewOutput.onClick.AddListener(delegate
                {
                    if (this.node != null)
                    {
                        this.node.AddOutput();
                    }
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                });
            }
            if (this.btSelectImage != null)
            {
                this.btSelectImage.onClick.AddListener(delegate
                {
                    EventEditorImage.OpenImageSelection(base.GetComponentInParent<ScreenBase>(), SelectImage);
                });
            }
            if (this.btDeleteImage != null)
            {
                this.btDeleteImage.onClick.AddListener(delegate
                {
                    this.GetBaseNode().image = null;
                    this.UpdateImage();
                });
            }
            if (this.btDeleteNode != null)
            {
                this.btDeleteNode.onClick.AddListener(delegate
                {
                    Adventure selectedAdventure = EventEditorAdventures.selectedAdventure;
                    if (selectedAdventure.nodes != null && selectedAdventure.nodes.Contains(this.GetBaseNode()))
                    {
                        this.GetBaseNode().DisconnectAll();
                        selectedAdventure.nodes.Remove(this.GetBaseNode());
                        EventEditorGraph.instance.RedrawGraph();
                    }
                });
            }
            if (this.btCloneNode != null)
            {
                this.btCloneNode.onClick.AddListener(delegate
                {
                    this.CloneToEvent(this.adventure);
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                });
            }
            if (this.btCloneToAdventure != null)
            {
                this.btCloneToAdventure.onClick.AddListener(delegate
                {
                    EventEditorCloneToModule.OpenPopup(base.GetComponentInParent<ScreenBase>(), CloneToEvent, null, typeof(Adventure));
                });
            }
            this.InitializeHeader();
        }

        public void SelectImage(object o)
        {
            Texture2D texture2D = o as Texture2D;
            if (texture2D != null)
            {
                this.GetBaseNode().image = texture2D.name;
            }
            else
            {
                this.GetBaseNode().image = null;
            }
            this.UpdateImage();
        }

        private void UpdateImage()
        {
            this.GetBaseNode().changed = true;
            MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
            if (this.GetBaseNode() is NodeStart)
            {
                MHEventSystem.TriggerEvent<EditorNode>(this, "AdventureUpdate");
            }
        }

        public void UpdateGraphic()
        {
            if (!(this.image == null))
            {
                this.image.texture = ImageLibrary.RequestSingleImage(this.GetBaseNode().image);
            }
        }

        private void OutputItem(GameObject itemSource, object source, object data, int index)
        {
            AdvOutput ao = source as AdvOutput;
            EditorNode en = data as EditorNode;
            if (ao == null)
            {
                return;
            }
            Button button = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonDelete");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                MHEventSystem.TriggerEvent<EditorNode>(this, new Multitype<string, object, GameObject>("ButtonDelete", source, itemSource));
            });
            TMP_InputField tMP_InputField = GameObjectUtils.FindByNameGetComponent<TMP_InputField>(itemSource, "InputAnswer", onlyDirectChildren: true);
            GameObjectUtils.FindByNameGetComponent<TextMeshProUGUI>(tMP_InputField.gameObject, "Text");
            BaseNode baseNode = en.node;
            if (tMP_InputField != null)
            {
                tMP_InputField.onValueChanged.RemoveAllListeners();
                baseNode.RequiresOutputText();
                if (string.IsNullOrEmpty(ao.name))
                {
                    tMP_InputField.text = "";
                }
                else
                {
                    tMP_InputField.text = ao.name;
                }
                tMP_InputField.onValueChanged.AddListener(delegate(string s)
                {
                    ao.name = s;
                });
            }
            button = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "SocketOut");
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    MHEventSystem.TriggerEvent<EditorNode>(data, new Multitype<string, object, GameObject>("SocketOut", source, itemSource));
                });
            }
            UIComponentFill.LinkDropdownEnum<AdvOutput.GroupHint>(GameObjectUtils.FindByNameGetComponent<DropDownFilters>(itemSource, "DropdownDialogueGroup").gameObject, ao, "DropdownDialogueGroup", "group");
            Button button2 = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonArrowUp");
            Button button3 = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonArrowDown");
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(delegate
            {
                List<AdvOutput> outputs2 = en.GetBaseNode().outputs;
                int num2 = outputs2.IndexOf(ao);
                if (num2 > 0 && num2 < outputs2.Count)
                {
                    outputs2.Remove(ao);
                    outputs2.Insert(num2 - 1, ao);
                    this.UpdateOutputs();
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                }
            });
            button3.onClick.RemoveAllListeners();
            button3.onClick.AddListener(delegate
            {
                List<AdvOutput> outputs = en.GetBaseNode().outputs;
                int num = outputs.IndexOf(ao);
                if (num > -1 && num < outputs.Count - 1)
                {
                    outputs.Remove(ao);
                    outputs.Insert(num + 1, ao);
                    this.UpdateOutputs();
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                }
            });
        }

        public void PopulateBy(BaseNode node, Adventure adventure)
        {
            this.node = node;
            this.adventure = adventure;
            TextMeshProUGUI componentInChildren = GameObjectUtils.FindByName(base.gameObject, "Heading").GetComponentInChildren<TextMeshProUGUI>();
            string[] array = componentInChildren.text.Split(':'); //, StringSplitOptions.None);
            componentInChildren.text = node.ID + ":" + array[array.Length - 1];
        }

        public void UpdateAllFields(BaseNode node)
        {
            this.node = node;
            node.UpdateVisuals(this);
            Button button = GameObjectUtils.FindByNameGetComponent<Button>(base.gameObject, "SocketIn");
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    MHEventSystem.TriggerEvent<EditorNode>(this, "SocketIn");
                });
            }
            this.UpdateHeaderColor();
        }

        public BaseNode GetBaseNode()
        {
            return this.node;
        }

        protected virtual void InitializeHeader()
        {
            EditorDraggableHeader orAddComponent = GameObjectUtils.FindByName(base.gameObject, "Heading").GetOrAddComponent<EditorDraggableHeader>();
            orAddComponent.Initialize(base.gameObject.transform as RectTransform, 1);
            orAddComponent.dragStarted = DragStarted;
            orAddComponent.dragUpdate = DragUpdate;
            orAddComponent.dragEnd = DragEnd;
            this.UpdateHeaderColor();
        }

        protected void UpdateHeaderColor()
        {
            if (this.node == null)
            {
                return;
            }
            ColorUtility.TryParseHtmlString("#2C221A", out var color);
            ColorUtility.TryParseHtmlString("#008E70", out var color2);
            TextMeshProUGUI componentInChildren = GameObjectUtils.FindByName(base.gameObject, "Heading").GetComponentInChildren<TextMeshProUGUI>();
            if (componentInChildren != null)
            {
                if (this.GetBaseNode().HaveModifiers())
                {
                    componentInChildren.color = color2;
                }
                else
                {
                    componentInChildren.color = color;
                }
            }
        }

        public void DragStarted(object o)
        {
        }

        public void DragEnd(object o)
        {
            _ = (Vector3)o;
            this.node.changed = true;
        }

        public void DragUpdate(object o)
        {
            Vector3 vector = (Vector3)o;
            this.node.PositionX = (int)vector.x;
            this.node.PositionY = (int)vector.y;
            EventEditorGraph.instance.UpdateNode(this);
            EventEditorGraph.instance.UpdateLinks(this);
        }

        public Dictionary<string, LogicEntry.LEntry> GetAvaliableLists()
        {
            if (this.avaliableLists == null || this.listValidityForFrame != Time.timeSinceLevelLoad)
            {
                this.listValidityForFrame = Time.timeSinceLevelLoad;
                BaseNode baseNode = this.GetBaseNode();
                Dictionary<string, LogicEntry.LEntry> dictionary = new Dictionary<string, LogicEntry.LEntry>();
                dictionary["NONE"] = LogicEntry.LEntry.NONE;
                HashSet<BaseNode> visitedNodes = new HashSet<BaseNode>();
                this.FindListsFromParents(baseNode, visitedNodes, dictionary);
                this.avaliableLists = dictionary;
            }
            return this.avaliableLists;
        }

        public List<string> GetAvaliableListsOf()
        {
            return new List<string>(this.GetAvaliableLists().Keys);
        }

        public List<string> GetAvaliableListsOf(LogicEntry.LEntry type)
        {
            Dictionary<string, LogicEntry.LEntry> dictionary = this.GetAvaliableLists();
            List<string> list = new List<string>(dictionary.Count);
            foreach (KeyValuePair<string, LogicEntry.LEntry> item in dictionary)
            {
                if (item.Value == type || item.Value == LogicEntry.LEntry.NONE)
                {
                    list.Add(item.Key);
                }
            }
            return list;
        }

        private void FindListsFromParents(BaseNode curentNode, HashSet<BaseNode> visitedNodes, Dictionary<string, LogicEntry.LEntry> lists, bool skipPrivateLists = false)
        {
            if (this.adventure.nodes == null)
            {
                return;
            }
            visitedNodes.Add(curentNode);
            if (curentNode.GetInputs() != null)
            {
                foreach (BaseNode input in curentNode.GetInputs())
                {
                    if (!visitedNodes.Contains(input))
                    {
                        this.FindListsFromParents(input, visitedNodes, lists, skipPrivateLists: true);
                    }
                }
            }
            if (curentNode.logic == null)
            {
                return;
            }
            foreach (AdvLogic item in curentNode.logic)
            {
                if (!(item is LogicEntry))
                {
                    continue;
                }
                LogicEntry logicEntry = item as LogicEntry;
                if ((!skipPrivateLists || logicEntry.makeListPublic) && !string.IsNullOrEmpty(logicEntry.listName))
                {
                    if (lists.ContainsKey(logicEntry.listName))
                    {
                        Debug.LogWarning("List override in filters! " + logicEntry.listName);
                    }
                    ScriptRetType scriptRetType = ScriptLibrary.Get(logicEntry.GetScriptName())?.GetCustomAttribute<ScriptRetType>();
                    if (scriptRetType != null)
                    {
                        lists[logicEntry.listName] = scriptRetType.type;
                    }
                    else
                    {
                        lists[logicEntry.listName] = logicEntry.logicType;
                    }
                }
            }
            foreach (AdvLogic item2 in curentNode.logic)
            {
                if (!(item2 is LogicProcessing))
                {
                    continue;
                }
                LogicProcessing logicProcessing = item2 as LogicProcessing;
                if ((!skipPrivateLists || logicProcessing.makeListPublic) && !string.IsNullOrEmpty(logicProcessing.listName))
                {
                    if (lists.ContainsKey(logicProcessing.listName))
                    {
                        Debug.LogWarning("List override in processing! " + logicProcessing.listName);
                    }
                    ScriptRetType scriptRetType2 = ScriptLibrary.Get(logicProcessing.GetScriptName())?.GetCustomAttribute<ScriptRetType>();
                    if (scriptRetType2 != null)
                    {
                        lists[logicProcessing.listName] = scriptRetType2.type;
                    }
                    else if (!string.IsNullOrEmpty(logicProcessing.listA) && lists.ContainsKey(logicProcessing.listA))
                    {
                        lists[logicProcessing.listName] = lists[logicProcessing.listA];
                    }
                    else
                    {
                        lists[logicProcessing.listName] = logicProcessing.processType;
                    }
                }
            }
        }

        public void UpdateOutputs()
        {
            if (!(this.outputGrid == null))
            {
                this.outputGrid.UpdateGrid(this.GetBaseNode().outputs, this);
            }
        }

        private void CloneToEvent(object obj)
        {
            Adventure adventure = obj as Adventure;
            if (adventure == null)
            {
                string advName = obj as string;
                if (!string.IsNullOrEmpty(advName))
                {
                    adventure = EventEditorModules.GetSelectedModule().adventures.Find((Adventure o) => o.name == advName);
                }
            }
            if (adventure == null)
            {
                return;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetBaseNode().GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, this.GetBaseNode());
                memoryStream.Position = 0L;
                Vector3 pos = ((adventure != this.adventure) ? Vector3.zero : new Vector3(this.GetBaseNode().PositionX + 20f, this.GetBaseNode().PositionY + 20f));
                BaseNode baseNode = adventure.AddNewNode(xmlSerializer.Deserialize(memoryStream) as BaseNode, pos);
                if (baseNode.outputs == null)
                {
                    return;
                }
                foreach (AdvOutput output in baseNode.outputs)
                {
                    output.ownerID = baseNode.ID;
                    if (adventure != this.adventure)
                    {
                        output.targetID = 0;
                    }
                    else
                    {
                        baseNode.PrepareForEdit(this.adventure);
                    }
                }
            }
        }
    }
}
