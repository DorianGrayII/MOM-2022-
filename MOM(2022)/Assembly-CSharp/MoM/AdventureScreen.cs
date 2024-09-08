using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class AdventureScreen : ScreenBase
    {
        public RawImage image;

        public Animator anim;

        public Button btOK;

        public Button btDetails;

        public CanvasGroup canvasGroup;

        public TextMeshProUGUI gold;

        public TextMeshProUGUI mana;

        public TextMeshProUGUI food;

        public TextMeshProUGUI fame;

        public ScrollRect scrollRect;

        public GameObject p_adventureText;

        public GameObject p_adventureAnswer;

        public GameObject resources;

        public List<AdventureOutcome> p_adventureOutcomes;

        public GameObject previousTextContainer;

        public GameObject textContainer;

        private BaseNode curentNode;

        private List<AdventureOutcomeDelta.Outcome> currentOutcomes;

        private AdventureData ad;

        private Adventure a;

        private List<GameObject> currentNodeGOs = new List<GameObject>();

        private Dictionary<AdventureOutcome.Types, AdventureOutcome> outcomePrefabs = new Dictionary<AdventureOutcome.Types, AdventureOutcome>();

        private bool fullMode;

        public override IEnumerator PreStart()
        {
            this.outcomePrefabs.Clear();
            foreach (AdventureOutcome p_adventureOutcome in this.p_adventureOutcomes)
            {
                if (p_adventureOutcome.typeData == null)
                {
                    Debug.LogError(p_adventureOutcome.name + " has no typeData so will be ignored");
                    continue;
                }
                foreach (AdventureOutcome.TypeData typeDatum in p_adventureOutcome.typeData)
                {
                    this.outcomePrefabs.Add(typeDatum.outcomeType, p_adventureOutcome);
                }
            }
            AudioLibrary.RequestSFX("OpenAdventureScreen");
            yield return base.PreStart();
        }

        private void UpdateResources()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            this.gold.text = humanWizard.money + "(" + humanWizard.CalculateMoneyIncome(includeUpkeep: true).SInt() + ")";
            this.mana.text = humanWizard.GetMana() + "(" + humanWizard.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
            this.food.text = humanWizard.CalculateFoodIncome(includeUpkeep: true).SInt();
            this.fame.text = humanWizard.GetFame().ToString();
        }

        public override IEnumerator PostStart()
        {
            yield return base.PostStart();
        }

        public bool UpdateNodeEnd(AdventureData ad, Adventure a, BaseNode curentNode, List<AdventureOutcomeDelta.Outcome> outcomes)
        {
            foreach (GameObject currentNodeGO in this.currentNodeGOs)
            {
                currentNodeGO.transform.SetParent(this.previousTextContainer.transform, worldPositionStays: false);
            }
            GameObjectUtils.RemoveChildren(this.textContainer.transform);
            this.currentOutcomes = outcomes;
            this.curentNode = curentNode;
            bool result = this.AddOutcomes(outcomes);
            this.btOK.interactable = true;
            this.btOK.gameObject.SetActive(value: true);
            this.btDetails.gameObject.SetActive(value: false);
            this.ScrollToNewContent();
            return result;
        }

        public void CloseMe()
        {
            UIManager.Close(this);
        }

        public void AddAnswer(int optionID)
        {
            GameObject gameObject = GameObjectUtils.Instantiate(this.p_adventureAnswer, this.textContainer.transform);
            this.currentNodeGOs.Add(gameObject);
            TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (optionID < this.curentNode.outputs.Count)
            {
                AdvOutput advOutput = this.curentNode.outputs[optionID];
                if (advOutput != null)
                {
                    componentInChildren.text = this.DisplayText(advOutput.name, this.ad);
                    gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }

        public bool UpdateScreenBy(AdventureData ad, Adventure a, BaseNode curentNode, List<AdventureOutcomeDelta.Outcome> outcomes)
        {
            bool flag = false;
            bool flag2 = false;
            if (this.curentNode == curentNode)
            {
                foreach (GameObject currentNodeGO in this.currentNodeGOs)
                {
                    Object.Destroy(currentNodeGO);
                }
                flag2 = true;
            }
            else
            {
                foreach (GameObject currentNodeGO2 in this.currentNodeGOs)
                {
                    currentNodeGO2.transform.SetParent(this.previousTextContainer.transform, worldPositionStays: false);
                }
            }
            this.UpdateResources();
            this.currentNodeGOs.Clear();
            this.currentOutcomes = outcomes;
            this.curentNode = curentNode;
            this.ad = ad;
            this.a = a;
            this.image.texture = ImageLibrary.RequestSingleImage(ad.imageName);
            if (curentNode is NodeStory)
            {
                NodeStory nodeStory = curentNode as NodeStory;
                GameObject gameObject = GameObjectUtils.Instantiate(this.p_adventureText, this.textContainer.transform);
                this.currentNodeGOs.Add(gameObject);
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = this.DisplayText(nodeStory.story, ad);
                flag = true;
                GameObject obj = this.btDetails.gameObject;
                List<int> avaliableOutputs = ad.avaliableOutputs;
                obj.SetActive(avaliableOutputs != null && avaliableOutputs.Count > 1);
            }
            if (curentNode is NodeStory || curentNode is NodeEnd)
            {
                flag |= this.AddOutcomes(outcomes);
            }
            if (this.fullMode && curentNode is NodeStory)
            {
                List<Button> buttons = new List<Button>();
                foreach (int avaliableOutput in ad.avaliableOutputs)
                {
                    int optionID = avaliableOutput;
                    GameObject gameObject2 = GameObjectUtils.Instantiate(this.p_adventureAnswer, this.textContainer.transform);
                    this.currentNodeGOs.Add(gameObject2);
                    TextMeshProUGUI componentInChildren = gameObject2.GetComponentInChildren<TextMeshProUGUI>();
                    AdvOutput advOutput = curentNode.outputs[optionID];
                    componentInChildren.text = this.DisplayText(advOutput.name, ad);
                    Button b = gameObject2.GetComponent<Button>();
                    buttons.Add(b);
                    b.onClick.AddListener(delegate
                    {
                        MHEventSystem.TriggerEvent<AdventureScreen>(curentNode, optionID);
                        foreach (Button item in buttons)
                        {
                            if (item == b)
                            {
                                item.interactable = false;
                            }
                            else
                            {
                                this.currentNodeGOs.Remove(item.gameObject);
                                Object.Destroy(item.gameObject);
                            }
                        }
                    });
                }
                this.btDetails.gameObject.SetActive(value: false);
            }
            if (!flag2)
            {
                this.ScrollToNewContent();
            }
            return flag;
        }

        private void ScrollToNewContent()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.scrollRect.content);
            RectTransform rectTransform = this.previousTextContainer.transform as RectTransform;
            RectTransform obj = this.scrollRect.content.transform as RectTransform;
            RectTransform rectTransform2 = this.scrollRect.transform as RectTransform;
            float num = obj.rect.height - rectTransform2.rect.height;
            if (num > 0f)
            {
                float num2 = rectTransform.rect.height / num;
                this.scrollRect.verticalNormalizedPosition = 1f - num2;
            }
            else
            {
                this.scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private bool AddOutcomes(List<AdventureOutcomeDelta.Outcome> outcomes)
        {
            bool result = false;
            foreach (AdventureOutcomeDelta.Outcome outcome in outcomes)
            {
                if (this.outcomePrefabs.TryGetValue(outcome.outcomeType, out var value))
                {
                    GameObject gameObject = GameObjectUtils.Instantiate(value.gameObject, this.textContainer.transform);
                    this.currentNodeGOs.Add(gameObject);
                    gameObject.GetComponent<AdventureOutcome>().Set(outcome);
                    result = true;
                }
            }
            return result;
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btOK)
            {
                int num = this.ad.avaliableOutputs[0];
                this.AddAnswer(num);
                MHEventSystem.TriggerEvent<AdventureScreen>(this.curentNode, num);
            }
            else if (s == this.btDetails)
            {
                this.btOK.gameObject.SetActive(value: false);
                this.btDetails.gameObject.SetActive(value: false);
                base.StartCoroutine(this.OpenBig());
            }
        }

        private IEnumerator OpenBig()
        {
            this.anim.SetTrigger("Details");
            this.fullMode = true;
            yield return new WaitForSeconds(1f);
            this.UpdateScreenBy(this.ad, this.a, this.curentNode, this.currentOutcomes);
        }

        private string DisplayText(string source, AdventureData ad)
        {
            int num = source.IndexOf("[");
            if (num < 0)
            {
                return source;
            }
            string text = "";
            string text2 = source;
            while (num >= 0)
            {
                int num2 = text2.IndexOf("]");
                if (num2 < 0)
                {
                    break;
                }
                text += text2.Substring(0, num);
                string text3 = text2.Substring(num + 1, num2 - num - 1);
                AdvList listByName = ad.GetListByName(text3, null);
                if (listByName != null)
                {
                    object obj = listByName.Get<object>(0);
                    text = ((obj == null) ? (text + "[missing element 0 on: " + text3 + "]") : ((!(obj is IEventDisplayName)) ? (text + obj.ToString()) : (text + (obj as IEventDisplayName).GetEventDisplayName())));
                }
                else
                {
                    text = text + "[missing: " + text3 + "]";
                }
                if (text2.Length <= num2 + 1)
                {
                    break;
                }
                text2 = text2.Substring(num2 + 1, text2.Length - num2 - 1);
                num = text2.IndexOf("[");
            }
            return text + text2;
        }
    }
}
