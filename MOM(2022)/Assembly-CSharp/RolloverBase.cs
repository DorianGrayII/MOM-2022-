using MHUtils;
using UnityEngine;
using UnityEngine.EventSystems;

public class RolloverBase : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool useMouseLocation = true;

    [Tooltip("The position on the tooltip")]
    public Vector2 anchor = Vector2.zero;

    [Tooltip("The position on the rect that you mouse over to open the tooltip")]
    public Vector2 position;

    public Vector2 offset;

    [Tooltip("Specify a different object to derive the position from")]
    public GameObject optionalPosition;

    [Tooltip("Show the title only until you right click")]
    public bool titleOnlyUntilRightClick;

    private bool isOpen;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        MHEventSystem.TriggerEvent<RolloverBase>(this, "OnPointerEnter");
        TooltipBase.Open(this);
        this.isOpen = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        this.Close();
    }

    public void Close()
    {
        if (this.isOpen)
        {
            TooltipBase.Close();
            this.isOpen = false;
        }
        MHEventSystem.TriggerEvent<RolloverBase>(this, "OnPointerExit");
    }

    private void OnDisable()
    {
        this.Close();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (this.titleOnlyUntilRightClick && eventData.button == PointerEventData.InputButton.Right)
        {
            TooltipBase.Expand();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            MHEventSystem.TriggerEvent<RolloverBase>(this, "OnPointerClick");
        }
    }
}
