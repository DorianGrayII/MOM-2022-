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
        Vector2 vector = default(Vector2);
        Vector2 vector2 = default(Vector2);
        if (!this.horizontal && !this.vertical)
        {
            return;
        }
        foreach (RectTransform item in base.transform)
        {
            if (vector.x > item.offsetMin.x)
            {
                vector.x = item.offsetMin.x;
            }
            if (vector.y > item.offsetMin.y)
            {
                vector.y = item.offsetMin.y;
            }
            if (vector2.x < item.offsetMax.x)
            {
                vector2.x = item.offsetMax.x;
            }
            if (vector2.y < item.offsetMax.y)
            {
                vector2.y = item.offsetMax.y;
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
