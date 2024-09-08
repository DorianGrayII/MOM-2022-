namespace MOM
{
    using MHUtils.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MarkerTown : MonoBehaviour
    {
        public TextMeshProUGUI labelName;
        public TextMeshProUGUI labelPopulation;
        public TextMeshProUGUI labelBuildTime;
        public TextMeshProUGUI labelPopIncreaseTime;
        public TextMeshProUGUI labelNumberOfUnits;
        public GameObject goProduction;
        public GameObject goPopulation;
        public GameObject goCapital;
        public GameObject goSummoningCircle;
        public GameObject goGreen;
        public GameObject goBlue;
        public GameObject goRed;
        public GameObject goPurple;
        public GameObject goYellow;
        public GameObject goBrown;
        public GameObject goGuard;
        public GameObject goUnrestWarning;
        public GameObject goStarvationWarning;
        public RawImage riCurrentTask;
        public GridItemManager gridEnchantments;
    }
}

