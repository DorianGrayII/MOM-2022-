using System.Collections;
using MOM;
using UnityEngine;

public class TutorialOpenOnEnable : MonoBehaviour
{
    public Tutorial_Generic openTutorial;

    private Coroutine coroutine;

    private void OnEnable()
    {
        if (this.coroutine != null)
        {
            base.StopCoroutine(this.coroutine);
            this.coroutine = null;
        }
        this.coroutine = base.StartCoroutine(EnsureWeStayVisible());
        IEnumerator EnsureWeStayVisible()
        {
            yield return null;
            if (base.isActiveAndEnabled && this.openTutorial.OpenIfNotSeen(base.gameObject) == null)
            {
                Tutorial_Generic.Hide(excludeTop: false);
            }
        }
    }

    private void OnDisable()
    {
        if (this.coroutine != null)
        {
            base.StopCoroutine(this.coroutine);
            this.coroutine = null;
        }
        Tutorial_Generic.CloseAllOnParent(base.gameObject);
    }
}
