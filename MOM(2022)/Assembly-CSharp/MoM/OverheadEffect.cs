using System.Collections;
using MHUtils.UI;
using TMPro;
using UnityEngine;

namespace MOM
{
    public class OverheadEffect : MonoBehaviour
    {
        public TextMeshProUGUI labelHP;

        public TextMeshProUGUI labelFigures;

        public GameObject goHPLost;

        public GameObject goHPGained;

        public GameObject goFiguresLost;

        public GameObject goFiguresGained;

        public Animator animator;

        public float destroyAfter = 2.5f;

        private CombatEffect combatEffect;

        public void Set(CombatEffect ce)
        {
            this.combatEffect = ce;
            int num = -ce.hpLost;
            int num2 = -ce.figuresLost;
            this.goHPLost.SetActive(num < 0);
            this.goHPGained.SetActive(num > 0);
            this.goFiguresLost.SetActive(num2 < 0);
            this.goFiguresGained.SetActive(num2 > 0);
            this.labelHP.text = Mathf.Abs(num).ToString();
            this.labelFigures.gameObject.SetActive(num2 != 0);
            this.labelFigures.text = Mathf.Abs(num2).ToString();
            this.animator.SetTrigger("Show");
            base.StartCoroutine(this.OnFinish());
        }

        private IEnumerator OnFinish()
        {
            yield return new WaitForSeconds(this.destroyAfter);
            VerticalMarkerManager.Get().DestroyMarker(this.combatEffect);
        }
    }
}
