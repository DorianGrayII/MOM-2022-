using DBDef;
using MHUtils;
using MOM;
using System;
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
        if (this.world)
        {
            this.world.SetActive(false);
        }
        if (this.target)
        {
            this.target.SetActive(!flag);
        }
        if (this.town)
        {
            this.town.SetActive(false);
        }
        if (!flag)
        {
            if (this.itemOwningWizard)
            {
                this.itemOwningWizard.gameObject.SetActive(!flag);
                this.itemOwningWizard.Set(manager.owner.GetWizardOwner());
            }
            MOM.Location owner = manager.owner as MOM.Location;
            if (owner != null)
            {
                TownLocation location2 = owner as TownLocation;
                if (this.townName)
                {
                    this.townName.text = location2.name;
                }
                if (this.town)
                {
                    this.town.SetActive(true);
                }
                if (this.target)
                {
                    this.target.SetActive(false);
                }
            }
            else
            {
                IEnchantable enchantable = manager.owner;
                BaseUnit unit = enchantable as BaseUnit;
                if (unit != null)
                {
                    this.targetIcon.texture = DescriptionInfoExtension.GetTexture(unit.GetDescriptionInfo());
                    GameObjectUtils.GetOrAddComponent<RolloverObject>(base.gameObject).source = enchantable;
                }
                else
                {
                    BattlePlayer player = enchantable as BattlePlayer;
                    if (player != null)
                    {
                        this.targetIcon.texture = player.wizard.Graphic;
                    }
                    else
                    {
                        PlayerWizard wizard = enchantable as PlayerWizard;
                        if (wizard != null)
                        {
                            this.targetIcon.texture = wizard.Graphic;
                        }
                    }
                }
            }
        }
    }
}

