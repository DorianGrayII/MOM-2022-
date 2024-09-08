using System;
using UnityEngine;

namespace MOM
{
    public class AdventureOutcomeStats : AdventureOutcome
    {
        [Serializable]
        public class StatIcon
        {
            public StatType statType;

            public GameObject gameObject;
        }

        public StatIcon[] statIcons;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            StatIcon[] array = this.statIcons;
            foreach (StatIcon statIcon in array)
            {
                statIcon.gameObject.SetActive(o.statType == statIcon.statType);
            }
        }
    }
}
