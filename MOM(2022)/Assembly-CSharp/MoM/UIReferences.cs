namespace MOM
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
            instance = this;
        }

        public static Material GetGrayscale()
        {
            return instance.grayscale;
        }

        public static GameObject GetLogicComponent(string name)
        {
            return instance.logicEditorComponents?.Find(o => o.name == name);
        }

        public static GameObject GetMarkerGOSource(VerticalMarkerManager.MarkerType t)
        {
            switch (t)
            {
                case VerticalMarkerManager.MarkerType.markerTown:
                    return instance.markerTown;

                case VerticalMarkerManager.MarkerType.markerArmy:
                    return instance.markerArmy;

                case VerticalMarkerManager.MarkerType.markerLocation:
                    return instance.markerLocation;

                case VerticalMarkerManager.MarkerType.markerCombatUnit:
                    return instance.markerCombatUnit;

                case VerticalMarkerManager.MarkerType.markerMovement:
                    return instance.markerMovement;

                case VerticalMarkerManager.MarkerType.markerCombatEffect:
                    return instance.markerCombatEffect;

                case VerticalMarkerManager.MarkerType.markerNode:
                    return instance.markerNode;

                case VerticalMarkerManager.MarkerType.markerResource:
                    return instance.markerResource;
            }
            Debug.LogError("Unknown marker type");
            return null;
        }

        public static GameObject GetNodeComponent(string name)
        {
            return instance.nodeEditorComponents?.Find(o => o.name == name);
        }

        public static Texture2D GetTransparent()
        {
            return instance.transparent;
        }
    }
}

