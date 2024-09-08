namespace MOM
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CasterListItem : MonoBehaviour
    {
        public TextMeshProUGUI labelMana;
        public RawImage riCasterIcon;
        public RawImage riCasterIconUsed;
        public GameObject goCasterUsed;
        public Material matCasterReady;
        public Material matCasterUsed;
    }
}

