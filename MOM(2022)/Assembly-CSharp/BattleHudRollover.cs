// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// BattleHudRollover
using MOM;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ArmyListItem))]
public class BattleHudRollover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
    private ArmyListItem armyListItem;

    private void Awake()
    {
        this.armyListItem = base.GetComponent<ArmyListItem>();
    }

    private void OnDestroy()
    {
        CursorsLibrary.SetRMBAvailiable(availiable: false, "BattleHudRollover");
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        BattleUnit battleUnit = this.armyListItem.Unit as BattleUnit;
        BattleHUD.Get().OnArmyOver(battleUnit);
        if (!CameraController.IsAlreadyTargeted(this.armyListItem.Unit.GetPosition()) && battleUnit.currentlyVisible)
        {
            CursorsLibrary.SetRMBAvailiable(availiable: true, "BattleHudRollover");
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        BattleHUD.Get().OnArmyExit(this.armyListItem.Unit as BattleUnit);
        CursorsLibrary.SetRMBAvailiable(availiable: false, "BattleHudRollover");
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BattleHUD.Get().OnArmyClick(this.armyListItem.Unit as BattleUnit);
        }
        else
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
            BattleUnit bu = this.armyListItem.Unit as BattleUnit;
            if (!bu.currentlyVisible)
            {
                if (Battle.Get()?.GetHumanPlayerUnits().Find((BattleUnit o) => o.GetID() == bu.GetID()) != null)
                {
                    CameraController.CenterAt(bu.GetPosition());
                    CursorsLibrary.SetRMBAvailiable(availiable: false, "BattleHudRollover");
                }
            }
            else
            {
                CameraController.CenterAt(bu.GetPosition());
                CursorsLibrary.SetRMBAvailiable(availiable: false, "BattleHudRollover");
            }
        }
    }
}
