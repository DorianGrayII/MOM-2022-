// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.HeroEquip
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquip : ScreenBase
{
    public Button btClose;

    public Button btConfirmTransfer;

    public Button btCancelTransfer;

    public GridItemManager gridHeroes;

    public GridItemManager gridFortressVault;

    public TextMeshProUGUI itemLocationInfo;

    public TextMeshProUGUI itemName;

    public TextMeshProUGUI transferCost;

    public TextMeshProUGUI manaReserve;

    public GameObject payToTransfer;

    public GameObject notEnoughMana;

    private BaseUnit rolloverHero;

    public override IEnumerator PreStart()
    {
        this.gridHeroes.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            HeroListItem component = itemSource.GetComponent<HeroListItem>();
            Reference<global::MOM.Unit> unit = source as Reference<global::MOM.Unit>;
            component.heroName.text = unit.Get().GetName();
            component.heroPortrait.texture = unit.Get().GetDescriptionInfo().GetTexture();
            component.heroPortrait.gameObject.GetOrAddComponent<RolloverUnitTooltip>().sourceAsUnit = (global::MOM.Unit)unit;
            component.gridItemSlots.CustomDynamicItem(delegate(GameObject itemSource2, object source2, object data2, int index2)
            {
                HeroSlotItem component2 = itemSource2.GetComponent<HeroSlotItem>();
                EquipmentSlot art = source2 as EquipmentSlot;
                component2.itemSlotArmor.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.ARMOUR && art.item == null);
                component2.itemSlotCaster.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.SPELLCASTER && art.item == null);
                component2.itemSlotCasterMelee.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.MELEE_SPELLCASTER && art.item == null);
                component2.itemSlotMelee.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.MELEE && art.item == null);
                component2.itemSlotMeleeRanged.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.RANGED && art.item == null);
                component2.itemSlotMisc.SetActive(art.slotType.Get() == (ArtefactSlot)ARTEFACT_SLOT.MISC && art.item == null);
                RawImage rawImage = GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource2, "ItemSlot");
                if (art.item != null)
                {
                    global::MOM.Artefact item = art.item;
                    rawImage.gameObject.SetActive(value: true);
                    rawImage.texture = AssetManager.Get<Texture2D>(item.graphic);
                    itemSource2.GetOrAddComponent<RolloverObject>().source = item;
                }
                else
                {
                    RolloverObject component3 = itemSource2.GetComponent<RolloverObject>();
                    if (component3 != null)
                    {
                        global::UnityEngine.Object.Destroy(component3);
                    }
                    rawImage.gameObject.SetActive(value: false);
                }
                DragAndDrop componentInChildren2 = itemSource2.gameObject.GetComponentInChildren<DragAndDrop>();
                if (componentInChildren2 != null)
                {
                    componentInChildren2.item = art.item;
                    componentInChildren2.owner = unit.Get();
                    componentInChildren2.dragScale = Vector3.one * 1.3f;
                    componentInChildren2.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
                    {
                        AudioLibrary.RequestSFX("SmashArtefact");
                        PopupGeneral.OpenPopup(null, "UI_SMASH_ARTEFACT", global::DBUtils.Localization.Get("UI_SMASH_ARTEFACT_DES", true, art.item.name, art.item.GetValue() / 2), "UI_OK", global::MOM.Artefact.SmashArtefact, "UI_CANCEL", null, null, null, art.item);
                    };
                }
            });
            component.gridItemSlots.UpdateGrid(unit.Get().artefactManager.equipmentSlots);
            itemSource.GetOrAddComponent<RollOverOutEvents>().data = unit.Get();
        });
        this.gridFortressVault.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            global::MOM.Artefact a = source as global::MOM.Artefact;
            GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "ItemIcon").texture = AssetManager.Get<Texture2D>(a.graphic);
            DragAndDrop componentInChildren = itemSource.gameObject.GetComponentInChildren<DragAndDrop>();
            if (componentInChildren != null)
            {
                componentInChildren.item = a;
                componentInChildren.owner = null;
                componentInChildren.dragScale = Vector3.one * 1.3f;
            }
            itemSource.GetOrAddComponent<RolloverObject>().source = a;
            componentInChildren.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
            {
                AudioLibrary.RequestSFX("SmashArtefact");
                PopupGeneral.OpenPopup(null, "UI_SMASH_ARTEFACT", global::DBUtils.Localization.Get("UI_SMASH_ARTEFACT_DES", true, a.name, a.GetValue() / 2), "UI_OK", global::MOM.Artefact.SmashArtefact, "UI_CANCEL", null, null, null, a);
            };
        }, UpdateScreen);
        MHEventSystem.RegisterListener<DragAndDropItem>(ItemDrop, this);
        MHEventSystem.RegisterListener<RollOverOutEvents>(EnterExitEvents, this);
        MHEventSystem.RegisterListener<global::MOM.Artefact>(ArtefactSmashed, this);
        AudioLibrary.RequestSFX("OpenHeroEquip");
        this.UpdateScreen();
        return base.PreStart();
    }

    private void ArtefactSmashed(object sender, object e)
    {
        if (e.ToString() == "Smashed")
        {
            this.UpdateScreen();
        }
    }

    private void EnterExitEvents(object sender, object e)
    {
        object data = (sender as RollOverOutEvents).data;
        if (this.rolloverHero == data && e.ToString() == "OnPointerExit")
        {
            this.rolloverHero = null;
        }
        else if (e.ToString() == "OnPointerEnter")
        {
            this.rolloverHero = data as BaseUnit;
        }
    }

    private void ItemDrop(object sender, object e)
    {
        global::MOM.Artefact art = (sender as DragAndDropItem).source.item as global::MOM.Artefact;
        PlayerWizard wizard = GameManager.GetHumanWizard();
        if (this.rolloverHero != null)
        {
            if (this.rolloverHero.artefactManager == null)
            {
                return;
            }
            EquipmentSlot slot = null;
            List<EquipmentSlot> equipmentSlots = this.rolloverHero.artefactManager.equipmentSlots;
            for (int num = equipmentSlots.Count - 1; num >= 0; num--)
            {
                if (Array.FindIndex(equipmentSlots[num].slotType.Get().eTypes, (EEquipmentType o) => o == art.equipmentType) >= 0 && (slot == null || equipmentSlots[num].item == null))
                {
                    slot = equipmentSlots[num];
                }
            }
            if (slot == null)
            {
                return;
            }
            if (!((e as DragAndDrop).owner is BaseUnit baseUnit))
            {
                if (!wizard.artefacts.Contains(art))
                {
                    Debug.LogError("(A) Transfer not valid for the item!");
                    return;
                }
                List<string> list = this.CollidedEnchantements(art, slot);
                if (list != null && list.Count > 0)
                {
                    string message2 = global::DBUtils.Localization.Get("UI_ENCH_DUPLICATE", true);
                    list.ForEach(delegate(string o)
                    {
                        message2 = message2 + "\n" + o;
                    });
                    PopupGeneral.OpenPopup(this, "UI_WARNING", message2, "UI_OK", delegate
                    {
                        wizard.artefacts.Remove(art);
                        if (slot.item != null)
                        {
                            wizard.artefacts.Add(slot.item);
                        }
                        slot.item = art;
                        this.UpdateScreen();
                        AudioLibrary.RequestSFX("DragAndDropRelease2");
                    }, "UI_CANCEL");
                }
                else
                {
                    wizard.artefacts.Remove(art);
                    if (slot.item != null)
                    {
                        wizard.artefacts.Add(slot.item);
                    }
                    slot.item = art;
                    this.UpdateScreen();
                    AudioLibrary.RequestSFX("DragAndDropRelease2");
                }
                return;
            }
            if (baseUnit == null || baseUnit.artefactManager == null)
            {
                Debug.LogError("(B) Transfer not valid for the item!");
                return;
            }
            EquipmentSlot prevSlot = baseUnit.artefactManager.equipmentSlots.Find((EquipmentSlot o) => o.item == art);
            if (prevSlot == null)
            {
                Debug.LogError("(B) Transfer from wrong slot!");
                return;
            }
            List<string> list2 = this.CollidedEnchantements(art, slot);
            if (list2 != null && list2.Count > 0)
            {
                string message = global::DBUtils.Localization.Get("UI_ENCH_DUPLICATE", true);
                list2.ForEach(delegate(string o)
                {
                    message = message + "\n" + o;
                });
                PopupGeneral.OpenPopup(this, "UI_WARNING", message, "UI_OK", delegate
                {
                    prevSlot.item = null;
                    if (slot.item != null)
                    {
                        wizard.artefacts.Add(slot.item);
                    }
                    slot.item = art;
                    this.UpdateScreen();
                    AudioLibrary.RequestSFX("DragAndDropRelease2");
                }, "UI_CANCEL");
            }
            else
            {
                prevSlot.item = null;
                if (slot.item != null)
                {
                    wizard.artefacts.Add(slot.item);
                }
                slot.item = art;
                this.UpdateScreen();
                AudioLibrary.RequestSFX("DragAndDropRelease2");
            }
        }
        else if ((e as DragAndDrop).owner is BaseUnit baseUnit2)
        {
            EquipmentSlot equipmentSlot = baseUnit2.artefactManager.equipmentSlots.Find((EquipmentSlot o) => o.item == art);
            if (equipmentSlot != null)
            {
                equipmentSlot.item = null;
                wizard.artefacts.Add(art);
                this.UpdateScreen();
                AudioLibrary.RequestSFX("DragAndDropRelease2");
            }
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            MHEventSystem.TriggerEvent<HeroEquip>(this, null);
            UIManager.Close(this);
        }
    }

    public void UpdateScreen()
    {
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        this.gridHeroes.UpdateGrid(humanWizard.heroes);
        this.gridFortressVault.UpdateGrid(humanWizard.artefacts);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    private List<string> CollidedEnchantements(global::MOM.Artefact art, EquipmentSlot slot)
    {
        List<EnchantmentInstance> enchantments = slot.owner.Get().GetEnchantments();
        List<string> list = null;
        foreach (DBReference<ArtefactPower> artefactPower in art.artefactPowers)
        {
            Enchantment[] relatedEnchantment = artefactPower.Get().skill.relatedEnchantment;
            if (relatedEnchantment == null || relatedEnchantment.Length == 0)
            {
                continue;
            }
            Enchantment[] array = relatedEnchantment;
            foreach (Enchantment e in array)
            {
                if (enchantments.Exists((EnchantmentInstance o) => o.source == e))
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    if (!list.Contains(e.GetDILocalizedName()))
                    {
                        list.Add(e.GetDILocalizedName());
                    }
                }
            }
        }
        return list;
    }
}
