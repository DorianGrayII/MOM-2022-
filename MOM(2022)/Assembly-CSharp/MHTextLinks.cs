using MHUtils.UI;
using TMPro;
using UnityEngine;

public class MHTextLinks : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private bool tooltipOpen;

    private void Start()
    {
        this.textMesh = base.GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        if (this.tooltipOpen)
        {
            TooltipBase.Close();
            this.tooltipOpen = false;
        }
    }

    private void Update()
    {
        int num = TMP_TextUtilities.FindIntersectingLink(this.textMesh, Input.mousePosition, null);
        if (num != -1)
        {
            TMP_LinkInfo tMP_LinkInfo = this.textMesh.textInfo.linkInfo[num];
            if (UIManager.IsTopForInput(base.gameObject.GetComponentInParent<ScreenBase>()) && !this.tooltipOpen)
            {
                TooltipBase.Open(tMP_LinkInfo.GetLinkID());
                this.tooltipOpen = true;
            }
        }
        else if (this.tooltipOpen)
        {
            TooltipBase.Close();
            this.tooltipOpen = false;
        }
    }
}
