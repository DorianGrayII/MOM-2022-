namespace MOM
{
    using System;
    using UnityEngine;

    public class AdventureOutcomeStats : AdventureOutcome
    {
        public StatIcon[] statIcons;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            foreach (StatIcon icon in this.statIcons)
            {
                icon.gameObject.SetActive(o.statType == icon.statType);
            }
        }

        [Serializable]
        public class StatIcon
        {
            public AdventureOutcome.StatType statType;
            public GameObject gameObject;
        }
    }
}

