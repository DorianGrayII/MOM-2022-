using System.Collections.Generic;
using MHUtils.UI;
using UnityEngine;

namespace MOM
{
    public class UIReferences : MonoBehaviour
    {
        public static UIReferences instance;

        public List<GameObject> logicEditorComponents;

        public List<GameObject> nodeEditorComponents;

        public Material grayscale;

        public Texture2D transparent;

        public GameObject goTooltipSimple;

        public GameObject goTooltipUnit;

        public GameObject goTooltipEnchantment;

        public GameObject goTooltipEnchantmentDetail;

        public GameObject goTooltipArtefact;

        public GameObject goTooltipSurveyor;

        public GameObject goTooltipSpellbook;

        public GameObject goTooltipSpellBlast;

        public GameObject goTooltipStatDetails;

        public GameObject goTooltipCitizens;

        public GameObject markerTown;

        public GameObject markerArmy;

        public GameObject markerLocation;

        public GameObject markerCombatUnit;

        public GameObject markerMovement;

        public GameObject markerCombatEffect;

        public GameObject markerNode;

        public GameObject markerResource;

        public GameObject cursorInfoAttackRanged;

        public GameObject overheadEffect;

        private void Awake()
        {
            UIReferences.instance = this;
        }

        public static GameObject GetLogicComponent(string name)
        {
            if (UIReferences.instance.logicEditorComponents == null)
            {
                return null;
            }
            return UIReferences.instance.logicEditorComponents.Find((GameObject o) => o.name == name);
        }

        public static GameObject GetNodeComponent(string name)
        {
            if (UIReferences.instance.nodeEditorComponents == null)
            {
                return null;
            }
            return UIReferences.instance.nodeEditorComponents.Find((GameObject o) => o.name == name);
        }

        public static Material GetGrayscale()
        {
            return UIReferences.instance.grayscale;
        }

        public static Texture2D GetTransparent()
        {
            return UIReferences.instance.transparent;
        }

        public static GameObject GetMarkerGOSource(VerticalMarkerManager.MarkerType t)
        {
            switch (t)
            {
            case VerticalMarkerManager.MarkerType.markerTown:
                return UIReferences.instance.markerTown;
            case VerticalMarkerManager.MarkerType.markerArmy:
                return UIReferences.instance.markerArmy;
            case VerticalMarkerManager.MarkerType.markerLocation:
                return UIReferences.instance.markerLocation;
            case VerticalMarkerManager.MarkerType.markerCombatUnit:
                return UIReferences.instance.markerCombatUnit;
            case VerticalMarkerManager.MarkerType.markerMovement:
                return UIReferences.instance.markerMovement;
            case VerticalMarkerManager.MarkerType.markerCombatEffect:
                return UIReferences.instance.markerCombatEffect;
            case VerticalMarkerManager.MarkerType.markerNode:
                return UIReferences.instance.markerNode;
            case VerticalMarkerManager.MarkerType.markerResource:
                return UIReferences.instance.markerResource;
            default:
                Debug.LogError("Unknown marker type");
                return null;
            }
        }
    }
}
