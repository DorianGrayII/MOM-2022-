using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUI)]
    [Tooltip("GUI Box.")]
    public class GUIBox : GUIContentAction
    {
        public override void OnGUI()
        {
            base.OnGUI();
            if (string.IsNullOrEmpty(base.style.Value))
            {
                GUI.Box(base.rect, base.content);
            }
            else
            {
                GUI.Box(base.rect, base.content, base.style.Value);
            }
        }
    }
}
