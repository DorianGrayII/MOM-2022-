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
    public abstract class SpellBookScreen : ScreenBase
    {
        private static Color32 RED = new Color32(231, 14, 0, byte.MaxValue);

        private static Color32 BROWN = new Color32(44, 34, 26, byte.MaxValue);

        public GameObject book;

        public GameObject spellDetails;

        public GameObject spellDomainNature;

        public GameObject spellDomainChaos;

        public GameObject spellDomainLife;

        public GameObject spellDomainDeath;

        public GameObject spellDomainSorcery;

        public GameObject spellDomainArcane;

        public GameObject spellDomainTech;

        public GameObject spellRarityCommon;

        public GameObject spellRarityUncommon;

        public GameObject spellRarityRare;

        public GameObject spellRarityVRare;

        public GameObject spellTypeWorld;

        public GameObject spellTypeCombat;

        public GameObject spellTypeSummon;

        public GameObject spellTypeEnchantment;

        public GameObject spellTypeInstant;

        public GameObject spellTypeSpecial;

        public GameObject spellCostBattle;

        public GameObject spellCostWorld;

        public GameObject spellUpkeep;

        public GameObject spellIconCommon;

        public GameObject spellIconUncommon;

        public GameObject spellIconRare;

        public GameObject spellIconVeryRare;

        public RawImage spellIcon;

        public GridItemManager spellsGrid;

        public GameObject spellsUnits;

        public GameObject spellsEnchantments;

        public Button btClose;

        public TextMeshProUGUI spellName;

        public TextMeshProUGUI spellDescription;

        public TextMeshProUGUI spellCastingCost;

        public TextMeshProUGUI spellCastingCostBattle;

        public TextMeshProUGUI spellCastingCostWorld;

        public TextMeshProUGUI spellTarget;

        public TextMeshProUGUI spellUpkeepCost;

        private EnchantmentGrid spellsEnchantmentsGrid;

        private GridItemManager spellsUnitsGrid;

        private List<Unit> tempUnits;

        private int researchPoints = -1;

        private Wizard wizard;

        public override IEnumerator PreStart()
        {
            this.spellsUnitsGrid = this.spellsUnits.GetComponentInChildren<GridItemManager>();
            this.spellsUnitsGrid.CustomDynamicItem(SubRaceItem);
            this.spellsEnchantmentsGrid = this.spellsEnchantments.GetComponentInChildren<EnchantmentGrid>();
            this.spellsGrid.CustomDynamicItem(SpellItem);
            this.spellsGrid.pageingNext.onClick.AddListener(NextPage);
            this.spellsGrid.pageingPrev.onClick.AddListener(PrevPage);
            MHEventSystem.RegisterListener<GenericAnimatorTrigger>(FlipPage, this);
            AudioLibrary.RequestSFX("OpenBook");
            yield return base.PreStart();
        }

        public void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (this.spellsGrid.GetPageNr() < this.spellsGrid.GetMaxPageNr() - 1)
                {
                    AudioLibrary.RequestSFX("PageTurn");
                    this.NextPage();
                    this.spellsGrid.NextPage();
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.spellsGrid.GetPageNr() > 0)
            {
                AudioLibrary.RequestSFX("PageTurn");
                this.PrevPage();
                this.spellsGrid.PrevPage();
            }
        }

        private void NextPage()
        {
            this.SetAnimatorBool("FlipPage", state: true);
            this.SetAnimatorBool("FlipPagePrev", state: false);
        }

        private void PrevPage()
        {
            this.SetAnimatorBool("FlipPagePrev", state: true);
            this.SetAnimatorBool("FlipPage", state: false);
        }

        private void SetAnimatorBool(string animationName, bool state)
        {
            Animator component = this.book.GetComponent<Animator>();
            if (component == null)
            {
                Debug.LogError("Missing animator on book in Spells book screen");
            }
            else
            {
                component.SetBool(animationName, state);
            }
        }

        private void FlipPage(object sender, object e)
        {
            if (e is string && e.ToString() == "FlipPage")
            {
                this.UpdateGridPage();
                this.SetAnimatorBool("FlipPage", state: false);
                this.SetAnimatorBool("FlipPagePrev", state: false);
            }
        }

        public abstract ISpellCaster GetSpellCaster();

        private void SubRaceItem(GameObject itemSource, object source, object data, int index)
        {
            ArmyListItem component = itemSource.GetComponent<ArmyListItem>();
            component.greyScaleWhenNoMP = false;
            Unit unit = Unit.CreateFrom(source as Subrace);
            GameManager.GetHumanWizard().ModifyUnitSkillsByTraits(unit);
            component.Unit = unit;
            if (this.tempUnits == null)
            {
                this.tempUnits = new List<Unit>();
            }
            this.tempUnits.Add(unit);
        }

        protected abstract List<DBReference<Spell>> GetSpells();

        protected abstract Spell GetSelectedSpell();

        protected abstract Spell GetOriginalSelection();

        protected abstract void UpdateMainButtonState();

        protected abstract void SelectSpell(Spell s);

        protected abstract bool WorldMode();

        protected virtual void SlotXFilled(int slotIndex, bool state)
        {
        }

        protected virtual void SpellItem(GameObject itemSource, object source, object data, int index)
        {
            Spell spell = (source as DBReference<Spell>).Get();
            SpellListItem component = itemSource.GetComponent<SpellListItem>();
            ISpellCaster spellCaster = this.GetSpellCaster();
            PlayerWizard playerWizard = spellCaster as PlayerWizard;
            if (this.WorldMode() && playerWizard != null)
            {
                if (playerWizard != null && this.researchPoints == -1)
                {
                    this.researchPoints = playerWizard.CalculateResearchIncome();
                }
                component.labelName.text = spell.GetDescriptionInfo().GetLocalizedName();
                if (component.labelCastingCost != null)
                {
                    int worldCastingCost = spell.GetWorldCastingCost(spellCaster);
                    if (worldCastingCost < 0)
                    {
                        component.labelCastingCost.text = global::DBUtils.Localization.Get("UI_UNKNOWN_COST", true);
                    }
                    else if (worldCastingCost == 0)
                    {
                        component.spellCastingCost.SetActive(value: false);
                        component.spellCastingTime.SetActive(value: false);
                    }
                    else
                    {
                        component.spellCastingCost.SetActive(value: true);
                        component.spellCastingTime.SetActive(value: true);
                        component.labelCastingCost.text = worldCastingCost.ToString();
                    }
                }
                if (component.labelCastingTime != null)
                {
                    component.labelCastingTime.text = spell.GetCastingTurns(playerWizard, includeExtraManaCost: false);
                }
                if (component.labelResearchCost != null)
                {
                    component.labelResearchCost.text = Mathf.Max(0, spell.GetResearchCost(spellCaster) - playerWizard.GetMagicAndResearch().researchProgress).ToString();
                }
                if (component.spellCastingSkill != null)
                {
                    component.spellCastingSkill.SetActive(value: false);
                }
                if (component.labelResearchTime != null)
                {
                    string text;
                    if (this.researchPoints > 0)
                    {
                        int b2 = Mathf.CeilToInt((float)(spell.GetResearchCost(spellCaster) - playerWizard.GetMagicAndResearch().researchProgress) / (float)this.researchPoints);
                        text = Mathf.Max(1, b2).ToString();
                    }
                    else
                    {
                        text = "∞";
                    }
                    component.labelResearchTime.text = text;
                }
            }
            else
            {
                component.labelName.text = spell.GetDescriptionInfo().GetLocalizedName();
                BattlePlayer battlePlayer = null;
                if (spellCaster is PlayerWizard)
                {
                    battlePlayer = Battle.Get().GetBattlePlayerForWizard(spellCaster as PlayerWizard);
                }
                component.spellCastingTime.SetActive(value: false);
                bool active = !string.IsNullOrEmpty(spell.battleScript);
                if (component.labelCastingCost != null)
                {
                    if (Battle.Get() != null)
                    {
                        int battleCastingCostByDistance = spell.GetBattleCastingCostByDistance(spellCaster);
                        component.spellCastingCost.SetActive(active);
                        component.labelCastingCost.text = battleCastingCostByDistance.ToString();
                        bool flag = true;
                        if ((battlePlayer == null) ? (battleCastingCostByDistance > spellCaster.GetMana()) : (battleCastingCostByDistance > battlePlayer.mana))
                        {
                            component.labelCastingCost.color = SpellBookScreen.RED;
                        }
                        else
                        {
                            component.labelCastingCost.color = SpellBookScreen.BROWN;
                        }
                    }
                    else
                    {
                        component.spellCastingCost.SetActive(value: false);
                    }
                }
                if (component.spellCastingSkill != null)
                {
                    if (Battle.Get() != null && battlePlayer != null)
                    {
                        int battleCastingCost = spell.GetBattleCastingCost(spellCaster);
                        component.spellCastingSkill.SetActive(active);
                        component.labelCastingSkill.text = battleCastingCost.ToString();
                        if (battlePlayer.castingSkill < battleCastingCost)
                        {
                            component.labelCastingSkill.color = SpellBookScreen.RED;
                        }
                        else
                        {
                            component.labelCastingSkill.color = SpellBookScreen.BROWN;
                        }
                    }
                    else
                    {
                        component.spellCastingSkill.SetActive(value: false);
                    }
                }
                component.spellTypes.SetActive(value: false);
            }
            this.SlotXFilled((data as List<int>)[index], state: true);
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
            component.spellTypeCombat.SetActive(!string.IsNullOrEmpty(spell.battleScript));
            if (playerWizard != null)
            {
                component.spellTypeWorld.SetActive(!string.IsNullOrEmpty(spell.worldScript));
            }
            else
            {
                component.spellTypeWorld.SetActive(value: false);
            }
            bool flag2 = spell.targetType.enumType == ETargetType.WorldSummon;
            bool flag3 = spell.enchantmentData != null;
            _ = spell.targetType.enumType;
            _ = spell.targetType.enumType;
            component.spellTypeSummon.SetActive(flag2);
            component.spellTypeInstant.SetActive(!flag2 && !flag3);
            component.spellTypeEnchantment.SetActive(flag3);
            component.spellTypeSpecial.SetActive(value: false);
            component.tgSpell.onValueChanged.RemoveAllListeners();
            component.tgSpell.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    this.SelectSpell(spell);
                    this.UpdateRightPage();
                }
            });
        }

        protected void UpdateGridPage()
        {
            List<DBReference<Spell>> spells = this.GetSpells();
            List<int> list = new List<int>();
            for (int i = 0; i < spells.Count; i++)
            {
                list.Add(i % this.spellsGrid.GetPageSize());
            }
            for (int j = 0; j < this.spellsGrid.GetPageSize(); j++)
            {
                this.SlotXFilled(j, state: false);
            }
            this.spellsGrid.UpdateGrid(this.GetSpells(), list);
            this.spellsGrid.Unselect();
            this.spellsGrid.SelectItem(0);
        }

        protected virtual void UpdateRightPage()
        {
            Spell selectedSpell = this.GetSelectedSpell();
            if (selectedSpell == null)
            {
                this.spellDetails.SetActive(value: false);
                return;
            }
            this.spellName.text = selectedSpell.GetDescriptionInfo().GetLocalizedName();
            this.spellDetails.SetActive(value: true);
            this.spellDescription.text = selectedSpell.GetDescriptionInfo().GetLocalizedDescription();
            this.spellTarget.text = selectedSpell.targetType.desType.GetLocalizedTargetTypeDescription();
            if (this.WorldMode())
            {
                int worldCastingCost = selectedSpell.GetWorldCastingCost(this.GetSpellCaster());
                int battleCastingCost = selectedSpell.GetBattleCastingCost(this.GetSpellCaster());
                this.spellCostWorld.SetActive(worldCastingCost > 0);
                this.spellCostBattle.SetActive(battleCastingCost > 0);
                this.spellCastingCost.gameObject.SetActive(value: false);
                if (worldCastingCost < 0)
                {
                    this.spellCastingCost.gameObject.SetActive(value: true);
                    this.spellCastingCost.text = global::DBUtils.Localization.Get("UI_UNKNOWN_COST", true);
                }
                else
                {
                    this.spellCastingCostWorld.text = worldCastingCost.ToString();
                    this.spellCastingCostBattle.text = battleCastingCost.ToString();
                }
            }
            else
            {
                this.spellCastingCost.gameObject.SetActive(value: true);
                this.spellCostBattle.SetActive(value: false);
                this.spellCostWorld.SetActive(value: false);
                this.spellCastingCost.text = selectedSpell.GetBattleCastingCost(this.GetSpellCaster()).ToString();
            }
            float upkeepCost = this.GetUpkeepCost();
            if (upkeepCost > 0f)
            {
                this.spellUpkeep.SetActive(value: true);
                this.spellUpkeepCost.text = upkeepCost + "/";
            }
            else
            {
                this.spellUpkeep.SetActive(value: false);
            }
            this.spellDomainChaos.SetActive(selectedSpell.realm == ERealm.Chaos);
            this.spellDomainDeath.SetActive(selectedSpell.realm == ERealm.Death);
            this.spellDomainLife.SetActive(selectedSpell.realm == ERealm.Life);
            this.spellDomainNature.SetActive(selectedSpell.realm == ERealm.Nature);
            this.spellDomainSorcery.SetActive(selectedSpell.realm == ERealm.Sorcery);
            this.spellDomainArcane.SetActive(selectedSpell.realm == ERealm.Arcane);
            this.spellDomainTech.SetActive(selectedSpell.realm == ERealm.Tech);
            this.spellRarityCommon.SetActive(selectedSpell.rarity == ERarity.Common);
            this.spellIconCommon.SetActive(selectedSpell.rarity == ERarity.Common);
            this.spellRarityUncommon.SetActive(selectedSpell.rarity == ERarity.Uncommon);
            this.spellIconUncommon.SetActive(selectedSpell.rarity == ERarity.Uncommon);
            this.spellRarityRare.SetActive(selectedSpell.rarity == ERarity.Rare);
            this.spellIconRare.SetActive(selectedSpell.rarity == ERarity.Rare);
            this.spellRarityVRare.SetActive(selectedSpell.rarity == ERarity.VeryRare);
            this.spellIconVeryRare.SetActive(selectedSpell.rarity == ERarity.VeryRare);
            this.spellTypeCombat.SetActive(!string.IsNullOrEmpty(selectedSpell.battleScript));
            if (this.GetSpellCaster() is PlayerWizard)
            {
                this.spellTypeWorld.SetActive(!string.IsNullOrEmpty(selectedSpell.worldScript));
            }
            else
            {
                this.spellTypeWorld.SetActive(value: false);
            }
            bool flag = selectedSpell.targetType.enumType == ETargetType.WorldSummon;
            bool flag2 = selectedSpell.enchantmentData != null;
            _ = selectedSpell.targetType.enumType;
            _ = selectedSpell.targetType.enumType;
            _ = selectedSpell.targetType.enumType;
            bool active = false;
            this.spellTypeSummon.SetActive(flag);
            this.spellTypeInstant.SetActive(!flag && !flag2);
            this.spellTypeEnchantment.SetActive(flag2);
            this.spellTypeSpecial.SetActive(active);
            this.spellIcon.texture = selectedSpell.GetDescriptionInfo().GetTexture();
            this.spellsUnits.SetActive(value: false);
            this.spellsEnchantments.SetActive(value: false);
            if (selectedSpell.enchantmentData != null && selectedSpell.enchantmentData.Length != 0 && selectedSpell.enchantmentData.Length != 0)
            {
                this.spellsEnchantmentsGrid.SetEnchantments(new List<object>(selectedSpell.enchantmentData));
                this.spellsEnchantments.gameObject.SetActive(value: true);
            }
            if (selectedSpell.stringData != null)
            {
                List<Subrace> list = new List<Subrace>();
                string[] stringData = selectedSpell.stringData;
                for (int i = 0; i < stringData.Length; i++)
                {
                    Subrace subrace = DataBase.Get<Subrace>(stringData[i], reportMissing: false);
                    if (subrace != null)
                    {
                        list.Add(subrace);
                    }
                    if (list.Count > 0)
                    {
                        this.spellsUnitsGrid.UpdateGrid(list);
                        this.spellsUnits.SetActive(value: true);
                    }
                }
            }
            this.UpdateMainButtonState();
        }

        public float GetUpkeepCost()
        {
            this.GetSpellCaster().GetWizardOwner();
            this.GetSpellCaster();
            Spell selectedSpell = this.GetSelectedSpell();
            if (selectedSpell == null)
            {
                return 0f;
            }
            if (selectedSpell == (Spell)SPELL.SPELL_WARD)
            {
                return selectedSpell.upkeepCost;
            }
            if (selectedSpell.enchantmentData != null && selectedSpell.enchantmentData.Length != 0)
            {
                float num = 0f;
                Enchantment[] enchantmentData = selectedSpell.enchantmentData;
                foreach (Enchantment enchantment in enchantmentData)
                {
                    num += (float)enchantment.upkeepCost;
                }
                return num;
            }
            if (selectedSpell.stringData != null)
            {
                float num2 = 0f;
                bool flag = true;
                Subrace subrace = null;
                string[] stringData = selectedSpell.stringData;
                for (int i = 0; i < stringData.Length; i++)
                {
                    subrace = DataBase.Get<Subrace>(stringData[i], reportMissing: false);
                    if (subrace == null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && subrace.tags != null)
                {
                    Tag tag = (Tag)TAG.UPKEEP_MANA;
                    CountedTag[] tags = subrace.tags;
                    foreach (CountedTag countedTag in tags)
                    {
                        if (countedTag.tag == tag)
                        {
                            num2 += countedTag.amount.ToFloat();
                        }
                    }
                }
                return num2;
            }
            return 0f;
        }

        public void OnDestroy()
        {
            if (this.tempUnits != null && this.tempUnits.Count > 0)
            {
                for (int num = this.tempUnits.Count - 1; num >= 0; num--)
                {
                    this.tempUnits[num].Destroy();
                }
            }
        }
    }
}
