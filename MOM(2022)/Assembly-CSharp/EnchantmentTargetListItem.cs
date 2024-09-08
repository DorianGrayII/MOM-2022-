using DBDef;
using MHUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnchantmentTargetListItem : MonoBehaviour
{
    public GameObject target;

    public GameObject world;

    public GameObject town;

    public TextMeshProUGUI townName;

    public RawImage targetIcon;

    public WizardItem itemOwningWizard;

    private object enchantment;

    public virtual void Set(EnchantmentInstance ei)
    {
        EnchantmentManager manager = ei.manager;
        bool flag = manager.owner is GameManager;
        if ((bool)this.world)
        {
            this.world.SetActive(value: false);
        }
        if ((bool)this.target)
        {
            this.target.SetActive(!flag);
        }
        if ((bool)this.town)
        {
            this.town.SetActive(value: false);
        }
        if (flag)
        {
            return;
        }
        if ((bool)this.itemOwningWizard)
        {
            this.itemOwningWizard.gameObject.SetActive(!flag);
            this.itemOwningWizard.Set(manager.owner.GetWizardOwner());
        }
        if (manager.owner is global::MOM.Location location)
        {
            TownLocation townLocation = location as TownLocation;
            if ((bool)this.townName)
            {
                this.townName.text = townLocation.name;
            }
            if ((bool)this.town)
            {
                this.town.SetActive(value: true);
            }
            if ((bool)this.target)
            {
                this.target.SetActive(value: false);
            }
        }
        else
        {
            IEnchantable owner = manager.owner;
            if (owner is BaseUnit baseUnit)
            {
                this.targetIcon.texture = baseUnit.GetDescriptionInfo().GetTexture();
                base.gameObject.GetOrAddComponent<RolloverObject>().source = owner;
            }
            else if (owner is BattlePlayer battlePlayer)
            {
                this.targetIcon.texture = battlePlayer.wizard.Graphic;
            }
            else if (owner is PlayerWizard playerWizard)
            {
                this.targetIcon.texture = playerWizard.Graphic;
            }
        }
    }
}
