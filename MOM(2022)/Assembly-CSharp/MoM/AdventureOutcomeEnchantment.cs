namespace MOM
{
    using System;
    using UnityEngine;

    public class AdventureOutcomeEnchantment : AdventureOutcome
    {
        public EnchantmentListItem enchantment;
        public EnchantmentTargetListItem enchantmentTarget;
        public GameObject arrowAdd;
        public GameObject arrowRemove;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            if (this.arrowAdd)
            {
                this.arrowAdd.SetActive(o.delta >= 0);
            }
            if (this.arrowRemove)
            {
                this.arrowRemove.SetActive(o.delta < 0);
            }
            EnchantmentInstance thing = o.thing as EnchantmentInstance;
            this.enchantment.Set(thing);
            this.enchantmentTarget.Set(thing);
        }
    }
}

