using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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
