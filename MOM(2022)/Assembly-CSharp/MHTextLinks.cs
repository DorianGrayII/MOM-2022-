using MHUtils.UI;
using System;
using TMPro;
using UnityEngine;

public class MHTextLinks : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private bool tooltipOpen;

    private void OnDestroy()
    {
        if (this.tooltipOpen)
        {
            TooltipBase.Close();
            this.tooltipOpen = false;
        }
    }

    private void Start()
    {
        this.textMesh = base.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int index = TMP_TextUtilities.FindIntersectingLink(this.textMesh, Input.mousePosition, null);
        if (index == -1)
        {
            if (this.tooltipOpen)
            {
                TooltipBase.Close();
                this.tooltipOpen = false;
            }
        }
        else
        {
            TMP_LinkInfo info = this.textMesh.textInfo.linkInfo[index];
            if (UIManager.IsTopForInput(base.gameObject.GetComponentInParent<ScreenBase>()) && !this.tooltipOpen)
            {
                TooltipBase.Open(info.GetLinkID(), null);
                this.tooltipOpen = true;
            }
        }
    }
}

