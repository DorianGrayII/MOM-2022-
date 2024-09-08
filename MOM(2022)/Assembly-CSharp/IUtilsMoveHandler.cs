using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IUtilsMoveHandler
{
    bool OnMove(Selectable from, AxisEventData eventData);
}
