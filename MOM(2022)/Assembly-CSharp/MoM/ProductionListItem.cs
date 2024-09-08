using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class ProductionListItem : MonoBehaviour
    {
        public TextMeshProUGUI label;

        public RawImage icon;

        public TextMeshProUGUI productionCost;

        public TextMeshProUGUI productionTime;

        public Button btAddToQueue;
    }
}
