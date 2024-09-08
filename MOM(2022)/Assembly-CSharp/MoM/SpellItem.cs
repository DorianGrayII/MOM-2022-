using DBDef;
using MHUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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
            if ((bool)this.spellTypeCombat)
            {
                this.spellTypeCombat.SetActive(!string.IsNullOrEmpty(spell.battleScript));
            }
            if ((bool)this.spellTypeWorld)
            {
                this.spellTypeWorld.SetActive(!string.IsNullOrEmpty(spell.worldScript));
            }
            if ((bool)this.riSpellIcon)
            {
                this.riSpellIcon.texture = spell.GetDescriptionInfo().GetTexture();
            }
            if ((bool)this.labelName)
            {
                this.labelName.text = spell.GetDescriptionInfo().GetLocalizedName();
            }
            if (this.addTooltip)
            {
                base.gameObject.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = spell.dbName;
            }
        }
    }
}
