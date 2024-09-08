using System;
using UnityEngine;

[ExecuteInEditMode]
public class ContentSizeFitter2 : MonoBehaviour
{
    private RectTransform rt;
    public bool horizontal;
    public bool vertical;

    private void Start()
    {
        this.rt = base.gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 vector = new Vector2();
        Vector2 vector2 = new Vector2();
        if (this.horizontal || this.vertical)
        {
            foreach (RectTransform transform in base.transform)
            {
                if (vector.x > transform.offsetMin.x)
                {
                    vector.x = transform.offsetMin.x;
                }
                if (vector.y > transform.offsetMin.y)
                {
                    vector.y = transform.offsetMin.y;
                }
                if (vector2.x < transform.offsetMax.x)
                {
                    vector2.x = transform.offsetMax.x;
                }
                if (vector2.y < transform.offsetMax.y)
                {
                    vector2.y = transform.offsetMax.y;
                }
            }
            Vector2 sizeDelta = this.rt.sizeDelta;
            if (this.horizontal)
            {
                sizeDelta.x = vector2.x - vector.x;
            }
            if (this.vertical)
            {
                sizeDelta.y = vector2.y - vector.y;
            }
            this.rt.sizeDelta = sizeDelta;
        }
    }
}

