using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;

namespace MOM
{
    public class ArmyListItem2 : MonoBehaviour
    {
        public ArmyGrid gridUnits;

        public TextMeshProUGUI labelArmyID;

        public TextMeshProUGUI labelArmySize;

        private void Awake()
        {
            MouseClickEvent orAddComponent = base.gameObject.GetOrAddComponent<MouseClickEvent>();
            orAddComponent.mouseDoubleClick = delegate
            {
                ArmyManager screen2 = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (!(screen2 == null))
                {
                    Unit unit2 = this.gridUnits.GetUnits()[0] as Unit;
                    if (unit2?.group != null)
                    {
                        UIManager.Close(screen2);
                        TownScreen townScreen = TownScreen.Get();
                        if (townScreen != null)
                        {
                            townScreen.Close();
                        }
                        FSMSelectionManager.Get().Select(unit2.group.Get(), focus: true);
                    }
                }
            };
            orAddComponent.mouseLeftClick = delegate
            {
                ArmyManager screen = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (!(screen == null) && FSMMapGame.Get().IsCasting())
                {
                    Unit unit = this.gridUnits.GetUnits()[0] as Unit;
                    FSMMapGame.Get().SetChosenTarget(unit.GetPosition());
                    if (unit?.group != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), focus: true);
                        UIManager.Close(screen);
                    }
                }
            };
        }
    }
}
