namespace MOM
{
    using DBDef;
    using MHUtils.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PowerListItem : GridItemBase
    {
        public Toggle toggle;
        public TextMeshProUGUI text;
        public GameObject divider;
        public ArtefactPower power;
        public RolloverSimpleTooltip tooltip;
    }
}

