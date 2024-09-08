using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithPulse : Button
{
    public void Pulse()
    {
        if (this.IsActive() && this.IsInteractable())
        {
            this.DoStateTransition(SelectionState.Pressed, instant: false);
            base.StartCoroutine(this.OnFinishPulse());
        }
    }

    private IEnumerator OnFinishPulse()
    {
        float fadeTime = base.colors.fadeDuration;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        this.DoStateTransition(base.currentSelectionState, instant: false);
    }
}
