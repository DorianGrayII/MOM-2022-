using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MoM.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class LevelUp : ScreenBase
    {
        public TextMeshProUGUI labelHeading;

        public TextMeshProUGUI labelMelee;

        public TextMeshProUGUI labelRange;

        public TextMeshProUGUI labelArmour;

        public TextMeshProUGUI labelResistance;

        public TextMeshProUGUI labelHits;

        public TextMeshProUGUI labelChanceToHit;

        public TextMeshProUGUI labelFireBreath;

        public TextMeshProUGUI labelThrown;

        public TextMeshProUGUI labelRankName;

        public Button btClose;

        public Button btGoToUnit;

        public GridItemManager skillsGrid;

        public RawImage riCharacterPortrait;

        public RawImage riCharacterPortraitOverdraw;

        public GameObject goMelee;

        public GameObject goRange;

        public GameObject goArmour;

        public GameObject goResistance;

        public GameObject goHits;

        public GameObject goChanceToHit;

        public GameObject goFireBreath;

        public GameObject goThrown;

        public UnitExperience experience;

        private List<Tuple<Skill, FInt>> skills = new List<Tuple<Skill, FInt>>();

        private BaseUnit unit;

        public static void PopUp(BaseUnit u, AttributeDelta d)
        {
            UIManager.Open<LevelUp>(UIManager.Layer.Popup).Set(u, d);
            AudioLibrary.RequestSFX("OpenLevelUp");
        }

        public void Set(BaseUnit u, AttributeDelta d)
        {
            if (d.deltaAttributeSets.Count == 0)
            {
                d.CalcDeltas(u);
            }
            this.unit = u;
            this.experience.Set(u);
            this.labelHeading.text = global::DBUtils.Localization.Get("UI_HAS_LEVELED_UP", true, u.GetName());
            this.riCharacterPortrait.texture = u.dbSource.Get().GetDescriptionInfo().GetTextureLarge();
            this.riCharacterPortraitOverdraw.texture = this.riCharacterPortrait.texture;
            Tag uType = ((u.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
            UnitLvl t = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == u.GetLevel());
            this.labelRankName.text = t.GetDILocalizedName();
            DBReference<Skill> skill = null;
            if (d.currentAttributeSets.ContainsKey((Skill)SKILL.HERO_LEVEL))
            {
                skill = (Skill)SKILL.HERO_LEVEL;
            }
            else if (d.currentAttributeSets.ContainsKey((Skill)SKILL.UNIT_LEVEL))
            {
                skill = (Skill)SKILL.UNIT_LEVEL;
            }
            this.SetMainStat((Tag)TAG.MELEE_ATTACK, this.goMelee, this.labelMelee, skill, d, FInt.ONE);
            this.SetMainStat((Tag)TAG.DEFENCE, this.goArmour, this.labelArmour, skill, d, FInt.ONE);
            this.SetMainStat((Tag)TAG.RESIST, this.goResistance, this.labelResistance, skill, d, FInt.ONE);
            this.SetMainStat((Tag)TAG.HIT_POINTS, this.goHits, this.labelHits, skill, d, FInt.ONE);
            this.SetMainStat((Tag)TAG.MELEE_ATTACK_CHANCE, this.goChanceToHit, this.labelChanceToHit, skill, d, new FInt(100));
            if (u.IsRangedUnit())
            {
                this.goRange.SetActive(value: true);
                this.SetMainStat((Tag)TAG.RANGE_ATTACK, this.goRange, this.labelRange, skill, d, FInt.ONE);
                if (!this.goChanceToHit.activeSelf)
                {
                    this.SetMainStat((Tag)TAG.RANGE_ATTACK_CHANCE, this.goChanceToHit, this.labelChanceToHit, skill, d, new FInt(100));
                }
            }
            else
            {
                this.goRange.SetActive(value: false);
            }
            if (!this.goChanceToHit.activeSelf)
            {
                this.SetMainStat((Tag)TAG.DEFENCE_CHANCE, this.goChanceToHit, this.labelChanceToHit, skill, d, new FInt(100));
            }
            this.skills.Clear();
            foreach (KeyValuePair<DBReference<Skill>, NetDictionary<DBReference<Tag>, FInt>> deltaAttributeSet in d.deltaAttributeSets)
            {
                if (!(deltaAttributeSet.Key != (Skill)SKILL.HERO_LEVEL) || !(deltaAttributeSet.Key != (Skill)SKILL.UNIT_LEVEL))
                {
                    continue;
                }
                FInt zERO = FInt.ZERO;
                foreach (KeyValuePair<DBReference<Tag>, FInt> item in deltaAttributeSet.Value)
                {
                    zERO += item.Value;
                }
                if (zERO > 0)
                {
                    this.skills.Add(new Tuple<Skill, FInt>(deltaAttributeSet.Key.Get(), zERO));
                }
            }
            this.skillsGrid.gameObject.SetActive(this.skills.Count > 0);
            this.skillsGrid.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                Tuple<Skill, FInt> tuple = source as Tuple<Skill, FInt>;
                DescriptionInfo descriptionInfo = tuple.Item1.GetDescriptionInfo();
                SkillEnchantmentListItem component = itemSource.GetComponent<SkillEnchantmentListItem>();
                component.skill.SetActive(value: true);
                component.skillImage.texture = descriptionInfo.GetTexture();
                component.label.text = descriptionInfo.GetLocalizedName();
                itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = tuple.Item1.dbName;
            }, UpdateSkillGrid);
            this.UpdateSkillGrid();
        }

        private void UpdateSkillGrid()
        {
            this.skillsGrid.UpdateGrid(this.skills);
        }

        private void SetMainStat(Tag t, GameObject visible, TextMeshProUGUI label, DBReference<Skill> skill, AttributeDelta d, FInt scale)
        {
            bool flag = false;
            if (skill != null)
            {
                NetDictionary<DBReference<Tag>, FInt> netDictionary = d.currentAttributeSets[skill];
                NetDictionary<DBReference<Tag>, FInt> netDictionary2 = d.deltaAttributeSets[skill];
                DBReference<Tag> key = t;
                flag = netDictionary2?.ContainsKey(key) ?? false;
                if (flag)
                {
                    label.text = (netDictionary[key] * scale).ToInt().ToString("+#;-#;0");
                }
            }
            visible.SetActive(flag);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                UIManager.Close(this);
            }
            else
            {
                if (!(s == this.btGoToUnit))
                {
                    return;
                }
                Unit unit = this.unit as Unit;
                if (unit == null || unit.group?.Get() != null)
                {
                    World.ActivatePlane(this.unit.GetPlane());
                    CameraController.CenterAt(this.unit.GetPosition());
                    if (unit != null && unit.group != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), focus: true);
                    }
                }
                UIManager.Close(this);
            }
        }
    }
}
