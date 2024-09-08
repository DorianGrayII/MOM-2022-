namespace MOM.Adventures
{
    using MHUtils;
    using MHUtils.UI;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

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
                this.outputGrid.CustomDynamicItem(new CustomDynamicItemMethod(this.OutputItem), null);
            }
        }

        private void CloneToEvent(object obj)
        {
            Adventure objA = obj as Adventure;
            if (objA == null)
            {
                string advName = obj as string;
                if (!string.IsNullOrEmpty(advName))
                {
                    objA = EventEditorModules.GetSelectedModule().adventures.Find(o => o.name == advName);
                }
            }
            if (objA != null)
            {
                XmlSerializer serializer = new XmlSerializer(this.GetBaseNode().GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize((Stream) stream, this.GetBaseNode());
                    stream.Position = 0L;
                    Vector3 pos = !ReferenceEquals(objA, this.adventure) ? Vector3.zero : new Vector3(this.GetBaseNode().PositionX + 20f, this.GetBaseNode().PositionY + 20f);
                    BaseNode node = objA.AddNewNode(serializer.Deserialize(stream) as BaseNode, pos);
                    if (node.outputs != null)
                    {
                        foreach (AdvOutput output in node.outputs)
                        {
                            output.ownerID = node.ID;
                            if (!ReferenceEquals(objA, this.adventure))
                            {
                                output.targetID = 0;
                                continue;
                            }
                            node.PrepareForEdit(this.adventure);
                        }
                    }
                }
            }
        }

        public void DragEnd(object o)
        {
            Vector3 vector1 = (Vector3) o;
            this.node.changed = true;
        }

        public void DragStarted(object o)
        {
        }

        public void DragUpdate(object o)
        {
            Vector3 vector = (Vector3) o;
            this.node.PositionX = (int) vector.x;
            this.node.PositionY = (int) vector.y;
            EventEditorGraph.instance.UpdateNode(this);
            EventEditorGraph.instance.UpdateLinks(this);
        }

        private void FindListsFromParents(BaseNode curentNode, HashSet<BaseNode> visitedNodes, Dictionary<string, LogicEntry.LEntry> lists, bool skipPrivateLists)
        {
            if (this.adventure.nodes != null)
            {
                visitedNodes.Add(curentNode);
                if (curentNode.GetInputs() != null)
                {
                    foreach (BaseNode node in curentNode.GetInputs())
                    {
                        if (!visitedNodes.Contains(node))
                        {
                            this.FindListsFromParents(node, visitedNodes, lists, true);
                        }
                    }
                }
                if (curentNode.logic != null)
                {
                    foreach (AdvLogic logic in curentNode.logic)
                    {
                        if (!(logic is LogicEntry))
                        {
                            continue;
                        }
                        LogicEntry entry = logic as LogicEntry;
                        if ((!skipPrivateLists || entry.makeListPublic) && !string.IsNullOrEmpty(entry.listName))
                        {
                            ScriptRetType customAttribute;
                            if (lists.ContainsKey(entry.listName))
                            {
                                Debug.LogWarning("List override in filters! " + entry.listName);
                            }
                            MethodInfo element = ScriptLibrary.Get(entry.GetScriptName());
                            if (element != null)
                            {
                                customAttribute = CustomAttributeExtensions.GetCustomAttribute<ScriptRetType>(element);
                            }
                            else
                            {
                                MethodInfo local1 = element;
                                customAttribute = null;
                            }
                            ScriptRetType type = customAttribute;
                            lists[entry.listName] = (type == null) ? entry.logicType : type.type;
                        }
                    }
                    foreach (AdvLogic logic2 in curentNode.logic)
                    {
                        if (!(logic2 is LogicProcessing))
                        {
                            continue;
                        }
                        LogicProcessing processing = logic2 as LogicProcessing;
                        if ((!skipPrivateLists || processing.makeListPublic) && !string.IsNullOrEmpty(processing.listName))
                        {
                            ScriptRetType customAttribute;
                            if (lists.ContainsKey(processing.listName))
                            {
                                Debug.LogWarning("List override in processing! " + processing.listName);
                            }
                            MethodInfo element = ScriptLibrary.Get(processing.GetScriptName());
                            if (element != null)
                            {
                                customAttribute = CustomAttributeExtensions.GetCustomAttribute<ScriptRetType>(element);
                            }
                            else
                            {
                                MethodInfo local2 = element;
                                customAttribute = null;
                            }
                            ScriptRetType type2 = customAttribute;
                            lists[processing.listName] = (type2 == null) ? ((string.IsNullOrEmpty(processing.listA) || !lists.ContainsKey(processing.listA)) ? processing.processType : lists[processing.listA]) : type2.type;
                        }
                    }
                }
            }
        }

        public Dictionary<string, LogicEntry.LEntry> GetAvaliableLists()
        {
            if ((this.avaliableLists == null) || (this.listValidityForFrame != Time.timeSinceLevelLoad))
            {
                this.listValidityForFrame = Time.timeSinceLevelLoad;
                BaseNode baseNode = this.GetBaseNode();
                Dictionary<string, LogicEntry.LEntry> lists = new Dictionary<string, LogicEntry.LEntry> {
                    ["NONE"] = LogicEntry.LEntry.NONE
                };
                this.FindListsFromParents(baseNode, new HashSet<BaseNode>(), lists, false);
                this.avaliableLists = lists;
            }
            return this.avaliableLists;
        }

        public List<string> GetAvaliableListsOf()
        {
            return new List<string>(this.GetAvaliableLists().Keys);
        }

        public List<string> GetAvaliableListsOf(LogicEntry.LEntry type)
        {
            Dictionary<string, LogicEntry.LEntry> avaliableLists = this.GetAvaliableLists();
            List<string> list = new List<string>(avaliableLists.Count);
            foreach (KeyValuePair<string, LogicEntry.LEntry> pair in avaliableLists)
            {
                if ((((LogicEntry.LEntry) pair.Value) == type) || (((LogicEntry.LEntry) pair.Value) == LogicEntry.LEntry.NONE))
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public BaseNode GetBaseNode()
        {
            return this.node;
        }

        protected virtual void InitializeHeader()
        {
            EditorDraggableHeader orAddComponent = GameObjectUtils.GetOrAddComponent<EditorDraggableHeader>(GameObjectUtils.FindByName(base.gameObject, "Heading", false));
            orAddComponent.Initialize(base.gameObject.transform as RectTransform, 1, null, null, null);
            orAddComponent.dragStarted = new Callback(this.DragStarted);
            orAddComponent.dragUpdate = new Callback(this.DragUpdate);
            orAddComponent.dragEnd = new Callback(this.DragEnd);
            this.UpdateHeaderColor();
        }

        private void OutputItem(GameObject itemSource, object source, object data, int index)
        {
            AdvOutput ao = source as AdvOutput;
            EditorNode en = data as EditorNode;
            if (ao != null)
            {
                Button button = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonDelete", false);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => MHEventSystem.TriggerEvent<EditorNode>(this, new Multitype<string, object, GameObject>("ButtonDelete", source, itemSource)));
                TMP_InputField field = GameObjectUtils.FindByNameGetComponent<TMP_InputField>(itemSource, "InputAnswer", true);
                GameObjectUtils.FindByNameGetComponent<TextMeshProUGUI>(field.gameObject, "Text", false);
                BaseNode node = en.node;
                if (field != null)
                {
                    field.onValueChanged.RemoveAllListeners();
                    node.RequiresOutputText();
                    field.text = !string.IsNullOrEmpty(ao.name) ? ao.name : "";
                    field.onValueChanged.AddListener(s => ao.name = s);
                }
                button = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "SocketOut", false);
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => MHEventSystem.TriggerEvent<EditorNode>(data, new Multitype<string, object, GameObject>("SocketOut", source, itemSource)));
                }
                UIComponentFill.LinkDropdownEnum<AdvOutput.GroupHint>(GameObjectUtils.FindByNameGetComponent<DropDownFilters>(itemSource, "DropdownDialogueGroup", false).gameObject, ao, "DropdownDialogueGroup", "group", null);
                Button button2 = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonArrowDown", false);
                Button local1 = GameObjectUtils.FindByNameGetComponent<Button>(itemSource, "ButtonArrowUp", false);
                local1.onClick.RemoveAllListeners();
                local1.onClick.AddListener(delegate {
                    List<AdvOutput> outputs = en.GetBaseNode().outputs;
                    int num = outputs.IndexOf(ao);
                    if ((num > 0) && (num < outputs.Count))
                    {
                        outputs.Remove(ao);
                        outputs.Insert(num - 1, ao);
                        this.UpdateOutputs();
                        MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                    }
                });
                button2.onClick.RemoveAllListeners();
                button2.onClick.AddListener(delegate {
                    List<AdvOutput> outputs = en.GetBaseNode().outputs;
                    int num = outputs.IndexOf(ao);
                    if ((num > -1) && (num < (outputs.Count - 1)))
                    {
                        outputs.Remove(ao);
                        outputs.Insert(num + 1, ao);
                        this.UpdateOutputs();
                        MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                    }
                });
            }
        }

        public void PopulateBy(BaseNode node, Adventure adventure)
        {
            this.node = node;
            this.adventure = adventure;
            TextMeshProUGUI componentInChildren = GameObjectUtils.FindByName(base.gameObject, "Heading", false).GetComponentInChildren<TextMeshProUGUI>();
            string[] strArray = componentInChildren.text.Split(':', StringSplitOptions.None);
            componentInChildren.text = node.ID.ToString() + ":" + strArray[strArray.Length - 1];
        }

        public void SelectImage(object o)
        {
            Texture2D textured = o as Texture2D;
            this.GetBaseNode().image = textured?.name;
            this.UpdateImage();
        }

        private void Start()
        {
            if (this.btLogicEditor != null)
            {
                this.btLogicEditor.onClick.AddListener(delegate {
                    EventEditorLogic.node = this;
                    MHEventSystem.TriggerEvent<EditorNode>(this, "LogicEditor");
                });
            }
            if (this.btNewOutput != null)
            {
                this.btNewOutput.onClick.AddListener(delegate {
                    if (this.node != null)
                    {
                        this.node.AddOutput(null);
                    }
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                });
            }
            if (this.btSelectImage != null)
            {
                this.btSelectImage.onClick.AddListener(() => EventEditorImage.OpenImageSelection(base.GetComponentInParent<ScreenBase>(), new Callback(this.SelectImage), null));
            }
            if (this.btDeleteImage != null)
            {
                this.btDeleteImage.onClick.AddListener(delegate {
                    this.GetBaseNode().image = null;
                    this.UpdateImage();
                });
            }
            if (this.btDeleteNode != null)
            {
                this.btDeleteNode.onClick.AddListener(delegate {
                    Adventure selectedAdventure = EventEditorAdventures.selectedAdventure;
                    if ((selectedAdventure.nodes != null) && selectedAdventure.nodes.Contains(this.GetBaseNode()))
                    {
                        this.GetBaseNode().DisconnectAll();
                        selectedAdventure.nodes.Remove(this.GetBaseNode());
                        EventEditorGraph.instance.RedrawGraph();
                    }
                });
            }
            if (this.btCloneNode != null)
            {
                this.btCloneNode.onClick.AddListener(delegate {
                    this.CloneToEvent(this.adventure);
                    MHEventSystem.TriggerEvent<EditorNode>(this, "Redraw");
                });
            }
            if (this.btCloneToAdventure != null)
            {
                this.btCloneToAdventure.onClick.AddListener(() => EventEditorCloneToModule.OpenPopup(base.GetComponentInParent<ScreenBase>(), new Callback(this.CloneToEvent), null, typeof(Adventure)));
            }
            this.InitializeHeader();
        }

        public void UpdateAllFields(BaseNode node)
        {
            this.node = node;
            node.UpdateVisuals(this);
            Button button = GameObjectUtils.FindByNameGetComponent<Button>(base.gameObject, "SocketIn", false);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => MHEventSystem.TriggerEvent<EditorNode>(this, "SocketIn"));
            }
            this.UpdateHeaderColor();
        }

        public void UpdateGraphic()
        {
            if (this.image != null)
            {
                this.image.texture = ImageLibrary.RequestSingleImage(this.GetBaseNode().image);
            }
        }

        protected void UpdateHeaderColor()
        {
            if (this.node != null)
            {
                UnityEngine.Color color;
                UnityEngine.Color color2;
                ColorUtility.TryParseHtmlString("#2C221A", out color);
                ColorUtility.TryParseHtmlString("#008E70", out color2);
                TextMeshProUGUI componentInChildren = GameObjectUtils.FindByName(base.gameObject, "Heading", false).GetComponentInChildren<TextMeshProUGUI>();
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

        public void UpdateOutputs()
        {
            if (this.outputGrid != null)
            {
                this.outputGrid.UpdateGrid<AdvOutput>(this.GetBaseNode().outputs, this);
            }
        }
    }
}

