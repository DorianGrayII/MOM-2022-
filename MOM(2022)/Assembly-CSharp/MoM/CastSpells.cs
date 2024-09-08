using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class CastSpells : SpellBookScreen
    {
        public Button btCast;

        public Button btStopCasting;

        public TextMeshProUGUI spellCastingTime;

        public TextMeshProUGUI labelCastSpell;

        public TextMeshProUGUI labelCastingSkill;

        public TextMeshProUGUI labelMana;

        public Spell selectedSpell;

        public GameObject[] emptySpellsGrid;

        public GameObject goSpellCastingTime;

        public GameObject goCastingSkill;

        public GameObject goCastingAffordability;

        public static object battleCaster;

        public Toggle tgFilterAllT;

        public Toggle tgFilterWorldT;

        public Toggle tgFilterBattleT;

        public ToggleGroup tgGroupD;

        public Toggle tgFilterAllD;

        public Toggle tgFilterChaosD;

        public Toggle tgFilterLifeD;

        public Toggle tgFilterDeathD;

        public Toggle tgFilterSorceryD;

        public Toggle tgFilterNatureD;

        public Toggle tgFilterArcaneD;

        public Toggle tgFilterTechD;

        private Dictionary<Toggle, ERealm> tgFilterToDomain = new Dictionary<Toggle, ERealm>();

        public Toggle tgFilterAllU;

        public Toggle tgFilterSummonsU;

        public Toggle tgFilterTargetingUnitsU;

        public Toggle tgFilterOtherU;

        public Toggle tgFilterAllA;

        public Toggle tgFilterAffordable;

        private Toggle pretendToggleActive;

        private ISpellCaster spellCasterOverride;

        public GameObject goFavourites;

        public GameObject goFlagPurple;

        public GameObject goFlagRed;

        public GameObject goFlagBlue;

        public GameObject goFlagGreen;

        public GameObject goFlagYellow;

        public GameObject p_goSpellFav;

        public GameObject highlightWorld;

        public GameObject highlightBattle;

        public GridItemManager gridBattleFavourites;

        public GridItemManager gridWorldFavourites;

        private int dropGroupFocussed;

        public override IEnumerator PreStart()
        {
            this.TgFilterToDomain();
            yield return base.PreStart();
            MHEventSystem.RegisterListener<DragAndDropItem>(ItemDrop, this);
            MHEventSystem.RegisterListener<DragAndDrop>(DragStart, this);
            MHEventSystem.RegisterListener<RollOverOut>(FocusOnOff, this);
            this.goFavourites.SetActive(this.GetSpellCaster() is PlayerWizard);
            this.gridBattleFavourites.CustomDynamicItem(FavoriteItem);
            this.gridWorldFavourites.CustomDynamicItem(FavoriteItem);
            this.highlightWorld.SetActive(value: false);
            this.highlightBattle.SetActive(value: false);
            if (this.GetSpellCaster() is PlayerWizard)
            {
                this.gridBattleFavourites.dragAndDropGroup = 1;
                this.gridWorldFavourites.dragAndDropGroup = 2;
                if (base.spellsGrid.itemBase != null)
                {
                    foreach (Transform item in base.spellsGrid.itemBase.transform.parent)
                    {
                        item.gameObject.GetOrAddComponent<DragAndDrop>().dragScale = Vector3.one * 1.5f;
                    }
                }
                this.UpdateFavorites();
                PlayerWizard playerWizard = this.GetSpellCaster() as PlayerWizard;
                this.goFlagPurple.SetActive(playerWizard.color == PlayerWizard.Color.Purple);
                this.goFlagRed.SetActive(playerWizard.color == PlayerWizard.Color.Red);
                this.goFlagBlue.SetActive(playerWizard.color == PlayerWizard.Color.Blue);
                this.goFlagGreen.SetActive(playerWizard.color == PlayerWizard.Color.Green);
                this.goFlagYellow.SetActive(playerWizard.color == PlayerWizard.Color.Yellow);
            }
            if (this.WorldMode())
            {
                this.tgFilterWorldT.isOn = true;
                MagicAndResearch magicAndResearch = (this.GetSpellCaster() as PlayerWizard)?.GetMagicAndResearch();
                if (magicAndResearch?.curentlyCastSpell != null)
                {
                    this.selectedSpell = magicAndResearch.curentlyCastSpell;
                }
                FSMMapGame.Get()?.CancelSpellTargetSelection(this);
                FSMSelectionManager.Get().Select(null, focus: false);
            }
            else
            {
                this.tgFilterBattleT.isOn = true;
            }
            base.UpdateGridPage();
            this.UpdateTypeFilters();
            if (this.GetSpellCaster() is PlayerWizard)
            {
                ISpellCaster spellCaster = this.GetSpellCaster();
                if (this.WorldMode())
                {
                    if (spellCaster is PlayerWizard playerWizard2)
                    {
                        this.goCastingSkill.SetActive(value: true);
                        this.labelCastingSkill.text = playerWizard2.turnSkillLeft + "/" + spellCaster.GetTotalCastingSkill();
                        this.labelMana.text = playerWizard2.GetMana() + "(" + playerWizard2.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
                    }
                }
                else
                {
                    this.goCastingSkill.SetActive(value: false);
                }
                this.btCast.gameObject.SetActive(value: true);
                this.btCast.onClick.AddListener(delegate
                {
                    Spell spell2 = this.GetSelectedSpell();
                    MagicAndResearch magicAndResearch3 = (this.GetSpellCaster() as PlayerWizard)?.GetMagicAndResearch();
                    if (this.WorldMode())
                    {
                        if (magicAndResearch3.castingProgress > 0)
                        {
                            PopupGeneral.OpenPopup(this, "UI_CHANGE_CASTING_TITLE", global::DBUtils.Localization.Get("UI_CHANGE_CASTING_DES", true, magicAndResearch3.curentlyCastSpell.GetDILocalizedName(), magicAndResearch3.castingProgress), "UI_CHANGE_CASTING", StartWorldCasting, "UI_CANCEL");
                        }
                        else
                        {
                            this.StartWorldCasting();
                        }
                    }
                    else
                    {
                        Battle battle2 = Battle.GetBattle();
                        if (battle2 != null && battle2.activeTurn != null)
                        {
                            if (spell2.changeableCost != null && spell2.changeableCost.maxMultipier > 1 && magicAndResearch3 != null)
                            {
                                PopupSpellCost.OpenPopup(HUD.Get(), magicAndResearch3, battle2, spell2, this.GetSpellCaster(), base.btClose);
                            }
                            else
                            {
                                battle2.activeTurn.StartCasting(spell2, this.GetSpellCaster());
                                base.btClose.onClick.Invoke();
                            }
                        }
                    }
                });
                yield break;
            }
            this.tgFilterWorldT.gameObject.SetActive(value: false);
            this.goCastingSkill.SetActive(value: false);
            if (Battle.Get() != null && !this.WorldMode())
            {
                this.btCast.gameObject.SetActive(value: true);
                this.btCast.onClick.AddListener(delegate
                {
                    Spell spell = this.GetSelectedSpell();
                    MagicAndResearch magicAndResearch2 = (this.GetSpellCaster() as BattleUnit)?.GetWizardOwner().GetMagicAndResearch();
                    Battle battle = Battle.GetBattle();
                    if (battle != null && battle.activeTurn != null)
                    {
                        if (spell.changeableCost != null && spell.changeableCost.maxMultipier > 1 && magicAndResearch2 != null)
                        {
                            PopupSpellCost.OpenPopup(HUD.Get(), magicAndResearch2, battle, spell, this.GetSpellCaster(), base.btClose);
                        }
                        else
                        {
                            battle.activeTurn.StartCasting(spell, this.GetSpellCaster());
                            base.btClose.onClick.Invoke();
                        }
                    }
                });
            }
            else
            {
                this.tgFilterAllT.isOn = true;
                base.UpdateGridPage();
                this.UpdateTypeFilters();
            }
        }

        private void FocusOnOff(object sender, object e)
        {
            RollOverOut rollOverOut = sender as RollOverOut;
            if (e as string == "Enter")
            {
                GridItemManager[] componentsInChildren = rollOverOut.gameObject.GetComponentsInChildren<GridItemManager>();
                foreach (GridItemManager gridItemManager in componentsInChildren)
                {
                    if (gridItemManager.dragAndDropGroup > 0)
                    {
                        this.dropGroupFocussed = gridItemManager.dragAndDropGroup;
                        return;
                    }
                }
            }
            this.dropGroupFocussed = 0;
        }

        public override IEnumerator PreClose()
        {
            if (this.GetSpellCaster() is PlayerWizard playerWizard)
            {
                playerWizard.AllowToStoreManaIncomeCache(b: false);
            }
            CastSpells.battleCaster = null;
            if (this.btCast != null)
            {
                this.btCast.interactable = false;
            }
            if (this.btStopCasting != null)
            {
                this.btStopCasting.interactable = false;
            }
            yield return base.PreClose();
        }

        public void SetSpellCaster(ISpellCaster sc)
        {
            this.spellCasterOverride = sc;
        }

        protected List<DBReference<Spell>> GetUnfilteredSpells()
        {
            ISpellCaster spellCaster = this.GetSpellCaster();
            if (spellCaster == null)
            {
                Debug.LogError("Spellcaster is null");
                return new List<DBReference<Spell>>();
            }
            List<DBReference<Spell>> spells = spellCaster.GetSpells();
            List<DBReference<Spell>> list = new List<DBReference<Spell>>(spells);
            if (spellCaster is PlayerWizard)
            {
                PlayerWizard obj = ((spellCaster is PlayerWizard) ? (spellCaster as PlayerWizard) : (spellCaster as BattlePlayer).GetWizardOwner());
                if (obj.banishedTurn > 0)
                {
                    list.Clear();
                    list.Add((Spell)SPELL.SPELL_OF_RETURN);
                }
                obj.GetMagicAndResearch()?.extensionItemSpellBattle?.Clear();
            }
            if (spellCaster is BattleUnit || spellCaster is Unit)
            {
                bool flag = false;
                if (spellCaster is BattleUnit battleUnit && battleUnit.dbSource.Get() is Hero)
                {
                    flag = true;
                }
                if (spellCaster is Unit unit && unit.dbSource.Get() is Hero)
                {
                    flag = true;
                }
                if (flag && (spellCaster as IAttributable).GetAttFinal((Tag)TAG.CASTER) > 0 && spellCaster.GetWizardOwner() != null)
                {
                    foreach (DBReference<Spell> s in spellCaster.GetWizardOwner().GetSpells())
                    {
                        if (!string.IsNullOrEmpty(s.Get().battleScript) && spells.Find((DBReference<Spell> o) => o.Get() == s.Get()) == null)
                        {
                            list.Add(s);
                        }
                    }
                }
                spellCaster.GetWizardOwner()?.GetMagicAndResearch()?.extensionItemSpellBattle?.Clear();
            }
            return list;
        }

        protected override List<DBReference<Spell>> GetSpells()
        {
            List<DBReference<Spell>> list = this.GetUnfilteredSpells();
            Toggle activeToggle = this.tgGroupD.GetFirstActiveToggle();
            if (!this.tgFilterAllD.isOn)
            {
                list = list.FindAll((DBReference<Spell> o) => o.Get().realm == this.tgFilterToDomain[activeToggle]);
            }
            if (!this.tgFilterAllU.isOn)
            {
                if (this.tgFilterSummonsU.isOn)
                {
                    list = list.FindAll((DBReference<Spell> o) => o.Get().summonFantasticUnitSpell);
                }
                else if (this.tgFilterTargetingUnitsU.isOn)
                {
                    list = list.FindAll((DBReference<Spell> o) => o.Get().targetType.enumType == ETargetType.TargetUnit);
                }
                else if (this.tgFilterOtherU.isOn)
                {
                    list = list.FindAll((DBReference<Spell> o) => !o.Get().summonFantasticUnitSpell && o.Get().targetType.enumType != ETargetType.TargetUnit);
                }
            }
            if (!this.tgFilterAllT.isOn)
            {
                list = list.FindAll((DBReference<Spell> o) => (!string.IsNullOrEmpty(o.Get().worldScript) && this.tgFilterWorldT.isOn) || (!string.IsNullOrEmpty(o.Get().battleScript) && this.tgFilterBattleT.isOn));
            }
            if (this.WorldMode())
            {
                this.goCastingAffordability?.SetActive(value: false);
            }
            else if (!this.tgFilterAllA.isOn)
            {
                BattlePlayer battlePlayer = CastSpells.battleCaster as BattlePlayer;
                ISpellCaster caster = this.GetSpellCaster();
                if (battlePlayer != null)
                {
                    int mana2 = battlePlayer.mana;
                    int skill = battlePlayer.castingSkill;
                    list = list.FindAll((DBReference<Spell> o) => o.Get().GetBattleCastingCostByDistance(caster) <= mana2 && o.Get().GetBattleCastingCost(caster) <= skill);
                }
                else
                {
                    int mana = caster.GetMana();
                    list = list.FindAll((DBReference<Spell> o) => o.Get().GetBattleCastingCostByDistance(caster) <= mana);
                }
            }
            return list;
        }

        protected override Spell GetSelectedSpell()
        {
            return this.selectedSpell;
        }

        protected override void SelectSpell(Spell s)
        {
            this.selectedSpell = s;
        }

        protected override Spell GetOriginalSelection()
        {
            PlayerWizard playerWizard = this.GetSpellCaster() as PlayerWizard;
            if (this.WorldMode() && playerWizard != null)
            {
                MagicAndResearch magicAndResearch = playerWizard.GetMagicAndResearch();
                if (magicAndResearch.curentlyCastSpell != null)
                {
                    return magicAndResearch.curentlyCastSpell;
                }
            }
            else
            {
                List<DBReference<Spell>> spells = this.GetSpells();
                if (spells != null && spells.Count > 0)
                {
                    return spells[0];
                }
            }
            return null;
        }

        public override ISpellCaster GetSpellCaster()
        {
            if (this.spellCasterOverride != null)
            {
                return this.spellCasterOverride;
            }
            if (this.WorldMode())
            {
                PlayerWizard humanWizard = GameManager.GetHumanWizard();
                if (base.stateStatus <= StateStatus.Active)
                {
                    humanWizard.AllowToStoreManaIncomeCache(b: true);
                }
                return humanWizard;
            }
            if (CastSpells.battleCaster is BattlePlayer)
            {
                return (CastSpells.battleCaster as BattlePlayer).wizard;
            }
            if (CastSpells.battleCaster is BattleUnit)
            {
                return CastSpells.battleCaster as BattleUnit;
            }
            Debug.LogError("Invalid caster " + CastSpells.battleCaster);
            return null;
        }

        protected override void UpdateMainButtonState()
        {
            ISpellCaster spellCaster = this.GetSpellCaster();
            if (this.WorldMode())
            {
                if (spellCaster is Unit || spellCaster is BattleUnit)
                {
                    this.btCast.gameObject.SetActive(value: false);
                    this.btStopCasting.gameObject.SetActive(value: false);
                    return;
                }
                bool flag = this.GetOriginalSelection() != this.GetSelectedSpell();
                bool flag2 = this.GetSelectedSpell() != null && !string.IsNullOrEmpty(this.GetSelectedSpell().worldScript);
                this.btCast.gameObject.SetActive(flag2 && flag);
                this.btStopCasting.gameObject.SetActive(flag2 && !flag);
                this.btCast.interactable = flag;
                return;
            }
            bool flag3 = this.GetSelectedSpell() != null && !string.IsNullOrEmpty(this.GetSelectedSpell().battleScript);
            bool flag4 = false;
            if (flag3)
            {
                if (spellCaster is BattleUnit)
                {
                    int battleCastingCost = this.GetSelectedSpell().GetBattleCastingCost(spellCaster);
                    flag4 = spellCaster.GetMana() >= battleCastingCost;
                    if (flag4)
                    {
                        this.labelCastSpell.text = global::DBUtils.Localization.Get("UI_CAST", true);
                    }
                    else
                    {
                        this.labelCastSpell.text = global::DBUtils.Localization.Get("UI_INSUFFICIENT_MANA", true);
                    }
                }
                if (CastSpells.battleCaster is BattlePlayer)
                {
                    BattlePlayer obj = CastSpells.battleCaster as BattlePlayer;
                    int battleCastingCostByDistance = this.GetSelectedSpell().GetBattleCastingCostByDistance(spellCaster);
                    flag4 = obj.mana >= battleCastingCostByDistance;
                    if (flag4)
                    {
                        int battleCastingCost2 = this.GetSelectedSpell().GetBattleCastingCost(spellCaster);
                        flag4 = (CastSpells.battleCaster as BattlePlayer).castingSkill >= battleCastingCost2;
                        if (flag4)
                        {
                            this.labelCastSpell.text = global::DBUtils.Localization.Get("UI_CAST", true);
                        }
                        else
                        {
                            this.labelCastSpell.text = global::DBUtils.Localization.Get("UI_INSUFFICIENT_CASTING_SKILL", true);
                        }
                    }
                    else
                    {
                        this.labelCastSpell.text = global::DBUtils.Localization.Get("UI_INSUFFICIENT_MANA", true);
                    }
                }
            }
            this.btCast.gameObject.SetActive(flag3);
            this.btCast.interactable = flag4;
        }

        protected override void UpdateRightPage()
        {
            base.UpdateRightPage();
            Spell spell = this.GetSelectedSpell();
            if (spell == null)
            {
                return;
            }
            PlayerWizard playerWizard = this.GetSpellCaster() as PlayerWizard;
            if (this.WorldMode() && playerWizard != null)
            {
                this.spellCastingTime.text = spell.GetCastingTurns(playerWizard);
            }
            else
            {
                this.goSpellCastingTime.gameObject.SetActive(value: false);
            }
            Toggle component = base.spellTypeWorld.GetComponent<Toggle>();
            Toggle component2 = base.spellTypeCombat.GetComponent<Toggle>();
            if (this.GetSpellCaster() is PlayerWizard)
            {
                SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
                component.onValueChanged.RemoveAllListeners();
                component2.onValueChanged.RemoveAllListeners();
                component.isOn = spellManager.GetWorldFav().Contains(spell);
                component2.isOn = spellManager.GetBattleFav().Contains(spell);
                component.onValueChanged.AddListener(delegate
                {
                    this.SwitchFavWorldSpell(spell);
                    this.UpdateFavorites();
                });
                component2.onValueChanged.AddListener(delegate
                {
                    this.SwitchFavBattleSpell(spell);
                    this.UpdateFavorites();
                });
            }
            else
            {
                component.interactable = false;
                component2.interactable = false;
            }
        }

        protected override bool WorldMode()
        {
            return CastSpells.battleCaster == null;
        }

        protected override void SlotXFilled(int slotIndex, bool state)
        {
            this.emptySpellsGrid[slotIndex].SetActive(!state);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.tgFilterAllT || s == this.tgFilterWorldT || s == this.tgFilterBattleT || s == this.tgFilterAllD || s == this.tgFilterChaosD || s == this.tgFilterLifeD || s == this.tgFilterDeathD || s == this.tgFilterSorceryD || s == this.tgFilterNatureD || s == this.tgFilterArcaneD || s == this.tgFilterAllU || s == this.tgFilterSummonsU || s == this.tgFilterTargetingUnitsU || s == this.tgFilterOtherU || s == this.tgFilterAllA || s == this.tgFilterAffordable)
            {
                base.UpdateGridPage();
                this.UpdateTypeFilters();
            }
            if (s.name == "ButtonClose")
            {
                UIManager.Close(this);
            }
            if (s == this.btStopCasting)
            {
                MagicAndResearch magicAndResearch = (this.GetSpellCaster() as PlayerWizard)?.GetMagicAndResearch();
                if (magicAndResearch.castingProgress > 0)
                {
                    PopupGeneral.OpenPopup(this, "UI_STOP_CASTING_TITLE", global::DBUtils.Localization.Get("UI_STOP_CASTING_DESC", true, magicAndResearch.curentlyCastSpell.GetDILocalizedName(), magicAndResearch.castingProgress), "UI_STOP_CASTING", CancelCasting, "UI_CANCEL");
                }
                else
                {
                    this.CancelCasting();
                }
            }
        }

        private void CancelCasting(object o = null)
        {
            ((this.GetSpellCaster() as PlayerWizard)?.GetMagicAndResearch()).ResetCasting();
            base.btClose.onClick.Invoke();
            HUD.Get().UpdateCastingButton();
        }

        private void TgFilterToDomain()
        {
            this.tgFilterToDomain.Add(this.tgFilterAllD, ERealm.None);
            this.tgFilterToDomain.Add(this.tgFilterChaosD, ERealm.Chaos);
            this.tgFilterToDomain.Add(this.tgFilterLifeD, ERealm.Life);
            this.tgFilterToDomain.Add(this.tgFilterDeathD, ERealm.Death);
            this.tgFilterToDomain.Add(this.tgFilterSorceryD, ERealm.Sorcery);
            this.tgFilterToDomain.Add(this.tgFilterNatureD, ERealm.Nature);
            this.tgFilterToDomain.Add(this.tgFilterArcaneD, ERealm.Arcane);
            this.tgFilterToDomain.Add(this.tgFilterTechD, ERealm.Tech);
        }

        private void UpdateTypeFilters()
        {
            List<DBReference<Spell>> unfilteredSpells = this.GetUnfilteredSpells();
            List<DBReference<Spell>> list = unfilteredSpells;
            List<DBReference<Spell>> spellsT = unfilteredSpells;
            List<DBReference<Spell>> spellsU = unfilteredSpells;
            Toggle activeToggle = this.tgGroupD.GetFirstActiveToggle();
            if (!this.tgFilterAllD.isOn)
            {
                list = unfilteredSpells.FindAll((DBReference<Spell> o) => o.Get().realm == this.tgFilterToDomain[activeToggle]);
            }
            if (!this.tgFilterAllU.isOn)
            {
                if (this.tgFilterSummonsU.isOn)
                {
                    spellsU = unfilteredSpells.FindAll((DBReference<Spell> o) => o.Get().summonFantasticUnitSpell);
                }
                else if (this.tgFilterTargetingUnitsU.isOn)
                {
                    spellsU = unfilteredSpells.FindAll((DBReference<Spell> o) => o.Get().targetType.enumType == ETargetType.TargetUnit);
                }
                else if (this.tgFilterOtherU.isOn)
                {
                    spellsU = unfilteredSpells.FindAll((DBReference<Spell> o) => !o.Get().summonFantasticUnitSpell && o.Get().targetType.enumType != ETargetType.TargetUnit);
                }
            }
            if (!this.tgFilterAllT.isOn)
            {
                spellsT = unfilteredSpells.FindAll((DBReference<Spell> o) => (!string.IsNullOrEmpty(o.Get().worldScript) && this.tgFilterWorldT.isOn) || (!string.IsNullOrEmpty(o.Get().battleScript) && this.tgFilterBattleT.isOn));
            }
            List<DBReference<Spell>> list2 = list.FindAll((DBReference<Spell> o) => spellsU.Contains(o));
            List<DBReference<Spell>> list3 = list.FindAll((DBReference<Spell> o) => spellsT.Contains(o));
            List<DBReference<Spell>> list4 = spellsU.FindAll((DBReference<Spell> o) => spellsT.Contains(o));
            if (this.GetSpellCaster() == GameManager.GetHumanWizard())
            {
                this.tgFilterWorldT.gameObject.SetActive(list2.Find((DBReference<Spell> o) => !string.IsNullOrEmpty(o.Get().worldScript)) != null);
            }
            this.tgFilterBattleT.gameObject.SetActive(list2.Find((DBReference<Spell> o) => !string.IsNullOrEmpty(o.Get().battleScript)) != null);
            this.tgFilterSummonsU.gameObject.SetActive(list3.Find((DBReference<Spell> o) => o.Get().summonFantasticUnitSpell) != null);
            this.tgFilterTargetingUnitsU.gameObject.SetActive(list3.Find((DBReference<Spell> o) => o.Get().targetType.enumType == ETargetType.TargetUnit) != null);
            this.tgFilterOtherU.gameObject.SetActive(list3.Find((DBReference<Spell> o) => !o.Get().summonFantasticUnitSpell && o.Get().targetType.enumType != ETargetType.TargetUnit) != null);
            foreach (KeyValuePair<Toggle, ERealm> tg in this.tgFilterToDomain)
            {
                if (tg.Value != 0)
                {
                    tg.Key.gameObject.SetActive(list4.Find((DBReference<Spell> o) => o.Get().realm == tg.Value) != null);
                }
            }
        }

        private void StartCastingSpell(MagicAndResearch sm, Spell curSpell)
        {
            sm.curentlyCastSpell = curSpell;
            sm.craftItemSpell = null;
            sm.CleanExtrensionItemSpellWorld();
            base.btClose.onClick.Invoke();
            HUD.Get()?.UpdateCastingButton();
        }

        private void StartWorldCasting(object o = null)
        {
            Spell spell = this.GetSelectedSpell();
            MagicAndResearch magicAndResearch = (this.GetSpellCaster() as PlayerWizard)?.GetMagicAndResearch();
            if (spell == (Spell)SPELL.ENCHANT_ITEM || spell == (Spell)SPELL.CREATE_ARTEFACT)
            {
                PopupCreateArtefact.Popup(spell, this);
            }
            else if (spell.changeableCost != null && spell.changeableCost.maxMultipier > 1 && magicAndResearch != null)
            {
                PopupSpellCost.OpenPopup(HUD.Get(), magicAndResearch, null, spell, this.GetSpellCaster(), base.btClose);
            }
            else
            {
                this.StartCastingSpell(magicAndResearch, spell);
            }
            if (spell == (Spell)SPELL.SPELL_OF_MASTERY)
            {
                PopupCastingSoM.OpenPopup(null, GameManager.GetHumanWizard());
            }
        }

        private void DragStart(object sender, object e)
        {
            DragAndDrop dragAndDrop = e as DragAndDrop;
            Debug.Log(e?.ToString() + " Spell DragStart from " + sender);
            if (!(dragAndDrop == null) && dragAndDrop.item is Spell spell)
            {
                this.highlightWorld.SetActive(!string.IsNullOrEmpty(spell.worldScript));
                this.highlightBattle.SetActive(!string.IsNullOrEmpty(spell.battleScript));
            }
        }

        private void ItemDrop(object sender, object e)
        {
            DragAndDrop dragAndDrop = e as DragAndDrop;
            this.highlightWorld.SetActive(value: false);
            this.highlightBattle.SetActive(value: false);
            if (dragAndDrop == null || !(dragAndDrop.item is Spell spell))
            {
                return;
            }
            if (this.dropGroupFocussed == 1)
            {
                if (!string.IsNullOrEmpty(spell.battleScript))
                {
                    this.SwitchFavBattleSpell(spell);
                    this.UpdateFavorites();
                    this.UpdateRightPage();
                    AudioLibrary.RequestSFX("DragAndDropRelease");
                }
            }
            else if (this.dropGroupFocussed == 2 && !string.IsNullOrEmpty(spell.worldScript))
            {
                this.SwitchFavWorldSpell(spell);
                this.UpdateFavorites();
                this.UpdateRightPage();
                AudioLibrary.RequestSFX("DragAndDropRelease");
            }
        }

        protected override void SpellItem(GameObject itemSource, object source, object data, int index)
        {
            base.SpellItem(itemSource, source, data, index);
            SpellListItem component = itemSource.GetComponent<SpellListItem>();
            DragAndDrop component2 = itemSource.GetComponent<DragAndDrop>();
            Spell item = (source as DBReference<Spell>).Get();
            if (component2 != null)
            {
                component2.item = item;
                component2.dragIcon = component.riSpellIcon.transform.parent.gameObject;
            }
        }

        public void SwitchFavWorldSpell(Spell s)
        {
            SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
            int num = spellManager.GetWorldFav().FindIndex((DBReference<Spell> o) => o.Get() == s);
            if (num >= 0)
            {
                spellManager.GetWorldFav().RemoveAt(num);
            }
            else
            {
                spellManager.GetWorldFav().Add(s);
            }
        }

        private bool CanSwitchBattleSpell(Spell s)
        {
            if (string.IsNullOrEmpty(s.battleScript))
            {
                return false;
            }
            SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
            if (spellManager.GetBattleFav().FindIndex((DBReference<Spell> o) => o.Get() == s) >= 0 || spellManager.GetBattleFav().Count < 9)
            {
                return true;
            }
            return false;
        }

        private bool CanSwitchWorldSpell(Spell s)
        {
            if (string.IsNullOrEmpty(s.worldScript))
            {
                return false;
            }
            SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
            if (spellManager.GetWorldFav().FindIndex((DBReference<Spell> o) => o.Get() == s) >= 0 || spellManager.GetWorldFav().Count < 9)
            {
                return true;
            }
            return false;
        }

        public void SwitchFavBattleSpell(Spell s)
        {
            SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
            int num = spellManager.GetBattleFav().FindIndex((DBReference<Spell> o) => o.Get() == s);
            if (num >= 0)
            {
                spellManager.GetBattleFav().RemoveAt(num);
            }
            else
            {
                spellManager.GetBattleFav().Add(s);
            }
        }

        public void UpdateFavorites()
        {
            SpellManager spellManager = GameManager.GetHumanWizard().GetSpellManager();
            this.gridBattleFavourites.UpdateGrid(spellManager.GetBattleFav());
            this.gridWorldFavourites.UpdateGrid(spellManager.GetWorldFav());
        }

        private void FavoriteItem(GameObject itemSource, object source, object data, int index)
        {
            Spell spell = (source as DBReference<Spell>).Get();
            SpellListItem component = itemSource.GetComponent<SpellListItem>();
            GridItemManager grid = itemSource.GetComponentInParent<GridItemManager>();
            component.riSpellIcon.texture = spell.GetDescriptionInfo().GetTexture();
            component.spellDomainChaos.SetActive(spell.realm == ERealm.Chaos);
            component.spellDomainDeath.SetActive(spell.realm == ERealm.Death);
            component.spellDomainLife.SetActive(spell.realm == ERealm.Life);
            component.spellDomainNature.SetActive(spell.realm == ERealm.Nature);
            component.spellDomainSorcery.SetActive(spell.realm == ERealm.Sorcery);
            component.spellDomainArcane.SetActive(spell.realm == ERealm.Arcane);
            component.spellDomainTech.SetActive(spell.realm == ERealm.Tech);
            component.spellRarityCommon.SetActive(spell.rarity == ERarity.Common);
            component.spellRarityUncommon.SetActive(spell.rarity == ERarity.Uncommon);
            component.spellRarityRare.SetActive(spell.rarity == ERarity.Rare);
            component.spellRarityVRare.SetActive(spell.rarity == ERarity.VeryRare);
            component.btRemoveFav.onClick.RemoveAllListeners();
            component.btRemoveFav.onClick.AddListener(delegate
            {
                if (grid.dragAndDropGroup == 1)
                {
                    this.SwitchFavBattleSpell(spell);
                    this.UpdateFavorites();
                    this.UpdateRightPage();
                }
                else if (grid.dragAndDropGroup == 2)
                {
                    this.SwitchFavWorldSpell(spell);
                    this.UpdateFavorites();
                    this.UpdateRightPage();
                }
            });
            if (!this.WorldMode() && grid.dragAndDropGroup == 1)
            {
                BattlePlayer battlePlayer = this.GetSpellCaster() as BattlePlayer;
                if (this.GetSpellCaster() is PlayerWizard)
                {
                    battlePlayer = Battle.Get().GetBattlePlayerForWizard(this.GetSpellCaster() as PlayerWizard);
                }
                int battleCastingCostByDistance = spell.GetBattleCastingCostByDistance(this.GetSpellCaster());
                int battleCastingCost = spell.GetBattleCastingCost(this.GetSpellCaster());
                bool flag = battlePlayer.mana >= battleCastingCostByDistance && battlePlayer.castingSkill >= battleCastingCost;
                component.favCannotCast.SetActive(!flag);
            }
            else
            {
                component.favCannotCast.SetActive(value: false);
            }
            itemSource.GetComponent<MouseClickEvent>().mouseLeftClick = delegate
            {
                int num = this.GetSpells().FindIndex((DBReference<Spell> o) => o.Get() == spell);
                if (num < 0)
                {
                    this.tgFilterAllD.isOn = true;
                    this.tgFilterAllT.isOn = true;
                    this.tgFilterAllU.isOn = true;
                    this.tgFilterAllA.isOn = true;
                    num = this.GetSpells().FindIndex((DBReference<Spell> o) => o.Get() == spell);
                    if (num < 0)
                    {
                        Debug.LogWarning("Unable to find spell " + spell);
                        return;
                    }
                }
                int pageNr = num / base.spellsGrid.GetPageSize();
                base.spellsGrid.SetPageNr(pageNr);
                List<DBReference<Spell>> spells = this.GetSpells();
                List<int> list = new List<int>();
                for (int i = 0; i < spells.Count; i++)
                {
                    list.Add(i % base.spellsGrid.GetPageSize());
                }
                for (int j = 0; j < base.spellsGrid.GetPageSize(); j++)
                {
                    this.SlotXFilled(j, state: false);
                }
                base.spellsGrid.UpdateGrid(spells, list);
                base.spellsGrid.SelectItem(num % base.spellsGrid.GetPageSize());
            };
        }
    }
}
