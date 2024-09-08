namespace MOM
{
    using System;
    using TMPro;
    using UnityEngine.EventSystems;

    public class CombatLogText : RolloverObject
    {
        public TextMeshProUGUI label;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if ((base.source as BattleHUD.CombatSummary).details != null)
            {
                base.OnPointerEnter(eventData);
            }
        }

        public void Set(BattleHUD.CombatSummary summary)
        {
            base.source = summary;
            if ((summary.details != null) && (summary.details.Count > 0))
            {
                this.label.text = summary.summary;
            }
            else
            {
                this.label.text = summary.summary;
            }
        }
    }
}

