using System.Collections;
using System.Collections.Generic;
using MHUtils.UI;
using UnityEngine;

namespace MOM
{
    public class CombatEffect
    {
        public BattleUnit unit;

        public int hpLost;

        public int figuresLost;

        private static Dictionary<int, List<CombatEffect>> queues = new Dictionary<int, List<CombatEffect>>();

        public CombatEffect(BattleUnit bu, int lost, int figures)
        {
            this.unit = bu;
            this.hpLost = lost;
            this.figuresLost = figures;
            CombatEffect.queues.TryGetValue(bu.ID, out var value);
            if (value == null)
            {
                value = new List<CombatEffect>();
                CombatEffect.queues[bu.ID] = value;
            }
            value.Add(this);
            if (value.Count == 1)
            {
                BattleHUD.Get().StartCoroutine(this.DelayMarker());
            }
        }

        private IEnumerator DelayMarker()
        {
            List<CombatEffect> queue = CombatEffect.queues[this.unit.ID];
            while (queue.Count > 0)
            {
                CombatEffect effect = queue[0];
                yield return new WaitForSeconds(0.1f);
                Formation formation = this.unit.GetOrCreateFormation();
                if (formation != null)
                {
                    while (true)
                    {
                        bool flag = false;
                        foreach (CharacterActor characterActor in formation.GetCharacterActors())
                        {
                            if (characterActor.IsAnimatingNotYetHit())
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(0.75f);
                VerticalMarkerManager.Get().Addmarker(effect);
                queue.RemoveAt(0);
            }
        }
    }
}
