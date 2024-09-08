// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CreditsAnim
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAnim : MonoBehaviour
{
    public GameObject imageParent;

    private List<CanvasGroup> pagesArray;

    public float pageDisplayTime = 4f;

    public float transitionTime = 1f;

    private void OnEnable()
    {
        this.pagesArray = new List<CanvasGroup>();
        foreach (Transform item in this.imageParent.transform)
        {
            CanvasGroup component = item.gameObject.GetComponent<CanvasGroup>();
            if (!(component == null))
            {
                component.alpha = 0f;
                item.gameObject.SetActive(value: true);
                this.pagesArray.Add(component);
            }
        }
        base.StartCoroutine(this.AnimatePages());
    }

    private IEnumerator AnimatePages()
    {
        int currentPage = 0;
        yield return this.OpenPage(currentPage);
        while (currentPage < this.pagesArray.Count - 1)
        {
            yield return new WaitForSeconds(this.pageDisplayTime);
            yield return this.ClosePage(currentPage);
            currentPage++;
            yield return this.OpenPage(currentPage);
        }
    }

    private IEnumerator OpenPage(int index)
    {
        float time = 0f;
        while (time < this.transitionTime)
        {
            time = Mathf.Min(this.transitionTime, time + Time.deltaTime);
            this.pagesArray[index].alpha = time / this.transitionTime;
            yield return null;
        }
    }

    private IEnumerator ClosePage(int index)
    {
        float time = 0f;
        while (time < this.transitionTime)
        {
            time = Mathf.Min(this.transitionTime, time + Time.deltaTime);
            this.pagesArray[index].alpha = 1f - time / this.transitionTime;
            yield return null;
        }
    }
}
