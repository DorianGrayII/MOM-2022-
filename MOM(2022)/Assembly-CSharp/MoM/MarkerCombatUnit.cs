using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class MarkerCombatUnit : MonoBehaviour
    {
        public GameObject goBrown;

        public GameObject goGreen;

        public GameObject goBlue;

        public GameObject goRed;

        public GameObject goPurple;

        public GameObject goYellow;

        public GameObject goUnitVisible;

        public GameObject goUnitInvisible;

        public GameObject goFlying;

        public GameObject goWaterOnly;

        public GameObject goGroundOnly;

        public GameObject goSpellcaster;

        public CanvasGroup canvasMarker;

        public RawImage classIcon;

        public Slider sliderHp;

        public GridItemManager gridEnchantments;

        public UnitExperience experience;
    }
}
