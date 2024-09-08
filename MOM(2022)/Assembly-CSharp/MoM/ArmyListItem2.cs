namespace MOM
{
    using MHUtils;
    using MHUtils.UI;
    using System;
    using TMPro;
    using UnityEngine;

    public class ArmyListItem2 : MonoBehaviour
    {
        public ArmyGrid gridUnits;
        public TextMeshProUGUI labelArmyID;
        public TextMeshProUGUI labelArmySize;

        private void Awake()
        {
            MouseClickEvent orAddComponent = GameObjectUtils.GetOrAddComponent<MouseClickEvent>(base.gameObject);
            orAddComponent.mouseDoubleClick = delegate (object d) {
                ArmyManager manager = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (manager != null)
                {
                    Unit unit = this.gridUnits.GetUnits()[0] as Unit;
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        UIManager.Close(manager);
                        TownScreen screen = TownScreen.Get();
                        if (screen != null)
                        {
                            screen.Close();
                        }
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                    }
                }
            };
            orAddComponent.mouseLeftClick = delegate (object d) {
                ArmyManager screen = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if ((screen != null) && FSMMapGame.Get().IsCasting())
                {
                    Unit unit = this.gridUnits.GetUnits()[0] as Unit;
                    FSMMapGame.Get().SetChosenTarget(unit.GetPosition());
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                        UIManager.Close(screen);
                    }
                }
            };
        }
    }
}

