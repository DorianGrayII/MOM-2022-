namespace MOM
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TradeListItem : MonoBehaviour
    {
        public RawImage icon;
        public TextMeshProUGUI labelQuantity;
        public TextMeshProUGUI labelValue;
        public Button button;
        public GameObject aiInterested;
        public GameObject aiNotInterested;
        public GameObject quantity;
        public GameObject value;
    }
}

