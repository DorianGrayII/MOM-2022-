using UnityEngine;

public class TutorialVisiblityByPage : MonoBehaviour
{
    public TutorialPage[] visibleWithPages;

    private void Awake()
    {
        TutorialPage[] array = this.visibleWithPages;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].Register(this);
        }
        this.UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        bool active = false;
        TutorialPage[] array = this.visibleWithPages;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].gameObject.activeSelf)
            {
                active = true;
                break;
            }
        }
        base.gameObject.SetActive(active);
    }
}
