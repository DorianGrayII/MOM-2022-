using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnrealByte.EasyJira;
using WorldCode;

namespace MOM
{
    public class DevMenu : ScreenBase
    {
        public DropDownFilters moduleDropdown;

        public DropDownFilters eventDropdown;

        public DropDownFilters scriptDropdown;

        public DropDownFilters parameter2Dropdown;

        public DropDownFilters unitDropdown;

        public DropDownFilters enemyWizardUnitDropdown;

        public DropDownFilters enemyNeutralUnitDropdown;

        public DropDownFilters buildingDropdown;

        public DropDownFilters spellDropdown;

        public DropDownFilters resDropdown;

        public DropDownFilters artefactDropdown;

        public DropDownFilters castSpellDropdown;

        public DropDownFilters oneSpellOnlyDropdown;

        public DropDownFilters traitDropdown;

        public TMP_InputField parameter1;

        public TMP_InputField XpInput;

        public Button triggerEvent;

        public Button triggerScript;

        public Button closeDevMenu;

        public Button addUnitOnMap;

        public Button addEnemyWizardUnitOnMap;

        public Button addEnemyNeutralUnitOnMap;

        public Button addEnemyTownOnMap;

        public Button fightInCapitol;

        public Button addBuilding;

        public Button addSpell;

        public Button addAllSpells;

        public Button addRes;

        public Button addXp;

        public Button addArtefact;

        public Button addWizards;

        public Button castSpell;

        public Button killEnemyWizard;

        public Button killYourself;

        public Button oneSpellOnly;

        public Button addTrait;

        public Button revealMap;

        public Toggle addResEachTurn;

        public Toggle eventsTestMode;

        private Module selectedModule;

        private List<Adventure> events;

        public override void OnStart()
        {
            base.OnStart();
            TLog.Get().usedDevMenu = true;
            if (GameManager.instance != null)
            {
                GameManager.instance.usedDevMenuInThisGame = true;
            }
            AdventureLibrary.currentLibrary.GetPerPlayerEvents();
            AdventureLibrary.currentLibrary.GetSimultaneusEvents();
            List<string> list = new List<string>();
            foreach (Module module in AdventureLibrary.currentLibrary.modules)
            {
                if (module.isAllowed)
                {
                    list.Add(module.name);
                }
            }
            this.moduleDropdown.onChange = delegate
            {
                string modName = this.moduleDropdown.GetSelection();
                this.selectedModule = AdventureLibrary.currentLibrary.modules.Find((Module o) => o.name == modName);
                if (this.selectedModule == null)
                {
                    Debug.LogError("[ERROR]Missing selected module! " + modName);
                }
                this.UpdateEventList();
            };
            this.moduleDropdown.SetOptions(list);
            this.FillUnitList(TAG.NORMAL_CLASS, TAG.FANTASTIC_CLASS, TAG.HERO_CLASS, TAG.CHAMPION_CLASS, this.unitDropdown);
            this.FillUnitList(TAG.NORMAL_CLASS, TAG.FANTASTIC_CLASS, TAG.HERO_CLASS, TAG.CHAMPION_CLASS, this.enemyWizardUnitDropdown);
            this.FillUnitList(TAG.NORMAL_CLASS, TAG.FANTASTIC_CLASS, TAG.HERO_CLASS, TAG.CHAMPION_CLASS, this.enemyNeutralUnitDropdown);
            this.FillBuildingList(this.buildingDropdown);
            this.FillResList(this.resDropdown);
            this.FillSpellList(this.spellDropdown);
            this.FillSpellList(this.castSpellDropdown);
            this.FillSpellList(this.oneSpellOnlyDropdown);
            this.FillArtefactList(this.artefactDropdown);
            this.FillTraitList(this.traitDropdown);
            this.SetToogleResButton();
            this.SetEventsTestMode();
        }

        private void UpdateEventList()
        {
            if (this.selectedModule == null)
            {
                return;
            }
            List<string> list = new List<string>();
            if (this.selectedModule.adventures != null)
            {
                this.events = this.selectedModule.adventures.FindAll((Adventure o) => o.isAllowed);
                if (this.events != null)
                {
                    foreach (Adventure @event in this.events)
                    {
                        string item = @event.name;
                        list.Add(item);
                    }
                }
            }
            this.eventDropdown.SetOptions(list);
            if (list.Count > 0)
            {
                this.eventDropdown.SelectOption(0);
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.addUnitOnMap)
            {
                PlayerWizard humanWizard = GameManager.GetHumanWizard();
                if (humanWizard.summoningCircle != null && this.unitDropdown.GetSelection() != null)
                {
                    Unit unit = Unit.CreateFrom((Subrace)DataBase.Get(this.unitDropdown.GetSelection(), reportMissing: false));
                    humanWizard.summoningCircle.Get().AddUnit(unit);
                    unit.UpdateMP();
                }
                UIManager.Close(this);
            }
            if (s == this.addEnemyWizardUnitOnMap)
            {
                PlayerWizard wizard = GameManager.GetWizard(2);
                if (this.enemyWizardUnitDropdown.GetSelection() != null)
                {
                    Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                    DBClass dBClass = DataBase.Get(this.enemyWizardUnitDropdown.GetSelection(), reportMissing: false);
                    Group group = new Group(World.GetActivePlane(), wizard.ID);
                    group.Position = hexCoordAt;
                    group.AddUnit(Unit.CreateFrom((Subrace)dBClass));
                    group.GetMapFormation();
                }
                UIManager.Close(this);
            }
            if (s == this.addEnemyNeutralUnitOnMap)
            {
                if (this.enemyNeutralUnitDropdown.GetSelection() != null)
                {
                    Vector3i hexCoordAt2 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                    DBClass dBClass2 = DataBase.Get(this.enemyNeutralUnitDropdown.GetSelection(), reportMissing: false);
                    Group group2 = new Group(World.GetActivePlane(), 0);
                    group2.Position = hexCoordAt2;
                    group2.AddUnit(Unit.CreateFrom((Subrace)dBClass2));
                    group2.GetMapFormation();
                }
                UIManager.Close(this);
            }
            if (s == this.addEnemyTownOnMap)
            {
                PlayerWizard v2 = GameManager.GetWizard(2);
                TownLocation.CreateLocation(HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition()), source: DataBase.GetType<Town>().Find((Town o) => o.race == v2.mainRace.Get()), p: World.GetActivePlane(), size: 4, owner: v2.ID);
                UIManager.Close(this);
            }
            if (s == this.fightInCapitol)
            {
                FSMMapGame.Get().InitializeRandomBattle(null, 2, ownCapitolBattle: true);
                UIManager.Close(this);
            }
            if (s == this.addBuilding)
            {
                PlayerWizard humanWizard2 = GameManager.GetHumanWizard();
                if (humanWizard2.summoningCircle != null && this.buildingDropdown.GetSelection() != null)
                {
                    Building b = DataBase.Get(this.buildingDropdown.GetSelection(), reportMissing: false) as Building;
                    humanWizard2.summoningCircle.Get().AddBuilding(b);
                }
                UIManager.Close(this);
            }
            if (s == this.addRes)
            {
                PlayerWizard humanWizard3 = GameManager.GetHumanWizard();
                if (humanWizard3.summoningCircle != null && this.resDropdown.GetSelection() != null)
                {
                    MHRandom mHRandom = new MHRandom();
                    global::WorldCode.Plane obj = humanWizard3.summoningCircle?.Get().plane;
                    Resource resource = DataBase.Get(this.resDropdown.GetSelection(), reportMissing: false) as Resource;
                    GameObject source2 = AssetManager.Get<GameObject>(resource.GetDescriptionInfo().graphic);
                    Vector3i townPosition = humanWizard3.summoningCircle.Get().Position;
                    TownLocation townLocation = (TownLocation)GameManager.Get().registeredLocations.Find((Location o) => o.Position == townPosition);
                    List<Vector3i> surroundingArea = townLocation.GetSurroundingArea(townLocation.GetTownRange());
                    surroundingArea.RandomSort();
                    Chunk chunkFor = obj.GetChunkFor(surroundingArea[0]);
                    Vector3 position = HexCoordinates.HexToWorld3D(surroundingArea[0]);
                    Hex hexAt = obj.GetHexAt(surroundingArea[0]);
                    GameObject gameObject = GameObjectUtils.Instantiate(source2, chunkFor.go.transform);
                    gameObject.transform.localRotation = Quaternion.Euler(Vector3.up * mHRandom.GetFloat(0f, 360f));
                    gameObject.transform.position = position;
                    hexAt.resourceInstance = gameObject;
                    if (obj == World.GetArcanus())
                    {
                        World.GetArcanus().GetHexAt(surroundingArea[0]).Resource = resource;
                    }
                    else
                    {
                        World.GetMyrror().GetHexAt(surroundingArea[0]).Resource = resource;
                    }
                    hexAt.UpdateHexProduction();
                }
                UIManager.Close(this);
            }
            if (s == this.addSpell)
            {
                PlayerWizard humanWizard4 = GameManager.GetHumanWizard();
                if (humanWizard4 != null && this.spellDropdown.GetSelection() != null)
                {
                    Spell s2 = DataBase.Get(this.spellDropdown.GetSelection(), reportMissing: false) as Spell;
                    humanWizard4.AddSpell(s2);
                }
                UIManager.Close(this);
            }
            if (s == this.addAllSpells)
            {
                PlayerWizard humanWizard5 = GameManager.GetHumanWizard();
                if (humanWizard5 != null)
                {
                    foreach (Spell item2 in DataBase.GetType<Spell>())
                    {
                        humanWizard5.AddSpell(item2);
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.addXp)
            {
                PlayerWizard humanWizard6 = GameManager.GetHumanWizard();
                if (humanWizard6.summoningCircle != null)
                {
                    Reference<TownLocation> city = humanWizard6.summoningCircle;
                    List<Group> list = GameManager.Get().registeredGroups.FindAll((Group o) => o.GetPosition() == city.Get().GetPosition() && o.GetPlane() == city.Get().GetPlane() && (o.GetLocationHost()?.otherPlaneLocation?.Get() == null || o.plane.arcanusType));
                    List<Reference<Unit>> list2 = new List<Reference<Unit>>();
                    foreach (Group item3 in list)
                    {
                        list2.AddRange(item3.GetUnits());
                    }
                    int result = (int.TryParse(this.XpInput.text, out result) ? result : 0);
                    if (list2.Count > 0)
                    {
                        foreach (Reference<Unit> item4 in list2)
                        {
                            item4.Get().xp += result;
                        }
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.addArtefact)
            {
                List<Artefact> artefacts = GameManager.GetHumanWizard().artefacts;
                if (artefacts != null && this.artefactDropdown.GetSelection() != null)
                {
                    Artefact item = Artefact.Craft(DataBase.Get(this.artefactDropdown.GetSelection(), reportMissing: false) as global::DBDef.Artefact);
                    artefacts.Add(item);
                }
                UIManager.Close(this);
            }
            if (s == this.addWizards)
            {
                foreach (PlayerWizard wizard2 in GameManager.GetWizards())
                {
                    if (wizard2.GetID() != GameManager.GetHumanWizard().GetID())
                    {
                        GameManager.GetHumanWizard().EnsureWizardIsKnown(wizard2.GetID());
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.killEnemyWizard)
            {
                foreach (PlayerWizard w2 in GameManager.GetWizards())
                {
                    if (w2.GetID() == 1 || w2.GetTowerLocation() == null)
                    {
                        continue;
                    }
                    List<Group> list3 = GameManager.Get().registeredGroups.FindAll((Group o) => o.GetOwnerID() == w2.ID && (o.GetLocationHost()?.otherPlaneLocation?.Get() == null || o.plane.arcanusType));
                    w2.GetTowerLocation().Destroy();
                    foreach (Group item5 in list3)
                    {
                        item5.Destroy();
                    }
                    break;
                }
                UIManager.Close(this);
            }
            if (s == this.killYourself)
            {
                PlayerWizard w = GameManager.GetHumanWizard();
                foreach (Group item6 in GameManager.Get().registeredGroups.FindAll((Group o) => o.GetOwnerID() == w.ID && (o.GetLocationHost()?.otherPlaneLocation?.Get() == null || o.plane.arcanusType)))
                {
                    item6.Destroy();
                }
                w.GetTowerLocation().Destroy();
                UIManager.Close(this);
            }
            if (s == this.castSpell)
            {
                PlayerWizard v = GameManager.GetHumanWizard();
                PlayerWizard caster = GameManager.GetWizard(2);
                if (this.castSpellDropdown.GetSelection() != null)
                {
                    Spell spell = DataBase.Get(this.castSpellDropdown.GetSelection(), reportMissing: false) as Spell;
                    switch (spell.targetType.enumType)
                    {
                    case ETargetType.TargetWizard:
                        if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.WIZARD_ENEMY))
                        {
                            ScriptLibrary.Call(spell.worldScript, caster, v, spell);
                        }
                        else if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.WIZARD_OWN))
                        {
                            ScriptLibrary.Call(spell.worldScript, caster, caster, spell);
                        }
                        else
                        {
                            ScriptLibrary.Call(spell.worldScript, caster, v, spell);
                        }
                        break;
                    case ETargetType.TargetGlobal:
                        ScriptLibrary.Call(spell.worldScript, caster, null, spell);
                        break;
                    case ETargetType.TargetUnit:
                    {
                        List<Group> registeredGroups2 = GameManager.Get().registeredGroups;
                        if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.UNIT_ENEMY))
                        {
                            Unit unit2 = registeredGroups2.Find((Group o) => o.GetOwnerID() == v.GetID()).GetUnits()[0];
                            ScriptLibrary.Call(spell.worldScript, caster, unit2, spell);
                        }
                        else if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.UNIT_FRIENDLY))
                        {
                            Unit unit3 = registeredGroups2.Find((Group o) => o.GetOwnerID() == caster.GetID()).GetUnits()[0];
                            ScriptLibrary.Call(spell.worldScript, caster, unit3, spell);
                        }
                        else
                        {
                            Unit unit4 = registeredGroups2.Find((Group o) => o.GetOwnerID() == v.GetID()).GetUnits()[0];
                            ScriptLibrary.Call(spell.worldScript, caster, unit4, spell);
                        }
                        break;
                    }
                    case ETargetType.TargetGroup:
                    {
                        List<Group> registeredGroups = GameManager.Get().registeredGroups;
                        if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.GROUP_ENEMY))
                        {
                            Group group3 = registeredGroups.Find((Group o) => o.GetOwnerID() == v.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, group3, spell);
                        }
                        else if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.GROUP_FRIENDLY))
                        {
                            Group group4 = registeredGroups.Find((Group o) => o.GetOwnerID() == caster.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, group4, spell);
                        }
                        else
                        {
                            Group group5 = registeredGroups.Find((Group o) => o.GetOwnerID() == v.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, group5, spell);
                        }
                        break;
                    }
                    case ETargetType.TargetLocation:
                    {
                        List<Location> registeredLocations = GameManager.Get().registeredLocations;
                        if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_ENEMY))
                        {
                            Location location = registeredLocations.Find((Location o) => o.GetOwnerID() == v.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, location, spell);
                        }
                        else if (spell.targetType == DataBase.Get<TargetType>(TARGET_TYPE.LOCATION_FRIENDLY))
                        {
                            Location location2 = registeredLocations.Find((Location o) => o.GetOwnerID() == caster.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, location2, spell);
                        }
                        else
                        {
                            Location location3 = registeredLocations.Find((Location o) => o.GetOwnerID() == v.GetID());
                            ScriptLibrary.Call(spell.worldScript, caster, location3, spell);
                        }
                        break;
                    }
                    case ETargetType.TargetHex:
                    {
                        Location towerLocation = v.GetTowerLocation();
                        List<Vector3i> surroundingArea2 = towerLocation.GetSurroundingArea(2);
                        ScriptLibrary.Call(spell.worldScript, caster, spell, surroundingArea2[1], towerLocation.plane);
                        break;
                    }
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.oneSpellOnly)
            {
                foreach (PlayerWizard wizard3 in GameManager.GetWizards())
                {
                    wizard3.GetSpellManager().GetSpells().Clear();
                    if (this.oneSpellOnlyDropdown.GetSelection() != null)
                    {
                        Spell spell2 = DataBase.Get(this.oneSpellOnlyDropdown.GetSelection(), reportMissing: false) as Spell;
                        wizard3.GetSpellManager().Add(spell2);
                        wizard3.mana += 1000;
                        wizard3.castingSkillBonus += 1000;
                        wizard3.AdvanceCastingSkill();
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.addTrait)
            {
                foreach (PlayerWizard wizard4 in GameManager.GetWizards())
                {
                    if (this.traitDropdown.GetSelection() != null)
                    {
                        Trait t = DataBase.Get(this.traitDropdown.GetSelection(), reportMissing: false) as Trait;
                        wizard4.AddTrait(t);
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.revealMap)
            {
                PlayerWizard humanWizard7 = GameManager.GetHumanWizard();
                Enchantment e = DataBase.Get("ENCH-NATURE_AWARENESS", reportMissing: false) as Enchantment;
                humanWizard7.AddEnchantment(e, humanWizard7);
                foreach (EnchantmentInstance enchantment in humanWizard7.GetEnchantments())
                {
                    if (enchantment.source.Get() == (Enchantment)ENCH.NATURE_AWARENESS)
                    {
                        enchantment.upkeepMana = 0;
                    }
                }
                UIManager.Close(this);
            }
            if (s == this.closeDevMenu)
            {
                UIManager.Close(this);
            }
            else
            {
                if (!(s == this.triggerEvent))
                {
                    return;
                }
                if (this.selectedModule == null || this.selectedModule.adventures == null || this.selectedModule.adventures.Count == 0)
                {
                    Debug.LogError("[ERROR]Error in starting event from module");
                    return;
                }
                if (this.events == null || this.events.Count < 1)
                {
                    Debug.LogWarning("No events in the collection");
                }
                Adventure adventure = this.events[this.eventDropdown.GetSelectionNR()];
                AdventureData adventureData = AdventureManager.TryToTriggerAdventure(adventure, GameManager.GetHumanWizard(), null, null, devMode: true);
                if (adventureData != null)
                {
                    this.triggerEvent.interactable = false;
                    this.triggerScript.interactable = false;
                    this.closeDevMenu.interactable = false;
                    base.StartCoroutine(this.AdventurePlay(adventureData, adventure));
                    UIManager.Close(this);
                }
                else
                {
                    UIManager.Close(this);
                }
            }
        }

        public void SetToogleResButton()
        {
            if (GameManager.GetHumanWizard().addCheatResEachTurn)
            {
                this.addResEachTurn.isOn = true;
            }
            else
            {
                this.addResEachTurn.isOn = false;
            }
            this.addResEachTurn.onValueChanged.AddListener(delegate
            {
                this.ToogleAddResEachTurn();
            });
        }

        public void SetEventsTestMode()
        {
            if (FSMCoreGame.GetAdvManager().eventsTestMode)
            {
                this.eventsTestMode.isOn = true;
            }
            else
            {
                this.eventsTestMode.isOn = false;
            }
            this.eventsTestMode.onValueChanged.AddListener(delegate
            {
                this.ToogleEventsTestMode();
            });
        }

        public void ToogleAddResEachTurn()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            if (humanWizard.addCheatResEachTurn)
            {
                humanWizard.addCheatResEachTurn = false;
            }
            else
            {
                humanWizard.addCheatResEachTurn = true;
            }
            UIManager.Close(this);
        }

        public void ToogleEventsTestMode()
        {
            AdventureManager advManager = FSMCoreGame.GetAdvManager();
            if (advManager.eventsTestMode)
            {
                advManager.EnableEventsTestMode(isTestMode: false);
            }
            else
            {
                advManager.EnableEventsTestMode(isTestMode: true);
            }
            UIManager.Close(this);
        }

        private IEnumerator AdventurePlay(AdventureData advData, Adventure ae)
        {
            AdventureManager.ResolveEvent(advData, ae);
            while (AdventureManager.IsAdventureRunning())
            {
                yield return null;
            }
            UIManager.Close(this);
        }

        private void FillUnitList(TAG tag, TAG tag2, TAG tag3, TAG tag4, DropDownFilters list)
        {
            List<global::DBDef.Unit> list2 = DataBase.GetType<global::DBDef.Unit>().FindAll((global::DBDef.Unit o) => o.GetTag(tag) > FInt.ZERO || o.GetTag(tag2) > FInt.ZERO);
            List<Hero> list3 = DataBase.GetType<Hero>().FindAll((Hero o) => o.GetTag(tag3) > FInt.ZERO || o.GetTag(tag4) > FInt.ZERO);
            List<string> sl = new List<string>();
            list2.ForEach(delegate(global::DBDef.Unit o)
            {
                sl.Add(o.dbName);
            });
            list3.ForEach(delegate(Hero o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }

        private void FillBuildingList(DropDownFilters list)
        {
            List<Building> type = DataBase.GetType<Building>();
            List<string> sl = new List<string>();
            type.ForEach(delegate(Building o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }

        private void FillResList(DropDownFilters list)
        {
            List<Resource> type = DataBase.GetType<Resource>();
            List<string> sl = new List<string>();
            type.ForEach(delegate(Resource o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }

        private void FillSpellList(DropDownFilters list)
        {
            List<Spell> type = DataBase.GetType<Spell>();
            List<string> sl = new List<string>();
            type.ForEach(delegate(Spell o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }

        private void FillArtefactList(DropDownFilters list)
        {
            List<global::DBDef.Artefact> type = DataBase.GetType<global::DBDef.Artefact>();
            List<string> sl = new List<string>();
            type.ForEach(delegate(global::DBDef.Artefact o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }

        private void FillTraitList(DropDownFilters list)
        {
            List<Trait> type = DataBase.GetType<Trait>();
            List<string> sl = new List<string>();
            type.ForEach(delegate(Trait o)
            {
                sl.Add(o.dbName);
            });
            list.SetOptions(sl);
        }
    }
}
