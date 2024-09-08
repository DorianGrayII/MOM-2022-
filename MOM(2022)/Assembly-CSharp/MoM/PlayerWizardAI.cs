using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class PlayerWizardAI : PlayerWizard
    {
        [ProtoMember(1)]
        public AIWorldCasting worldCasting;

        [ProtoMember(2)]
        public HashSet<Vector3i> arcanusKnownLocations;

        [ProtoMember(3)]
        public HashSet<Vector3i> myrrorKnownLocations;

        [ProtoMember(4)]
        public AIPriorityTargets priorityTargets;

        [ProtoMember(5)]
        public List<AIWarEffort> warEfforts;

        [ProtoIgnore]
        public AIPlaneVisibility arcanusVisibility;

        [ProtoIgnore]
        public AIPlaneVisibility myrrorVisibility;

        [ProtoIgnore]
        private float lastFarmerOverflow;

        [ProtoIgnore]
        public List<TransportRequest> transportRequests = new List<TransportRequest>();

        [ProtoIgnore]
        public AIMoveManager aiMoveManager;

        public PlayerWizardAI()
        {
        }

        public PlayerWizardAI(Wizard w, Race r = null)
            : base(w, r)
        {
        }

        public PlayerWizardAI(Wizard w, bool myrranForced)
        {
            this.RegisterEntity(GameManager.Get().wizards.Count + 1);
            List<Trait> list = new List<Trait>();
            List<Tag> list2 = new List<Tag>();
            int num = 0;
            int num2 = 11;
            Attributes attributes = new Attributes(null);
            Trait trait = (Trait)TRAIT.MYRRAN;
            switch (DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL"))
            {
            case 3:
                num2 = 12;
                break;
            case 4:
                num2 = 13;
                break;
            }
            if (myrranForced)
            {
                list.Add(trait);
                num += trait.cost;
            }
            else if (!string.IsNullOrEmpty(trait.prerequisiteScript) && (bool)ScriptLibrary.Call(trait.prerequisiteScript, attributes, list))
            {
                list.Add(trait);
                num += trait.cost;
            }
            CountedTag[] array = Array.FindAll(w.tags, (CountedTag o) => o.tag.parent == (Tag)TAG.MAGIC_BOOK);
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].amount - 3; j++)
                {
                    if (array[i].amount - 2 < 1)
                    {
                        break;
                    }
                    num++;
                    if (num <= num2)
                    {
                        Tag tag = array[i].tag;
                        list2.Add(tag);
                        attributes.AddToBase(tag, FInt.ONE);
                    }
                }
            }
            if (list2.Count < 1 || ((double)global::UnityEngine.Random.Range(0f, 1f) < 0.2 && num <= num2 - 2))
            {
                List<Tag> list3 = DataBase.GetType<Tag>().FindAll((Tag o) => o.parent == (Tag)TAG.MAGIC_BOOK && o != (Tag)TAG.LIFE_MAGIC_BOOK && o != (Tag)TAG.DEATH_MAGIC_BOOK && o != (Tag)TAG.ARCANE_BOOK && o != (Tag)TAG.TECH_BOOK);
                Tag tag2 = list3[global::UnityEngine.Random.Range(0, list3.Count)];
                while (num < num2)
                {
                    num++;
                    list2.Add(tag2);
                    attributes.AddToBase(tag2, FInt.ONE);
                }
            }
            List<Race> list4;
            while (num < num2)
            {
                if (global::UnityEngine.Random.Range(0f, 1f) < 0.75f)
                {
                    list4 = DataBase.GetType<Race>().FindAll((Race o) => o.baseRace);
                    foreach (Trait item in list)
                    {
                        if (!string.IsNullOrEmpty(item.raceFilteringScript))
                        {
                            list4 = ScriptLibrary.Call(item.raceFilteringScript, list4) as List<Race>;
                        }
                    }
                    List<Trait> type = DataBase.GetType<Trait>();
                    List<Trait> list5 = new List<Trait>();
                    foreach (Trait item2 in type)
                    {
                        if (item2 == trait || list.IndexOf(item2) > -1 || item2.cost < 0 || num2 - num < item2.cost)
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(item2.raceFilteringScript))
                        {
                            list5.Add(item2);
                            continue;
                        }
                        List<Race> list6 = ScriptLibrary.Call(item2.raceFilteringScript, list4) as List<Race>;
                        if (item2.raceFilteringScript != "TRAC_Myrran" && list.Find((Trait k) => k.raceFilteringScript == "TRAC_Myrran") == null)
                        {
                            list6 = list6.FindAll((Race k) => k.arcanusRace);
                        }
                        if (list6.Count > 0)
                        {
                            list5.Add(item2);
                        }
                    }
                    for (int num3 = list5.Count - 1; num3 >= 0; num3--)
                    {
                        if (!string.IsNullOrEmpty(list5[num3].prerequisiteScript) && !(bool)ScriptLibrary.Call(list5[num3].prerequisiteScript, attributes, list))
                        {
                            list5.RemoveAt(num3);
                        }
                    }
                    if (list5.Count < 1)
                    {
                        continue;
                    }
                    if (global::UnityEngine.Random.Range(0f, 1f) < 0.8f && w.traits != null)
                    {
                        int num4 = list5.FindIndex((Trait k) => Array.IndexOf(w.traits, k) > -1);
                        if (num4 > -1)
                        {
                            Trait trait2 = list5[num4];
                            list.Add(trait2);
                            num += trait2.cost;
                            continue;
                        }
                    }
                    if (global::UnityEngine.Random.Range(0, 100) <= 40)
                    {
                        Trait trait3 = list5[global::UnityEngine.Random.Range(0, list5.Count)];
                        list.Add(trait3);
                        num += trait3.cost;
                    }
                }
                else
                {
                    num++;
                    Tag tag3 = list2[global::UnityEngine.Random.Range(0, list2.Count)];
                    list2.Add(tag3);
                    attributes.AddToBase(tag3, FInt.ONE);
                }
            }
            list4 = DataBase.GetType<Race>().FindAll((Race o) => o.baseRace);
            foreach (Trait item3 in list)
            {
                if (!string.IsNullOrEmpty(item3.raceFilteringScript))
                {
                    list4 = ScriptLibrary.Call(item3.raceFilteringScript, list4) as List<Race>;
                }
            }
            if (list4.Find((Race o) => o.arcanusRace) != null && list4.Find((Race o) => !o.arcanusRace) != null)
            {
                list4 = list4.FindAll((Race o) => o.arcanusRace);
            }
            Race r = list4[global::UnityEngine.Random.Range(0, list4.Count)];
            base.Initialize(w, r, list2, null, list);
        }

        public void PreparationAfterDeserialization()
        {
            if (this.arcanusVisibility == null)
            {
                this.arcanusVisibility = new AIPlaneVisibility(World.GetArcanus(), this);
            }
            if (this.myrrorVisibility == null)
            {
                this.myrrorVisibility = new AIPlaneVisibility(World.GetMyrror(), this);
            }
            this.arcanusVisibility.Update();
            this.myrrorVisibility.Update();
        }

        public IEnumerator PlayTurn(int playerDoomStackValue)
        {
            if (this.arcanusVisibility == null)
            {
                this.arcanusVisibility = new AIPlaneVisibility(World.GetArcanus(), this);
            }
            if (this.myrrorVisibility == null)
            {
                this.myrrorVisibility = new AIPlaneVisibility(World.GetMyrror(), this);
            }
            this.arcanusVisibility.Update();
            this.myrrorVisibility.Update();
            this.GetMoveManager().Validate();
            if (base.heroes == null)
            {
                base.heroes = new List<Reference<Unit>>();
            }
            if (base.heroes.Count > 0)
            {
                if (base.artefacts != null && base.artefacts.Count > 0)
                {
                    for (int num = base.artefacts.Count - 1; num >= 0; num--)
                    {
                        Artefact artefact = base.artefacts[num];
                        foreach (Reference<Unit> hero in base.heroes)
                        {
                            if (artefact == null)
                            {
                                break;
                            }
                            foreach (EquipmentSlot equipmentSlot2 in hero.Get().artefactManager.equipmentSlots)
                            {
                                if (equipmentSlot2.IsCompatible(artefact) && (equipmentSlot2.item == null || equipmentSlot2.item.GetValue() < artefact.GetValue()))
                                {
                                    if (equipmentSlot2.item != null)
                                    {
                                        base.artefacts.Add(equipmentSlot2.item);
                                    }
                                    equipmentSlot2.item = artefact;
                                    base.artefacts.Remove(artefact);
                                    artefact = null;
                                    break;
                                }
                            }
                        }
                    }
                }
                foreach (Reference<Unit> hero2 in base.heroes)
                {
                    int num2 = 0;
                    List<EquipmentSlot> equipmentSlots = hero2.Get().artefactManager.equipmentSlots;
                    EquipmentSlot equipmentSlot = null;
                    if (equipmentSlots != null)
                    {
                        foreach (EquipmentSlot item in equipmentSlots)
                        {
                            if (item?.item != null)
                            {
                                int value = item.item.GetValue();
                                num2 += value;
                                if (equipmentSlot == null || (equipmentSlot?.item?.GetValue()).GetValueOrDefault() > value)
                                {
                                    equipmentSlot = item;
                                }
                            }
                            else if (equipmentSlot == null || (equipmentSlot.item != null && item.item == null))
                            {
                                equipmentSlot = item;
                            }
                        }
                    }
                    if (num2 >= hero2.Get().xp * 15)
                    {
                        continue;
                    }
                    int num3 = hero2.Get().xp * 30 - num2;
                    if (equipmentSlot.item != null)
                    {
                        num3 += equipmentSlot.item.GetValue();
                    }
                    Artefact artefact2 = Artefact.RandomFactory(num3, equipmentSlot.slotType.Get().eTypes, reportError: false);
                    if (artefact2 != null)
                    {
                        if (equipmentSlot.item != null)
                        {
                            base.artefacts.Add(equipmentSlot.item);
                        }
                        equipmentSlot.item = artefact2;
                        base.artefacts.Remove(artefact2);
                    }
                }
            }
            List<Group> list = new List<Group>(GameManager.GetGroupsOfWizard(base.ID));
            List<Multitype<Group, int>> list2 = new List<Multitype<Group, int>>();
            if (list.Count > 0)
            {
                for (int num4 = list.Count - 1; num4 >= 0; num4--)
                {
                    Group group = list[num4];
                    group.doomStack = false;
                    if (group.GetLocationHost() == null)
                    {
                        AIGroupDesignation.Designation designation = group.GetDesignation().designation;
                        if ((designation == AIGroupDesignation.Designation.Defend || designation == AIGroupDesignation.Designation.AggressionShort || designation == AIGroupDesignation.Designation.AggressionMedium || designation == AIGroupDesignation.Designation.AggressionLong) && group.alive && group.GetUnits().Count > 0)
                        {
                            list2.Add(new Multitype<Group, int>(group, group.GetValue()));
                        }
                    }
                }
            }
            list2.Sort((Multitype<Group, int> a, Multitype<Group, int> b) => -a.t1.CompareTo(b.t1));
            for (int j = 0; j < list2.Count; j++)
            {
                Multitype<Group, int> multitype = list2[j];
                if (j < 3 || j < list2.Count / 3)
                {
                    multitype.t0.doomStack = true;
                    multitype.t0.doomStackPowerMiss = Mathf.Max(0, playerDoomStackValue - multitype.t1);
                }
            }
            if (this.GetWarefforts() != null)
            {
                for (int i = this.GetWarefforts().Count - 1; i >= 0; i--)
                {
                    AIWarEffort effort = this.GetWarefforts()[i];
                    yield return effort.ValidateWarEffort();
                    if (!effort.valid)
                    {
                        this.GetWarefforts().RemoveAt(i);
                    }
                    else
                    {
                        yield return effort.ActivateWarEfforts();
                    }
                }
            }
            if (this.priorityTargets == null)
            {
                this.priorityTargets = new AIPriorityTargets(base.ID);
            }
            yield return this.priorityTargets.Update();
            this.EnsureSpellTransport();
            List<Location> locations = GameManager.GetLocationsOfWizard(base.ID);
            yield return this.PrepareLocations(locations);
            this.ResetFarmers(locations);
            this.Oszustwa(locations);
            this.PlanTaxes(locations);
            this.MoneyManagement();
            this.PlanFoodSupply(locations);
            PlayerWizardAI playerWizardAI = base.GetWizardOwner() as PlayerWizardAI;
            PlayerWizardAI playerWizardAI2 = playerWizardAI;
            if (playerWizardAI2.worldCasting == null)
            {
                playerWizardAI2.worldCasting = new AIWorldCasting(playerWizardAI);
            }
            yield return playerWizardAI.worldCasting.ConsiderMagicalResources();
            yield return this.ResolveLocationTactics(locations);
            this.PlanMelding();
            list = new List<Group>(GameManager.GetGroupsOfWizard(base.ID));
            foreach (Group v in list)
            {
                if (v == null || !v.alive || v.GetLocationHost() != null || this.GetMoveManager().IsAssignedAsTransport(v))
                {
                    continue;
                }
                yield return v.GetDesignation().ResolveTurn();
                if (v.alive)
                {
                    _ = v.GetDesignation().designation;
                    _ = v.GetDesignation().destinationPosition;
                    v.GetPosition();
                    v.CurentMP();
                    Group group2 = GameManager.GetGroupsOfPlane(v.GetPlane()).Find((Group o) => o != v && o.GetOwnerID() == v.GetOwnerID() && o.GetPosition() == v.GetPosition());
                    if (group2 != null)
                    {
                        v.TransferUnits(group2);
                    }
                }
            }
            if (base.banishedTurn == 0 && global::UnityEngine.Random.Range(0, 100) < 40)
            {
                Location location = this.FindNodeToMeld();
                if (location != null)
                {
                    bool flag = false;
                    foreach (Group item2 in GameManager.GetGroupsOfWizard(base.ID))
                    {
                        List<Reference<Unit>> units = item2.GetUnits();
                        if (units.Count >= 1 && !(units.Find((Reference<Unit> o) => o.Get().IsMelder()) == null) && item2.aiDesignation != null)
                        {
                            if (item2.aiDesignation.designation != AIGroupDesignation.Designation.Melder || !(item2.aiDesignation.destinationPosition?.position == location.Position))
                            {
                                flag = true;
                            }
                            break;
                        }
                    }
                    if (flag)
                    {
                        Location location2 = null;
                        int num5 = int.MaxValue;
                        foreach (Location item3 in GameManager.GetLocationsOfWizard(base.ID))
                        {
                            if (item3 is TownLocation)
                            {
                                int distanceTo = item3.GetDistanceTo(location);
                                if (location2 == null || distanceTo < num5)
                                {
                                    location2 = item3;
                                    num5 = distanceTo;
                                }
                            }
                        }
                        if (location2 == null)
                        {
                            DBReference<Spell> dBReference = base.GetSpellManager().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.MAGIC_SPIRIT);
                            if (dBReference != null && dBReference.Get().worldCost < base.mana)
                            {
                                Unit u = Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.ARC_MAGIC_SPIRIT);
                                location2.GetLocalGroup().AddUnit(u);
                            }
                        }
                    }
                }
            }
            this.GetMoveManager().EndTurnValidation();
        }

        private void EnsureSpellTransport()
        {
            if (base.GetMagicAndResearch().curentlyCastSpell != null)
            {
                return;
            }
            DBReference<Spell> dBReference = base.GetSpellManager().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.WATER_WALKING);
            if (dBReference == null)
            {
                base.GetSpellManager().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.WIND_WALKING);
            }
            if (dBReference == null)
            {
                return;
            }
            Spell spell = dBReference.Get();
            if (base.turnSkillLeft <= 0 || base.mana < spell.worldCost * 2)
            {
                return;
            }
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (!(entity.Value is Group group) || group.GetOwnerID() != base.ID || group.GetUnits().Count <= 4 || (!group.doomStack && this.priorityTargets?.GetAssignedTarget(group) == null) || group.waterMovement)
                {
                    continue;
                }
                if (spell == (Spell)SPELL.WATER_WALKING)
                {
                    foreach (Reference<Unit> unit2 in group.GetUnits())
                    {
                        if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(this, null), unit2.Get(), spell))
                        {
                            continue;
                        }
                        int num = (int)ScriptLibrary.Call(spell.aiWorldEvaluationScript, this, unit2.Get(), spell);
                        if (num <= 0)
                        {
                            continue;
                        }
                        Multitype<object, float> target = new Multitype<object, float>(unit2.Get(), num);
                        if (this.worldCasting.Cast(spell, target))
                        {
                            base.turnSkillLeft = Mathf.Max(0, base.turnSkillLeft - spell.worldCost);
                            if (base.turnSkillLeft == 0)
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (spell != (Spell)SPELL.WIND_WALKING)
                    {
                        continue;
                    }
                    Unit unit = null;
                    int num2 = 0;
                    foreach (Reference<Unit> unit3 in group.GetUnits())
                    {
                        int worldUnitValue = unit3.Get().GetWorldUnitValue();
                        if (num2 < worldUnitValue)
                        {
                            unit = unit3;
                        }
                    }
                    if (unit == null || (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(this, null), unit, spell)))
                    {
                        continue;
                    }
                    int num3 = (int)ScriptLibrary.Call(spell.aiWorldEvaluationScript, this, unit, spell);
                    if (num3 <= 0)
                    {
                        continue;
                    }
                    Multitype<object, float> target2 = new Multitype<object, float>(unit, num3);
                    if (this.worldCasting.Cast(spell, target2))
                    {
                        base.turnSkillLeft = Mathf.Max(0, base.turnSkillLeft - spell.worldCost);
                        if (base.turnSkillLeft == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private IEnumerator PrepareLocations(List<Location> locations)
        {
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            foreach (Location location in locations)
            {
                if (location.locationTactic == null)
                {
                    location.locationTactic = new AILocationTactic(location);
                }
                location.locationTactic.TurnPreparation();
                if (Time.realtimeSinceStartup - realtimeSinceStartup > 0.04f)
                {
                    yield return null;
                    realtimeSinceStartup = Time.realtimeSinceStartup;
                }
            }
        }

        public void ResetFarmers(List<Location> locations)
        {
            foreach (Location location in locations)
            {
                if (location is TownLocation townLocation)
                {
                    townLocation.farmers = townLocation.MinFarmers();
                }
            }
        }

        public void PlanTaxes(List<Location> wizardLocations)
        {
            int count = DataBase.GetType<Tax>().Count;
            List<Location> list = wizardLocations.FindAll((Location o) => o is TownLocation);
            int count2 = list.Count;
            bool flag = false;
            bool flag2 = false;
            if (base.money < 50 + 10 * count2)
            {
                flag = true;
            }
            else if (base.money > 200 + 100 * count2)
            {
                flag2 = true;
            }
            bool flag3 = false;
            for (int i = 0; i < count; i++)
            {
                if (i == 0 && !flag2)
                {
                    continue;
                }
                int num = base.money / (3 + i * 3);
                base.TaxRank = i;
                int num2 = base.CalculateMoneyIncome(includeUpkeep: true);
                int num3 = 0;
                int num4 = 0;
                foreach (Location item in list)
                {
                    if (item is TownLocation townLocation)
                    {
                        int popUnits = townLocation.GetPopUnits();
                        int rebels = townLocation.GetRebels();
                        if ((float)popUnits * 0.15f < (float)rebels)
                        {
                            num3++;
                        }
                        if ((float)popUnits * 0.3f < (float)rebels)
                        {
                            num4++;
                        }
                    }
                }
                if (flag3)
                {
                    if (20 * num3 >= count2)
                    {
                        base.TaxRank = Mathf.Max(0, i - 1);
                        break;
                    }
                    if (10 * num3 >= count2)
                    {
                        break;
                    }
                }
                else
                {
                    if (10 * num4 > count2)
                    {
                        base.TaxRank = Mathf.Max(0, i - 1);
                        break;
                    }
                    if (5 * num3 >= count2)
                    {
                        break;
                    }
                }
                if ((!(i < 3 && flag) || num2 >= count2) && (num2 > 0 || num2 >= -num))
                {
                    flag3 = true;
                }
            }
        }

        public void MoneyManagement()
        {
            if (base.money > base.mana * (1 + base.alchemyRatio) && base.money > 200)
            {
                int num = base.money - base.mana * 2;
                int num2 = Mathf.Min((base.money - 100) / base.alchemyRatio, num / 2);
                base.mana += num2;
                base.money -= num2 * base.alchemyRatio;
            }
            if (base.mana > base.money * (1 + base.alchemyRatio) && base.mana > 200)
            {
                int num3 = base.mana - base.money * 2;
                int num4 = Mathf.Min((base.mana - 100) / base.alchemyRatio, num3 / 2);
                base.mana -= num4 * base.alchemyRatio;
                base.money += num4;
            }
        }

        private void Oszustwa(List<Location> locations)
        {
            int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
            int num = Mathf.Clamp(settingAsInt, 0, 3);
            int num2 = (base.GetDiplomacy().IsAtWar() ? 2 : 0);
            num += num2;
            if (settingAsInt > 1 && base.wizardTower?.Get() != null && (TurnManager.GetTurnNumber() == 10 || (settingAsInt > 2 && TurnManager.GetTurnNumber() == 25) || (settingAsInt > 3 && TurnManager.GetTurnNumber() == 50)))
            {
                TownLocation townLocation = base.wizardTower.Get();
                Tag tag = (Tag)TAG.SETTLER_UNIT;
                global::DBDef.Unit unit = townLocation.PossibleUnits().Find((global::DBDef.Unit o) => Array.Find(o.tags, (CountedTag k) => k.tag == tag) != null);
                if (unit != null)
                {
                    townLocation.AddUnit(Unit.CreateFrom(unit));
                }
            }
            int num3 = 0;
            int num4 = 0;
            foreach (Location location in locations)
            {
                if (location is TownLocation townLocation2)
                {
                    num4 += townLocation2.GetPopUnits();
                    num3++;
                    CraftingItem first = townLocation2.craftingQueue.GetFirst();
                    if (townLocation2.CalculateProductionIncome() > 0 && first.requirementValue - first.progress > num)
                    {
                        first.progress += num;
                    }
                }
            }
            base.money += (int)((float)Mathf.Max(0, num3 - 1) * (0.5f + (float)num));
            base.money += Mathf.Max(0, (int)((float)(num * num4) / 15f) - 1);
            base.mana += Mathf.Max(0, (int)((float)(num * num4) / 15f) - 1);
            base.castingSkillDevelopment += num;
        }

        private void PlanMelding()
        {
            Location location = null;
            foreach (Location visibleLocation in this.GetVisibleLocations())
            {
                if (visibleLocation.melding == null || visibleLocation.melding.meldOwner == this.GetID() || visibleLocation.GetUnits().Count != 0)
                {
                    continue;
                }
                DiplomaticStatus statusToward = base.GetDiplomacy().GetStatusToward(visibleLocation.melding.meldOwner);
                if (visibleLocation.melding.meldOwner != 0 && (statusToward == null || !statusToward.openWar))
                {
                    continue;
                }
                if (location == null)
                {
                    location = visibleLocation;
                }
                foreach (Group ownGroup in this.GetOwnGroups())
                {
                    if (ownGroup.GetDesignation() != null && ownGroup.GetDesignation().designation == AIGroupDesignation.Designation.Melder && ownGroup.GetDesignation().destinationPosition != null && ownGroup.GetDesignation().destinationPosition.GetAsLocation() == visibleLocation)
                    {
                        if (location == visibleLocation)
                        {
                            location = null;
                        }
                        break;
                    }
                }
                if (location != null)
                {
                    break;
                }
            }
            if (location == null)
            {
                return;
            }
            foreach (Group ownGroup2 in this.GetOwnGroups())
            {
                if (ownGroup2.GetPlane() != location.GetPlane() || (ownGroup2.GetDesignation() != null && ownGroup2.GetDesignation().designation != AIGroupDesignation.Designation.Retreat) || !(ownGroup2.GetUnits().Find((Reference<Unit> o) => o.Get().IsMelder()) != null))
                {
                    continue;
                }
                Unit unit = ownGroup2.KickOutMelder();
                if (unit == null)
                {
                    continue;
                }
                Group group = unit.group?.Get();
                if (group != null && !group.IsHosted())
                {
                    AIGroupDesignation designation = group.GetDesignation();
                    if (designation != null)
                    {
                        designation.NewDesignation(AIGroupDesignation.Designation.Melder, new Destination(location, aggressive: true));
                        return;
                    }
                }
            }
            if (global::UnityEngine.Random.Range(0, 100) >= 10)
            {
                return;
            }
            Location location2 = null;
            int num = int.MaxValue;
            foreach (Location visibleLocation2 in this.GetVisibleLocations())
            {
                if (visibleLocation2.GetOwnerID() == this.GetID() && visibleLocation2.GetPlane() == location.GetPlane())
                {
                    int distanceTo = visibleLocation2.GetDistanceTo(location);
                    if (distanceTo < num)
                    {
                        location2 = visibleLocation2;
                        num = distanceTo;
                    }
                }
            }
            if (location2 == null)
            {
                return;
            }
            DBReference<Spell> dBReference = base.GetSpellManager().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.GUARDIAN_SPIRIT);
            if ((object)dBReference == null)
            {
                dBReference = base.GetSpellManager().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.MAGIC_SPIRIT);
            }
            if (dBReference != null && base.banishedTurn == 0 && dBReference.Get().worldCost < base.mana)
            {
                base.mana -= dBReference.Get().worldCost;
                base.turnSkillLeft = Mathf.Max(0, base.turnSkillLeft - dBReference.Get().worldCost);
                Unit unit2 = null;
                unit2 = ((dBReference.Get() != (Spell)SPELL.GUARDIAN_SPIRIT) ? Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.ARC_MAGIC_SPIRIT) : Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.LIF_GUARDIAN_SPIRIT));
                location2.AddUnit(unit2);
            }
        }

        public void PlanFoodSupply(List<Location> locations)
        {
            float num = 0.2f;
            if (base.CalculateFoodIncome(includeUpkeep: true) >= 0)
            {
                return;
            }
            while (num < 1f)
            {
                num = Mathf.Clamp01(num + 0.1f);
                foreach (Location location in locations)
                {
                    if (!(location is TownLocation townLocation))
                    {
                        continue;
                    }
                    int popUnits = townLocation.GetPopUnits();
                    int rebels = townLocation.GetRebels();
                    int num2 = popUnits - rebels;
                    if (townLocation.farmers < num2)
                    {
                        int num3 = (int)((float)num2 * num);
                        if (num3 > townLocation.farmers)
                        {
                            townLocation.farmers = num3;
                        }
                    }
                }
                if (base.CalculateFoodIncome(includeUpkeep: true) >= 0)
                {
                    break;
                }
            }
        }

        public int AIIncomeScallar(int value, float bonusMultiplier = 1f)
        {
            float num = 0f;
            if (base.magicAndResearch?.curentlyCastSpell?.dbName == "SPELL-SPELL_OF_MASTERY")
            {
                num = 1f;
            }
            int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
            value = ((value <= 0) ? ((int)((float)value * (1f - num - (float)settingAsInt * 0.22f * bonusMultiplier))) : ((int)((float)value * (1f + num + (float)settingAsInt * 0.22f * bonusMultiplier + Mathf.Clamp((float)settingAsInt - 2.5f, 0f, 1f)))));
            return value;
        }

        private IEnumerator ResolveLocationTactics(List<Location> locations)
        {
            foreach (Location location in locations)
            {
                if (!(location is TownLocation))
                {
                    continue;
                }
                TownLocation obj = location as TownLocation;
                int popUnits = obj.GetPopUnits();
                bool seaside = obj.seaside;
                int num = int.MaxValue;
                int num2 = int.MaxValue;
                if (popUnits > 5 && locations.Count > 3)
                {
                    foreach (Location location2 in locations)
                    {
                        if (!(location2 is TownLocation) || location2 == location)
                        {
                            continue;
                        }
                        if (seaside && location2.locationTactic.locationDesignation == AILocationTactic.LocationDesignation.SeaFaring)
                        {
                            int distanceTo = location2.GetDistanceTo(location);
                            if (distanceTo < num2)
                            {
                                num2 = distanceTo;
                            }
                        }
                        if (location2.locationTactic.locationDesignation == AILocationTactic.LocationDesignation.Military)
                        {
                            int distanceTo2 = location2.GetDistanceTo(location);
                            if (distanceTo2 < num)
                            {
                                num = distanceTo2;
                            }
                        }
                    }
                    if (seaside && num2 > 10)
                    {
                        location.locationTactic.locationDesignation = AILocationTactic.LocationDesignation.SeaFaring;
                        continue;
                    }
                    if (num > 10)
                    {
                        location.locationTactic.locationDesignation = AILocationTactic.LocationDesignation.Military;
                        continue;
                    }
                }
                location.locationTactic.locationDesignation = AILocationTactic.LocationDesignation.Economic;
            }
            foreach (Location location3 in locations)
            {
                yield return location3.locationTactic.ResolveTurn();
            }
        }

        public bool IsKnownLocation(Vector3i pos, global::WorldCode.Plane p)
        {
            if (p.arcanusType)
            {
                return this.arcanusVisibility.knownLocationPositions.Contains(pos);
            }
            return this.myrrorVisibility.knownLocationPositions.Contains(pos);
        }

        public AIPlaneVisibility GetPlaneVisibility(global::WorldCode.Plane p)
        {
            if (p.arcanusType)
            {
                return this.arcanusVisibility;
            }
            return this.myrrorVisibility;
        }

        public IEnumerable<Location> GetVisibleLocations()
        {
            return ListUtils.MultiEnumerable(this.arcanusVisibility?.knownLocations, this.myrrorVisibility?.knownLocations);
        }

        public IEnumerable<Group> GetVisibleGroups()
        {
            return ListUtils.MultiEnumerable(this.arcanusVisibility.sensedGroups, this.myrrorVisibility.sensedGroups);
        }

        public IEnumerable<Group> GetOwnGroups()
        {
            return ListUtils.MultiEnumerable(this.arcanusVisibility.ownGroups, this.myrrorVisibility.ownGroups);
        }

        public int WaitTimeForTransporter(PlanePathStage stage, Group cargoGroup, bool bookTransport = false)
        {
            if (stage == null)
            {
                return -1;
            }
            Vector3i transportPosition = stage.GetTransportPosition();
            if (this.transportRequests.Find((TransportRequest o) => o.cargo == cargoGroup) != null)
            {
                return 0;
            }
            global::WorldCode.Plane plane = (stage.arcanus ? World.GetArcanus() : World.GetMyrror());
            foreach (Group v in GameManager.GetGroupsOfPlane(plane))
            {
                if (!v.alive || v.GetOwnerID() != this.GetID() || plane.GetDistanceWrapping(v.GetPosition(), transportPosition) >= 15 || !(v.transporter != null) || !v.waterMovement || this.transportRequests.Find((TransportRequest o) => o.transport == v) != null)
                {
                    continue;
                }
                Group group = v;
                if (v.locationHost == null && v.GetDesignation().designation != AIGroupDesignation.Designation.Defend)
                {
                    continue;
                }
                bool flag = v.locationHost != null;
                Reference<Unit> reference = null;
                if (!flag && v.GetUnits().Count > 1)
                {
                    Reference<Unit> reference2 = null;
                    bool flag2 = true;
                    foreach (Reference<Unit> unit2 in v.GetUnits())
                    {
                        bool flag3 = unit2.Get().GetAttributes().Contains(TAG.CAN_SWIM) || unit2.Get().GetAttributes().Contains(TAG.CAN_FLY);
                        if (!flag3)
                        {
                            flag2 = false;
                        }
                        if (!unit2.Get().GetAttributes().Contains(TAG.TRANSPORTER))
                        {
                            continue;
                        }
                        if (flag3 && reference == null)
                        {
                            reference = unit2;
                        }
                        else if (reference2 != null)
                        {
                            if (reference2.Get().GetMaxMP() > unit2.Get().GetMaxMP())
                            {
                                reference2 = unit2;
                            }
                        }
                        else
                        {
                            reference2 = unit2;
                        }
                    }
                    if (!flag2 && reference2 != null)
                    {
                        flag2 = reference2.Get().GetAttributes().Contains(TAG.CAN_SWIM) || reference2.Get().GetAttributes().Contains(TAG.CAN_FLY);
                    }
                    if (!flag2 && !v.GetPlane().GetHexAt(v.GetPosition()).IsLand())
                    {
                        continue;
                    }
                    flag = true;
                }
                if (!flag || reference == null)
                {
                    continue;
                }
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), transportPosition, group);
                requestDataV.MakePathOverWater();
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                if (path != null && path.Count > 1)
                {
                    if (bookTransport)
                    {
                        this.BookUnitTransportForCargo(v, reference, cargoGroup, transportPosition);
                    }
                    return 0;
                }
            }
            foreach (Location item in GameManager.GetLocationsOfThePlane(plane))
            {
                if (!(item is TownLocation townLocation) || item.GetOwnerID() != cargoGroup.GetOwnerID() || townLocation.WaterInClosestVicinity() == 0 || townLocation.GetPlane().GetDistanceWrapping(townLocation.GetPosition(), transportPosition) > 15 || townLocation.locationTactic == null || townLocation.locationTactic.dangerRank > 1 || townLocation.buildings == null || townLocation.buildings.Find((DBReference<Building> o) => o.Get() == (Building)BUILDING.SHIP_YARD) == null)
                {
                    continue;
                }
                RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(townLocation.GetPlane(), townLocation.GetPosition(), transportPosition, townLocation);
                requestDataV2.MakePathOverWater();
                PathfinderV2.FindPath(requestDataV2);
                List<Vector3i> path2 = requestDataV2.GetPath();
                if (path2 == null || path2.Count <= 1)
                {
                    continue;
                }
                List<global::DBDef.Unit> list = townLocation.PossibleUnits();
                Tag tTag = (Tag)TAG.TRANSPORTER;
                Tag fTag = (Tag)TAG.CAN_FLY;
                Tag sTag = (Tag)TAG.CAN_SWIM;
                global::DBDef.Unit unit = list.Find((global::DBDef.Unit o) => Array.Find(o.tags, (CountedTag k) => k.tag == tTag) != null && (Array.Find(o.tags, (CountedTag k) => k.tag == fTag) != null || Array.Find(o.tags, (CountedTag k) => k.tag == sTag) != null));
                if (unit != null)
                {
                    if (!townLocation.craftingQueue.ReturnWorkToTransportUnit())
                    {
                        townLocation.craftingQueue.InsertFirstItem(unit, 1);
                    }
                    return 8;
                }
            }
            return -1;
        }

        private void BookUnitTransportForCargo(Group sourceGroup, Unit source, Group cargo, Vector3i transportPos)
        {
            if (source == null)
            {
                return;
            }
            Group group = sourceGroup;
            if (sourceGroup.locationHost != null || sourceGroup.GetUnits().Count > 0)
            {
                group = new Group(sourceGroup.GetPlane(), sourceGroup.GetOwnerID());
                group.Position = sourceGroup.GetPosition();
                sourceGroup.TransferUnit(group, source);
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), transportPos, group);
                requestDataV.MakePathOverWater();
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                if (path == null || path.Count < 2)
                {
                    return;
                }
                group.Position = path[1];
                if (group.IsGroupDiscoveredAndVisible())
                {
                    group.GetMapFormation();
                }
            }
            TransportRequest transportRequest = new TransportRequest();
            transportRequest.cargo = cargo;
            transportRequest.loadingPosition = transportPos;
            transportRequest.transport = group;
            transportRequest.Initialize();
            this.transportRequests.Add(transportRequest);
        }

        public List<AIWarEffort> GetWarefforts()
        {
            if (this.warEfforts == null)
            {
                this.warEfforts = new List<AIWarEffort>();
            }
            return this.warEfforts;
        }

        public AIWarEffort GetWareffortForGroup(Group g)
        {
            foreach (AIWarEffort wareffort in this.GetWarefforts())
            {
                if (wareffort.armies != null && wareffort.armies.FindIndex((AIWarArmy o) => o.group == g) > -1)
                {
                    return wareffort;
                }
            }
            return null;
        }

        public bool PrepareCastingForWarEffort()
        {
            foreach (AIWarEffort wareffort in this.GetWarefforts())
            {
                if (wareffort.PrepareByWarCasting())
                {
                    return true;
                }
            }
            return false;
        }

        public AIMoveManager GetMoveManager()
        {
            if (this.aiMoveManager == null)
            {
                this.aiMoveManager = new AIMoveManager();
            }
            return this.aiMoveManager;
        }

        public static IEnumerator MoveGroup(Group g, Vector3i destination, bool arcanus)
        {
            yield return PlayerWizardAI.MoveGroup(g, destination, arcanus ? World.GetArcanus() : World.GetMyrror());
        }

        public static IEnumerator MoveGroup(Group g, Vector3i destination, global::WorldCode.Plane destinationPlane)
        {
            if (g.alive && GameManager.GetWizard(g.GetOwnerID()) is PlayerWizardAI playerWizardAI)
            {
                yield return playerWizardAI.GetMoveManager().Move(g, destination, destinationPlane);
            }
        }

        public Location FindNodeToMeld(Group closetoThisGroup = null)
        {
            Location location = null;
            foreach (Location visibleLocation in this.GetVisibleLocations())
            {
                if (visibleLocation.melding == null)
                {
                    continue;
                }
                Location location2 = null;
                if (visibleLocation.melding.meldOwner != base.ID && (visibleLocation.GetGroup().GetUnits().Count == 0 || visibleLocation.GetOwnerID() == base.ID))
                {
                    DiplomaticStatus statusToward = base.GetDiplomacy().GetStatusToward(visibleLocation.melding.meldOwner);
                    if (statusToward == null || statusToward.openWar)
                    {
                        location2 = visibleLocation;
                    }
                }
                if (location2 == null)
                {
                    continue;
                }
                if (closetoThisGroup == null)
                {
                    return location2;
                }
                if (location == null)
                {
                    location = location2;
                    continue;
                }
                int distanceTo = location.GetDistanceTo(closetoThisGroup);
                if (location2.GetDistanceTo(closetoThisGroup) < distanceTo)
                {
                    location = location2;
                }
            }
            return location;
        }
    }
}
