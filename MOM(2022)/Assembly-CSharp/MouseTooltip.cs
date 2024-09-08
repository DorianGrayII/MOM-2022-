using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;

public class MouseTooltip : MonoBehaviour
{
    private static MouseTooltip instance;

    private static GameObject cursorAttachment;

    private static string tempMessage;

    public static void Open(object source, string message = null)
    {
        if (MouseTooltip.instance == null)
        {
            GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
            if (source is GameObject)
            {
                MouseTooltip.cursorAttachment = GameObjectUtils.Instantiate(source as GameObject, layer.transform);
                MouseTooltip.cursorAttachment.transform.position = Vector3.zero;
                if (message != null)
                {
                    MouseTooltip.cursorAttachment.GetComponentInChildren<TextMeshProUGUI>().text = message + "%";
                    MouseTooltip.tempMessage = message;
                }
            }
            MouseTooltip.instance = MouseTooltip.cursorAttachment.GetOrAddComponent<MouseTooltip>();
        }
        if (MouseTooltip.tempMessage != message)
        {
            MouseTooltip.cursorAttachment.GetComponentInChildren<TextMeshProUGUI>().text = message + "%";
            MouseTooltip.tempMessage = message;
        }
    }

    private void Update()
    {
        if (MouseTooltip.instance != null && MouseTooltip.instance == this && MouseTooltip.cursorAttachment != null)
        {
            MouseTooltip.cursorAttachment.transform.position = Input.mousePosition;
        }
    }

    public static void Close()
    {
        if (MouseTooltip.cursorAttachment != null)
        {
            Object.Destroy(MouseTooltip.cursorAttachment.gameObject);
        }
        if (MouseTooltip.instance != null)
        {
            Object.Destroy(MouseTooltip.instance.gameObject);
        }
    }
}
