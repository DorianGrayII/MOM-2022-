using MHUtils;
using MHUtils.UI;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MouseTooltip : MonoBehaviour
{
    private static MouseTooltip instance;
    private static GameObject cursorAttachment;
    private static string tempMessage;

    public static void Close()
    {
        if (cursorAttachment != null)
        {
            Destroy(cursorAttachment.gameObject);
        }
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
    }

    public static void Open(object source, string message)
    {
        if (instance == null)
        {
            GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
            if (source is GameObject)
            {
                cursorAttachment = GameObjectUtils.Instantiate(source as GameObject, layer.transform);
                cursorAttachment.transform.position = Vector3.zero;
                if (message != null)
                {
                    cursorAttachment.GetComponentInChildren<TextMeshProUGUI>().text = message + "%";
                    tempMessage = message;
                }
            }
            instance = GameObjectUtils.GetOrAddComponent<MouseTooltip>(cursorAttachment);
        }
        if (tempMessage != message)
        {
            cursorAttachment.GetComponentInChildren<TextMeshProUGUI>().text = message + "%";
            tempMessage = message;
        }
    }

    private void Update()
    {
        if ((instance != null) && ((instance == this) && (cursorAttachment != null)))
        {
            cursorAttachment.transform.position = Input.mousePosition;
        }
    }
}

