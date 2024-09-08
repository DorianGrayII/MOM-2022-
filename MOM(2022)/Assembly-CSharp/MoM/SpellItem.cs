namespace MOM
{
    using DBDef;
    using MHUtils;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpellItem : MonoBehaviour
    {
        public TextMeshProUGUI labelName;
        public RawImage riSpellIcon;
        public GameObject spellTypeWorld;
        public GameObject spellTypeCombat;
        public GameObjectEnabler<ERealm> realmGameObjects;
        public GameObjectEnabler<ERarity> rarityGameObjects;
        public GameObjectEnabler<ETargetType> targetTypeGameObjects;
        public bool addTooltip = true;

        public void Set(Spell spell)
        {
            this.realmGameObjects.Set(spell.realm);
            this.rarityGameObjects.Set(spell.rarity);
            this.targetTypeGameObjects.Set(spell.targetType.enumType);
            if (this.spellTypeCombat)
            {
                this.spellTypeCombat.SetActive(!string.IsNullOrEmpty(spell.battleScript));
            }
            if (this.spellTypeWorld)
            {
                this.spellTypeWorld.SetActive(!string.IsNullOrEmpty(spell.worldScript));
            }
            if (this.riSpellIcon)
            {
                this.riSpellIcon.texture = DescriptionInfoExtension.GetTexture(spell.GetDescriptionInfo());
            }
            if (this.labelName)
            {
                this.labelName.text = spell.GetDescriptionInfo().GetLocalizedName();
            }
            if (this.addTooltip)
            {
                GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(base.gameObject).sourceAsDbName = spell.dbName;
            }
        }
    }
}

