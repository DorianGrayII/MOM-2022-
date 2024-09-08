using System.Collections;
using DBDef;
using DBEnum;
using MHUtils.UI;
using UnityEngine.UI;

namespace MOM
{
    public class PopupSpellWard : ScreenBase
    {
        public Button btCancel;

        public Button btNatureWard;

        public Button btSorceryWard;

        public Button btChaosWard;

        public Button btLifeWard;

        public Button btDeathWard;

        private static PopupSpellWard instance;

        private Callback select;

        private Callback cancel;

        public static void OpenPopup(ScreenBase parent, Callback cancel = null, Callback select = null)
        {
            PopupSpellWard.instance = UIManager.Open<PopupSpellWard>(UIManager.Layer.Popup, parent);
            PopupSpellWard.instance.cancel = cancel;
            PopupSpellWard.instance.select = select;
        }

        public static bool IsOpen()
        {
            return PopupSpellWard.instance != null;
        }

        public override IEnumerator Closing()
        {
            PopupSpellWard.instance = null;
            yield return base.Closing();
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
            else if (s == this.btNatureWard)
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select((Enchantment)ENCH.SPELL_WARD_NATURE);
                }
            }
            else if (s == this.btSorceryWard)
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select((Enchantment)ENCH.SPELL_WARD_SORCERY);
                }
            }
            else if (s == this.btChaosWard)
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select((Enchantment)ENCH.SPELL_WARD_CHAOS);
                }
            }
            else if (s == this.btLifeWard)
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select((Enchantment)ENCH.SPELL_WARD_LIFE);
                }
            }
            else if (s == this.btDeathWard)
            {
                UIManager.Close(this);
                if (this.select != null)
                {
                    this.select((Enchantment)ENCH.SPELL_WARD_DEATH);
                }
            }
        }
    }
}
