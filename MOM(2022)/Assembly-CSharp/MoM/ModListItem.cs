namespace MOM
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ModListItem : MonoBehaviour
    {
        public RawImage icon;
        public Toggle toggle;
        public TextMeshProUGUI text;
        public GameObject modOn;
        public GameObject modOff;
        public GameObject warning;
        public RolloverSimpleTooltip tooltip;
    }
}

