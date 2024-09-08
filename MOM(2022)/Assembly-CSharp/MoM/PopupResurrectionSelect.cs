using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupResurrectionSelect : ScreenBase
    {
        private static PopupResurrectionSelect instance;

        public Button btCancel;

        public GridItemManager gridUnits;

        private List<DeadHero> dhs;

        private Callback select;

        private Callback cancel;

        public static void OpenPopup(ScreenBase parent, List<DeadHero> dhs, Callback cancel = null, Callback select = null)
        {
            PopupResurrectionSelect.instance = UIManager.Open<PopupResurrectionSelect>(UIManager.Layer.Popup, parent);
            PopupResurrectionSelect.instance.dhs = dhs;
            PopupResurrectionSelect.instance.cancel = cancel;
            PopupResurrectionSelect.instance.select = select;
            PopupResurrectionSelect.instance.gridUnits.CustomDynamicItem(PopupResurrectionSelect.instance.UnitItem, PopupResurrectionSelect.instance.UpdateHeroesGrid);
            PopupResurrectionSelect.instance.gridUnits.UpdateGrid(dhs);
        }

        public static bool IsOpen()
        {
            return PopupResurrectionSelect.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupResurrectionSelect.instance = null;
            yield return base.Closing();
        }

        private void UnitItem(GameObject itemSource, object source, object data, int index)
        {
            DeadHero deadHero = source as DeadHero;
            CharacterListItem component = itemSource.GetComponent<CharacterListItem>();
            component.hp.value = 1f;
            component.portrait.texture = deadHero.dbSource.Get().GetDescriptionInfo().GetTexture();
            component.button.onClick.RemoveAllListeners();
            component.button.onClick.AddListener(delegate
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select(source);
                }
            });
            if (component.race != null)
            {
                component.race.gameObject.SetActive(value: false);
            }
            ((Unit)(itemSource.GetOrAddComponent<RolloverUnitTooltip>().sourceAsUnit = DeadHero.ConvertDeadHeroToUnit(deadHero))).Destroy();
        }

        private void UpdateHeroesGrid()
        {
            this.gridUnits.UpdateGrid(this.dhs);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btCancel)
            {
                UIManager.Close(this);
                if (this.cancel != null)
                {
                    this.cancel(null);
                }
            }
        }
    }
}
