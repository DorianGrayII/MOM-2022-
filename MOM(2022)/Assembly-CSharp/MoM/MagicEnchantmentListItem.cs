using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class MagicEnchantmentListItem : MonoBehaviour
    {
        public EnchantmentListItem enchantment;

        public EnchantmentTargetListItem enchantmentTarget;

        public TextMeshProUGUI labelUpkeep;

        public GameObject goUpkeep;

        public Button btCancelEnchantment;
    }
}
