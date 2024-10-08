using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUI)]
    [Tooltip("GUI Label.")]
    public class GUILabel : GUIContentAction
    {
        public override void OnGUI()
        {
            base.OnGUI();
            if (string.IsNullOrEmpty(base.style.Value))
            {
                GUI.Label(base.rect, base.content);
            }
            else
            {
                GUI.Label(base.rect, base.content, base.style.Value);
            }
        }
    }
}
