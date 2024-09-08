using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class SpellListItem : MonoBehaviour
    {
        public TextMeshProUGUI labelName;

        public TextMeshProUGUI labelResearchCost;

        public TextMeshProUGUI labelResearchTime;

        public TextMeshProUGUI labelCastingCost;

        public TextMeshProUGUI labelCastingTime;

        public TextMeshProUGUI labelCastingSkill;

        public RawImage riSpellIcon;

        public GameObject spellDomainNature;

        public GameObject spellDomainChaos;

        public GameObject spellDomainLife;

        public GameObject spellDomainDeath;

        public GameObject spellDomainSorcery;

        public GameObject spellDomainArcane;

        public GameObject spellDomainTech;

        public GameObject spellTypes;

        public GameObject spellTypeWorld;

        public GameObject spellTypeCombat;

        public GameObject spellTypeSummon;

        public GameObject spellTypeEnchantment;

        public GameObject spellTypeInstant;

        public GameObject spellTypeSpecial;

        public GameObject spellRarityCommon;

        public GameObject spellRarityUncommon;

        public GameObject spellRarityRare;

        public GameObject spellRarityVRare;

        public GameObject currentlySelected;

        public GameObject spellCastingCost;

        public GameObject spellCastingTime;

        public GameObject spellCastingSkill;

        public GameObject favCannotCast;

        public GameObject favMenu;

        public Toggle tgSpell;

        public Button btRemoveFav;

        public void Awake()
        {
            base.gameObject.GetOrAddComponent<MouseClickEvent>().mouseDoubleClick = delegate
            {
                ResearchSpells screen = UIManager.GetScreen<ResearchSpells>(UIManager.Layer.Standard);
                if (screen != null)
                {
                    screen.btResearch.onClick.Invoke();
                }
                else
                {
                    CastSpells screen2 = UIManager.GetScreen<CastSpells>(UIManager.Layer.Standard);
                    if (screen2 == null)
                    {
                        screen2 = UIManager.GetScreen<CastSpells>(UIManager.Layer.Popup);
                    }
                    if (screen2 != null && screen2.btCast.isActiveAndEnabled && screen2.btCast.IsInteractable())
                    {
                        screen2.btCast.onClick.Invoke();
                    }
                }
            };
        }
    }
}
