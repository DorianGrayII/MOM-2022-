using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugRollover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
    private void Start()
    {
        Debug.LogWarning("Debug rollover in use");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StringBuilder stringBuilder = new StringBuilder();
        Transform parent = base.transform;
        while (parent != null)
        {
            stringBuilder.Append("->" + parent.gameObject.name);
            parent = parent.parent;
        }
        Debug.LogWarning("->" + stringBuilder);
    }
}
