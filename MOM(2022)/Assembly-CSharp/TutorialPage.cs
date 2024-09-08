using MOM;
using System;
using System.Collections.Generic;
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
        this.labelPage.text = pageNo.ToString() + "/" + this.tutorial.pages.Length.ToString();
        this.next.interactable = pageNo < this.tutorial.pages.Length;
        this.previous.interactable = pageNo > 1;
        this.next.onClick.AddListener(delegate {
            this.gameObject.SetActive(false);
            this.tutorial.pages[pageNo].gameObject.SetActive(true);
        });
        this.previous.onClick.AddListener(delegate {
            this.gameObject.SetActive(false);
            this.tutorial.pages[pageNo - 2].gameObject.SetActive(true);
        });
    }

    private void OnDisable()
    {
        this.UpdateVisibility();
    }

    private void OnEnable()
    {
        this.UpdateVisibility();
    }

    public void Register(TutorialVisiblityByPage visibility)
    {
        this.visible.Add(visibility);
    }

    private void UpdateVisibility()
    {
        using (List<TutorialVisiblityByPage>.Enumerator enumerator = this.visible.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                enumerator.Current.UpdateVisibility();
            }
        }
    }
}

