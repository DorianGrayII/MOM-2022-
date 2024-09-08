using System;
using UnityEngine;

public class TutorialVisiblityByPage : MonoBehaviour
{
    public TutorialPage[] visibleWithPages;

    private void Awake()
    {
        TutorialPage[] visibleWithPages = this.visibleWithPages;
        for (int i = 0; i < visibleWithPages.Length; i++)
        {
            visibleWithPages[i].Register(this);
        }
        this.UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        bool flag = false;
        TutorialPage[] visibleWithPages = this.visibleWithPages;
        int index = 0;
        while (true)
        {
            if (index < visibleWithPages.Length)
            {
                if (!visibleWithPages[index].gameObject.activeSelf)
                {
                    index++;
                    continue;
                }
                flag = true;
            }
            base.gameObject.SetActive(flag);
            return;
        }
    }
}

