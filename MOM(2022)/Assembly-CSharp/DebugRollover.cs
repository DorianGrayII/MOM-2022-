using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugRollover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        string text1;
        StringBuilder builder = new StringBuilder();
        for (Transform transform = base.transform; transform != null; transform = transform.parent)
        {
            builder.Append("->" + transform.gameObject.name);
        }
        if (builder != null)
        {
            text1 = builder.ToString();
        }
        else
        {
            StringBuilder local1 = builder;
            text1 = null;
        }
        Debug.LogWarning("->" + text1);
    }

    private void Start()
    {
        Debug.LogWarning("Debug rollover in use");
    }
}

