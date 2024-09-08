namespace MOM
{
    using MHUtils.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TooltipSpell : TooltipBase
    {
        public TextMeshProUGUI spellName;
        public TextMeshProUGUI spellDescription;
        public TextMeshProUGUI spellCastingCostWorld;
        public TextMeshProUGUI spellCastingCostCombat;
        public TextMeshProUGUI spellTarget;
        public GridItemManager spellsEnchantments;
        public RawImage icon;
        public GameObject graphicWrapper;
        public GameObject spellDomainNature;
        public GameObject spellDomainChaos;
        public GameObject spellDomainLife;
        public GameObject spellDomainDeath;
        public GameObject spellDomainSorcery;
        public GameObject spellDomainArcane;
        public GameObject spellTypeWorld;
        public GameObject spellTypeCombat;
        public GameObject spellTypeSummon;
        public GameObject spellTypeEnchantment;
        public GameObject spellTypeInstant;
        public GameObject spellTypeSpecial;
        public GameObject spellCostWorld;
        public GameObject spellCostCombat;
    }
}

