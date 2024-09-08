using System;
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MOM
{
    public class EventEditorGraph : ScreenBase, IPointerClickHandler, IEventSystemHandler
    {
        public static EventEditorGraph instance;

        public Button btExit;

        public GameObject rbmMenu;

        public GameObject scrollingContent;

        public GameObject linksRoot;

        public GameObject newLinkSource;

        public TMP_InputField nodeSearchField;

        public TextMeshProUGUI tfEventName;

        public Button nodeSearchAction;

        public Button btSave;

        public Button btRename;

        public Button btPrevNode;

        public Button btNextNode;

        public Button btScrollLeft;

        public Button btScrollRight;

        public Button btScrollUp;

        public Button btScrollDown;

        public Button btNewAdventureNode;

        public Button btNewBattleNode;

        public Button btNewTradeNode;

        public Button btNewEnchantmentNode;

        public Button btNewSpawnLocationNode;

        public Button btNodeLocationAward;

        public Button btNewRelayNode;

        public Button btNewAdventureEndNode;

        public EditorNode selectedEditorNode;

        private float scaleGraph = 1f;

        private Dictionary<AdvOutput, LineConnector2> existingConnectors = new Dictionary<AdvOutput, LineConnector2>();

        private Vector3 scrollPosition = Vector3.zero;

        private Vector4 containerStatus;

        private Vector2 rmbOpenPosition;

        private Dictionary<BaseNode, GameObject> nodeGOs = new Dictionary<BaseNode, GameObject>();

        private Dictionary<Type, List<GameObject>> cachedGOs = new Dictionary<Type, List<GameObject>>();

        private Dictionary<AdvOutput, LineConnector2> links2 = new Dictionary<AdvOutput, LineConnector2>();

        private List<BaseNode> linksForRemoval = new List<BaseNode>();

        private Dictionary<Type, Vector2> baseNodeSize;

        private BaseNode targetForConnection;

        private Adventure selectedEvent;

        private Vector2 scrollDelta;

        protected override void Awake()
        {
            EventEditorGraph.instance = this;
            this.selectedEvent = EventEditorAdventures.selectedAdventure;
            this.scrollingContent.transform.localScale = Vector3.one * this.scaleGraph;
            LineConnector2.root = this.linksRoot.transform;
            this.rbmMenu.SetActive(value: false);
        }

        public override IEnumerator PreStart()
        {
            this.baseNodeSize = new Dictionary<Type, Vector2>();
            Dictionary<Type, GameObject> gos = new Dictionary<Type, GameObject>();
            Vector2 vector = new Vector2(0f, 2000f);
            Func<Type, GameObject, object> obj = delegate(Type t, GameObject go)
            {
                if (!this.cachedGOs.ContainsKey(t))
                {
                    this.cachedGOs[t] = new List<GameObject>();
                }
                this.cachedGOs[t].Add(go);
                return (object)null;
            };
            gos[typeof(NodeStart)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeStart"), this.scrollingContent.transform);
            gos[typeof(NodeStart)].transform.localPosition = vector;
            obj(typeof(NodeStart), gos[typeof(NodeStart)]);
            gos[typeof(NodeStory)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeAdventure"), this.scrollingContent.transform);
            gos[typeof(NodeStory)].transform.localPosition = vector;
            obj(typeof(NodeStory), gos[typeof(NodeStory)]);
            gos[typeof(NodeEnd)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeEnd"), this.scrollingContent.transform);
            gos[typeof(NodeEnd)].transform.localPosition = vector;
            obj(typeof(NodeEnd), gos[typeof(NodeEnd)]);
            gos[typeof(NodeBattle)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeBattle"), this.scrollingContent.transform);
            gos[typeof(NodeBattle)].transform.localPosition = vector;
            obj(typeof(NodeBattle), gos[typeof(NodeBattle)]);
            gos[typeof(NodeTrade)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeTrade"), this.scrollingContent.transform);
            gos[typeof(NodeTrade)].transform.localPosition = vector;
            obj(typeof(NodeTrade), gos[typeof(NodeTrade)]);
            gos[typeof(NodeWorldEnchantment)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeWorldEnchantment"), this.scrollingContent.transform);
            gos[typeof(NodeWorldEnchantment)].transform.localPosition = vector;
            obj(typeof(NodeWorldEnchantment), gos[typeof(NodeWorldEnchantment)]);
            gos[typeof(NodeSpawnLocation)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeSpawnLocation"), this.scrollingContent.transform);
            gos[typeof(NodeSpawnLocation)].transform.localPosition = vector;
            obj(typeof(NodeSpawnLocation), gos[typeof(NodeSpawnLocation)]);
            gos[typeof(NodeRelay)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeRelay"), this.scrollingContent.transform);
            gos[typeof(NodeRelay)].transform.localPosition = vector;
            obj(typeof(NodeRelay), gos[typeof(NodeRelay)]);
            gos[typeof(NodeLocationAward)] = GameObjectUtils.Instantiate(UIReferences.GetNodeComponent("NodeLocationAward"), this.scrollingContent.transform);
            gos[typeof(NodeLocationAward)].transform.localPosition = vector;
            obj(typeof(NodeLocationAward), gos[typeof(NodeLocationAward)]);
            this.tfEventName.text = this.selectedEvent.name;
            bool found = false;
            while (!found)
            {
                yield return null;
                foreach (KeyValuePair<Type, GameObject> item in gos)
                {
                    float width = (item.Value.transform as RectTransform).rect.width;
                    float height = (item.Value.transform as RectTransform).rect.height;
                    this.baseNodeSize[item.Key] = new Vector2(width, height);
                    if (height > 100f)
                    {
                        found = true;
                    }
                }
            }
            foreach (KeyValuePair<Type, GameObject> item2 in gos)
            {
                item2.Value.SetActive(value: false);
            }
            yield return base.PreStart();
        }

        public override void OnStart()
        {
            base.OnStart();
            this.UpdateGraph();
            this.FocusAtNode(1);
            if (this.nodeSearchField != null)
            {
                this.nodeSearchField.onEndEdit.AddListener(delegate(string s)
                {
                    try
                    {
                        int i = Convert.ToInt32(s);
                        this.FocusAtNode(i);
                    }
                    catch
                    {
                        Debug.Log(this.nodeSearchField.text + " is not a number!");
                    }
                });
            }
            MHEventSystem.RegisterListener<EditorNode>(NodeButton, this);
            MHEventSystem.RegisterListener<EventEditorLogic>(LogicEvents, this);
        }

        private void LogicEvents(object sender, object e)
        {
            if (sender as EventEditorLogic == null)
            {
                Debug.LogError("Incorrect data send, is it a mistake?");
            }
            if (e is string text && text == "Redraw")
            {
                this.RedrawGraph();
            }
        }

        private void NodeButton(object sender, object e)
        {
            EditorNode editorNode = sender as EditorNode;
            if (editorNode == null)
            {
                Debug.LogError("Incorrect data send, is it a mistake?");
            }
            switch (e as string)
            {
            case "LogicEditor":
                this.selectedEditorNode = editorNode;
                MHEventSystem.TriggerEvent(this, "Advance");
                break;
            case "Redraw":
                this.RedrawGraph();
                break;
            case "SocketIn":
                this.targetForConnection = editorNode.GetBaseNode();
                break;
            }
            if (e is Multitype<string, object, GameObject> multitype)
            {
                if (multitype.t0 == "SocketOut")
                {
                    Debug.Log("Socket out");
                    this.StartLinking(multitype.t2, multitype.t1 as AdvOutput);
                }
                else if (multitype.t0 == "ButtonDelete")
                {
                    Debug.Log("Delete");
                    (multitype.t1 as AdvOutput).Disconnect();
                    editorNode.GetBaseNode().RemoveOutput(multitype.t1 as AdvOutput);
                    editorNode.UpdateOutputs();
                    this.UpdateLinks();
                }
            }
        }

        private void FixedUpdate()
        {
            if (base.stateStatus != StateStatus.Active)
            {
                return;
            }
            RectTransform rectTransform = this.scrollingContent.transform as RectTransform;
            if (this.scrollDelta.x != 0f || this.scrollDelta.y != 0f)
            {
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.x += 20f * this.scrollDelta.x;
                localPosition.y += 20f * this.scrollDelta.y;
                rectTransform.localPosition = localPosition;
            }
            Vector4 vector = default(Vector4);
            vector.x = rectTransform.localPosition.x;
            vector.y = rectTransform.localPosition.y;
            vector.z = rectTransform.lossyScale.x;
            vector.w = rectTransform.lossyScale.y;
            if (vector != this.containerStatus)
            {
                this.containerStatus = vector;
                this.RedrawGraph();
            }
            if (base.GetChildScreens() == null || base.GetChildScreens().Count == 0)
            {
                bool flag = false;
                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (DropDownFilters.singleOpenedDropDown == null)
                {
                    if (axis > 0f)
                    {
                        this.scaleGraph += 0.02f;
                        flag = true;
                    }
                    else if (axis < 0f)
                    {
                        this.scaleGraph -= 0.02f;
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.scaleGraph = Mathf.Clamp(this.scaleGraph, 0.3f, 1f);
                    this.scrollingContent.transform.localScale = Vector3.one * this.scaleGraph;
                }
            }
            if ((bool)this.linksRoot)
            {
                Vector3 position = this.linksRoot.transform.position;
                if (position != this.scrollPosition)
                {
                    this.scrollPosition = position;
                    this.UpdateLinks();
                }
            }
        }

        public void RedrawGraph()
        {
            this.UpdateGraph();
            this.UpdateLinks();
        }

        private bool RequireRedraw()
        {
            if (this.selectedEvent.nodes == null)
            {
                return false;
            }
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            RectTransform obj = this.scrollingContent.transform as RectTransform;
            Vector3 lossyScale = obj.lossyScale;
            Vector3 position = obj.position;
            foreach (BaseNode node in this.selectedEvent.nodes)
            {
                Vector2 size = node.GetSize(this.baseNodeSize);
                float x = lossyScale.x * node.PositionX + position.x;
                float width = lossyScale.x * size.x;
                float num = lossyScale.y * node.PositionY + position.y;
                float num2 = lossyScale.x * size.y;
                Rect other = new Rect(x, num - num2, width, num2);
                if (rect.Overlaps(other))
                {
                    if (!this.nodeGOs.ContainsKey(node))
                    {
                        return true;
                    }
                }
                else if (this.nodeGOs.ContainsKey(node))
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateNode(EditorNode en)
        {
            if (this.nodeGOs != null && this.nodeGOs.ContainsKey(en.GetBaseNode()))
            {
                GameObject obj = this.nodeGOs[en.GetBaseNode()];
                BaseNode baseNode = en.GetBaseNode();
                obj.transform.localPosition = new Vector3(baseNode.PositionX, baseNode.PositionY, 0f);
            }
        }

        private void UpdateGraph()
        {
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            RectTransform rectTransform = this.scrollingContent.transform as RectTransform;
            Vector3 lossyScale = rectTransform.lossyScale;
            Vector3 position = rectTransform.position;
            List<BaseNode> visibleAdvNodes = new List<BaseNode>();
            foreach (BaseNode node in this.selectedEvent.nodes)
            {
                bool flag = false;
                if (rectTransform != null)
                {
                    Vector2 size = node.GetSize(this.baseNodeSize);
                    int num = 80;
                    float num2 = lossyScale.x * node.PositionX + position.x;
                    float num3 = lossyScale.x * size.x;
                    float num4 = lossyScale.y * node.PositionY + position.y;
                    float num5 = lossyScale.x * (size.y + (float)(num * node.GetOutputCount()));
                    Rect other = new Rect(num2 - num3 / 2f, num4 - num5, num3, num5);
                    if (rect.Overlaps(other))
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    visibleAdvNodes.Add(node);
                }
            }
            Dictionary<BaseNode, GameObject> temp = this.nodeGOs;
            this.nodeGOs = new Dictionary<BaseNode, GameObject>();
            List<GameObject> usedGameObjects = new List<GameObject>();
            bool flag2 = false;
            foreach (BaseNode item in visibleAdvNodes)
            {
                if (temp != null && temp.ContainsKey(item))
                {
                    this.nodeGOs[item] = temp[item];
                    temp[item].SetActive(value: true);
                    usedGameObjects.Add(temp[item]);
                    if (item.changed)
                    {
                        EditorNode component = this.nodeGOs[item].GetComponent<EditorNode>();
                        component.PopulateBy(item, this.selectedEvent);
                        component.UpdateAllFields(item);
                    }
                    continue;
                }
                Type type = item.GetType();
                if (!this.cachedGOs.ContainsKey(type))
                {
                    this.cachedGOs[type] = new List<GameObject>();
                }
                GameObject gameObject = this.cachedGOs[type].Find((GameObject o) => !usedGameObjects.Contains(o) && visibleAdvNodes.Find((BaseNode k) => temp.ContainsKey(k) && temp[k] == o) == null);
                if (gameObject == null)
                {
                    gameObject = this.CreateNode(item);
                    this.cachedGOs[type].Add(gameObject);
                    flag2 = true;
                }
                else
                {
                    gameObject.SetActive(value: true);
                }
                usedGameObjects.Add(gameObject);
                this.nodeGOs[item] = gameObject;
                gameObject.transform.localPosition = new Vector3(item.PositionX, item.PositionY, 0f);
            }
            foreach (KeyValuePair<BaseNode, GameObject> nodeGO in this.nodeGOs)
            {
                if (!temp.ContainsKey(nodeGO.Key))
                {
                    BaseNode key = nodeGO.Key;
                    EditorNode component2 = nodeGO.Value.GetComponent<EditorNode>();
                    component2.PopulateBy(key, this.selectedEvent);
                    component2.UpdateAllFields(key);
                }
            }
            foreach (KeyValuePair<BaseNode, GameObject> item2 in temp)
            {
                if (!usedGameObjects.Contains(item2.Value))
                {
                    item2.Value.SetActive(value: false);
                }
            }
            if (flag2)
            {
                this.linksRoot.transform.SetAsLastSibling();
            }
        }

        public void UpdateLinks(EditorNode bn)
        {
            foreach (KeyValuePair<AdvOutput, LineConnector2> item in this.links2)
            {
                if (item.Key.ownerID != bn.GetBaseNode().ID && item.Key.targetID != bn.GetBaseNode().ID)
                {
                    continue;
                }
                AdvOutput key = item.Key;
                BaseNode owner = key.GetOwner(this.selectedEvent);
                BaseNode target = key.GetTarget(this.selectedEvent);
                Vector2 @in = new Vector2(item.Key.FromX, item.Key.FromY);
                Vector2 in2 = new Vector2(target.PositionX, target.PositionY);
                GameObject gameObject = (this.nodeGOs.ContainsKey(owner) ? this.nodeGOs[owner] : null);
                GameObject gameObject2 = (this.nodeGOs.ContainsKey(target) ? this.nodeGOs[target] : null);
                if (gameObject != null)
                {
                    if (this.nodeGOs.ContainsKey(owner))
                    {
                        GameObject gameObject3 = GameObjectUtils.FindByName(gameObject.GetComponent<EditorNode>().outputGrid.GetGameObjectForData(key), "SocketOut");
                        Vector3 position = gameObject3.transform.parent.TransformPoint(gameObject3.transform.localPosition);
                        @in = this.scrollingContent.transform.InverseTransformPoint(position);
                    }
                    key.FromX = (int)@in.x;
                    key.FromY = (int)@in.y;
                }
                if (gameObject2 != null)
                {
                    GameObject gameObject4 = GameObjectUtils.FindByName(gameObject2, "SocketIn");
                    Vector3 position2 = gameObject4.transform.parent.TransformPoint(gameObject4.transform.localPosition);
                    in2 = this.scrollingContent.transform.InverseTransformPoint(position2);
                }
                item.Value.Update(@in, in2);
            }
            this.linksRoot.transform.SetAsLastSibling();
        }

        public void UpdateLinks()
        {
            List<AdvOutput> list = null;
            if (this.existingConnectors != null && this.selectedEvent != null)
            {
                foreach (KeyValuePair<AdvOutput, LineConnector2> v2 in this.existingConnectors)
                {
                    if (this.selectedEvent.nodes == null || this.selectedEvent.nodes.Find((BaseNode o) => o.ID == v2.Key.ownerID) == null || this.selectedEvent.nodes.Find((BaseNode o) => o.ID == v2.Key.targetID) == null)
                    {
                        if (list == null)
                        {
                            list = new List<AdvOutput>();
                        }
                        list.Add(v2.Key);
                    }
                }
            }
            if (list != null)
            {
                foreach (AdvOutput item in list)
                {
                    this.RemoveLink(item);
                }
            }
            foreach (BaseNode item2 in this.linksForRemoval)
            {
                this.ClearRelatedLinks(item2);
            }
            this.linksForRemoval.Clear();
            if (this.nodeGOs == null)
            {
                return;
            }
            List<AdvOutput> list2 = new List<AdvOutput>();
            foreach (KeyValuePair<BaseNode, GameObject> v in this.nodeGOs)
            {
                HashSet<BaseNode> inputs = v.Key.GetInputs();
                if (inputs == null)
                {
                    continue;
                }
                foreach (BaseNode item3 in inputs)
                {
                    if (item3.outputs != null)
                    {
                        List<AdvOutput> collection = item3.outputs.FindAll((AdvOutput o) => o.targetID == v.Key.ID);
                        list2.AddRange(collection);
                    }
                }
                if (v.Key.outputs != null)
                {
                    list2.AddRange(v.Key.outputs);
                }
            }
            foreach (AdvOutput item4 in list2)
            {
                this.AddLinkConnection(item4);
            }
            this.linksRoot.transform.SetAsLastSibling();
        }

        private void AddLinkConnection(AdvOutput o)
        {
            LineConnector2 lineConnector = this.DrawLink(o);
            if (lineConnector != null)
            {
                this.links2[o] = lineConnector;
            }
        }

        private LineConnector2 DrawLink(AdvOutput ao)
        {
            BaseNode owner = ao.GetOwner(this.selectedEvent);
            BaseNode target = ao.GetTarget(this.selectedEvent);
            if (owner == null || target == null)
            {
                return null;
            }
            RectTransform obj = this.scrollingContent.transform as RectTransform;
            _ = obj.lossyScale;
            _ = obj.position;
            GameObject gameObject = (this.nodeGOs.ContainsKey(owner) ? this.nodeGOs[owner] : null);
            GameObject gameObject2 = (this.nodeGOs.ContainsKey(target) ? this.nodeGOs[target] : null);
            Vector2 vector = new Vector2(ao.FromX, ao.FromY);
            Vector2 vector2 = new Vector2(target.PositionX, target.PositionY);
            if (gameObject != null)
            {
                if (this.nodeGOs.ContainsKey(owner))
                {
                    GameObject gameObject3 = GameObjectUtils.FindByName(gameObject.GetComponent<EditorNode>().outputGrid.GetGameObjectForData(ao), "SocketOut");
                    Vector3 position = gameObject3.transform.parent.TransformPoint(gameObject3.transform.localPosition);
                    vector = this.scrollingContent.transform.InverseTransformPoint(position);
                }
                ao.FromX = (int)vector.x;
                ao.FromY = (int)vector.y;
            }
            if (gameObject2 != null)
            {
                GameObject gameObject4 = GameObjectUtils.FindByName(gameObject2, "SocketIn");
                Vector3 position2 = gameObject4.transform.parent.TransformPoint(gameObject4.transform.localPosition);
                vector2 = this.scrollingContent.transform.InverseTransformPoint(position2);
            }
            Color color = Color.red;
            if (ao.GetTarget(this.selectedEvent) != null && ao.GetTarget(this.selectedEvent).HaveFilter())
            {
                color = Color.blue;
            }
            LineConnector2 lineConnector;
            if (this.existingConnectors.ContainsKey(ao))
            {
                lineConnector = this.existingConnectors[ao];
                lineConnector.Update(vector, vector2);
                lineConnector.SetColor(color);
            }
            else
            {
                lineConnector = LineConnector2.Connect(vector, vector2, this.newLinkSource);
                lineConnector.SetColor(color);
                this.existingConnectors[ao] = lineConnector;
            }
            return lineConnector;
        }

        private void ClearRelatedLinks(BaseNode n)
        {
            List<AdvOutput> list = null;
            foreach (KeyValuePair<AdvOutput, LineConnector2> item in this.links2)
            {
                if (item.Key.ownerID == n.ID || item.Key.targetID == n.ID)
                {
                    if (list == null)
                    {
                        list = new List<AdvOutput>();
                    }
                    list.Add(item.Key);
                }
            }
            if (list == null)
            {
                return;
            }
            foreach (AdvOutput item2 in list)
            {
                this.ClearLink(item2);
            }
        }

        private void ClearLink(AdvOutput n)
        {
            this.links2[n].Remove();
            this.links2.Remove(n);
        }

        private void RemoveLink(AdvOutput ao)
        {
            if (this.existingConnectors.ContainsKey(ao))
            {
                this.existingConnectors[ao].Remove();
                this.existingConnectors.Remove(ao);
            }
        }

        private GameObject CreateNode(BaseNode an)
        {
            GameObject gameObject = null;
            if (an is NodeStart)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeStart"), this.scrollingContent.transform);
            }
            else if (an is NodeEnd)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeEnd"), this.scrollingContent.transform);
            }
            else if (an is NodeStory)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeAdventure"), this.scrollingContent.transform);
            }
            else if (an is NodeBattle)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeBattle"), this.scrollingContent.transform);
            }
            else if (an is NodeTrade)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeTrade"), this.scrollingContent.transform);
            }
            else if (an is NodeWorldEnchantment)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeWorldEnchantment"), this.scrollingContent.transform);
            }
            else if (an is NodeSpawnLocation)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeSpawnLocation"), this.scrollingContent.transform);
            }
            else if (an is NodeRelay)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeRelay"), this.scrollingContent.transform);
            }
            else if (an is NodeLocationAward)
            {
                gameObject = GameObjectUtils.InstantiateWithLocalization(UIReferences.GetNodeComponent("NodeLocationAward"), this.scrollingContent.transform);
            }
            if (gameObject == null)
            {
                Debug.LogError("[ERROR]Missing object for " + an.ToString());
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(an.PositionX, an.PositionY, 0f);
            }
            return gameObject;
        }

        public void FocusAtNode(int i)
        {
            if (this.selectedEvent.nodes != null)
            {
                BaseNode baseNode = this.selectedEvent.nodes.Find((BaseNode o) => o.ID == i);
                if (baseNode != null)
                {
                    Vector2 size = baseNode.GetSize(this.baseNodeSize);
                    Vector3 zero = Vector3.zero;
                    zero.x = 0f - baseNode.PositionX - size.x / 2f;
                    zero.y = 0f - baseNode.PositionY + size.y / 2f;
                    this.scaleGraph = 1f;
                    RectTransform obj = this.scrollingContent.transform as RectTransform;
                    obj.localScale = Vector3.one;
                    obj.localPosition = zero;
                    this.RedrawGraph();
                }
            }
        }

        private void ClearGraph()
        {
            LineConnector2.Clear();
            foreach (KeyValuePair<BaseNode, GameObject> nodeGO in this.nodeGOs)
            {
                nodeGO.Value.SetActive(value: false);
            }
        }

        public void SwitchRMBMenu(bool? requiredState = null)
        {
            this.rmbOpenPosition = this.scrollingContent.transform.InverseTransformPoint(Input.mousePosition);
            if (requiredState.HasValue)
            {
                this.rbmMenu.SetActive(requiredState.Value);
            }
            else
            {
                this.rbmMenu.SetActive(!this.rbmMenu.activeSelf);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.SwitchRMBMenu();
                Vector2 position = eventData.position;
                RectTransform rectTransform = this.rbmMenu.transform as RectTransform;
                Vector2 vector = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
                if (position.x > (float)Screen.width - vector.x)
                {
                    position.x = (float)Screen.width - vector.x;
                }
                else if (position.x < vector.x)
                {
                    position.x = vector.x;
                }
                if (position.y > (float)Screen.height - vector.y)
                {
                    position.y = (float)Screen.height - vector.y;
                }
                else if (position.y < vector.y * 2f)
                {
                    position.y = vector.y * 2f;
                }
                this.rbmMenu.transform.position = position;
            }
        }

        private void StartLinking(GameObject go, AdvOutput ao)
        {
            this.targetForConnection = null;
            base.StartCoroutine(this.AnimateConnectingNodes(go, ao));
        }

        private IEnumerator AnimateConnectingNodes(GameObject go, AdvOutput from)
        {
            Debug.LogWarning("Coroutine started");
            this.RemoveLink(from);
            GameObject outGO = GameObjectUtils.FindByName(go, "SocketOut");
            Vector3 vector = outGO.transform.parent.TransformPoint(outGO.transform.localPosition);
            Vector2 vector2 = this.scrollingContent.transform.InverseTransformPoint(vector);
            LineConnector2 lc = LineConnector2.Connect(vector2, vector2, this.newLinkSource);
            Vector3 vector3 = vector;
            string text = vector3.ToString();
            Vector2 vector4 = vector2;
            Debug.Log("Start :" + text + " " + vector4.ToString());
            while (true)
            {
                yield return null;
                if (Input.GetMouseButtonUp(0))
                {
                    break;
                }
                vector = outGO.transform.parent.TransformPoint(outGO.transform.localPosition);
                vector2 = this.scrollingContent.transform.InverseTransformPoint(vector);
                Vector3 mousePosition = Input.mousePosition;
                Vector3 vector5 = this.scrollingContent.transform.InverseTransformPoint(mousePosition);
                lc.Update(vector2, vector5);
            }
            lc.Remove();
            Debug.Log("Coroutine setting up");
            if (from != null)
            {
                if (this.targetForConnection == null)
                {
                    Debug.Log("Disconnect");
                    from.Disconnect();
                }
                else
                {
                    Debug.Log("Connect to " + this.targetForConnection.ID);
                    from.Connect(this.targetForConnection);
                }
            }
            if (this.selectedEvent != null)
            {
                BaseNode owner = from.GetOwner(this.selectedEvent);
                BaseNode target = from.GetTarget(this.selectedEvent);
                if (owner != null && target != null)
                {
                    owner.PrepareForEdit(this.selectedEvent);
                    target.PrepareForEdit(this.selectedEvent);
                    this.AddLinkConnection(from);
                }
            }
            this.RedrawGraph();
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btNewAdventureNode)
            {
                this.selectedEvent.AddNewNode<NodeStory>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewAdventureEndNode)
            {
                this.selectedEvent.AddNewNode<NodeEnd>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewBattleNode)
            {
                this.selectedEvent.AddNewNode<NodeBattle>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewEnchantmentNode)
            {
                this.selectedEvent.AddNewNode<NodeWorldEnchantment>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewSpawnLocationNode)
            {
                this.selectedEvent.AddNewNode<NodeSpawnLocation>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewTradeNode)
            {
                this.selectedEvent.AddNewNode<NodeTrade>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNodeLocationAward)
            {
                this.selectedEvent.AddNewNode<NodeLocationAward>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btNewRelayNode)
            {
                this.selectedEvent.AddNewNode<NodeRelay>(this.rmbOpenPosition);
                this.RedrawGraph();
                this.SwitchRMBMenu();
                return;
            }
            if (s == this.btExit)
            {
                MHEventSystem.TriggerEvent(this, "Back");
                UIManager.Close(this);
                return;
            }
            if (s == this.nodeSearchAction)
            {
                try
                {
                    int i = Convert.ToInt32(this.nodeSearchField.text);
                    this.FocusAtNode(i);
                    return;
                }
                catch
                {
                    Debug.Log(this.nodeSearchField.text + " is not a number!");
                    return;
                }
            }
            if (s == this.btSave)
            {
                EventEditorModules.MarkSelectedModuleAsModified();
                EventEditorModules.SaveNow();
            }
            else if (s == this.btRename)
            {
                PopupName.OpenPopup(this.selectedEvent.name, RenameAdventureToName, null, this);
            }
        }

        private void RenameAdventureToName(object o)
        {
            string value = o as string;
            if (string.IsNullOrEmpty(value))
            {
                value = "NameUndefined";
            }
            Adventure adventure = this.selectedEvent;
            value = EventEditorAdventures.SanitizeName(value, adventure);
            _ = adventure.name;
            adventure.name = value;
            this.tfEventName.text = this.selectedEvent.name;
            MHEventSystem.TriggerEvent<EventEditorGraph>(this, "AdventureUpdate");
            EventEditorModules.MarkSelectedModuleAsModified();
        }

        public void ScrollUp(bool on)
        {
            if (on)
            {
                this.scrollDelta.y = -1f;
            }
            else
            {
                this.scrollDelta.y = 0f;
            }
        }

        public void ScrollDown(bool on)
        {
            if (on)
            {
                this.scrollDelta.y = 1f;
            }
            else
            {
                this.scrollDelta.y = 0f;
            }
        }

        public void ScrollLeft(bool on)
        {
            if (on)
            {
                this.scrollDelta.x = 1f;
            }
            else
            {
                this.scrollDelta.x = 0f;
            }
        }

        public void ScrollRight(bool on)
        {
            if (on)
            {
                this.scrollDelta.x = -1f;
            }
            else
            {
                this.scrollDelta.x = 0f;
            }
        }
    }
}
