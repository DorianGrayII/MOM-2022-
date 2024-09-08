// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMMapGame
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using WorldCode;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMMapGame : FSMStateBase
{
    public static FSMMapGame instance;

    private Coroutine casting;

    private Vector3i chosenTarget;

    private Vector3i tryingTarget;

    public override void OnEnter()
    {
        base.OnEnter();
        FSMMapGame.instance = this;
        MHEventSystem.RegisterListener<Formation>(UnitUpdate, this);
        MHEventSystem.RegisterListener<SummaryInfo>(MapGameEvents, this);
        UIManager.Open<HUD>(UIManager.Layer.HUD);
    }

    private IPlanePosition GetSelectedGroup()
    {
        return FSMSelectionManager.Get().GetSelectedGroup();
    }

    private void UnitUpdate(object sender, object e)
    {
        if (sender is Formation && (sender as Formation).owner is global::MOM.Group group)
        {
            if (group.GetUnits() == null || group.GetUnits().Count < 1)
            {
                group.Destroy();
                FSMSelectionManager.Get().Select(null, focus: false);
            }
            else
            {
                group.UpdateMapFormation();
            }
        }
    }

    public void MapGameEvents(object sender, object e)
    {
        if (!(e is string))
        {
            return;
        }
        switch (e as string)
        {
        case "TownScreen":
        {
            TownLocation g = ((sender is TownListItem) ? (sender as TownListItem).GetTown() : (sender as TownLocation));
            FSMSelectionManager.Get().Select(g, focus: true);
            UIManager.GetOrOpen<TownScreen>(UIManager.Layer.Standard);
            break;
        }
        case "OutpostScreen":
        {
            TownLocation g2 = ((sender is TownListItem) ? (sender as TownListItem).GetTown() : (sender as TownLocation));
            FSMSelectionManager.Get().Select(g2, focus: true);
            UIManager.GetOrOpen<OutpostScreen>(UIManager.Layer.Standard);
            break;
        }
        case "CastingSpell":
        {
            if (FSMSelectionManager.Get() != null)
            {
                FSMSelectionManager.Get().Select(null, focus: false);
            }
            CursorsLibrary.SetMode(CursorsLibrary.Mode.CastSpell);
            this.chosenTarget = Vector3i.invalid;
            this.tryingTarget = Vector3i.invalid;
            SummaryInfo summaryInfo = sender as SummaryInfo;
            HUD.Get().SetCasting(summaryInfo.spell);
            this.casting = base.StartCoroutine(this.CastingSpell(summaryInfo, summaryInfo.spell.Get()));
            break;
        }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        FSMMapGame.instance = null;
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
        HUD screen = UIManager.GetScreen<HUD>(UIManager.Layer.HUD);
        if (screen != null)
        {
            UIManager.Close(screen);
        }
    }

    public static FSMMapGame Get()
    {
        return FSMMapGame.instance;
    }

    private void SpellTargettingOtherWizard(int w)
    {
        this.SpellTargettingOtherWizard(GameManager.GetWizard(w));
    }

    private void SpellTargettingOtherWizard(PlayerWizard w)
    {
        if (w is PlayerWizardAI)
        {
            DiplomaticStatus statusToward = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
            if (statusToward != null)
            {
                statusToward.ChangeRelationshipBy(-200, affectTreaties: true);
                statusToward.willOfWar = 100;
            }
        }
    }

    private IEnumerator CastingSpell(SummaryInfo s, Spell spell)
    {
        PlayerWizard caster = GameManager.GetHumanWizard();
        bool resolved = false;
        List<PlayerWizard> wizardTargets = null;
        MHDataStorage.Set("Unit", null);
        MHDataStorage.Set("Ward", null);
        MHDataStorage.Set("Wizard", null);
        MHDataStorage.Set("DeadHero", null);
        MHDataStorage.Set("EnchantmentInstance", null);
        while (true)
        {
            ETargetType enumType = spell.targetType.enumType;
            if (!resolved)
            {
                if (enumType == ETargetType.TargetWizard)
                {
                    if (spell.targetType == (TargetType)TARGET_TYPE.WIZARD_ENEMY)
                    {
                        wizardTargets = GameManager.GetWizards().FindAll((PlayerWizard o) => o.isAlive && o != caster);
                    }
                    else if (spell.targetType == (TargetType)TARGET_TYPE.WIZARD_OWN)
                    {
                        wizardTargets = new List<PlayerWizard> { caster };
                    }
                    if (wizardTargets.Count == 0)
                    {
                        this.InvalidWorldTarget(spell);
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                        this.casting = null;
                        HUD.Get().SetCasting(null);
                        yield break;
                    }
                    if (spell == (Spell)SPELL.SPELL_BLAST)
                    {
                        PopupTargetWizard.OpenPopup(null, spell, wizardTargets, SelectSpellWizardTarget, CancelSpellTargetSelection, useSpellBlastTooltip: true);
                    }
                    else
                    {
                        PopupTargetWizard.OpenPopup(null, spell, wizardTargets, SelectSpellWizardTarget, CancelSpellTargetSelection);
                    }
                    PlayerWizard playerWizard;
                    while (true)
                    {
                        playerWizard = MHDataStorage.Get<PlayerWizard>("Wizard");
                        if (playerWizard != null || !PopupTargetWizard.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (playerWizard == null)
                    {
                        resolved = true;
                        continue;
                    }
                    if (!caster.TargetForSpellValid(null, playerWizard, spell))
                    {
                        resolved = true;
                        this.InvalidWorldTarget(spell);
                        this.CancelSpellTargetSelection(null);
                        continue;
                    }
                    ScriptLibrary.Call(spell.worldScript, caster, playerWizard, spell);
                    FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                    GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                    this.SpellTargettingOtherWizard(playerWizard);
                    break;
                }
                if (enumType == ETargetType.TargetGlobal && (spell == (Spell)SPELL.RESURRECTION || spell == (Spell)SPELL.UNDEAD_HERO))
                {
                    if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(caster, null), null, spell))
                    {
                        this.InvalidWorldTarget(spell);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    List<DeadHero> dhs = new List<DeadHero>(caster.GetDeadHeroes());
                    dhs.FindAll(delegate(DeadHero o)
                    {
                        if (!o.dbSource.Get().unresurrectable)
                        {
                            Attributes attributes = o.attributes;
                            if (attributes == null)
                            {
                                return false;
                            }
                            return attributes.GetFinal((Tag)TAG.FANTASTIC_CLASS) > 0;
                        }
                        return true;
                    })?.ForEach(delegate(DeadHero o)
                    {
                        dhs.Remove(o);
                    });
                    if (dhs == null || dhs.Count == 0)
                    {
                        this.InvalidWorldTarget(spell);
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                        this.casting = null;
                        HUD.Get().SetCasting(null);
                        yield break;
                    }
                    PopupResurrectionSelect.OpenPopup(HUD.Get(), dhs, CancelSpellTargetSelection, SelectSpellDeadHeroTarget);
                    DeadHero deadHero;
                    while (true)
                    {
                        deadHero = MHDataStorage.Get<DeadHero>("DeadHero");
                        if (deadHero != null || !PopupResurrectionSelect.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (deadHero == null)
                    {
                        resolved = true;
                        continue;
                    }
                    TownLocation townLocation = caster.summoningCircle.Get();
                    if (townLocation == null)
                    {
                        resolved = true;
                        continue;
                    }
                    global::MOM.Unit unit = DeadHero.ConvertDeadHeroToUnit(deadHero);
                    if ((bool)ScriptLibrary.Call(spell.worldScript, caster, spell, townLocation, unit))
                    {
                        caster.RemoveFromDeadHeroesList(deadHero);
                        FSMMapGame.CastEffect(townLocation.GetPosition(), spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    Debug.LogError("Resurrection script failed. Zombie unit registered with error!");
                    resolved = true;
                    continue;
                }
                if (enumType == ETargetType.TargetGlobal && spell == (Spell)SPELL.SPELL_BINDING)
                {
                    wizardTargets = GameManager.GetWizards().FindAll((PlayerWizard o) => caster.TargetForSpellValid(null, o, spell));
                    PopupSpellBinding.OpenPopup(null, spell, wizardTargets, caster, SelectEnchTarget, CancelSpellTargetSelection);
                    EnchantmentInstance enchantmentInstance;
                    while (true)
                    {
                        enchantmentInstance = MHDataStorage.Get<EnchantmentInstance>("EnchantmentInstance");
                        if (enchantmentInstance != null || !PopupSpellBinding.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (enchantmentInstance == null)
                    {
                        resolved = true;
                        continue;
                    }
                    this.SpellTargettingOtherWizard(enchantmentInstance.owner?.Get<PlayerWizard>());
                    if ((bool)ScriptLibrary.Call(spell.worldScript, caster, enchantmentInstance, spell))
                    {
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                    this.casting = null;
                    HUD.Get().SetCasting(null);
                    yield break;
                }
                if (enumType == ETargetType.TargetGlobal && spell.showWizardsEnchantments)
                {
                    if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(caster, null), null, spell))
                    {
                        this.InvalidWorldTarget(spell);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    wizardTargets = GameManager.GetWizards().FindAll((PlayerWizard o) => caster.TargetForSpellValid(null, o, spell));
                    PopupDisjunction.OpenPopup(null, spell, wizardTargets, caster, SelectEnchTarget, CancelSpellTargetSelection);
                    EnchantmentInstance enchantmentInstance2;
                    while (true)
                    {
                        enchantmentInstance2 = MHDataStorage.Get<EnchantmentInstance>("EnchantmentInstance");
                        if (enchantmentInstance2 != null || !PopupDisjunction.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                    if (enchantmentInstance2 == null)
                    {
                        resolved = true;
                        continue;
                    }
                    this.SpellTargettingOtherWizard(enchantmentInstance2.owner?.Get<PlayerWizard>());
                    if ((bool)ScriptLibrary.Call(spell.worldScript, caster, enchantmentInstance2, spell))
                    {
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                    }
                    else
                    {
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                    }
                    break;
                }
                if (enumType == ETargetType.TargetGlobal)
                {
                    if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(caster, null), GameManager.Get(), spell))
                    {
                        this.InvalidWorldTarget(spell);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    ScriptLibrary.Call(spell.worldScript, caster, GameManager.Get(), spell);
                    FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                    GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                    break;
                }
            }
            if (this.chosenTarget != Vector3i.invalid)
            {
                this.chosenTarget = World.GetActivePlane().GetPositionWrapping(this.chosenTarget);
                if (enumType == ETargetType.TargetUnit)
                {
                    global::MOM.Group group2 = GameManager.GetGroupsOfPlane(World.GetActivePlane()).Find((global::MOM.Group o) => o.GetPosition() == this.chosenTarget);
                    if (group2 != null)
                    {
                        if ((spell.targetType == (TargetType)TARGET_TYPE.UNIT_FRIENDLY || spell.targetType == (TargetType)TARGET_TYPE.UNIT_FRIENDLY_HERO) && group2.GetOwnerID() != caster.GetWizardOwnerID())
                        {
                            this.chosenTarget = Vector3i.invalid;
                            continue;
                        }
                        PopupCastSelect.OpenPopup(HUD.Get(), group2.GetUnits(), CancelSpellTargetSelection, SelectSpellUnitTarget, spell);
                        MHDataStorage.Set("Unit", null);
                        global::MOM.Unit unit2;
                        while (true)
                        {
                            unit2 = MHDataStorage.Get<global::MOM.Unit>("Unit");
                            if (unit2 != null || !PopupCastSelect.IsOpen())
                            {
                                break;
                            }
                            yield return null;
                        }
                        if (unit2 == null)
                        {
                            this.chosenTarget = Vector3i.invalid;
                            continue;
                        }
                        if (!caster.TargetForSpellValid(null, unit2, spell))
                        {
                            this.chosenTarget = Vector3i.invalid;
                            this.InvalidWorldTarget(spell);
                            continue;
                        }
                        this.SpellTargettingOtherWizard(unit2.GetWizardOwner());
                        ScriptLibrary.Call(spell.worldScript, caster, unit2, spell);
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                }
                if (enumType == ETargetType.TargetGroup)
                {
                    List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(World.GetActivePlane());
                    global::MOM.Group group = groupsOfPlane.Find((global::MOM.Group o) => o.GetPosition() == this.chosenTarget);
                    if (group != null)
                    {
                        if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(GameManager.GetHumanWizard(), null), group, spell))
                        {
                            this.chosenTarget = Vector3i.invalid;
                            this.InvalidWorldTarget(spell);
                            continue;
                        }
                        this.SpellTargettingOtherWizard(group.GetOwnerID());
                        ScriptLibrary.Call(spell.worldScript, GameManager.GetHumanWizard(), group, spell);
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        if (group.GetOwnerID() != GameManager.GetHumanWizard().ID)
                        {
                            group.GetMapFormation(createIfMissing: false)?.UpdateFigureCount();
                            group.GetMapFormation(createIfMissing: false)?.GetHit();
                            if (group != null && group.GetMapFormation(createIfMissing: false) != null)
                            {
                                yield return new WaitForSeconds(1.5f);
                            }
                            while (group != null && !(group.GetMapFormation(createIfMissing: false) == null) && group.GetMapFormation(createIfMissing: false).IsAnimating())
                            {
                                yield return null;
                            }
                            group.UpdateGroupUnits();
                        }
                        break;
                    }
                }
                else if (enumType == ETargetType.TargetLocation)
                {
                    List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(World.GetActivePlane());
                    global::MOM.Location tar = locationsOfThePlane.Find((global::MOM.Location o) => o.GetPosition() == this.chosenTarget);
                    if (tar == null)
                    {
                        bool flag = false;
                        if (World.GetActivePlane() == World.GetArcanus())
                        {
                            locationsOfThePlane = GameManager.GetLocationsOfThePlane(World.GetMyrror());
                        }
                        else
                        {
                            locationsOfThePlane = GameManager.GetLocationsOfThePlane(World.GetArcanus());
                            flag = true;
                        }
                        tar = locationsOfThePlane.Find((global::MOM.Location o) => o.GetPosition() == this.chosenTarget);
                        if (tar != null && caster.IsHuman)
                        {
                            if (flag)
                            {
                                World.ActivatePlane(World.GetArcanus());
                            }
                            else
                            {
                                World.ActivatePlane(World.GetMyrror());
                            }
                        }
                    }
                    if (tar != null)
                    {
                        if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(GameManager.GetHumanWizard(), null), tar, spell))
                        {
                            this.InvalidWorldTarget(spell);
                            this.chosenTarget = Vector3i.invalid;
                            continue;
                        }
                        if (spell == (Spell)SPELL.SPELL_WARD && tar is TownLocation)
                        {
                            PopupSpellWard.OpenPopup(HUD.Get(), CancelSpellTargetSelection, SelectSpellWardTarget);
                            Enchantment enchantment;
                            while (true)
                            {
                                enchantment = MHDataStorage.Get<Enchantment>("Ward");
                                if (enchantment != null || !PopupSpellWard.IsOpen())
                                {
                                    break;
                                }
                                yield return null;
                            }
                            tar.AddEnchantment(enchantment, GameManager.GetHumanWizard(), enchantment.lifeTime, null, spell.worldCost);
                            Enchantment hex = (Enchantment)ENCH.SPELL_WARD_ENFEEBLING_HEX;
                            if (tar.GetEnchantments().Find((EnchantmentInstance o) => o.source == hex) == null)
                            {
                                tar.AddEnchantment(hex, GameManager.GetHumanWizard(), hex.lifeTime);
                            }
                            FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                            GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        }
                        else
                        {
                            this.SpellTargettingOtherWizard(tar.GetOwnerID());
                            ScriptLibrary.Call(spell.worldScript, GameManager.GetHumanWizard(), tar, spell);
                            FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                            GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        }
                        break;
                    }
                }
                else
                {
                    if (enumType == ETargetType.TargetHex)
                    {
                        global::WorldCode.Plane activePlane = World.GetActivePlane();
                        if (!string.IsNullOrEmpty(spell.targetingScript))
                        {
                            Hex hexAt = activePlane.GetHexAt(this.chosenTarget);
                            if (!(bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(GameManager.GetHumanWizard(), null), hexAt, spell))
                            {
                                this.chosenTarget = Vector3i.invalid;
                                this.InvalidWorldTarget(spell);
                                continue;
                            }
                        }
                        foreach (global::MOM.Location item in GameManager.GetLocationsOfThePlane(World.GetActivePlane()))
                        {
                            if (item is TownLocation && item.GetDistanceTo(this.chosenTarget) <= 2)
                            {
                                this.SpellTargettingOtherWizard(item.GetOwnerID());
                                break;
                            }
                        }
                        ScriptLibrary.Call(spell.worldScript, GameManager.GetHumanWizard(), spell, this.chosenTarget, activePlane);
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    if (enumType == ETargetType.WorldHexBattleGlobal)
                    {
                        if (!string.IsNullOrEmpty(spell.targetingScript))
                        {
                            Hex hexAt2 = World.GetActivePlane().GetHexAt(this.chosenTarget);
                            bool flag2 = (bool)ScriptLibrary.Call(spell.targetingScript, new SpellCastData(GameManager.GetHumanWizard(), null), hexAt2, spell);
                            foreach (global::MOM.Location item2 in GameManager.GetLocationsOfThePlane(World.GetActivePlane()))
                            {
                                if (item2 is TownLocation && item2.GetDistanceTo(this.chosenTarget) <= 2)
                                {
                                    this.SpellTargettingOtherWizard(item2.GetOwnerID());
                                    break;
                                }
                            }
                            if (!flag2)
                            {
                                this.chosenTarget = Vector3i.invalid;
                                this.InvalidWorldTarget(spell);
                                continue;
                            }
                        }
                        ScriptLibrary.Call(spell.worldScript, GameManager.GetHumanWizard(), this.chosenTarget, World.GetActivePlane(), spell);
                        FSMMapGame.CastEffect(this.chosenTarget, spell, caster);
                        GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                        break;
                    }
                    Debug.Log("Unsupported spell type");
                }
                this.chosenTarget = Vector3i.invalid;
                this.InvalidWorldTarget(spell);
            }
            yield return null;
        }
        this.casting = null;
        GameManager.GetHumanWizard().RemoveSummaryInfo(s);
        HUD.Get().UpdateNotificationGrid();
        HUD.Get().UpdateCastingButton();
        HUD.Get().SetCasting(null);
        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
    }

    public static void CastEffect(Vector3i pos, Spell spell, PlayerWizard caster)
    {
        if (!string.IsNullOrEmpty(spell.castEffect) && pos != Vector3i.invalid)
        {
            GameObject gameObject = AssetManager.Get<GameObject>(spell.castEffect);
            if (gameObject != null)
            {
                global::WorldCode.Plane activePlane = World.GetActivePlane();
                Chunk chunkFor = activePlane.GetChunkFor(pos);
                GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, chunkFor.go.transform);
                Vector3 vector = HexCoordinates.HexToWorld3D(pos);
                vector.y = activePlane.GetHeightAt(vector);
                gameObject2.transform.localPosition = vector;
            }
        }
        else if (!string.IsNullOrEmpty(spell.additionalGraphic))
        {
            Debug.LogWarning("Missing popup showing general target spells");
            UIManager.Open<PopupGlobalCast>(UIManager.Layer.Popup).Set(spell, caster);
        }
        if (!string.IsNullOrEmpty(spell.audioEffect))
        {
            AudioLibrary.RequestSFX(spell.audioEffect);
        }
    }

    private void SelectSpellUnitTarget(object o)
    {
        Reference<global::MOM.Unit> reference = o as Reference<global::MOM.Unit>;
        if (reference == null)
        {
            Debug.LogError("Received invalid spell target " + o);
        }
        else
        {
            MHDataStorage.Set("Unit", reference.Get());
        }
    }

    public void CancelSpellTargetSelection(object o)
    {
        if (this.casting != null)
        {
            base.StopCoroutine(this.casting);
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            this.casting = null;
            HUD.Get().SetCasting(null);
        }
    }

    private void SelectSpellDeadHeroTarget(object o)
    {
        if (!(o is DeadHero d))
        {
            Debug.LogError("Received invalid spell target " + o);
        }
        else
        {
            MHDataStorage.Set("DeadHero", d);
        }
    }

    private void SelectSpellWardTarget(object o)
    {
        if (!(o is Enchantment d))
        {
            Debug.LogError("received invalid spell target " + o);
        }
        else
        {
            MHDataStorage.Set("Ward", d);
        }
    }

    private void SelectSpellWizardTarget(object o)
    {
        if (!(o is PlayerWizard d))
        {
            Debug.LogError("received invalid spell target " + o);
        }
        else
        {
            MHDataStorage.Set("Wizard", d);
        }
    }

    private void SelectEnchTarget(object o)
    {
        if (!(o is EnchantmentInstance d))
        {
            Debug.LogError("received invalid enchantment target " + o);
        }
        else
        {
            MHDataStorage.Set("EnchantmentInstance", d);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        ProtoLibrary.GetInstance()?.Update();
        if (!UIManager.IsTopForInput(HUD.Get()))
        {
            HUD.Get()?.Hide();
            if (this.casting == null)
            {
                return;
            }
            if (Input.GetMouseButtonUp(1))
            {
                this.CancelSpellTargetSelection(this);
                return;
            }
        }
        else
        {
            HUD.Get()?.Show();
        }
        if (!TurnManager.Get().playerTurn)
        {
            return;
        }
        if (SettingsBlock.IsKeyUp(Settings.KeyActions.UI_CHANGE_PLANE))
        {
            List<global::WorldCode.Plane> planes = World.GetPlanes();
            global::WorldCode.Plane aPlane = World.GetActivePlane();
            global::WorldCode.Plane plane = planes.Find((global::WorldCode.Plane o) => !o.battlePlane && o != aPlane);
            if (plane != null)
            {
                World.ActivatePlane(plane);
            }
        }
        if (this.casting != null)
        {
            if (Input.GetMouseButtonUp(1))
            {
                this.CancelSpellTargetSelection(this);
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!UIManager.IsOverUI())
                {
                    Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                    this.chosenTarget = hexCoordAt;
                }
                return;
            }
        }
        if (Input.GetMouseButtonUp(1) && HUD.Get().tgSurveyor.isOn)
        {
            HUD.SwitchSurveyor();
        }
        else if (Input.GetMouseButtonUp(0) && !UIManager.IsOverUI() && Battle.Get() == null && FSMSelectionManager.Get().roadBuildingMode)
        {
            Vector3i hexCoordAt2 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
            IPlanePosition selectedGroup = this.GetSelectedGroup();
            HUD.SwitchSurveyor();
            if (selectedGroup != null && selectedGroup is global::MOM.Group && (selectedGroup as global::MOM.Group).GetOwnerID() == PlayerWizard.HumanID())
            {
                global::MOM.Group group = selectedGroup as global::MOM.Group;
                if (group.destination != Vector3i.invalid)
                {
                    group.destination = Vector3i.invalid;
                    return;
                }
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(selectedGroup.GetPlane(), selectedGroup.GetPosition(), hexCoordAt2, selectedGroup);
                requestDataV.allowAllyPassMode = false;
                requestDataV.water = false;
                requestDataV.nonCorporeal = false;
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                if (group.engineerManager != null)
                {
                    if (path == null || path.Count < 2)
                    {
                        group.engineerManager.AddRoadPath(null);
                        group.engineerManager.AddRoadNode(group.Position);
                    }
                    else
                    {
                        group.engineerManager.AddRoadPath(path);
                    }
                }
                AudioLibrary.RequestSFX("BuildRoad");
                FSMSelectionManager.SetRoadPathMode(b: false);
            }
        }
        else if (Input.GetMouseButtonUp(1) && !UIManager.IsOverUI() && Battle.Get() == null)
        {
            Vector3i hexCoordAt3 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
            IPlanePosition selectedGroup2 = this.GetSelectedGroup();
            HUD.SwitchSurveyor();
            if (selectedGroup2 != null && selectedGroup2 is global::MOM.Group && (selectedGroup2 as global::MOM.Group).GetOwnerID() == PlayerWizard.HumanID())
            {
                global::MOM.Group group2 = selectedGroup2 as global::MOM.Group;
                if (group2.destination != Vector3i.invalid)
                {
                    group2.destination = Vector3i.invalid;
                    return;
                }
                if (!selectedGroup2.GetPlane().pathfindingArea.IsInside(hexCoordAt3))
                {
                    return;
                }
                Vector3i vector3i = hexCoordAt3;
                Debug.Log("Hex pos " + vector3i.ToString());
                RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(selectedGroup2.GetPlane(), selectedGroup2.GetPosition(), hexCoordAt3, selectedGroup2);
                PathfinderV2.FindPath(requestDataV2);
                List<Vector3i> path2 = requestDataV2.GetPath();
                if (path2 == null || path2.Count < 2)
                {
                    Debug.Log("Path impossible");
                }
                else
                {
                    string text = "";
                    foreach (Vector3i item in path2)
                    {
                        string text2 = text;
                        vector3i = item;
                        text = text2 + "->" + vector3i.ToString();
                    }
                    Debug.Log("Path " + text);
                    global::MOM.Group group3 = selectedGroup2 as global::MOM.Group;
                    if (FSMSelectionManager.Get().roadBuildingMode)
                    {
                        group3.engineerManager.Destroy();
                        FSMSelectionManager.SetRoadPathMode(b: false);
                    }
                    else
                    {
                        List<global::MOM.Unit> selectedUnits = FSMSelectionManager.Get().selectedUnits;
                        if (selectedUnits == null || selectedUnits.Count == 0 || selectedUnits.Count == group3.GetUnits().Count)
                        {
                            group3.MoveViaPath(path2, mergeCollidedAlliedGroups: true);
                            MHEventSystem.TriggerEvent<FSMMapGame>(group3, null);
                        }
                        else
                        {
                            bool flag = true;
                            foreach (global::MOM.Unit item2 in selectedUnits)
                            {
                                if (item2.Mp <= 0)
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                global::MOM.Group group4 = new global::MOM.Group(group3.GetPlane(), group3.GetOwnerID());
                                group4.Position = group3.GetPosition();
                                foreach (global::MOM.Unit item3 in new List<global::MOM.Unit>(selectedUnits))
                                {
                                    group3.RemoveUnit(item3);
                                    group4.AddUnit(item3);
                                }
                                group4.GetMapFormation();
                                group4.ChangeBeforeMovingAway(group3.beforeMovingAway);
                                group4.MoveViaPath(path2, mergeCollidedAlliedGroups: true);
                                if (GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle) && GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure))
                                {
                                    FSMSelectionManager.Get().Select(group4, focus: false);
                                }
                                group3.UpateSearcherPositionData();
                                group3.UpdateMapFormation(createIfMissing: false);
                                MHEventSystem.TriggerEvent<FSMMapGame>(group4, null);
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && !UIManager.IsOverUI() && Battle.Get() == null)
        {
            Vector3i vector3i2 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            if (activePlane != null)
            {
                vector3i2 = activePlane.area.KeepHorizontalInside(vector3i2);
            }
            IPlanePosition selectedGroup3 = this.GetSelectedGroup();
            HUD.SwitchSurveyor();
            if (selectedGroup3 != null && selectedGroup3.GetPosition() == vector3i2 && selectedGroup3 is global::MOM.Group)
            {
                global::MOM.Location locationAt = GameManager.Get().GetLocationAt(vector3i2);
                if (locationAt is TownLocation)
                {
                    selectedGroup3 = null;
                    FSMSelectionManager.Get().Select(locationAt, focus: true);
                    if (locationAt.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        if ((locationAt as TownLocation).IsAnOutpost())
                        {
                            UIManager.GetOrOpen<OutpostScreen>(UIManager.Layer.Standard);
                        }
                        else
                        {
                            UIManager.GetOrOpen<TownScreen>(UIManager.Layer.Standard);
                        }
                    }
                    else if (locationAt.discovered)
                    {
                        UIManager.GetOrOpen<PopupTownPreview>(UIManager.Layer.Standard);
                    }
                    return;
                }
            }
            global::MOM.Group group5 = GameManager.Get().GetGroupAt(vector3i2);
            if (group5 != null && group5.GetOwnerID() != PlayerWizard.HumanID())
            {
                global::MOM.Location locationAt2 = GameManager.Get().GetLocationAt(vector3i2);
                if (locationAt2 != null)
                {
                    if (!(locationAt2 is TownLocation) && locationAt2.discovered && locationAt2.explored)
                    {
                        List<Reference<global::MOM.Unit>> units = group5.GetUnits();
                        if (units.Count > 0)
                        {
                            PopupUnits.OpenPopup(HUD.Get(), units, locationAt2.GetName());
                        }
                    }
                    group5 = null;
                }
            }
            if (group5 != null)
            {
                FSMSelectionManager.Get().Select(group5, focus: false);
            }
            else
            {
                global::MOM.Location locationAt3 = GameManager.Get().GetLocationAt(vector3i2);
                if (locationAt3 is TownLocation)
                {
                    selectedGroup3 = null;
                    FSMSelectionManager.Get().Select(locationAt3, focus: true);
                    if (locationAt3.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        if ((locationAt3 as TownLocation).IsAnOutpost())
                        {
                            UIManager.GetOrOpen<OutpostScreen>(UIManager.Layer.Standard);
                        }
                        else
                        {
                            UIManager.GetOrOpen<TownScreen>(UIManager.Layer.Standard);
                        }
                    }
                    else if (locationAt3.discovered)
                    {
                        UIManager.GetOrOpen<PopupTownPreview>(UIManager.Layer.Standard);
                    }
                    return;
                }
                FSMSelectionManager.Get().Select(null, focus: false);
            }
        }
        if (Input.GetKeyUp(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && UIManager.GetScreen<DevMenu>(UIManager.Layer.Standard) == null)
        {
            UIManager.Open<DevMenu>(UIManager.Layer.Standard);
        }
    }

    public void InitializeRandomBattle(Subrace unit = null, int oponentID = 0)
    {
        global::MOM.Group group = new global::MOM.Group(World.GetActivePlane(), 1);
        group.Position = GameManager.GetWizard(1).summoningCircle.Get().Position;
        if (unit == null)
        {
            foreach (Subrace item in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
            {
                group.AddUnit(global::MOM.Unit.CreateFrom(item));
            }
        }
        else
        {
            group.AddUnit(global::MOM.Unit.CreateFrom(unit));
        }
        global::MOM.Group group2 = new global::MOM.Group(World.GetActivePlane(), 2);
        global::WorldCode.Plane p = World.GetActivePlane();
        List<Vector3i> list = new List<Vector3i>(p.GetLandHexes());
        if (Input.GetKey(KeyCode.T))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Tundra);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Swamp);
        }
        else if (Input.GetKey(KeyCode.G))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.GrassLand);
        }
        else if (Input.GetKey(KeyCode.H))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Hill);
        }
        else if (Input.GetKey(KeyCode.M))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Mountain);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Desert);
        }
        else if (Input.GetKey(KeyCode.F))
        {
            list = list.FindAll((Vector3i o) => p.GetHexAt(o).GetTerrain().terrainType == ETerrainType.Forest);
        }
        if (list.Count < 1)
        {
            list = new List<Vector3i>(p.GetLandHexes());
        }
        group2.Position = list[global::UnityEngine.Random.Range(0, list.Count)];
        if (unit == null)
        {
            foreach (Subrace item2 in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
            {
                group2.AddUnit(global::MOM.Unit.CreateFrom(item2));
            }
        }
        else
        {
            group2.AddUnit(global::MOM.Unit.CreateFrom(unit));
        }
        Battle battle = new Battle(group.GetUnits(), group2.GetUnits(), PlayerWizard.HumanID(), oponentID);
        battle.DebugMode(value: true);
        battle.landBattle = true;
        battle.temperature = 0.5f;
        battle.humidity = 0.5f;
        battle.forest = 0.38f;
        FSMCoreGame.Get().StartBattle(battle);
    }

    public void InitializeRandomBattle(Subrace unit = null, int oponentID = 0, bool ownCapitolBattle = false)
    {
        global::MOM.Group group = new global::MOM.Group(World.GetActivePlane(), 1);
        if (ownCapitolBattle)
        {
            TownLocation townLocation = GameManager.GetWizard(1).summoningCircle.Get();
            List<Reference<global::MOM.Unit>> units = townLocation.GetUnits();
            if (units.Count > 0)
            {
                group = units[0].Get().group;
            }
            if (FSMSelectionManager.Get() != null && FSMSelectionManager.Get().GetSelectedGroup() is global::MOM.Group group2 && group2.beforeMovingAway == townLocation)
            {
                group = group2;
            }
        }
        if (group.GetUnits().Count < 1)
        {
            if (unit == null)
            {
                group.Position = GameManager.GetWizard(1).summoningCircle.Get().Position;
                foreach (Subrace item in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
                {
                    group.AddUnit(global::MOM.Unit.CreateFrom(item));
                }
            }
            else
            {
                group.AddUnit(global::MOM.Unit.CreateFrom(unit));
            }
        }
        global::MOM.Group group3 = new global::MOM.Group(World.GetActivePlane(), 2);
        World.GetActivePlane();
        if (unit == null)
        {
            foreach (Subrace item2 in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
            {
                group3.AddUnit(global::MOM.Unit.CreateFrom(item2));
            }
        }
        else
        {
            group3.AddUnit(global::MOM.Unit.CreateFrom(unit));
        }
        Battle battle = new Battle(group3.GetUnits(), group.GetUnits(), oponentID, PlayerWizard.HumanID());
        battle.DebugMode(value: true);
        battle.landBattle = true;
        battle.temperature = 0.5f;
        battle.humidity = 0.5f;
        battle.forest = 0.38f;
        FSMCoreGame.Get().StartBattle(battle);
    }

    public void InitializeRandomBattle(Subrace unit1, int numUnit1, Subrace unit2, int numUnit2, bool waterBattle = false)
    {
        global::MOM.Group group = new global::MOM.Group(World.GetActivePlane(), 1);
        group.Position = GameManager.GetWizard(1).summoningCircle.Get().Position;
        if (unit1 == null)
        {
            foreach (Subrace item in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
            {
                group.AddUnit(global::MOM.Unit.CreateFrom(item));
            }
        }
        else if (numUnit1 < 1)
        {
            group.AddUnit(global::MOM.Unit.CreateFrom(unit1));
        }
        else
        {
            for (int i = 0; i < numUnit1; i++)
            {
                group.AddUnit(global::MOM.Unit.CreateFrom(unit1));
            }
        }
        global::MOM.Group group2 = new global::MOM.Group(World.GetActivePlane(), 2);
        group2.Position = GameManager.GetWizard(2).summoningCircle.Get().Position;
        if (unit2 == null)
        {
            foreach (Subrace item2 in ScriptLibrary.Call("GeneralGroup", 2500) as List<Subrace>)
            {
                group2.AddUnit(global::MOM.Unit.CreateFrom(item2));
            }
        }
        else if (numUnit2 < 1)
        {
            group2.AddUnit(global::MOM.Unit.CreateFrom(unit2));
        }
        else
        {
            for (int j = 0; j < numUnit2; j++)
            {
                group2.AddUnit(global::MOM.Unit.CreateFrom(unit2));
            }
        }
        Battle battle = new Battle(group.GetUnits(), group2.GetUnits(), PlayerWizard.HumanID(), 0);
        battle.DebugMode(value: true);
        battle.landBattle = !waterBattle;
        battle.temperature = 0.5f;
        battle.humidity = 0.5f;
        battle.forest = 0.38f;
        FSMCoreGame.Get().StartBattle(battle);
    }

    public void InitializeTestBattle()
    {
        global::MOM.Group group = new global::MOM.Group(World.GetActivePlane(), 1);
        Subrace source = (Subrace)UNIT.BARB_SPEARMEN;
        Subrace source2 = (Subrace)UNIT.SOR_DJINN;
        Subrace source3 = (Subrace)UNIT.BARB_CAVALRY;
        group.AddUnit(global::MOM.Unit.CreateFrom(source2));
        for (int i = 0; i < 2; i++)
        {
            group.AddUnit(global::MOM.Unit.CreateFrom(source3));
        }
        global::MOM.Group group2 = new global::MOM.Group(World.GetActivePlane(), 0);
        for (int j = 0; j < 2; j++)
        {
            group2.AddUnit(global::MOM.Unit.CreateFrom(source));
        }
        Battle battle = new Battle(group.GetUnits(), group2.GetUnits(), PlayerWizard.HumanID(), 0);
        battle.DebugMode(value: true);
        battle.landBattle = true;
        battle.temperature = 0.5f;
        battle.humidity = 0.5f;
        battle.forest = 0.38f;
        FSMCoreGame.Get().StartBattle(battle);
    }

    public bool PickKeepOrRaze(int attackerID, TownLocation town, global::MOM.Group group)
    {
        if (town.IsAnOutpost())
        {
            if (attackerID == PlayerWizard.HumanID())
            {
                PopupGeneral.OpenPopup(HUD.Get(), town.GetName(), "UI_OUTPOST_CONQUERED", "UI_OKAY");
            }
            town.Raze(attackerID);
            return true;
        }
        if (attackerID == PlayerWizard.HumanID())
        {
            FsmEventTarget fsmEventTarget = new FsmEventTarget();
            fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
            base.Fsm.Event(fsmEventTarget, "PopupTownCaptured");
        }
        else
        {
            if (attackerID == 0 && (group == null || group.aiNeturalExpedition == null || group.aiNeturalExpedition.rampage))
            {
                if (global::UnityEngine.Random.Range(0f, 1f) < 0.5f)
                {
                    PlayerWizard wizardOwner = town.GetWizardOwner();
                    Vector3i position = town.GetPosition();
                    global::WorldCode.Plane plane = town.GetPlane();
                    town.Raze(attackerID);
                    if (wizardOwner.IsHuman)
                    {
                        HUD.Get()?.Update();
                    }
                    List<global::DBDef.Location> type = DataBase.GetType<global::DBDef.Location>();
                    type = type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.Ruins);
                    if (type.Count < 1)
                    {
                        return false;
                    }
                    int index = global::UnityEngine.Random.Range(0, type.Count);
                    global::MOM.Location location = global::MOM.Location.CreateLocation(position, plane, type[index], 0, skipMarker: true, spawnDefenders: false);
                    group.TransferUnits(location.GetLocalGroup());
                    if (group.GetUnits().Count > 0)
                    {
                        location.guardianRace = group.GetUnits()[0].Get().race;
                    }
                    else
                    {
                        List<Race> type2 = DataBase.GetType<Race>();
                        location.guardianRace = type2[global::UnityEngine.Random.Range(0, type2.Count)];
                    }
                    if (group != null && !group.IsHosted())
                    {
                        group.Destroy();
                    }
                }
                else
                {
                    town.SetOwnerID(attackerID, -1, collateralDamage: true);
                    if (group != null)
                    {
                        town.AddUnitsIfPossible(group.GetUnits());
                    }
                }
                return false;
            }
            town.Conquer();
        }
        return false;
    }

    public void InvalidWorldTarget(Spell spell)
    {
        PopupGeneral.OpenPopup(HUD.Get(), "UI_NO_VALID_TARGET", "UI_SPELL_NO_VALID_TARGET", "UI_OK");
    }

    public void InvalidWorldTarget(Spell spell, string header, string message, string confirmation)
    {
        if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(confirmation))
        {
            Debug.LogWarning("InvalidWorldTarget PopupGeneral.OpenPopup do not have all valid parameters.");
        }
        PopupGeneral.OpenPopup(HUD.Get(), header, message, confirmation);
    }

    public bool IsCasting()
    {
        return this.casting != null;
    }

    public void SetChosenTarget(Vector3i target)
    {
        this.chosenTarget = target;
    }

    public bool PickSumon(PlayerWizard w, Spell spell, List<Reference<global::MOM.Unit>> prevUnits)
    {
        List<Hero> list = new List<Hero>(DataBase.GetType<Hero>());
        if (spell == (Spell)SPELL.SUMMON_HERO)
        {
            list = list.FindAll((Hero o) => !o.champion && !global::MOM.Unit.HeroInUseByWizard(o, w.GetID()) && o.GetTag(TAG.EVENT_ONLY_UNIT) == 0);
        }
        else if (spell == (Spell)SPELL.SUMMON_CHAMPION)
        {
            list = list.FindAll((Hero o) => o.champion && !o.unresurrectable && !global::MOM.Unit.HeroInUseByWizard(o, w.GetID()) && o.GetTag(TAG.EVENT_ONLY_UNIT) == 0);
            if (w.GetAttFinal((Tag)TAG.LIFE_MAGIC_BOOK) <= 0)
            {
                if (list.Find((Hero o) => o == (Hero)(Enum)HERO.ELANA) != null)
                {
                    list.Remove((Hero)(Enum)HERO.ELANA);
                }
                if (list.Find((Hero o) => o == (Hero)(Enum)HERO.ROLAND) != null)
                {
                    list.Remove((Hero)(Enum)HERO.ROLAND);
                }
            }
            if (w.GetAttFinal((Tag)TAG.DEATH_MAGIC_BOOK) <= 0)
            {
                if (list.Find((Hero o) => o == (Hero)(Enum)HERO.MORTU) != null)
                {
                    list.Remove((Hero)(Enum)HERO.MORTU);
                }
                if (list.Find((Hero o) => o == (Hero)(Enum)HERO.RAVASHACK) != null)
                {
                    list.Remove((Hero)(Enum)HERO.RAVASHACK);
                }
            }
        }
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("HeroesList is empty or null");
            if (w.IsHuman)
            {
                PopupGeneral.OpenPopup(null, "UI_HERO_SUMMONING_FAILED", "UI_HERO_NOT_AVAILABLE", "UI_OK");
            }
            return false;
        }
        List<UnitOffer> list2 = new List<UnitOffer>();
        int num = 0;
        MHRandom mHRandom = new MHRandom();
        int num2 = 0;
        if (list.Count > 0)
        {
            Hero heroUnit;
            while (num < w.GetOfferHeroCount())
            {
                num2 = mHRandom.GetInt(0, list.Count);
                heroUnit = list[num2];
                if (!global::MOM.Unit.HeroInUseByWizard(heroUnit, w.GetID()) && list2.Find((UnitOffer o) => o.unit.dbSource.Get() == heroUnit) == null)
                {
                    global::MOM.Unit unit = global::MOM.Unit.CreateFrom(heroUnit);
                    w.ModifyUnitSkillsByTraits(unit);
                    list2.Add(new UnitOffer
                    {
                        unit = unit,
                        quantity = 1
                    });
                    list.Remove(heroUnit);
                    num++;
                    if (list.Count == 0)
                    {
                        break;
                    }
                }
            }
            base.StartCoroutine(this.PickAUnitToSummon(list2, w, spell, prevUnits));
            return true;
        }
        return false;
    }

    private IEnumerator PickAUnitToSummon(List<UnitOffer> unitOffer, PlayerWizard w, Spell spell, List<Reference<global::MOM.Unit>> prevUnits)
    {
        global::MOM.Unit picked = null;
        if (w.IsHuman)
        {
            UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Standard);
            unitInfo.SetBuyInfo(unitOffer, delegate(object o)
            {
                if (o is UnitOffer unitOffer2)
                {
                    picked = unitOffer2.unit;
                }
            }, spellSummon: true);
            while (UIManager.IsOpen(UIManager.Layer.Standard, unitInfo))
            {
                yield return null;
            }
        }
        else
        {
            int num = 0;
            foreach (UnitOffer item in unitOffer)
            {
                int unitStrength = BaseUnit.GetUnitStrength(item.unit.dbSource);
                if (unitStrength > num)
                {
                    picked = item.unit;
                    num = unitStrength;
                }
            }
        }
        if (picked == null)
        {
            yield break;
        }
        ScriptLibrary.Call(spell.worldScript, w, spell, w.summoningCircle.Get(), picked);
        foreach (UnitOffer item2 in unitOffer)
        {
            global::MOM.Unit unit = item2.unit;
            if (unit != null && unit.group == null)
            {
                unit.Destroy();
            }
        }
        if (!w.IsHuman)
        {
            yield break;
        }
        List<Reference<global::MOM.Unit>> units = w.summoningCircle.Get().GetUnits();
        global::MOM.Unit unit2 = null;
        foreach (Reference<global::MOM.Unit> item3 in units)
        {
            if (!prevUnits.Contains(item3))
            {
                unit2 = item3;
                break;
            }
        }
        if (unit2 != null)
        {
            w.AddNotification(new SummaryInfo
            {
                summaryType = SummaryInfo.SummaryType.eUnitSummoned,
                unit = unit2,
                graphic = unit2.GetDescriptionInfo().graphic
            });
        }
    }
}
