using MHUtils;
using UnityEngine;
using UnityEngine.EventSystems;

public class RollOverOut : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
    public object data;

    private bool rolledOver;

    private void DoExit()
    {
        if (this.rolledOver)
        {
            MHEventSystem.TriggerEvent<RollOverOut>(this, "Exit");
            this.rolledOver = false;
        }
    }

    private void DoEnter()
    {
        if (!this.rolledOver)
        {
            MHEventSystem.TriggerEvent<RollOverOut>(this, "Enter");
            this.rolledOver = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.DoEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.DoExit();
    }

    private void OnDestroy()
    {
        this.DoExit();
    }

    private void OnDisable()
    {
        this.DoExit();
    }
}
