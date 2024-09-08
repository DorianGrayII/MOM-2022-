using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoInclude(400, typeof(TownLocation))]
    [ProtoContract]
    public class Location : Entity, IGroup, IEnchantable, IPlanePosition, IHashableGroup, IVisibilityChange, IEventDisplayName
    {
        [ProtoMember(1)]
        public int ID;

        [ProtoMember(2)]
        private Vector3i position;

        [ProtoMember(4)]
        public int hash;

        [ProtoMember(5)]
        public Reference<Group> localGroup;

        [ProtoMember(6)]
        public ELocationType locationType;

        [ProtoMember(7)]
        public bool allowUnderwater;

        [ProtoMember(8)]
        public bool discovered;

        [ProtoMember(9)]
        public int power;

        [ProtoMember(10)]
        public int budget;

        [ProtoMember(11)]
        public string modelName;

        [ProtoMember(12)]
        public DBReference<DBClass> source;

        [ProtoMember(13)]
        protected EnchantmentManager enchantmentManager;

        [ProtoMember(14)]
        public DBReference<global::DBDef.Plane> planeSerializableReference;

        [ProtoMember(21)]
        public AdventureTrigger advTrigger;

        [ProtoMember(24)]
        public string name;

        [ProtoMember(25)]
        public bool skipMarker;

        [ProtoMember(26)]
        public bool explored;

        [ProtoMember(27)]
        public Melding melding;

        [ProtoMember(29)]
        public Reference<Location> otherPlaneLocation;

        [ProtoMember(30)]
        public DBReference<Race> guardianRace;

        [ProtoIgnore]
        public AILocationTactic locationTactic;

        [ProtoIgnore]
        public global::WorldCode.Plane plane;

        [ProtoIgnore]
        public GameObject model;

        [ProtoIgnore]
        public bool spawnedCorrectly;

        [ProtoIgnore]
        public Group preBattleAttackers;

        [ProtoIgnore]
        public bool tempDestroyLocationSpecial;

        [ProtoIgnore]
        private int _owner;

        public static int locationCount;

        [ProtoIgnore]
        public Vector3i Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = this.GetPlane().area.KeepHorizontalInside(value);
            }
        }

        [ProtoMember(3)]
        public int owner
        {
            get
            {
                return this._owner;
            }
            set
            {
                this._owner = value;
                if (TurnManager.GetTurnNumber() <= 0)
                {
                    return;
                }
                List<Vector3i> surroundingArea = this.GetSurroundingArea(TownLocation.GetGeneralTownRange());
                foreach (Location loc in GameManager.GetLocationsOfThePlane(this.plane))
                {
                    if (loc is TownLocation)
                    {
                        surroundingArea.Find((Vector3i o) => o == loc.GetPosition());
                        (loc as TownLocation).UpdateTownLocations();
                    }
                }
            }
        }

        public Location()
        {
            MHZombieMemoryDetector.Track(this);
        }

        [ProtoAfterDeserialization]
        public void AfterDeserialize()
        {
            if (this.enchantmentManager != null)
            {
                this.enchantmentManager.owner = this;
            }
        }

        public bool IsExplored()
        {
            if (!this.explored)
            {
                return this.owner == PlayerWizard.HumanID();
            }
            return true;
        }

        public void MarkAsExplored()
        {
            this.explored = true;
            if (!(this.model == null))
            {
                GameObject gameObject = GameObjectUtils.FindByName(this.model, "LocationLight1");
                GameObject gameObject2 = GameObjectUtils.FindByName(this.model, "LocationLight2");
                if (gameObject != null)
                {
                    gameObject.SetActive(value: false);
                }
                if (gameObject != null)
                {
                    gameObject2.SetActive(value: false);
                }
            }
        }

        public static Location CreateLocation(Vector3i position, global::WorldCode.Plane p, global::DBDef.Location source, int owner, bool skipMarker = true, bool spawnDefenders = true)
        {
            int num = (int)ScriptLibrary.Call("LocationPower", p, source);
            int num2 = (int)ScriptLibrary.Call("LocationBudget", p, source, num);
            int num3 = num2 / 2;
            int num4 = num2 * 4;
            switch (DifficultySettingsData.GetSettingAsInt("UI_DIFF_LAIR_DEFENCE"))
            {
            case 1:
                num4 = (int)((float)num4 * 0.5f);
                break;
            case 2:
                num3 = (int)((float)num3 * 0.9f);
                num4 = (int)((float)num4 * 1.1f);
                break;
            case 3:
                num4 = (int)((float)num4 * 1.3f);
                break;
            case 4:
                num4 = (int)((float)num4 * 1.5f);
                num3 = (int)((float)num3 * 1.1f);
                break;
            }
            p.isSettlerDataReady = false;
            Location location = new Location();
            location.skipMarker = skipMarker;
            location.modelName = source.GetDescriptionInfo().graphic;
            location.plane = p;
            location.planeSerializableReference = p.planeSource;
            location.position = p.GetPositionWrapping(position);
            location.source = source;
            location.owner = owner;
            location.locationType = source.locationType;
            location.budget = num3;
            location.power = num;
            location.name = source.GetDescriptionInfo().GetLocalizedName();
            Group group = null;
            p.ClearSearcherData();
            GameManager.Get().RegisterLocation(location);
            if (source is MagicNode)
            {
                location.allowUnderwater = true;
                MagicNode magicNode = source as MagicNode;
                if (magicNode.customTerrainType != null)
                {
                    p.UpdateTerrainAt(position, magicNode.customTerrainType);
                }
            }
            if (source.unitBonus != null)
            {
                Enchantment[] unitBonus = source.unitBonus;
                foreach (Enchantment e in unitBonus)
                {
                    location.AddEnchantment(e, null);
                }
            }
            if (spawnDefenders)
            {
                if (source.locationType == ELocationType.PlaneTower)
                {
                    if (p.planeSource.Get() == (global::DBDef.Plane)PLANE.MYRROR)
                    {
                        Location locationAt = GameManager.Get().GetLocationAt(position, World.GetArcanus());
                        if (locationAt.locationType == ELocationType.PlaneTower)
                        {
                            location.otherPlaneLocation = locationAt;
                            locationAt.otherPlaneLocation = location;
                            location.GetLocalGroup().groupUnits = locationAt.GetLocalGroup().GetUnits();
                        }
                        else
                        {
                            Vector3i vector3i = position;
                            Debug.LogError("Plane Towers positions doesn't match, Tower 1 position: " + vector3i.ToString());
                        }
                    }
                    else if (!string.IsNullOrEmpty(source.guardianCreationScript))
                    {
                        group = (Group)ScriptLibrary.Call(source.guardianCreationScript, location, num4, false);
                        int num5 = 1;
                        while (group.GetUnits().Count <= 0)
                        {
                            num4 = 100;
                            group = (Group)ScriptLibrary.Call(source.guardianCreationScript, location, num4 * num5, false);
                            num5++;
                        }
                    }
                    p.GetRoadManagers().SetRoadMode(position, p);
                }
                else if (!string.IsNullOrEmpty(source.guardianCreationScript))
                {
                    group = (Group)ScriptLibrary.Call(source.guardianCreationScript, location, num4, false);
                }
                if (group != null && group.GetUnits().Count > 0)
                {
                    location.guardianRace = (Race)group.GetUnits()[0].Get().race;
                }
                if (location.localGroup == null)
                {
                    location.localGroup = new Group(location);
                }
            }
            if (source.locationEvent != null)
            {
                location.advTrigger = new AdventureTrigger();
                location.advTrigger.adventure = source.locationEvent.adventure;
                location.advTrigger.module = source.locationEvent.module;
            }
            if (TurnManager.GetTurnNumber() > 0)
            {
                List<Vector3i> surroundingArea = location.GetSurroundingArea(TownLocation.GetGeneralTownRange());
                foreach (Location loc in GameManager.GetLocationsOfThePlane(location.plane))
                {
                    if (loc is TownLocation)
                    {
                        surroundingArea.Find((Vector3i o) => o == loc.GetPosition());
                        (loc as TownLocation).UpdateTownLocations();
                    }
                }
            }
            location.UpdateVisibility();
            return location;
        }

        public void UpdateVisibility()
        {
            if (!this.discovered && FOW.Get().IsDiscovered(this.GetPosition(), this.GetPlane()))
            {
                this.MakeDiscovered();
            }
        }

        public void MakeDiscovered()
        {
            if (!this.discovered)
            {
                this.discovered = true;
                if (this.locationType == ELocationType.Node)
                {
                    HUD.Get()?.OpenMagicNodeTutorial();
                }
                if (World.GetActivePlane() == this.GetPlane())
                {
                    this.InitializeModel();
                    VerticalMarkerManager.Get().Addmarker(this);
                    this.UpdateMarkers();
                }
                if (this.owner > PlayerWizard.HumanID())
                {
                    GameManager.GetHumanWizard().EnsureWizardIsKnown(this.owner);
                }
                MHEventSystem.TriggerEvent(this, null);
            }
        }

        public virtual void InitializeModel()
        {
            string text = this.modelName;
            Chunk chunkFor = this.GetPlane().GetChunkFor(this.GetPosition());
            if (text == null)
            {
                Debug.LogError(this.source.dbName + " di graphic is null!");
            }
            GameObject gameObject = AssetManager.Get<GameObject>(text);
            if (gameObject == null)
            {
                Debug.LogError("Model " + text + " is missing!");
                return;
            }
            GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
            if (gameObject2 == null)
            {
                return;
            }
            MHZombieMemoryDetector.Track(gameObject2);
            if (this.power == 0 && this.locationType != ELocationType.PlaneTower)
            {
                gameObject2.transform.localRotation = Quaternion.Euler(Vector3.up * global::UnityEngine.Random.Range(0, 360));
            }
            this.model = gameObject2;
            if (this.explored)
            {
                GameObject gameObject3 = GameObjectUtils.FindByName(this.model, "LocationLight1");
                GameObject gameObject4 = GameObjectUtils.FindByName(this.model, "LocationLight2");
                if (gameObject3 != null)
                {
                    gameObject3.SetActive(value: false);
                }
                if (gameObject3 != null)
                {
                    gameObject4.SetActive(value: false);
                }
            }
            if (this.otherPlaneLocation != null)
            {
                GameObject gameObject5 = GameObjectUtils.FindByName(this.model, "PlanarSeal");
                if (gameObject5 != null)
                {
                    gameObject5.SetActive(!GameManager.Get().allowPlaneSwitch);
                }
            }
            Vector3 offset = HexCoordinates.HexToWorld3D(this.position);
            this.SetHexPosition(this.position, offset);
            VerticalMarkerManager.Get().Addmarker(this);
            this.UpdateOwnerModels();
            if (this.power > 0)
            {
                int num = Mathf.CeilToInt((float)this.power / 3f) + 1;
                while (true)
                {
                    GameObject gameObject6 = GameObjectUtils.FindByName(this.model, "Effect" + num);
                    if (!(gameObject6 != null))
                    {
                        break;
                    }
                    gameObject6.SetActive(value: false);
                    num++;
                }
            }
            if (!this.IsModelVisible() && this.model != null)
            {
                Renderer component = this.model.GetComponent<Renderer>();
                if (this.model.GetComponent<ParticleSystem>() != null || component == null)
                {
                    this.model.SetActive(value: false);
                }
                else
                {
                    component.enabled = false;
                }
            }
        }

        public void SetHexPosition(Vector3i position, Vector3 offset)
        {
            Chunk chunkFor = this.GetPlane().GetChunkFor(position);
            this.model.transform.parent = chunkFor.go.transform;
            if (this.model.transform.childCount < 1)
            {
                return;
            }
            this.model.transform.position = chunkFor.go.transform.position + offset;
            List<GameObject> list = null;
            bool flag = this.GetPlane().IsLand(position);
            this.spawnedCorrectly = !flag;
            foreach (Transform item in this.model.transform)
            {
                Vector3 pos = item.position;
                pos.y = (flag ? this.plane.GetHeightAt(pos, allowUnderwater: true) : 0f);
                GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
                if (!this.allowUnderwater && (double)pos.y < -0.1)
                {
                    if (component != null && item.gameObject.name.StartsWith("Effect"))
                    {
                        pos.y = component.heightOffset;
                    }
                    else if (this is TownLocation)
                    {
                        if (list == null)
                        {
                            list = new List<GameObject>();
                        }
                        list.Add(item.gameObject);
                        continue;
                    }
                }
                else if (component != null)
                {
                    pos.y += component.heightOffset;
                }
                item.position = pos;
                if (pos.y >= -0.1f)
                {
                    this.spawnedCorrectly = true;
                }
            }
            if (!this.spawnedCorrectly)
            {
                float y = 0f;
                MeshRenderer componentInChildren = this.model.GetComponentInChildren<MeshRenderer>();
                if (componentInChildren == null)
                {
                    Debug.Log("Location model " + this.model.name + " does not have any mesh renderer");
                }
                else
                {
                    y = 0.5f;
                    componentInChildren.gameObject.AddComponent<LocationSpawnController>().owner = this;
                }
                {
                    foreach (Transform item2 in this.model.transform)
                    {
                        Vector3 vector = item2.position;
                        vector.y = y;
                        item2.position = vector;
                    }
                    return;
                }
            }
            if (list == null)
            {
                return;
            }
            foreach (GameObject item3 in list)
            {
                item3.SetActive(value: false);
            }
        }

        public int GetOwnerID()
        {
            return this.owner;
        }

        public virtual List<Reference<Unit>> GetUnits()
        {
            return this.GetLocalGroup()?.GetUnits();
        }

        public Vector3i GetPosition()
        {
            return this.position;
        }

        public global::WorldCode.Plane GetPlane()
        {
            if (this.plane == null)
            {
                global::DBDef.Plane p = this.planeSerializableReference.Get();
                this.plane = World.GetPlanes().Find((global::WorldCode.Plane o) => o.planeSource.Get() == p);
            }
            return this.plane;
        }

        public AdventureTrigger GetAdventureTrigger()
        {
            return this.advTrigger;
        }

        public EnchantmentManager GetEnchantmentManager()
        {
            if (this.enchantmentManager == null)
            {
                this.enchantmentManager = new EnchantmentManager(this);
            }
            return this.enchantmentManager;
        }

        public Group GetLocalGroup()
        {
            if (this.localGroup == null && EntityManager.GetEntity(this.ID) != null)
            {
                this.localGroup = new Group(this);
            }
            return this.localGroup;
        }

        public void AddUnit(Unit u, bool updateMovementFlags = true)
        {
            this.GetLocalGroup().AddUnit(u);
        }

        public void RemoveUnit(Unit u, bool allowGroupDestruction = true, bool updateGroup = true)
        {
            this.GetLocalGroup().RemoveUnit(u, allowGroupDestruction, updateGroup);
        }

        public virtual int ConquerFame()
        {
            return 0;
        }

        public virtual int LoseFame()
        {
            return 0;
        }

        public virtual int ConquerGold()
        {
            return 0;
        }

        public virtual void SetOwnerID(int id, int attackerID = -1, bool collateralDamage = false)
        {
            int value = 0;
            int num = 0;
            if (id != this.owner)
            {
                value = this.ConquerFame();
                num = this.ConquerGold();
            }
            if (id != this.owner && this.owner > 0)
            {
                PlayerWizard wizardOwner = this.GetWizardOwner();
                wizardOwner.TakeFame(this.LoseFame());
                wizardOwner.money = Mathf.Max(0, wizardOwner.money - num);
                if (wizardOwner.wizardTower == this)
                {
                    wizardOwner.SetTowerLocation(null);
                    wizardOwner.SetSummoningLocation(null);
                    wizardOwner.BanishWizard((attackerID > -1) ? attackerID : id);
                    if (wizardOwner.IsHuman)
                    {
                        AchievementManager.Progress(AchievementManager.Achievement.TasteOfDespair);
                    }
                }
                else if (wizardOwner.summoningCircle == this)
                {
                    wizardOwner.SetSummoningLocation(wizardOwner.wizardTower);
                    if (wizardOwner.IsHuman)
                    {
                        SummaryInfo s = new SummaryInfo
                        {
                            summaryType = SummaryInfo.SummaryType.eSummoningCircleMoved,
                            location = wizardOwner.GetSummoningLocation(),
                            name = this.GetName(),
                            graphic = ((Spell)SPELL.SUMMONING_CIRCLE).GetDescriptionInfo().graphic
                        };
                        wizardOwner.AddNotification(s);
                    }
                }
                List<EnchantmentInstance> enchantments = this.GetEnchantments();
                if (enchantments != null && enchantments.Count > 0)
                {
                    for (int num2 = enchantments.Count - 1; num2 >= 0; num2--)
                    {
                        if (enchantments[num2].owner?.ID == this.owner)
                        {
                            this.RemoveEnchantment(enchantments[num2]);
                        }
                        else
                        {
                            Reference reference = enchantments[num2].owner;
                            if ((object)reference != null && reference.ID == id && enchantments[num2].source.Get().enchCategory == EEnchantmentCategory.Negative)
                            {
                                this.RemoveEnchantment(enchantments[num2]);
                            }
                        }
                    }
                }
            }
            this.owner = id;
            if (this.locationTactic != null)
            {
                this.locationTactic.OwnerChanged();
            }
            if (id > 0)
            {
                PlayerWizard wizard = GameManager.GetWizard(id);
                wizard.TriggerScripts(EEnchantmentType.WizardOrGlobalToTownEnchantment, this);
                if (wizard.traitThePirat > 0)
                {
                    wizard.TakeFame(this.LoseFame());
                }
                else
                {
                    wizard.AddFame(value);
                }
                wizard.money += num;
            }
            if (this is TownLocation townLocation)
            {
                townLocation.farmers = townLocation.MinFarmers();
            }
            if (this.otherPlaneLocation != null && this.otherPlaneLocation.Get().GetOwnerID() != id)
            {
                this.otherPlaneLocation.Get().SetOwnerID(id);
            }
            VerticalMarkerManager verticalMarkerManager = VerticalMarkerManager.Get();
            verticalMarkerManager.UpdateMarkerColors(this.localGroup.Get());
            verticalMarkerManager.DestroyMarker(this);
            if (this.discovered)
            {
                verticalMarkerManager.Addmarker(this);
            }
            this.UpdateOwnerModels();
        }

        public virtual void UpdateOwnerModels()
        {
            Transform transform = this.model?.transform?.Find("OwnerFlags");
            if (!(transform != null))
            {
                return;
            }
            string value = "none";
            if (this.melding != null && this.melding.meldOwner > 0)
            {
                value = GameManager.GetWizard(this.melding.meldOwner).color.ToString();
            }
            for (int num = transform.childCount - 1; num >= 0; num--)
            {
                Transform child = transform.GetChild(num);
                GameObject gameObject = child.gameObject;
                bool flag = child.name.EndsWith(value);
                if (gameObject.activeSelf != flag)
                {
                    gameObject.SetActive(flag);
                }
            }
        }

        public int GetHash()
        {
            return this.hash;
        }

        public void UpdateHash()
        {
            List<Reference<Unit>> units = this.GetUnits();
            if (units != null)
            {
                this.hash = this.ID + units.Count << 8;
            }
            else
            {
                this.hash = this.ID;
            }
        }

        public virtual int GetStrategicValue()
        {
            if (this.locationType == ELocationType.Node)
            {
                return 500;
            }
            if (this.locationType == ELocationType.PlaneTower)
            {
                return 500;
            }
            return 0;
        }

        public bool IsMarkerVisible()
        {
            return FOW.Get().IsDiscovered(this.GetPosition(), this.GetPlane());
        }

        public bool IsModelVisible()
        {
            return this.discovered;
        }

        public int NodePowerIncome()
        {
            if (!(this.source.Get() is MagicNode magicNode))
            {
                return this.power;
            }
            int num = 0;
            float settingAsFloat = DifficultySettingsData.GetSettingAsFloat("UI_DIFF_MAGIC_INTENSITY");
            num = ((settingAsFloat != 0f) ? ((int)((float)this.power * settingAsFloat)) : this.power);
            if (this.melding == null || this.melding.meldOwner != this.GetOwnerID())
            {
                return num;
            }
            List<EnchantmentInstance> list = GameManager.GetWizard(this.melding.meldOwner)?.GetEnchantments();
            if (list == null)
            {
                return num;
            }
            foreach (EnchantmentInstance item in list)
            {
                if ((item.source == (Enchantment)ENCH.NATURE_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.NATURE) || (item.source == (Enchantment)ENCH.SORCERY_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.SORCERY) || (item.source == (Enchantment)ENCH.CHAOS_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.CHAOS))
                {
                    float num2 = (int)ScriptLibrary.Call("UTIL_GetStringParameterValue", item.parameters);
                    num2 /= 100f;
                    num = Mathf.RoundToInt((float)num + (float)num * num2);
                }
                if ((item.source == (Enchantment)ENCH.TAKE_NATURE_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.NATURE) || (item.source == (Enchantment)ENCH.TAKE_SORCERY_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.SORCERY) || (item.source == (Enchantment)ENCH.TAKE_CHAOS_MOON && magicNode == (MagicNode)(Enum)MAGIC_NODE.CHAOS))
                {
                    float num2 = (int)ScriptLibrary.Call("UTIL_GetStringParameterValue", item.parameters);
                    num2 /= 100f;
                    num = Mathf.RoundToInt((float)num - (float)num * num2);
                }
            }
            return num;
        }

        public PlayerWizard GetWizardOwner()
        {
            return GameManager.GetWizard(this.GetOwnerID());
        }

        public virtual void Destroy()
        {
            if (FSMSelectionManager.Get().GetSelectedGroup() is Group group && group.beforeMovingAway != null)
            {
                group.ChangeBeforeMovingAway(null);
            }
            if (this.enchantmentManager != null && this.enchantmentManager.localActiveIterators > 0)
            {
                this.tempDestroyLocationSpecial = true;
                return;
            }
            global::WorldCode.Plane obj = this.GetPlane();
            obj.isSettlerDataReady = false;
            if (this.owner > 0)
            {
                PlayerWizard wizardOwner = this.GetWizardOwner();
                if (wizardOwner.wizardTower == this)
                {
                    wizardOwner.SetTowerLocation(null);
                    wizardOwner.SetSummoningLocation(null);
                    wizardOwner.BanishWizard(0);
                }
                else if (wizardOwner.summoningCircle == this)
                {
                    wizardOwner.SetSummoningLocation(wizardOwner.wizardTower);
                    if (wizardOwner.IsHuman)
                    {
                        SummaryInfo s = new SummaryInfo
                        {
                            summaryType = SummaryInfo.SummaryType.eSummoningCircleMoved,
                            location = wizardOwner.GetSummoningLocation(),
                            name = this.GetName(),
                            graphic = ((Spell)SPELL.SUMMONING_CIRCLE).GetDescriptionInfo().graphic
                        };
                        wizardOwner.AddNotification(s);
                    }
                }
            }
            this.DestroyMarkers();
            if (this.localGroup != null)
            {
                this.localGroup.Get().Destroy();
            }
            if (this.model != null)
            {
                global::UnityEngine.Object.Destroy(this.model);
            }
            this.enchantmentManager?.Destroy();
            GameManager.Get().Unregister(this);
            obj?.ClearSearcherData();
        }

        public void UpdateMarkers()
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
            TerrainMarkers markers_ = this.GetPlane().GetMarkers_();
            if (this.GetOwnerID() == PlayerWizard.HumanID())
            {
                markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: true);
            }
            else
            {
                markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: false);
            }
        }

        public void DestroyMarkers()
        {
            VerticalMarkerManager.Get().DestroyMarker(this);
            TerrainMarkers markers_ = this.GetPlane().GetMarkers_();
            if (this.GetOwnerID() == PlayerWizard.HumanID())
            {
                markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: false);
            }
        }

        public override int GetID()
        {
            return this.ID;
        }

        public override void SetID(int id)
        {
            this.ID = id;
        }

        public void RemoveAdventureTrigger()
        {
            if (this.locationType == ELocationType.PlaneTower)
            {
                Location locationAt = GameManager.Get().GetLocationAt(this.position, World.GetOtherPlane(this.plane));
                if (locationAt.locationType == ELocationType.PlaneTower)
                {
                    locationAt.advTrigger = null;
                }
                else
                {
                    Vector3i vector3i = this.position;
                    Debug.LogError("Plane Towers positions doesn't match, Tower 1 position: " + vector3i.ToString());
                }
            }
            this.advTrigger = null;
        }

        public Vector3 GetPhysicalPosition()
        {
            if (this.model != null)
            {
                return this.model.transform.position;
            }
            return Vector3.zero;
        }

        public virtual string GetName()
        {
            return this.name;
        }

        public void GuaranteeDefenders()
        {
            int num = this.budget * 4;
            global::DBDef.Location location = (global::DBDef.Location)(DBClass)this.source;
            while (this.localGroup.Get().GetUnits().Count == 0)
            {
                num += 50;
                if (!string.IsNullOrEmpty(location.guardianCreationScript))
                {
                    ScriptLibrary.Call(location.guardianCreationScript, this, num);
                    continue;
                }
                Debug.LogError("Location " + location.dbName + " have no guardianCreationScript");
                break;
            }
        }

        public static FInt GetProductionInArea(global::WorldCode.Plane p, Vector3i pos, int range = 2)
        {
            List<Vector3i> range2 = HexNeighbors.GetRange(pos, range);
            FInt zERO = FInt.ZERO;
            foreach (Vector3i item in range2)
            {
                Hex hexAt = p.GetHexAt(item);
                if (hexAt != null)
                {
                    zERO += hexAt.GetProduction();
                }
            }
            return zERO;
        }

        public static string CanBuildTownAtLocation(global::WorldCode.Plane p, Vector3i pos)
        {
            if (!p.GetHexAt(pos).IsLand())
            {
                return "UI_CANNOT_BUILD_CITY_WATER";
            }
            if (GameManager.Get().GetLocationAt(pos, p) != null)
            {
                return "UI_CANNOT_BUILD_CITY_LOCATION";
            }
            int townDistance = DifficultySettingsData.GetTownDistance();
            List<Vector3i> area = HexNeighbors.GetRange(pos, townDistance);
            if (GameManager.GetLocationsOfThePlane(p).Find((Location o) => o is TownLocation && area.FindIndex((Vector3i k) => o.Position == k) > -1) == null)
            {
                return null;
            }
            return global::DBUtils.Localization.Get("UI_CANNOT_BUILD_CITY_PROXIMITY", true, townDistance);
        }

        public string GetEventDisplayName()
        {
            return this.name;
        }

        public void Conquer()
        {
            this.Conquer(this.preBattleAttackers);
        }

        public virtual void Conquer(Group g)
        {
            this.SetOwnerID(g.GetOwnerID(), -1, collateralDamage: true);
            this.AddUnitsIfPossible(g.GetUnits());
        }

        public static void PreBattle(Location location, Group attackers)
        {
            if (location == null)
            {
                return;
            }
            int num = 0;
            if (location is TownLocation townLocation)
            {
                townLocation.preBattlePopulation = townLocation.Population;
                foreach (DBReference<Building> building in townLocation.buildings)
                {
                    num += building.Get().buildCost;
                }
                townLocation.preBattleValue = num;
                TownLocation.lastBattle = townLocation;
            }
            location.preBattleAttackers = attackers;
        }

        public bool MeldAttempt(Unit u)
        {
            if (this.power <= 0)
            {
                return false;
            }
            int num = 100;
            if (this.melding != null && this.melding.strength > 1)
            {
                num = 25;
            }
            if (global::UnityEngine.Random.Range(0, 100) < num)
            {
                int ownerID = u.group.Get().GetOwnerID();
                if (ownerID == PlayerWizard.HumanID())
                {
                    if (this.source.Get() is MagicNode magicNode)
                    {
                        if (magicNode.nodeType == ENodeType.Chaos)
                        {
                            GameManager.Get().achievementChaosNode = true;
                        }
                        else if (magicNode.nodeType == ENodeType.Nature)
                        {
                            GameManager.Get().achievementNatureNode = true;
                        }
                        else if (magicNode.nodeType == ENodeType.Sorcery)
                        {
                            GameManager.Get().achievementSorceryNode = true;
                        }
                        AchievementManager.Progress(AchievementManager.Achievement.NodeMaster);
                    }
                    else
                    {
                        Debug.Log("Incorrect info for node conquering achievement!");
                    }
                }
                this.melding = new Melding();
                this.melding.meldOwner = ownerID;
                this.melding.strength = u.GetAttFinal(TAG.MELDER_UNIT).ToInt();
                this.UpdateOwnerModels();
                this.UpdateMarkers();
                return true;
            }
            return false;
        }

        public Group FormExpedition()
        {
            List<Reference<Unit>> units = this.GetUnits();
            units = units.FindAll((Reference<Unit> o) => !o.Get().IsSettler() && !o.Get().IsEngineer() && !o.Get().IsMelder());
            if (units.Count < 1)
            {
                return null;
            }
            if (this.locationTactic == null || this.locationTactic.MinimumExpectedUnits() > units.Count)
            {
                return null;
            }
            Unit unit = null;
            if (this is TownLocation)
            {
                if (units.Count < 2)
                {
                    return null;
                }
                int num = 0;
                foreach (Reference<Unit> item in units)
                {
                    int worldUnitValue = item.Get().GetWorldUnitValue();
                    if (worldUnitValue > num)
                    {
                        num = worldUnitValue;
                        unit = item;
                    }
                }
            }
            if (this.locationTactic == null || this.locationTactic.MinimumExpectedUnits() > units.Count)
            {
                return null;
            }
            Group group = new Group(this.GetPlane(), this.GetOwnerID());
            group.Position = this.GetPosition();
            foreach (Reference<Unit> item2 in units)
            {
                if (!(this is TownLocation) || !(item2 == unit))
                {
                    if (this.locationTactic.MinimumExpectedUnits() < this.GetUnits().Count)
                    {
                        break;
                    }
                    group.AddUnit(item2);
                }
            }
            if (group.IsModelVisible())
            {
                group.GetMapFormation(createIfMissing: false);
            }
            return group;
        }

        public int SimpleStrengthOfPossibleExpedition()
        {
            List<Reference<Unit>> units = this.GetUnits();
            units = units.FindAll((Reference<Unit> o) => !o.Get().IsSettler() && !o.Get().IsEngineer() && !o.Get().IsMelder());
            if (units.Count < 1)
            {
                return 0;
            }
            Unit unit = null;
            if (this is TownLocation)
            {
                if (units.Count < 2)
                {
                    return 0;
                }
                int num = int.MaxValue;
                foreach (Reference<Unit> item in units)
                {
                    int worldUnitValue = item.Get().GetWorldUnitValue();
                    if (worldUnitValue < num)
                    {
                        num = worldUnitValue;
                        unit = item;
                    }
                }
            }
            int num2 = 0;
            foreach (Reference<Unit> item2 in units)
            {
                if (!(item2 == unit))
                {
                    num2 += item2.Get().GetWorldUnitValue();
                }
            }
            return num2;
        }

        public Group GetGroup()
        {
            return this.GetLocalGroup();
        }

        public virtual void FinishedIteratingEnchantments()
        {
            if (this.tempDestroyLocationSpecial)
            {
                this.Destroy();
            }
        }
    }
}
