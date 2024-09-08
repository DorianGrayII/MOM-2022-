// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMBattleTurn
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMBattleTurn : FSMStateBase
{
    public bool isAttackerTurn;

    private Battle battle;

    private Coroutine messageAnimation;

    private bool endTurn;

    private Coroutine aiActivity;

    private static Coroutine casting;

    private Coroutine attacking;

    private Vector3i chosenTarget;

    private Vector3i potentialTarget;

    private bool castingStopLimiter;

    public static FSMBattleTurn instance;

    private Coroutine updateVortexes;

    private Coroutine turnAutomation;

    private Coroutine switchAIControlMode;

    private bool switchToAIMode;

    private bool busy;

    public override void OnEnter()
    {
        FSMBattleTurn.instance = this;
        this.busy = true;
        base.OnEnter();
        this.turnAutomation = base.StartCoroutine(this.TurnAutomationCoroutine());
        if (this.battle == null)
        {
            return;
        }
        if (this.isAttackerTurn)
        {
            this.battle.attacker.spellCasted = false;
            {
                foreach (BattleUnit attackerUnit in this.battle.attackerUnits)
                {
                    attackerUnit.spellCasted = false;
                }
                return;
            }
        }
        this.battle.defender.spellCasted = false;
        foreach (BattleUnit defenderUnit in this.battle.defenderUnits)
        {
            defenderUnit.spellCasted = false;
        }
    }

    public override void OnExit()
    {
        if (FSMBattleTurn.casting != null)
        {
            base.StopCoroutine(FSMBattleTurn.casting);
            BattleHUD.Get().SetCasting(null);
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            FSMBattleTurn.casting = null;
        }
        base.OnExit();
    }

    private IEnumerator TurnAutomationCoroutine()
    {
        this.endTurn = false;
        this.battle = Battle.GetBattle();
        if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
        {
            yield break;
        }
        this.battle.activeTurn = this;
        BattleHUD.Get().UIUpdateFor(this.isAttackerTurn);
        if (!this.isAttackerTurn)
        {
            this.battle.turn++;
            if (this.battle.turn == 1)
            {
                this.TriggerEvent(EEnchantmentType.BattleStartEffect, ESkillType.BattleStartEffect);
                this.NewTurn(this.battle.attackerUnits);
                this.NewTurn(this.battle.defenderUnits);
                yield return new WaitForSeconds(3f);
                if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
                {
                    yield break;
                }
            }
            this.TriggerEventForSide(attacker: false, includeBattle: true, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
            this.NewTurnHudMessage();
            BattleHUD.Get().attackerInfo.DisableAllHighlights();
            BattleHUD.Get().SelectUnit(this.battle.defenderUnits.Find((BattleUnit o) => o.IsAlive()), attacker: false, focus: true);
        }
        else
        {
            this.TriggerEventForSide(attacker: true, includeBattle: false, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
            this.NewTurnHudMessage();
            BattleHUD.Get().defenderInfo.DisableAllHighlights();
            BattleHUD.Get().SelectUnit(this.battle.attackerUnits.Find((BattleUnit o) => o.IsAlive()), attacker: true, focus: true);
        }
        yield return this.UpdateVortexes(this.isAttackerTurn);
        yield return this.UpdateConfusedUnits(this.isAttackerTurn);
        if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
        {
            yield break;
        }
        if (this.isAttackerTurn)
        {
            this.busy = false;
            if (this.battle.attacker.autoPlayByAI)
            {
                yield return this.AITurn(attacker: true);
                if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
                {
                    yield break;
                }
            }
        }
        else
        {
            yield return this.BattleWizardTowerBolts();
            if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
            {
                yield break;
            }
            this.busy = false;
            if (this.battle.defender.autoPlayByAI)
            {
                yield return this.AITurn(attacker: false);
                if (BattleHUD.Get() == null || this.battle == null || this.battle.battleEnd)
                {
                    yield break;
                }
            }
        }
        BattleHUD.Get().UpdateGeneralInfo();
        MHEventSystem.RegisterListener<BattleHUD>(HUDEvent, this);
        this.turnAutomation = null;
    }

    private IEnumerator BattleWizardTowerBolts()
    {
        this.battle = Battle.GetBattle();
        if (this.battle.battleEnd || this.battle.wizardTower == null)
        {
            yield break;
        }
        List<EnchantmentInstance> enchantmentsOfType = this.battle.GetEnchantmentManager().GetEnchantmentsOfType(EEnchantmentType.BattleWizardTowerEffect);
        if (enchantmentsOfType == null)
        {
            yield break;
        }
        foreach (EnchantmentInstance v in enchantmentsOfType)
        {
            List<BattleUnit> list = this.battle.attackerUnits.FindAll((BattleUnit o) => o.IsAlive());
            if (list.Count == 0)
            {
                break;
            }
            BattleUnit target = list[Random.Range(0, list.Count)];
            if (this.battle.battleEnd)
            {
                break;
            }
            yield return this.TowerBolt(this.battle, v, target);
            if (this.battle.battleEnd)
            {
                break;
            }
            if (v.source.Get().scripts != null)
            {
                EnchantmentScript[] scripts = v.source.Get().scripts;
                foreach (EnchantmentScript enchantmentScript in scripts)
                {
                    ScriptLibrary.Call(enchantmentScript.script, this.battle, enchantmentScript, v, target);
                    BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_FORTRESS_BOLT", true, target.GetName(), WizardColors.GetHex(target.GetWizardOwner()), v.source.Get().GetDILocalizedName()));
                }
                target.GetOrCreateFormation().UpdateFigureCount();
            }
        }
    }

    private IEnumerator TowerBolt(Battle battle, EnchantmentInstance ei, BattleUnit target)
    {
        FortressRefs component = battle.wizardTower.GetComponent<FortressRefs>();
        string hitEffect = "";
        GameObject onHit = null;
        GameObject source = null;
        if (ei.source.Get() == (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_CHAOS)
        {
            onHit = component.onHitEffectChaos;
            source = component.projectileChaos;
            AudioLibrary.RequestSFX("FortressBoltChaos");
            hitEffect = "FortressBoltChaosHit";
        }
        else if (ei.source.Get() == (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_DEATH)
        {
            onHit = component.onHitEffectDeath;
            source = component.projectileDeath;
            AudioLibrary.RequestSFX("FortressBoltDeath");
            hitEffect = "FortressBoltDeathHit";
        }
        else if (ei.source.Get() == (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_LIFE)
        {
            onHit = component.onHitEffectLife;
            source = component.projectileLife;
            AudioLibrary.RequestSFX("FortressBoltLife");
            hitEffect = "FortressBoltLifeHit";
        }
        else if (ei.source.Get() == (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_NATURE)
        {
            onHit = component.onHitEffectNature;
            source = component.projectileNature;
            AudioLibrary.RequestSFX("FortressBoltNature");
            hitEffect = "FortressBoltNatureHit";
        }
        else if (ei.source.Get() == (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_SORCERY)
        {
            onHit = component.onHitEffectSorcery;
            source = component.projectileSorcery;
            AudioLibrary.RequestSFX("FortressBoltSorcery");
            hitEffect = "FortressBoltSorceryHit";
        }
        Vector3 start = component.location.position;
        Vector3 destination = HexCoordinates.HexToWorld3D(target.GetPosition());
        destination.y = battle.plane.GetHeightAt(destination) + 0.5f;
        Chunk chunk = battle.plane.GetChunkFor(target.GetPosition());
        GameObject p2 = GameObjectUtils.Instantiate(source, chunk.go.transform);
        p2.transform.position = start;
        Vector3 delta = destination - start;
        float progress = 0f;
        float flightTime = 1.5f;
        while (progress < 1f)
        {
            yield return null;
            if (battle == null || battle.battleEnd)
            {
                yield break;
            }
            progress += Time.deltaTime / flightTime;
            if (progress > 1f)
            {
                progress = 1f;
            }
            Vector3 position = start + delta * progress;
            p2.transform.position = position;
        }
        Object.Destroy(p2);
        p2 = GameObjectUtils.Instantiate(onHit, chunk.go.transform);
        p2.transform.position = destination;
        AudioLibrary.RequestSFX(hitEffect);
    }

    private IEnumerator UpdateVortexes(bool attacker)
    {
        if (this.battle.vortexList != null)
        {
            for (int i = this.battle.vortexList.Count - 1; i >= 0; i--)
            {
                Vortex vortex = this.battle.vortexList[i];
                if (vortex.attacker == attacker)
                {
                    yield return vortex.Update(this.battle);
                }
            }
        }
        this.updateVortexes = null;
    }

    private IEnumerator UpdateConfusedUnits(bool attacker)
    {
        if (this.battle.confusedList == null)
        {
            yield break;
        }
        for (int i = this.battle.confusedList.Count - 1; i >= 0; i--)
        {
            BattleUnit u = this.battle.confusedList[i];
            if (u.IsAlive() && u.attackingSide == attacker)
            {
                int mp = u.Mp.ToInt();
                for (int j = 0; j < mp; j++)
                {
                    int num = Random.Range(0, 6);
                    Vector3i vector3i = u.battlePosition + HexNeighbors.neighbours[num];
                    if (!this.battle.plane.area.IsInside(vector3i))
                    {
                        Random.Range(0, 6);
                        continue;
                    }
                    Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(HexCoordinates.HexToWorld3D(vector3i));
                    bool flag = this.battle.GetUnitAt(hexCoordAt) == null;
                    if (this.battle.plane.exclusionPoints != null)
                    {
                        flag = !this.battle.plane.exclusionPoints.Contains(hexCoordAt) && flag;
                    }
                    if (flag)
                    {
                        u.MoveAnimatedTo(hexCoordAt, this.battle);
                        yield return this.battle.WaitForAttention();
                    }
                }
                this.battle.confusedList.Remove(u);
                u.Mp = FInt.ZERO;
            }
        }
    }

    public void StartAI(bool attacker)
    {
        if (this.aiActivity == null && !this.endTurn)
        {
            this.aiActivity = base.StartCoroutine(this.AITurn(attacker));
        }
    }

    public void StopAI()
    {
    }

    private IEnumerator AITurn(bool attacker)
    {
        while (this.updateVortexes != null)
        {
            yield return null;
        }
        yield return this.battle.WaitForAttention();
        Debug.Log("AI turn " + this.battle.turn + " start");
        MHTimer t = MHTimer.StartNew();
        yield return ScriptLibrary.Call("AITurnV02", this.battle, attacker);
        Debug.Log("AI turn " + this.battle.turn + " took " + t.GetTime());
        this.aiActivity = null;
    }

    public void AITurnEnd()
    {
        this.endTurn = true;
    }

    public static bool IsCastingSpells()
    {
        return FSMBattleTurn.casting != null;
    }

    internal void StartCasting(Spell spell, ISpellCaster spellCaster)
    {
        CursorsLibrary.SetMode(CursorsLibrary.Mode.CastSpell);
        BattleHUD.Get().SetCasting(spell);
        FSMBattleTurn.casting = base.StartCoroutine(this.CastingSpell(spell, spellCaster));
        this.castingStopLimiter = true;
    }

    private IEnumerator CastingSpell(Spell spell, ISpellCaster sc)
    {
        if ((bool)ScriptLibrary.Call("CounterMagicBattle", this.battle, spell, sc))
        {
            BattleHUD.Get().SetCasting(null);
            FSMBattleTurn.casting = null;
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            Battle.GetBattle()?.ResistedSpell(Vector3i.invalid, counterMagickedSpell: true);
            if (sc is PlayerWizard)
            {
                ((this.battle.attacker.wizard == sc) ? this.battle.attacker : this.battle.defender).UseResourcesFor(spell);
            }
            else if (sc is BattleUnit)
            {
                (sc as BattleUnit).UseManaFor(spell);
            }
            BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_SPELL_COUNTERED", true, spell.GetDescriptionInfo().GetLocalizedName()));
            if (sc is BattlePlayer battlePlayer)
            {
                battlePlayer.spellCasted = true;
            }
            if (sc is PlayerWizard w)
            {
                this.battle.GetBattlePlayerForWizard(w).spellCasted = true;
            }
            yield break;
        }
        List<BattleUnit> sourceCopy = null;
        List<BattleUnit> currentCopy = null;
        if (sc is BattleUnit)
        {
            VerticalMarkerManager.Get().UpdateSpelcasterIcon(sc, active: true);
        }
        else if (this.battle.buToSource != null)
        {
            currentCopy = new List<BattleUnit>(this.battle.buToSource.Keys);
            sourceCopy = Serializer.DeepClone(currentCopy);
        }
        this.chosenTarget = Vector3i.invalid;
        ETargetType spellTargetEnum = spell.targetType.enumType;
        bool castingSuccesful = false;
        Debug.Log("CastingSpell " + spell.dbName);
        SpellCastData castData = new SpellCastData(sc, this.battle);
        if (spellTargetEnum == ETargetType.TargetWizard)
        {
            BattlePlayer battlePlayer2 = this.battle.attacker;
            bool flag = false;
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battlePlayer2, spell))
            {
                flag = true;
                Debug.Log(spell.battleScript);
                Battle.CastBattleSpell(spell, castData, battlePlayer2);
            }
            else
            {
                battlePlayer2 = this.battle.defender;
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battlePlayer2, spell))
                {
                    flag = true;
                    Debug.Log(spell.battleScript);
                    BattleHUD.CombatLogSpell(sc, spell, null);
                    Battle.CastBattleSpell(spell, castData, battlePlayer2);
                    BattleHUD.CombatLogSpellAddEffect();
                }
            }
            if (flag)
            {
                this.CastEffect(Vector3i.invalid, spell, sc);
                castingSuccesful = true;
            }
            else if (this.AreSameEnchsOnTarget(spell.enchantmentData, battlePlayer2.GetEnchantments()))
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_ALREADY_ACTIVE");
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_WIZARD");
            }
        }
        else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.RAISE_DEAD)
        {
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.battle, spell))
            {
                MHDataStorage.Set("Unit", null);
                List<BattleUnit> list = new List<BattleUnit>(sc.IsAttackerInBattle(this.battle) ? this.battle.attackerUnits : this.battle.defenderUnits);
                list = list.FindAll((BattleUnit o) => !o.IsAlive() && !o.dbSource.Get().unresurrectable && o.GetAttributes().DoesNotContains((Tag)TAG.FANTASTIC_CLASS));
                foreach (KeyValuePair<BattleUnit, global::MOM.Unit> item in this.battle.buToSource)
                {
                    for (int num = list.Count - 1; num >= 0; num--)
                    {
                        if (item.Key.IsAlive() && item.Key.GetPosition() == list[num].GetPosition())
                        {
                            list.RemoveAt(num);
                        }
                    }
                }
                BattleUnit battleUnit = null;
                if (sc.GetWizardOwner() == GameManager.GetHumanWizard())
                {
                    PopupCastSelect.OpenPopup(HUD.Get(), list, CancelSpellTargetSelection, SelectSpellUnitTarget);
                    MHDataStorage.Set("Unit", null);
                    while (true)
                    {
                        battleUnit = MHDataStorage.Get<BattleUnit>("Unit");
                        if (battleUnit != null || !PopupCastSelect.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else
                {
                    int num2 = 0;
                    foreach (BattleUnit item2 in list)
                    {
                        if (num2 < item2.GetBattleUnitValue())
                        {
                            battleUnit = item2;
                            num2 = item2.GetBattleUnitValue();
                        }
                    }
                }
                Debug.Log(spell.battleScript);
                BattleHUD.CombatLogSpell(sc, spell, null);
                Battle.CastBattleSpell(spell, castData, battleUnit);
                this.CastEffect(battleUnit.GetPosition(), spell, sc);
                BattleHUD.CombatLogSpellAddEffect();
                castingSuccesful = true;
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_RAISE_DEAD");
            }
        }
        else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.ANIMATE_DEAD)
        {
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.battle, spell))
            {
                MHDataStorage.Set("Unit", null);
                List<BattleUnit> list2 = new List<BattleUnit>();
                foreach (BattleUnit item3 in ListUtils.MultiEnumerable(this.battle.attackerUnits, this.battle.defenderUnits))
                {
                    int num3 = item3.GetBaseFigure().maxHitPoints * item3.maxCount;
                    if (item3.IsAlive() || item3.dbSource.Get() is Hero || !(item3.race != (Race)RACE.REALM_DEATH) || item3.summon || item3.irreversibleDamages >= num3 / 2 || (sc.GetWizardOwner() != item3.GetWizardOwner() && item3.GetAttributes().Contains(TAG.MAGIC_IMMUNITY)))
                    {
                        continue;
                    }
                    bool flag2 = false;
                    foreach (KeyValuePair<BattleUnit, global::MOM.Unit> item4 in this.battle.buToSource)
                    {
                        if (item4.Key.IsAlive() && item4.Key.GetPosition() == item3.GetPosition())
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        list2.Add(item3);
                    }
                }
                BattleUnit battleUnit2 = null;
                if (sc.GetWizardOwner() == GameManager.GetHumanWizard())
                {
                    PopupCastSelect.OpenPopup(HUD.Get(), list2, CancelSpellTargetSelection, SelectSpellUnitTarget);
                    MHDataStorage.Set("Unit", null);
                    while (true)
                    {
                        battleUnit2 = MHDataStorage.Get<BattleUnit>("Unit");
                        if (battleUnit2 != null || !PopupCastSelect.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else
                {
                    int num4 = 0;
                    foreach (BattleUnit item5 in list2)
                    {
                        if (num4 < item5.GetBattleUnitValue())
                        {
                            battleUnit2 = item5;
                            num4 = item5.GetBattleUnitValue();
                        }
                    }
                }
                Debug.Log(spell.battleScript);
                BattleHUD.CombatLogSpell(sc, spell, null);
                Battle.CastBattleSpell(spell, castData, battleUnit2);
                this.CastEffect(battleUnit2.GetPosition(), spell, sc);
                BattleHUD.CombatLogSpellAddEffect();
                castingSuccesful = true;
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_ANIMATE_DEAD");
            }
        }
        else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.RECONSTRUCT)
        {
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.battle, spell))
            {
                MHDataStorage.Set("Unit", null);
                List<BattleUnit> list3 = new List<BattleUnit>();
                PlayerWizard wizardOwner = sc.GetWizardOwner();
                List<BattleUnit> list4 = ((this.battle.attacker.GetID() != wizardOwner.ID) ? this.battle.defenderUnits : this.battle.attackerUnits);
                foreach (BattleUnit item6 in list4)
                {
                    int num5 = item6.GetBaseFigure().maxHitPoints * item6.maxCount;
                    if (!item6.IsAlive() && !(item6.dbSource.Get() is Hero) && item6.GetAttFinal(TAG.MECHANICAL_UNIT) > FInt.ZERO && this.battle.buToSource[item6].group != null && item6.irreversibleDamages < num5 / 2)
                    {
                        list3.Add(item6);
                    }
                }
                BattleUnit battleUnit3 = null;
                if (wizardOwner == GameManager.GetHumanWizard())
                {
                    PopupCastSelect.OpenPopup(HUD.Get(), list3, CancelSpellTargetSelection, SelectSpellUnitTarget);
                    MHDataStorage.Set("Unit", null);
                    while (true)
                    {
                        battleUnit3 = MHDataStorage.Get<BattleUnit>("Unit");
                        if (battleUnit3 != null || !PopupCastSelect.IsOpen())
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                else
                {
                    int num6 = 0;
                    foreach (BattleUnit item7 in list3)
                    {
                        if (num6 < item7.GetBattleUnitValue())
                        {
                            battleUnit3 = item7;
                            num6 = item7.GetBattleUnitValue();
                        }
                    }
                }
                Debug.Log(spell.battleScript);
                BattleHUD.CombatLogSpell(sc, spell, null);
                Battle.CastBattleSpell(spell, castData, battleUnit3);
                this.CastEffect(battleUnit3.GetPosition(), spell, sc);
                BattleHUD.CombatLogSpellAddEffect();
                castingSuccesful = true;
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_ANIMATE_DEAD");
            }
        }
        else if (spellTargetEnum == ETargetType.TargetGlobal)
        {
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.battle, spell))
            {
                Debug.Log(spell.battleScript);
                BattleHUD.CombatLogSpell(sc, spell, null);
                Battle.CastBattleSpell(spell, castData, this.battle);
                this.CastEffect(Vector3i.invalid, spell, sc);
                BattleHUD.CombatLogSpellAddEffect();
                castingSuccesful = true;
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_GLOBAL");
            }
        }
        else if (spellTargetEnum == ETargetType.WorldHexBattleGlobal)
        {
            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.battle, spell))
            {
                Debug.Log(spell.battleScript);
                BattleHUD.CombatLogSpell(sc, spell, null);
                Battle.CastBattleSpell(spell, castData, this.chosenTarget);
                this.CastEffect(Vector3i.invalid, spell, sc);
                BattleHUD.CombatLogSpellAddEffect();
                castingSuccesful = true;
            }
            else
            {
                this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_HEX_OR_GLOBAL");
            }
        }
        else if (spellTargetEnum == ETargetType.TargetUnit || spellTargetEnum == ETargetType.TargetHex || spellTargetEnum == ETargetType.WorldHexBattleGlobal)
        {
            this.HighlightTargets(spell, sc);
            while (true)
            {
                if (this.chosenTarget != Vector3i.invalid)
                {
                    if (spellTargetEnum == ETargetType.TargetUnit)
                    {
                        BattleUnit unitAt = this.battle.GetUnitAt(this.chosenTarget);
                        if (unitAt != null)
                        {
                            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, unitAt, spell))
                            {
                                int num7 = unitAt.FigureCount();
                                Debug.Log(spell.battleScript);
                                BattleHUD.CombatLogSpell(sc, spell, unitAt);
                                Battle.CastBattleSpell(spell, castData, unitAt);
                                this.CastEffect(this.chosenTarget, spell, sc);
                                if (num7 != unitAt.FigureCount())
                                {
                                    unitAt.GetOrCreateFormation()?.UpdateFigureCount();
                                }
                                BattleHUD.CombatLogSpellAddEffect();
                                castingSuccesful = true;
                            }
                            else
                            {
                                this.InvalidBattleTarget(spell, "UI_NO_VALID_TARGET", "UI_INVALID_SPELL_TARGET", "UI_OK");
                            }
                            break;
                        }
                    }
                    else if (spellTargetEnum == ETargetType.TargetHex)
                    {
                        if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, this.chosenTarget, spell))
                        {
                            Debug.Log(spell.battleScript);
                            BattleHUD.CombatLogSpell(sc, spell, this.chosenTarget);
                            Battle.CastBattleSpell(spell, castData, this.chosenTarget);
                            this.CastEffect(this.chosenTarget, spell, sc);
                            BattleHUD.CombatLogSpellAddEffect();
                            castingSuccesful = true;
                        }
                        else
                        {
                            this.InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_HEX");
                        }
                        break;
                    }
                    this.chosenTarget = Vector3i.invalid;
                }
                yield return null;
            }
        }
        BattleHUD.Get().SetCasting(null);
        FSMBattleTurn.casting = null;
        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
        if (sc is BattleUnit)
        {
            VerticalMarkerManager.Get().UpdateSpelcasterIcon(sc, active: false);
        }
        if (castingSuccesful)
        {
            if (sc is PlayerWizard)
            {
                ((this.battle.attacker.wizard == sc) ? this.battle.attacker : this.battle.defender).UseResourcesFor(spell);
            }
            else if (sc is BattleUnit)
            {
                (sc as BattleUnit).UseManaFor(spell);
            }
        }
        if (sourceCopy == null)
        {
            yield break;
        }
        bool flag3 = false;
        foreach (BattleUnit old in sourceCopy)
        {
            if (!old.IsAlive())
            {
                continue;
            }
            BattleUnit battleUnit4 = currentCopy.Find((BattleUnit o) => o.GetID() == old.GetID());
            int num8 = battleUnit4.figureCount - old.figureCount;
            int num9 = battleUnit4.currentFigureHP - old.currentFigureHP;
            if (num8 != 0 || num9 != 0)
            {
                flag3 = true;
                Formation orCreateFormation = battleUnit4.GetOrCreateFormation(null, createIfMissing: false);
                if (orCreateFormation != null)
                {
                    orCreateFormation.UpdateFigureCount();
                }
            }
        }
        if (flag3)
        {
            BattleHUD.CalcUnitDelta(sourceCopy, currentCopy, spell.descriptionInfo);
        }
    }

    private void HighlightTargets(Spell spell, ISpellCaster sc)
    {
        ETargetType enumType = spell.targetType.enumType;
        int num = ((sc.GetWizardOwner() != null) ? sc.GetWizardOwner().GetID() : 0);
        List<Vector3i> list = new List<Vector3i>();
        SpellCastData spellCastData = new SpellCastData(sc, this.battle);
        switch (enumType)
        {
        case ETargetType.TargetUnit:
            foreach (KeyValuePair<BattleUnit, global::MOM.Unit> item in this.battle.buToSource)
            {
                if (item.Key.currentlyVisible || item.Key.ownerID == num)
                {
                    BattleUnit key = item.Key;
                    if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, spellCastData, key, spell))
                    {
                        list.Add(key.GetPosition());
                    }
                }
            }
            break;
        case ETargetType.TargetHex:
            this.battle.UnitBlind();
            foreach (KeyValuePair<Vector3i, Hex> hex in this.battle.plane.GetHexes())
            {
                if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, spellCastData, hex.Key, spell))
                {
                    continue;
                }
                bool flag = false;
                foreach (KeyValuePair<BattleUnit, global::MOM.Unit> item2 in this.battle.buToSource)
                {
                    if (item2.Key.GetPosition() == hex.Key && (item2.Key.currentlyVisible || item2.Key.ownerID == num))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    list.Add(hex.Key);
                }
            }
            this.battle.UnitNormal();
            break;
        }
        this.battle.plane.GetMarkers_().HighlightHexes(list);
    }

    public GameObject CastEffect(Vector3i pos, string effectName, string addAudioSFX = null)
    {
        GameObject gameObject = null;
        if (!string.IsNullOrEmpty(effectName))
        {
            if (pos != Vector3i.invalid)
            {
                GameObject gameObject2 = AssetManager.Get<GameObject>(effectName);
                if (gameObject2 != null)
                {
                    global::WorldCode.Plane activePlane = World.GetActivePlane();
                    Chunk chunkFor = activePlane.GetChunkFor(pos);
                    gameObject = GameObjectUtils.Instantiate(gameObject2, chunkFor.go.transform);
                    Vector3 vector = HexCoordinates.HexToWorld3D(pos);
                    vector.y = activePlane.GetHeightAt(vector);
                    gameObject.transform.localPosition = vector;
                }
            }
            else
            {
                GameObject gameObject3 = AssetManager.Get<GameObject>(effectName);
                if (gameObject3 != null)
                {
                    GameObject gameObject4 = GameObjectUtils.FindByName(Camera.main.gameObject, "GlobalEffectContainer");
                    gameObject = GameObjectUtils.Instantiate(gameObject3, gameObject4.transform);
                }
            }
        }
        if (!string.IsNullOrEmpty(addAudioSFX))
        {
            AudioLibrary.RequestSFX(addAudioSFX);
        }
        return gameObject;
    }

    public void CastEffect(Vector3i pos, Spell spell, object spellCaster = null)
    {
        if (spellCaster != null)
        {
            if (spellCaster is PlayerWizard)
            {
                ((this.battle.attacker.wizard == spellCaster) ? this.battle.attacker : this.battle.defender).spellCasted = true;
            }
            else if (spellCaster is BattleUnit battleUnit)
            {
                battleUnit.spellCasted = true;
                battleUnit.Mp = FInt.ZERO;
                battleUnit.battleFormation?.PlayMagicCast();
                if (BattleHUD.GetSelectedUnit() == spellCaster)
                {
                    BattleHUD.RefreshSelection();
                }
            }
        }
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
        }
        if (!string.IsNullOrEmpty(spell.battleCastEffect))
        {
            GameObject gameObject3 = AssetManager.Get<GameObject>(spell.battleCastEffect);
            if (gameObject3 != null)
            {
                GameObject gameObject4 = GameObjectUtils.FindByName(Camera.main.gameObject, "GlobalEffectContainer");
                GameObjectUtils.Instantiate(gameObject3, gameObject4.transform);
            }
        }
        if (!string.IsNullOrEmpty(spell.audioEffect))
        {
            AudioLibrary.RequestSFX(spell.audioEffect);
        }
    }

    public void CastFailedEffect(Vector3i pos, bool counterMagickedSpell)
    {
        if (!(pos == Vector3i.invalid))
        {
            GameObject gameObject = AssetManager.Get<GameObject>("SpellResisted");
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
        AudioLibrary.RequestSFX("SpellResisted");
    }

    private void SelectSpellUnitTarget(object o)
    {
        if (!(o is BattleUnit d))
        {
            Debug.LogError("Received invalid spell target " + o);
        }
        else
        {
            MHDataStorage.Set("Unit", d);
        }
    }

    private void CancelSpellTargetSelection(object o)
    {
        if (FSMBattleTurn.casting != null)
        {
            base.StopCoroutine(FSMBattleTurn.casting);
            BattleHUD.Get().SetCasting(null);
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            FSMBattleTurn.casting = null;
        }
    }

    private void HUDEvent(object sender, object e)
    {
        if (!(e is string))
        {
            return;
        }
        BattlePlayer player = this.battle.GetPlayer(this.isAttackerTurn);
        if (!this.battle.debugMode && !player.playerOwner)
        {
            return;
        }
        switch (e as string)
        {
        case "Wait":
            if (!player.autoPlayByAI)
            {
                this.SelectedUnit().Mp = FInt.ZERO;
                this.SelectNextUnit();
            }
            break;
        case "Skip":
            if (!player.autoPlayByAI)
            {
                this.SelectNextUnit();
            }
            break;
        case "Surrender":
            if (!player.autoPlayByAI)
            {
                player.surrendered = true;
                this.battle.ApplyFleeDamages(this.battle.playerIsAttacker);
                base.Fsm.Event("End Battle");
            }
            break;
        case "EndTurn":
            if (!player.autoPlayByAI)
            {
                this.endTurn = true;
            }
            break;
        }
    }

    private bool IsEndTurn()
    {
        int num = 0;
        if (this.battle.attackerUnits != null)
        {
            foreach (BattleUnit attackerUnit in this.battle.attackerUnits)
            {
                if (attackerUnit.IsAlive() && (!this.isAttackerTurn || attackerUnit.Mp > 0))
                {
                    num++;
                    break;
                }
            }
        }
        if (this.battle.defenderUnits != null)
        {
            foreach (BattleUnit defenderUnit in this.battle.defenderUnits)
            {
                if (defenderUnit.IsAlive() && (this.isAttackerTurn || defenderUnit.Mp > 0))
                {
                    num++;
                    break;
                }
            }
        }
        return num < 2;
    }

    private void SelectNextUnit()
    {
        if (this.battle.IsFinished())
        {
            return;
        }
        List<BattleUnit> units = this.battle.GetUnits(this.isAttackerTurn);
        int num = units.IndexOf(this.SelectedUnit());
        for (int i = 1; i < units.Count + 1; i++)
        {
            BattleUnit battleUnit = units[(i + num) % units.Count];
            if (battleUnit.figureCount > 0 && battleUnit.Mp > 0)
            {
                BattleHUD.Get()?.SelectUnit(battleUnit, this.battle.playerIsAttacker, focus: true);
                break;
            }
        }
    }

    private void NewTurn(List<BattleUnit> units)
    {
        if (units == null)
        {
            return;
        }
        foreach (BattleUnit unit in units)
        {
            if (unit.IsAlive())
            {
                unit.UpdateUnitMP();
            }
            if (unit.GetAttributes().Contains(TAG.REGENERATION) || unit.GetAttributes().Contains(TAG.MINOR_REGENERATION))
            {
                int num = unit.GetAttributes().GetFinal(TAG.REGENERATION).ToInt();
                num += unit.GetAttributes().GetFinal(TAG.MINOR_REGENERATION).ToInt();
                BattleHUD.CombatLogSkill(unit.GetAttributes().Contains(TAG.REGENERATION) ? ((Skill)SKILL.REGENERATION) : ((Skill)SKILL.MINOR_REGENERATION), unit);
                unit.Regeneration(postBattle: false, num);
                BattleHUD.CombatLogSpellAddEffect();
            }
            if (unit.GetAttributes().Contains(TAG.BLEEDING) && unit.Bleeding() > 0)
            {
                BattleHUD.CombatLogSkill((Skill)SKILL.BLEEDING_UNIT, unit);
                BattleHUD.CombatLogSpellAddEffect();
            }
        }
    }

    private void NewTurnHudMessage()
    {
        if (this.isAttackerTurn)
        {
            bool playerOwner = this.battle.attacker.playerOwner;
            BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_TURN", true, this.battle.attacker.GetName(), WizardColors.GetHex(this.battle.attacker.wizard)));
            BattleHUD.SetMessageAnim(playerOwner);
        }
        else
        {
            bool playerOwner2 = this.battle.defender.playerOwner;
            BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_TURN", true, this.battle.defender.GetName(), WizardColors.GetHex(this.battle.defender.wizard)));
            BattleHUD.SetMessageAnim(playerOwner2);
        }
    }

    private void MoveAttackAction(BattleUnit bu, BattleUnit targetUnit)
    {
        this.attacking = base.StartCoroutine(this.MoveAttackCoroutine(bu, targetUnit));
    }

    private IEnumerator MoveAttackCoroutine(BattleUnit bu, BattleUnit targetUnit)
    {
        if (bu == null || targetUnit == null)
        {
            yield break;
        }
        Vector3i destination = targetUnit.GetPosition();
        if (Battle.AttackFormPossible(bu, targetUnit) == Battle.AttackForm.eNone)
        {
            if (Battle.AttackFormPossible(bu, targetUnit, 1) == Battle.AttackForm.eNone)
            {
                yield return null;
                if (bu == null || targetUnit == null)
                {
                    yield break;
                }
            }
            if (bu.GetDistanceTo(destination) > 1)
            {
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(bu.GetPlane(), bu.GetPosition(), destination, bu);
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                if (path == null || path.Count < 3)
                {
                    yield break;
                }
                bu.MoveAnimatedTo(path[path.Count - 2], this.battle);
                while (this.battle != null && !this.battle.IsAttentionAvaliable())
                {
                    yield return null;
                }
            }
        }
        if (bu == null || targetUnit == null)
        {
            yield break;
        }
        if (Battle.AttackFormPossible(bu, targetUnit) != 0 && bu.Mp > 0)
        {
            yield return this.AttackUnit(targetUnit);
            if (bu == null)
            {
                yield break;
            }
        }
        this.attacking = null;
        MHEventSystem.TriggerEvent("BattleHUDInfoChange", this, targetUnit);
        MHEventSystem.TriggerEvent("BattleHUDInfoChange", this, bu);
        MHEventSystem.TriggerEvent<FSMBattleTurn>(bu, null);
        BattleHUD.Get()?.BaseUpdate();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (this.updateVortexes != null)
        {
            return;
        }
        if (this.endTurn)
        {
            if (this.messageAnimation == null && this.attacking == null && this.battle.IsAttentionAvaliable())
            {
                this.EndState();
            }
        }
        else if ((this.isAttackerTurn && this.battle.attacker.autoPlayByAI) || (!this.isAttackerTurn && this.battle.defender.autoPlayByAI))
        {
            CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
        }
        else
        {
            if (!this.AllowsForInteration() || this.battle == null || !this.battle.IsAttentionAvaliable())
            {
                return;
            }
            if (FSMBattleTurn.casting != null)
            {
                if (!UIManager.IsOverUI())
                {
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.CastSpell);
                }
                if (this.castingStopLimiter)
                {
                    this.castingStopLimiter = false;
                    return;
                }
                Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                BattleUnit unitAt = this.battle.GetUnitAt(hexCoordAt);
                this.RollOverUnit(unitAt, hexCoordAt, FSMBattleTurn.casting == null);
                if ((Input.GetMouseButtonUp(1) && !UIManager.AnyPopupScreen()) || (Input.GetMouseButtonUp(0) && UIManager.IsOverUI() && !UIManager.AnyPopupScreen()))
                {
                    base.StopCoroutine(FSMBattleTurn.casting);
                    BattleHUD.Get().SetCasting(null);
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                    FSMBattleTurn.casting = null;
                    VerticalMarkerManager.Get().UpdateSpelcasterIcon(this.isAttackerTurn ? this.battle.attackerUnits : this.battle.defenderUnits, active: false);
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (!UIManager.IsOverUI() && (unitAt == null || unitAt.attackingSide == this.isAttackerTurn || unitAt.currentlyVisible))
                    {
                        this.chosenTarget = hexCoordAt;
                    }
                    return;
                }
            }
            if (UIManager.IsOverUI())
            {
                CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            }
            else if (FSMBattleTurn.casting == null && Input.GetMouseButtonUp(1))
            {
                Vector3 clickWorldPosition = CameraController.GetClickWorldPosition();
                Vector3i pos3i = HexCoordinates.GetHexCoordAt(clickWorldPosition);
                BattleUnit unitAt2 = this.battle.GetUnitAt(pos3i);
                BattleUnit battleUnit = this.SelectedUnit();
                if (battleUnit != null && battleUnit.IsAlive() && battleUnit.Mp > 0 && battleUnit.attackingSide == this.isAttackerTurn)
                {
                    if (unitAt2 != null && unitAt2.attackingSide != this.isAttackerTurn && unitAt2.currentlyVisible)
                    {
                        this.MoveAttackAction(battleUnit, unitAt2);
                        return;
                    }
                    if (unitAt2 == null)
                    {
                        bool flag = false;
                        if (battleUnit != null && battleUnit.attackingSide && this.battle.battleWalls != null)
                        {
                            BattleWall battleWall = this.battle.battleWalls.Find((BattleWall o) => o.position == pos3i);
                            if (battleWall != null && battleWall.standing)
                            {
                                int num = HexCoordinates.HexDistance(pos3i, battleUnit.GetPosition());
                                bool flag2 = battleUnit.GetAttFinal((Tag)TAG.WALL_CRUSHER) > FInt.ZERO;
                                bool num2 = flag2 && battleUnit.GetCurentFigure().rangedAmmo > FInt.ZERO;
                                flag = true;
                                if (num2 || (num <= 1 && flag2) || (num <= 1 && battleWall.gate))
                                {
                                    this.attacking = base.StartCoroutine(this.AttackWall(battleWall));
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.MoveTo(pos3i);
                        }
                    }
                    MHEventSystem.TriggerEvent("BattleHUDInfoChange", this, battleUnit);
                    MHEventSystem.TriggerEvent<FSMBattleTurn>(battleUnit, null);
                    BattleHUD.Get().BaseUpdate();
                }
            }
            else if (FSMBattleTurn.casting == null && Input.GetMouseButtonUp(0))
            {
                Vector3i hexCoordAt2 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition(flat: true));
                BattleUnit unitAt3 = this.battle.GetUnitAt(hexCoordAt2);
                if (unitAt3 != null)
                {
                    this.LeftClickUnit(unitAt3);
                }
            }
            else if (FSMBattleTurn.casting == null)
            {
                Vector3i hexCoordAt3 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                BattleUnit unitAt4 = this.battle.GetUnitAt(hexCoordAt3);
                BattleUnit battleUnit2 = this.SelectedUnit();
                if (battleUnit2 == null || battleUnit2.ownerID != PlayerWizard.HumanID() || battleUnit2.Mp <= 0)
                {
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                }
                else
                {
                    this.RollOverUnit(unitAt4, hexCoordAt3, FSMBattleTurn.casting == null);
                    if (unitAt4 != null && battleUnit2.GetWizardOwnerID() != unitAt4.GetWizardOwnerID() && !unitAt4.IsInvisibleUnit())
                    {
                        if (Battle.AttackFormPossible(battleUnit2, unitAt4) == Battle.AttackForm.eRanged)
                        {
                            CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackRanged);
                        }
                        else if (Battle.AttackFormPossible(battleUnit2, unitAt4, 1) == Battle.AttackForm.eMelee)
                        {
                            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.battle.plane, battleUnit2.GetPosition(), battleUnit2.Mp, battleUnit2);
                            PathfinderV2.FindArea(requestDataV);
                            List<Vector3i> area = requestDataV.GetArea();
                            if (area == null || !area.Contains(unitAt4.GetPosition()))
                            {
                                CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                            }
                            else
                            {
                                CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackMelee);
                            }
                        }
                    }
                }
            }
            if (FSMBattleTurn.casting == null)
            {
                if (SettingsBlock.IsKeyUp(Settings.KeyActions.UI_END_TURN2) && UIManager.IsTopForInput(BattleHUD.Get()))
                {
                    this.endTurn = true;
                }
                else if (this.battle.IsFinished() || (this.IsEndTurn() && Settings.GetData().GetAutoEndTurn()))
                {
                    this.endTurn = true;
                }
            }
        }
    }

    public BattleUnit RollOverUnit(BattleUnit rollOverUnit, Vector3i pos3i, bool cursorUpdate = true)
    {
        bool flag = false;
        BattleUnit battleUnit = this.SelectedUnit();
        if (rollOverUnit != null && rollOverUnit != battleUnit)
        {
            if (pos3i == Vector3i.invalid)
            {
                flag = true;
                pos3i = rollOverUnit.GetPosition();
            }
            bool flag2 = false;
            if (rollOverUnit.ownerID == PlayerWizard.HumanID())
            {
                flag2 = true;
            }
            else if (rollOverUnit.currentlyVisible || flag)
            {
                flag2 = true;
            }
            if (flag2)
            {
                if (this.isAttackerTurn)
                {
                    BattleHUD.Get().defenderInfo.UpdateUnitInfoDisplay(rollOverUnit, showButtons: false);
                }
                else
                {
                    BattleHUD.Get().attackerInfo.UpdateUnitInfoDisplay(rollOverUnit, showButtons: false);
                }
            }
            if (cursorUpdate && battleUnit != null && battleUnit != rollOverUnit && rollOverUnit.currentlyVisible && rollOverUnit.attackingSide != this.SelectedUnit().attackingSide)
            {
                Battle.AttackForm attackForm = Battle.AttackFormPossible(battleUnit, rollOverUnit);
                int num = HexCoordinates.HexDistance(battleUnit.GetPosition(), rollOverUnit.GetPosition());
                if (num == 1)
                {
                    if (attackForm == Battle.AttackForm.eMelee)
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackMelee);
                    }
                    else
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.InvalidMelee);
                    }
                }
                else if (battleUnit.GetCurentFigure().rangedAmmo > 0)
                {
                    if (attackForm == Battle.AttackForm.eRanged)
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackRanged);
                        if (!flag)
                        {
                            float rangedPenalty = battleUnit.RangedPenalty(num, battleUnit.GetRangedTag()).ToFloat();
                            int num2 = Mathf.RoundToInt(battleUnit.HitChance(battleUnit, rollOverUnit, rangedPenalty) * 100f);
                            MouseTooltip.Open(UIReferences.instance.cursorInfoAttackRanged, num2.ToString());
                        }
                    }
                    else
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.InvalidRanged);
                    }
                }
            }
        }
        else
        {
            if (this.isAttackerTurn)
            {
                BattleHUD.Get().defenderInfo.UpdateUnitInfoDisplay(null, showButtons: false);
            }
            else
            {
                BattleHUD.Get().attackerInfo.UpdateUnitInfoDisplay(null, showButtons: false);
            }
            if (cursorUpdate)
            {
                bool flag3 = true;
                if (pos3i != Vector3i.invalid && battleUnit != null && battleUnit.attackingSide && this.battle.battleWalls != null)
                {
                    BattleWall battleWall = this.battle.battleWalls.Find((BattleWall o) => o.position == pos3i);
                    if (battleWall != null && battleWall.standing)
                    {
                        int distanceTo = battleUnit.GetDistanceTo(pos3i);
                        if ((distanceTo <= 1 && battleWall.gate) || (battleUnit.GetAttFinal((Tag)TAG.WALL_CRUSHER) > 0 && (distanceTo <= 1 || battleUnit.GetCurentFigure().rangedAmmo > 0)))
                        {
                            flag3 = false;
                            if (distanceTo <= 1)
                            {
                                CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackMelee);
                            }
                            else
                            {
                                CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackRanged);
                            }
                        }
                    }
                }
                if (flag3)
                {
                    CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                }
            }
        }
        if (rollOverUnit == null)
        {
            MouseTooltip.Close();
        }
        return battleUnit;
    }

    public void LeftClickUnit(BattleUnit clickedUnit)
    {
        if (clickedUnit.attackingSide == this.battle.playerIsAttacker)
        {
            if (clickedUnit == this.SelectedUnit())
            {
                UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup);
                List<BattleUnit> units = this.battle.GetUnits(clickedUnit.attackingSide);
                unitInfo.SetData(units, clickedUnit);
            }
            else
            {
                BattleHUD.Get().SelectUnit(clickedUnit, clickedUnit.attackingSide, focus: false);
            }
        }
        else
        {
            UnitInfo unitInfo2 = UIManager.Open<UnitInfo>(UIManager.Layer.Popup);
            List<BattleUnit> units2 = this.battle.GetUnits(clickedUnit.attackingSide);
            unitInfo2.SetData(units2, clickedUnit);
        }
    }

    private void EndState()
    {
        if (!this.battle.IsAttentionAvaliable())
        {
            return;
        }
        FsmEventTarget fsmEventTarget = new FsmEventTarget();
        fsmEventTarget.target = FsmEventTarget.EventTarget.Self;
        if (this.battle.IsFinished() || this.battle.turn == this.battle.lastTurn)
        {
            this.TriggerEvent(EEnchantmentType.BattleEndEffect, ESkillType.BattleEndEffect);
            for (int num = this.battle.attackerUnits.Count - 1; num >= 0; num--)
            {
                BattleUnit battleUnit = this.battle.attackerUnits[num];
                global::MOM.Unit unit = this.battle.buToSource[battleUnit];
                if (!battleUnit.isHopingToJoin && unit.GetWizardOwner() != battleUnit.GetWizardOwner() && unit.group != null)
                {
                    this.battle.attackerUnits.Remove(battleUnit);
                    this.battle.defenderUnits.Add(battleUnit);
                    this.battle.UnitListsDirty();
                    battleUnit.ownerID = this.battle.defender.GetID();
                    battleUnit.attackingSide = !battleUnit.attackingSide;
                    battleUnit.currentFigureHP = 0;
                    battleUnit.figureCount = 0;
                }
            }
            for (int num2 = this.battle.defenderUnits.Count - 1; num2 >= 0; num2--)
            {
                BattleUnit battleUnit2 = this.battle.defenderUnits[num2];
                global::MOM.Unit unit2 = this.battle.buToSource[battleUnit2];
                if (!battleUnit2.isHopingToJoin && unit2.GetWizardOwner() != battleUnit2.GetWizardOwner() && unit2.group != null)
                {
                    this.battle.defenderUnits.Remove(battleUnit2);
                    this.battle.attackerUnits.Add(battleUnit2);
                    battleUnit2.ownerID = this.battle.attacker.GetID();
                    battleUnit2.attackingSide = !battleUnit2.attackingSide;
                    battleUnit2.currentFigureHP = 0;
                    battleUnit2.figureCount = 0;
                }
            }
            base.Fsm.Event(fsmEventTarget, "End Battle");
        }
        else
        {
            this.battle.BattleCountdownUpdate(this.isAttackerTurn);
            this.battle.attacker.BattleCountdownUpdate(this.isAttackerTurn);
            this.battle.defender.BattleCountdownUpdate(this.isAttackerTurn);
            for (int num3 = this.battle.attackerUnits.Count - 1; num3 >= 0; num3--)
            {
                this.battle.attackerUnits[num3].BattleCountdownUpdate(this.isAttackerTurn);
            }
            for (int num4 = this.battle.defenderUnits.Count - 1; num4 >= 0; num4--)
            {
                this.battle.defenderUnits[num4].BattleCountdownUpdate(this.isAttackerTurn);
            }
            if (this.isAttackerTurn)
            {
                this.TriggerEventForSide(attacker: true, includeBattle: true, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
                this.NewTurn(this.battle.attackerUnits);
            }
            else
            {
                this.TriggerEventForSide(attacker: false, includeBattle: false, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
                this.NewTurn(this.battle.defenderUnits);
            }
            base.Fsm.Event(fsmEventTarget, "NextTurn");
        }
    }

    public IEnumerator AttackUnit(BattleUnit bu)
    {
        this.battle.GainAttention(this.SelectedUnit().GetOrCreateFormation());
        yield return this.battle.AttackUnit(this.SelectedUnit(), bu);
        yield return this.battle.WaitForAttention();
        if (this.SelectedUnit() == null || this.SelectedUnit().Mp == 0 || !this.SelectedUnit().IsAlive())
        {
            this.SelectNextUnit();
        }
        this.attacking = null;
    }

    public IEnumerator AttackWall(BattleWall bw)
    {
        yield return this.battle.AttackWall(this.SelectedUnit(), bw);
        yield return this.battle.WaitForAttention();
        if (this.SelectedUnit() == null || this.SelectedUnit().Mp == 0)
        {
            this.SelectNextUnit();
        }
        BattleHUD.RefreshSelection();
        this.attacking = null;
    }

    private void MoveTo(Vector3i pos)
    {
        this.SelectedUnit().MoveAnimatedTo(pos, this.battle);
        if (this.SelectedUnit().Mp == 0)
        {
            this.HUDEvent(null, "Skip");
        }
        else
        {
            BattleHUD.Get().SetUnitDirty(this.SelectedUnit());
        }
    }

    private BattleUnit SelectedUnit()
    {
        return BattleHUD.GetSelectedUnit();
    }

    private void TriggerEvent(EEnchantmentType eType, ESkillType sType)
    {
        if (this.battle.buToSource == null)
        {
            return;
        }
        List<BattleUnit> list = new List<BattleUnit>(this.battle.buToSource.Keys);
        List<BattleUnit> list2 = Serializer.DeepClone(list);
        this.battle.TriggerScripts(eType);
        this.battle.GetPlayer(attacker: true).TriggerScripts(eType);
        this.battle.GetPlayer(attacker: false).TriggerScripts(eType);
        if (this.battle.buToSource != null)
        {
            foreach (BattleUnit item in list)
            {
                if (item.IsAlive())
                {
                    item.TriggerScripts(eType);
                    if (item.IsAlive())
                    {
                        item.TriggerSkillScripts(sType);
                    }
                }
            }
        }
        bool flag = false;
        bool flag2 = false;
        foreach (BattleUnit old in list2)
        {
            if (!old.IsAlive())
            {
                continue;
            }
            BattleUnit battleUnit = list.Find((BattleUnit o) => o.GetID() == old.GetID());
            int num = battleUnit.figureCount - old.figureCount;
            int num2 = battleUnit.currentFigureHP - old.currentFigureHP;
            if (num != 0 || num2 != 0)
            {
                flag = true;
                flag2 = flag2 || (battleUnit.figureCount == 0 && old.figureCount > 0);
                Formation orCreateFormation = battleUnit.GetOrCreateFormation(null, createIfMissing: false);
                if (orCreateFormation != null)
                {
                    orCreateFormation.UpdateFigureCount();
                }
            }
        }
        if (flag)
        {
            BattleHUD.CalcUnitDelta(list2, list, null);
        }
    }

    private void TriggerEventForSide(bool attacker, bool includeBattle, EEnchantmentType eType, ESkillType sType)
    {
        if (this.battle.buToSource == null)
        {
            return;
        }
        List<BattleUnit> list = new List<BattleUnit>(this.battle.buToSource.Keys);
        List<BattleUnit> list2 = Serializer.DeepClone(list);
        if (includeBattle)
        {
            this.battle.TriggerScripts(eType);
        }
        this.battle.GetPlayer(attacker).TriggerScripts(eType);
        if (this.battle.buToSource != null)
        {
            foreach (BattleUnit item in new List<BattleUnit>(attacker ? this.battle.attackerUnits : this.battle.defenderUnits))
            {
                item.TriggerScripts(eType);
                item.TriggerSkillScripts(sType);
            }
        }
        bool flag = false;
        foreach (BattleUnit old in list2)
        {
            if (!old.IsAlive())
            {
                continue;
            }
            BattleUnit battleUnit = list.Find((BattleUnit o) => o.GetID() == old.GetID());
            int num = battleUnit.figureCount - old.figureCount;
            int num2 = battleUnit.currentFigureHP - old.currentFigureHP;
            if (num != 0 || num2 != 0)
            {
                flag = true;
                Formation orCreateFormation = battleUnit.GetOrCreateFormation(null, createIfMissing: false);
                if (orCreateFormation != null)
                {
                    orCreateFormation.UpdateFigureCount();
                }
            }
        }
        if (flag)
        {
            BattleHUD.CalcUnitDelta(list2, list, null);
        }
    }

    public void InvalidBattleTarget(Spell spell, string message)
    {
        PopupGeneral.OpenPopup(null, "UI_INVALID_TARGET", message, "UI_OK");
    }

    public void InvalidBattleTarget(Spell spell, string header, string message, string confirmation)
    {
        if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(confirmation))
        {
            Debug.LogWarning("InvalidBattleTarget PopupGeneral.OpenPopup do not have all valid parameters.");
        }
        PopupGeneral.OpenPopup(null, header, message, confirmation);
    }

    private bool AreSameEnchsOnTarget(Enchantment[] enchsApplyByAttacer, List<EnchantmentInstance> enchsOnTarget)
    {
        if (enchsApplyByAttacer != null && enchsOnTarget != null)
        {
            foreach (Enchantment e in enchsApplyByAttacer)
            {
                if (enchsOnTarget.Find((EnchantmentInstance o) => o.source == e) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AllowsForInteration()
    {
        return !this.busy;
    }
}
