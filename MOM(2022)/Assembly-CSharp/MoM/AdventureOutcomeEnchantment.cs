using UnityEngine;

namespace MOM
{
    public class AdventureOutcomeEnchantment : AdventureOutcome
    {
        public EnchantmentListItem enchantment;

        public EnchantmentTargetListItem enchantmentTarget;

        public GameObject arrowAdd;

        public GameObject arrowRemove;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            if ((bool)this.arrowAdd)
            {
                this.arrowAdd.SetActive(o.delta >= 0);
            }
            if ((bool)this.arrowRemove)
            {
                this.arrowRemove.SetActive(o.delta < 0);
            }
            EnchantmentInstance enchantmentInstance = o.thing as EnchantmentInstance;
            this.enchantment.Set(enchantmentInstance);
            this.enchantmentTarget.Set(enchantmentInstance);
        }
    }
}
