using System.Collections.Generic;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPage : MonoBehaviour
{
    private List<TutorialVisiblityByPage> visible = new List<TutorialVisiblityByPage>();

    public TextMeshProUGUI labelPage;

    public Button previous;

    public Button next;

    private Tutorial_Generic tutorial;

    private void Awake()
    {
        this.tutorial = base.GetComponentInParent<Tutorial_Generic>();
        int pageNo = this.tutorial.GetPageNo(this);
        this.labelPage.text = pageNo + "/" + this.tutorial.pages.Length;
        this.next.interactable = pageNo < this.tutorial.pages.Length;
        this.previous.interactable = pageNo > 1;
        this.next.onClick.AddListener(delegate
        {
            base.gameObject.SetActive(value: false);
            this.tutorial.pages[pageNo].gameObject.SetActive(value: true);
        });
        this.previous.onClick.AddListener(delegate
        {
            base.gameObject.SetActive(value: false);
            this.tutorial.pages[pageNo - 2].gameObject.SetActive(value: true);
        });
    }

    public void Register(TutorialVisiblityByPage visibility)
    {
        this.visible.Add(visibility);
    }

    private void OnEnable()
    {
        this.UpdateVisibility();
    }

    private void OnDisable()
    {
        this.UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        foreach (TutorialVisiblityByPage item in this.visible)
        {
            item.UpdateVisibility();
        }
    }
}
