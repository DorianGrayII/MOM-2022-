using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
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
            Debug.Log("FSMBattleTurn::OnEnter()");

            FSMBattleTurn.instance = this;
            busy = true;
            base.OnEnter();
            turnAutomation = base.StartCoroutine(TurnAutomationCoroutine());
            if (battle == null)
            {
                return;
            }
            if (isAttackerTurn)
            {
                battle.attacker.spellCasted = false;
                {
                    foreach (BattleUnit attackerUnit in battle.attackerUnits)
                    {
                        attackerUnit.spellCasted = false;
                    }
                    return;
                }
            }
            battle.defender.spellCasted = false;
            foreach (BattleUnit defenderUnit in battle.defenderUnits)
            {
                defenderUnit.spellCasted = false;
            }
        }

        public override void OnExit()
        {
            Debug.Log("FSMBattleTurn::OnExit()");

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
            endTurn = false;
            battle = Battle.GetBattle();
            if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
            {
                yield break;
            }
            battle.activeTurn = this;
            BattleHUD.Get().UIUpdateFor(isAttackerTurn);
            if (!isAttackerTurn)
            {
                battle.turn++;
                if (battle.turn == 1)
                {
                    TriggerEvent(EEnchantmentType.BattleStartEffect, ESkillType.BattleStartEffect);
                    NewTurn(battle.attackerUnits);
                    NewTurn(battle.defenderUnits);
                    yield return new WaitForSeconds(3f);
                    if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
                    {
                        yield break;
                    }
                }
                TriggerEventForSide(attacker: false, includeBattle: true, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
                NewTurnHudMessage();
                BattleHUD.Get().attackerInfo.DisableAllHighlights();
                BattleHUD.Get().SelectUnit(battle.defenderUnits.Find((BattleUnit o) => o.IsAlive()), attacker: false, focus: true);
            }
            else
            {
                TriggerEventForSide(attacker: true, includeBattle: false, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
                NewTurnHudMessage();
                BattleHUD.Get().defenderInfo.DisableAllHighlights();
                BattleHUD.Get().SelectUnit(battle.attackerUnits.Find((BattleUnit o) => o.IsAlive()), attacker: true, focus: true);
            }
            yield return UpdateVortexes(isAttackerTurn);
            yield return UpdateConfusedUnits(isAttackerTurn);
            if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
            {
                yield break;
            }
            if (isAttackerTurn)
            {
                busy = false;
                if (battle.attacker.autoPlayByAI)
                {
                    yield return AITurn(attacker: true);
                    if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
                    {
                        yield break;
                    }
                }
            }
            else
            {
                yield return BattleWizardTowerBolts();
                if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
                {
                    yield break;
                }
                busy = false;
                if (battle.defender.autoPlayByAI)
                {
                    yield return AITurn(attacker: false);
                    if (BattleHUD.Get() == null || battle == null || battle.battleEnd)
                    {
                        yield break;
                    }
                }
            }
            BattleHUD.Get().UpdateGeneralInfo();
            MHEventSystem.RegisterListener<BattleHUD>(HUDEvent, this);
            turnAutomation = null;
        }

        private IEnumerator BattleWizardTowerBolts()
        {
            battle = Battle.GetBattle();
            if (battle.battleEnd || battle.wizardTower == null)
            {
                yield break;
            }
            List<EnchantmentInstance> enchantmentsOfType = battle.GetEnchantmentManager().GetEnchantmentsOfType(EEnchantmentType.BattleWizardTowerEffect);
            if (enchantmentsOfType == null)
            {
                yield break;
            }
            foreach (EnchantmentInstance v in enchantmentsOfType)
            {
                List<BattleUnit> list = battle.attackerUnits.FindAll((BattleUnit o) => o.IsAlive());
                if (list.Count == 0)
                {
                    break;
                }
                BattleUnit target = list[Random.Range(0, list.Count)];
                if (battle.battleEnd)
                {
                    break;
                }
                yield return TowerBolt(battle, v, target);
                if (battle.battleEnd)
                {
                    break;
                }
                if (v.source.Get().scripts != null)
                {
                    EnchantmentScript[] scripts = v.source.Get().scripts;
                    foreach (EnchantmentScript enchantmentScript in scripts)
                    {
                        ScriptLibrary.Call(enchantmentScript.script, battle, enchantmentScript, v, target);
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
            if (battle.vortexList != null)
            {
                for (int i = battle.vortexList.Count - 1; i >= 0; i--)
                {
                    Vortex vortex = battle.vortexList[i];
                    if (vortex.attacker == attacker)
                    {
                        yield return vortex.Update(battle);
                    }
                }
            }
            updateVortexes = null;
        }

        private IEnumerator UpdateConfusedUnits(bool attacker)
        {
            if (battle.confusedList == null)
            {
                yield break;
            }
            for (int i = battle.confusedList.Count - 1; i >= 0; i--)
            {
                BattleUnit u = battle.confusedList[i];
                if (u.IsAlive() && u.attackingSide == attacker)
                {
                    int mp = u.Mp.ToInt();
                    for (int j = 0; j < mp; j++)
                    {
                        int num = Random.Range(0, 6);
                        Vector3i vector3i = u.battlePosition + HexNeighbors.neighbours[num];
                        if (!battle.plane.area.IsInside(vector3i))
                        {
                            Random.Range(0, 6);
                            continue;
                        }
                        Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(HexCoordinates.HexToWorld3D(vector3i));
                        bool flag = battle.GetUnitAt(hexCoordAt) == null;
                        if (battle.plane.exclusionPoints != null)
                        {
                            flag = !battle.plane.exclusionPoints.Contains(hexCoordAt) && flag;
                        }
                        if (flag)
                        {
                            u.MoveAnimatedTo(hexCoordAt, battle);
                            yield return battle.WaitForAttention();
                        }
                    }
                    battle.confusedList.Remove(u);
                    u.Mp = FInt.ZERO;
                }
            }
        }

        public void StartAI(bool attacker)
        {
            if (aiActivity == null && !endTurn)
            {
                aiActivity = base.StartCoroutine(AITurn(attacker));
            }
        }

        public void StopAI()
        {
        }

        private IEnumerator AITurn(bool attacker)
        {
            while (updateVortexes != null)
            {
                yield return null;
            }
            yield return battle.WaitForAttention();
            Debug.Log("AI turn " + battle.turn + " start");
            MHTimer t = MHTimer.StartNew();
            yield return ScriptLibrary.Call("AITurnV02", battle, attacker);
            Debug.Log("AI turn " + battle.turn + " took " + t.GetTime());
            aiActivity = null;
        }

        public void AITurnEnd()
        {
            endTurn = true;
        }

        public static bool IsCastingSpells()
        {
            return FSMBattleTurn.casting != null;
        }

        internal void StartCasting(Spell spell, ISpellCaster spellCaster)
        {
            CursorsLibrary.SetMode(CursorsLibrary.Mode.CastSpell);
            BattleHUD.Get().SetCasting(spell);
            FSMBattleTurn.casting = base.StartCoroutine(CastingSpell(spell, spellCaster));
            castingStopLimiter = true;
        }

        private IEnumerator CastingSpell(Spell spell, ISpellCaster sc)
        {
            if ((bool)ScriptLibrary.Call("CounterMagicBattle", battle, spell, sc))
            {
                BattleHUD.Get().SetCasting(null);
                FSMBattleTurn.casting = null;
                CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                Battle.GetBattle()?.ResistedSpell(Vector3i.invalid, counterMagickedSpell: true);
                if (sc is PlayerWizard)
                {
                    ((battle.attacker.wizard == sc) ? battle.attacker : battle.defender).UseResourcesFor(spell);
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
                    battle.GetBattlePlayerForWizard(w).spellCasted = true;
                }
                yield break;
            }
            List<BattleUnit> sourceCopy = null;
            List<BattleUnit> currentCopy = null;
            if (sc is BattleUnit)
            {
                VerticalMarkerManager.Get().UpdateSpelcasterIcon(sc, active: true);
            }
            else if (battle.buToSource != null)
            {
                currentCopy = new List<BattleUnit>(battle.buToSource.Keys);
                sourceCopy = Serializer.DeepClone(currentCopy);
            }
            chosenTarget = Vector3i.invalid;
            ETargetType spellTargetEnum = spell.targetType.enumType;
            bool castingSuccesful = false;
            Debug.Log("CastingSpell " + spell.dbName);
            SpellCastData castData = new SpellCastData(sc, battle);
            if (spellTargetEnum == ETargetType.TargetWizard)
            {
                BattlePlayer battlePlayer2 = battle.attacker;
                bool flag = false;
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battlePlayer2, spell))
                {
                    flag = true;
                    Debug.Log(spell.battleScript);
                    Battle.CastBattleSpell(spell, castData, battlePlayer2);
                }
                else
                {
                    battlePlayer2 = battle.defender;
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
                    CastEffect(Vector3i.invalid, spell, sc);
                    castingSuccesful = true;
                }
                else if (AreSameEnchsOnTarget(spell.enchantmentData, battlePlayer2.GetEnchantments()))
                {
                    InvalidBattleTarget(spell, "UI_SPELL_ALREADY_ACTIVE");
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_WIZARD");
                }
            }
            else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.RAISE_DEAD)
            {
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battle, spell))
                {
                    MHDataStorage.Set("Unit", null);
                    List<BattleUnit> list = new List<BattleUnit>(sc.IsAttackerInBattle(battle) ? battle.attackerUnits : battle.defenderUnits);
                    list = list.FindAll((BattleUnit o) => !o.IsAlive() && !o.dbSource.Get().unresurrectable && o.GetAttributes().DoesNotContains((Tag)TAG.FANTASTIC_CLASS));
                    foreach (KeyValuePair<BattleUnit, Unit> item in battle.buToSource)
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
                    CastEffect(battleUnit.GetPosition(), spell, sc);
                    BattleHUD.CombatLogSpellAddEffect();
                    castingSuccesful = true;
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_RAISE_DEAD");
                }
            }
            else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.ANIMATE_DEAD)
            {
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battle, spell))
                {
                    MHDataStorage.Set("Unit", null);
                    List<BattleUnit> list2 = new List<BattleUnit>();
                    foreach (BattleUnit item3 in ListUtils.MultiEnumerable(battle.attackerUnits, battle.defenderUnits))
                    {
                        int num3 = item3.GetBaseFigure().maxHitPoints * item3.maxCount;
                        if (item3.IsAlive() || item3.dbSource.Get() is Hero || !(item3.race != (Race)RACE.REALM_DEATH) || item3.summon || item3.irreversibleDamages >= num3 / 2 || (sc.GetWizardOwner() != item3.GetWizardOwner() && item3.GetAttributes().Contains(TAG.MAGIC_IMMUNITY)))
                        {
                            continue;
                        }
                        bool flag2 = false;
                        foreach (KeyValuePair<BattleUnit, Unit> item4 in battle.buToSource)
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
                    CastEffect(battleUnit2.GetPosition(), spell, sc);
                    BattleHUD.CombatLogSpellAddEffect();
                    castingSuccesful = true;
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_ANIMATE_DEAD");
                }
            }
            else if (spellTargetEnum == ETargetType.TargetGlobal && spell == (Spell)SPELL.RECONSTRUCT)
            {
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battle, spell))
                {
                    MHDataStorage.Set("Unit", null);
                    List<BattleUnit> list3 = new List<BattleUnit>();
                    PlayerWizard wizardOwner = sc.GetWizardOwner();
                    List<BattleUnit> list4 = ((battle.attacker.GetID() != wizardOwner.ID) ? battle.defenderUnits : battle.attackerUnits);
                    foreach (BattleUnit item6 in list4)
                    {
                        int num5 = item6.GetBaseFigure().maxHitPoints * item6.maxCount;
                        if (!item6.IsAlive() && !(item6.dbSource.Get() is Hero) && item6.GetAttFinal(TAG.MECHANICAL_UNIT) > FInt.ZERO && battle.buToSource[item6].group != null && item6.irreversibleDamages < num5 / 2)
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
                    CastEffect(battleUnit3.GetPosition(), spell, sc);
                    BattleHUD.CombatLogSpellAddEffect();
                    castingSuccesful = true;
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_ANIMATE_DEAD");
                }
            }
            else if (spellTargetEnum == ETargetType.TargetGlobal)
            {
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battle, spell))
                {
                    Debug.Log(spell.battleScript);
                    BattleHUD.CombatLogSpell(sc, spell, null);
                    Battle.CastBattleSpell(spell, castData, battle);
                    CastEffect(Vector3i.invalid, spell, sc);
                    BattleHUD.CombatLogSpellAddEffect();
                    castingSuccesful = true;
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_GLOBAL");
                }
            }
            else if (spellTargetEnum == ETargetType.WorldHexBattleGlobal)
            {
                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, battle, spell))
                {
                    Debug.Log(spell.battleScript);
                    BattleHUD.CombatLogSpell(sc, spell, null);
                    Battle.CastBattleSpell(spell, castData, chosenTarget);
                    CastEffect(Vector3i.invalid, spell, sc);
                    BattleHUD.CombatLogSpellAddEffect();
                    castingSuccesful = true;
                }
                else
                {
                    InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_HEX_OR_GLOBAL");
                }
            }
            else if (spellTargetEnum == ETargetType.TargetUnit || spellTargetEnum == ETargetType.TargetHex || spellTargetEnum == ETargetType.WorldHexBattleGlobal)
            {
                HighlightTargets(spell, sc);
                while (true)
                {
                    if (chosenTarget != Vector3i.invalid)
                    {
                        if (spellTargetEnum == ETargetType.TargetUnit)
                        {
                            BattleUnit unitAt = battle.GetUnitAt(chosenTarget);
                            if (unitAt != null)
                            {
                                if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, unitAt, spell))
                                {
                                    int num7 = unitAt.FigureCount();
                                    Debug.Log(spell.battleScript);
                                    BattleHUD.CombatLogSpell(sc, spell, unitAt);
                                    Battle.CastBattleSpell(spell, castData, unitAt);
                                    CastEffect(chosenTarget, spell, sc);
                                    if (num7 != unitAt.FigureCount())
                                    {
                                        unitAt.GetOrCreateFormation()?.UpdateFigureCount();
                                    }
                                    BattleHUD.CombatLogSpellAddEffect();
                                    castingSuccesful = true;
                                }
                                else
                                {
                                    InvalidBattleTarget(spell, "UI_NO_VALID_TARGET", "UI_INVALID_SPELL_TARGET", "UI_OK");
                                }
                                break;
                            }
                        }
                        else if (spellTargetEnum == ETargetType.TargetHex)
                        {
                            if (string.IsNullOrEmpty(spell.targetingScript) || (bool)ScriptLibrary.Call(spell.targetingScript, castData, chosenTarget, spell))
                            {
                                Debug.Log(spell.battleScript);
                                BattleHUD.CombatLogSpell(sc, spell, chosenTarget);
                                Battle.CastBattleSpell(spell, castData, chosenTarget);
                                CastEffect(chosenTarget, spell, sc);
                                BattleHUD.CombatLogSpellAddEffect();
                                castingSuccesful = true;
                            }
                            else
                            {
                                InvalidBattleTarget(spell, "UI_SPELL_NO_VALID_TARGET_HEX");
                            }
                            break;
                        }
                        chosenTarget = Vector3i.invalid;
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
                    ((battle.attacker.wizard == sc) ? battle.attacker : battle.defender).UseResourcesFor(spell);
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
            SpellCastData spellCastData = new SpellCastData(sc, battle);
            switch (enumType)
            {
            case ETargetType.TargetUnit:
                foreach (KeyValuePair<BattleUnit, Unit> item in battle.buToSource)
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
                battle.UnitBlind();
                foreach (KeyValuePair<Vector3i, Hex> hex in battle.plane.GetHexes())
                {
                    if (!string.IsNullOrEmpty(spell.targetingScript) && !(bool)ScriptLibrary.Call(spell.targetingScript, spellCastData, hex.Key, spell))
                    {
                        continue;
                    }
                    bool flag = false;
                    foreach (KeyValuePair<BattleUnit, Unit> item2 in battle.buToSource)
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
                battle.UnitNormal();
                break;
            }
            battle.plane.GetMarkers_().HighlightHexes(list);
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
                    ((battle.attacker.wizard == spellCaster) ? battle.attacker : battle.defender).spellCasted = true;
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
            BattlePlayer player = battle.GetPlayer(isAttackerTurn);
            if (!battle.debugMode && !player.playerOwner)
            {
                return;
            }
            switch (e as string)
            {
            case "Wait":
                if (!player.autoPlayByAI)
                {
                    SelectedUnit().Mp = FInt.ZERO;
                    SelectNextUnit();
                }
                break;
            case "Skip":
                if (!player.autoPlayByAI)
                {
                    SelectNextUnit();
                }
                break;
            case "Surrender":
                if (!player.autoPlayByAI)
                {
                    Debug.Log("HUDEvent - Surrender");
                    player.surrendered = true;
                    battle.ApplyFleeDamages(battle.playerIsAttacker);

                    FsmEventTarget fsmEventTarget = new FsmEventTarget();
                    fsmEventTarget.target = FsmEventTarget.EventTarget.Self;
                    base.Fsm.Event(fsmEventTarget, "End Battle");
                }
                break;
            case "EndTurn":
                if (!player.autoPlayByAI)
                {
                    Debug.Log("HUDEvent - EndTurn");
                    endTurn = true;
                }
                break;
            }
        }

        private bool IsEndTurn()
        {
            int num = 0;
            if (battle.attackerUnits != null)
            {
                foreach (BattleUnit attackerUnit in battle.attackerUnits)
                {
                    if (attackerUnit.IsAlive() && (!isAttackerTurn || attackerUnit.Mp > 0))
                    {
                        num++;
                        break;
                    }
                }
            }
            if (battle.defenderUnits != null)
            {
                foreach (BattleUnit defenderUnit in battle.defenderUnits)
                {
                    if (defenderUnit.IsAlive() && (isAttackerTurn || defenderUnit.Mp > 0))
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
            if (battle.IsFinished())
            {
                return;
            }
            List<BattleUnit> units = battle.GetUnits(isAttackerTurn);
            int num = units.IndexOf(SelectedUnit());
            for (int i = 1; i < units.Count + 1; i++)
            {
                BattleUnit battleUnit = units[(i + num) % units.Count];
                if (battleUnit.figureCount > 0 && battleUnit.Mp > 0)
                {
                    BattleHUD.Get()?.SelectUnit(battleUnit, battle.playerIsAttacker, focus: true);
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
            if (isAttackerTurn)
            {
                bool playerOwner = battle.attacker.playerOwner;
                BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_TURN", true, battle.attacker.GetName(), WizardColors.GetHex(battle.attacker.wizard)));
                BattleHUD.SetMessageAnim(playerOwner);
            }
            else
            {
                bool playerOwner2 = battle.defender.playerOwner;
                BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_TURN", true, battle.defender.GetName(), WizardColors.GetHex(battle.defender.wizard)));
                BattleHUD.SetMessageAnim(playerOwner2);
            }
        }

        private void MoveAttackAction(BattleUnit bu, BattleUnit targetUnit)
        {
            attacking = base.StartCoroutine(MoveAttackCoroutine(bu, targetUnit));
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
                    bu.MoveAnimatedTo(path[path.Count - 2], battle);
                    while (battle != null && !battle.IsAttentionAvaliable())
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
                yield return AttackUnit(targetUnit);
                if (bu == null)
                {
                    yield break;
                }
            }
            attacking = null;
            MHEventSystem.TriggerEvent("BattleHUDInfoChange", this, targetUnit);
            MHEventSystem.TriggerEvent("BattleHUDInfoChange", this, bu);
            MHEventSystem.TriggerEvent<FSMBattleTurn>(bu, null);
            BattleHUD.Get()?.BaseUpdate();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (updateVortexes != null)
            {
                return;
            }
            if (endTurn)
            {
                if (messageAnimation == null && attacking == null && battle.IsAttentionAvaliable())
                {
                    EndState();
                }
            }
            else if ((isAttackerTurn && battle.attacker.autoPlayByAI) || (!isAttackerTurn && battle.defender.autoPlayByAI))
            {
                CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
            }
            else
            {
                if (!AllowsForInteration() || battle == null || !battle.IsAttentionAvaliable())
                {
                    return;
                }
                if (FSMBattleTurn.casting != null)
                {
                    if (!UIManager.IsOverUI())
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.CastSpell);
                    }
                    if (castingStopLimiter)
                    {
                        castingStopLimiter = false;
                        return;
                    }
                    Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                    BattleUnit unitAt = battle.GetUnitAt(hexCoordAt);
                    RollOverUnit(unitAt, hexCoordAt, FSMBattleTurn.casting == null);
                    if ((Input.GetMouseButtonUp(1) && !UIManager.AnyPopupScreen()) || (Input.GetMouseButtonUp(0) && UIManager.IsOverUI() && !UIManager.AnyPopupScreen()))
                    {
                        base.StopCoroutine(FSMBattleTurn.casting);
                        BattleHUD.Get().SetCasting(null);
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                        FSMBattleTurn.casting = null;
                        VerticalMarkerManager.Get().UpdateSpelcasterIcon(isAttackerTurn ? battle.attackerUnits : battle.defenderUnits, active: false);
                        return;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (!UIManager.IsOverUI() && (unitAt == null || unitAt.attackingSide == isAttackerTurn || unitAt.currentlyVisible))
                        {
                            chosenTarget = hexCoordAt;
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
                    BattleUnit unitAt2 = battle.GetUnitAt(pos3i);
                    BattleUnit battleUnit = SelectedUnit();
                    if (battleUnit != null && battleUnit.IsAlive() && battleUnit.Mp > 0 && battleUnit.attackingSide == isAttackerTurn)
                    {
                        if (unitAt2 != null && unitAt2.attackingSide != isAttackerTurn && unitAt2.currentlyVisible)
                        {
                            MoveAttackAction(battleUnit, unitAt2);
                            return;
                        }
                        if (unitAt2 == null)
                        {
                            bool flag = false;
                            if (battleUnit != null && battleUnit.attackingSide && battle.battleWalls != null)
                            {
                                BattleWall battleWall = battle.battleWalls.Find((BattleWall o) => o.position == pos3i);
                                if (battleWall != null && battleWall.standing)
                                {
                                    int num = HexCoordinates.HexDistance(pos3i, battleUnit.GetPosition());
                                    bool flag2 = battleUnit.GetAttFinal((Tag)TAG.WALL_CRUSHER) > FInt.ZERO;
                                    bool num2 = flag2 && battleUnit.GetCurentFigure().rangedAmmo > FInt.ZERO;
                                    flag = true;
                                    if (num2 || (num <= 1 && flag2) || (num <= 1 && battleWall.gate))
                                    {
                                        attacking = base.StartCoroutine(AttackWall(battleWall));
                                    }
                                }
                            }
                            if (!flag)
                            {
                                MoveTo(pos3i);
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
                    BattleUnit unitAt3 = battle.GetUnitAt(hexCoordAt2);
                    if (unitAt3 != null)
                    {
                        LeftClickUnit(unitAt3);
                    }
                }
                else if (FSMBattleTurn.casting == null)
                {
                    Vector3i hexCoordAt3 = HexCoordinates.GetHexCoordAt(CameraController.GetClickWorldPosition());
                    BattleUnit unitAt4 = battle.GetUnitAt(hexCoordAt3);
                    BattleUnit battleUnit2 = SelectedUnit();
                    if (battleUnit2 == null || battleUnit2.ownerID != PlayerWizard.HumanID() || battleUnit2.Mp <= 0)
                    {
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
                    }
                    else
                    {
                        RollOverUnit(unitAt4, hexCoordAt3, FSMBattleTurn.casting == null);
                        if (unitAt4 != null && battleUnit2.GetWizardOwnerID() != unitAt4.GetWizardOwnerID() && !unitAt4.IsInvisibleUnit())
                        {
                            if (Battle.AttackFormPossible(battleUnit2, unitAt4) == Battle.AttackForm.eRanged)
                            {
                                CursorsLibrary.SetMode(CursorsLibrary.Mode.AttackRanged);
                            }
                            else if (Battle.AttackFormPossible(battleUnit2, unitAt4, 1) == Battle.AttackForm.eMelee)
                            {
                                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(battle.plane, battleUnit2.GetPosition(), battleUnit2.Mp, battleUnit2);
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
                        endTurn = true;
                    }
                    else if (battle.IsFinished() || (IsEndTurn() && Settings.GetData().GetAutoEndTurn()))
                    {
                        endTurn = true;
                    }
                }
            }
        }

        public BattleUnit RollOverUnit(BattleUnit rollOverUnit, Vector3i pos3i, bool cursorUpdate = true)
        {
            bool flag = false;
            BattleUnit battleUnit = SelectedUnit();
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
                    if (isAttackerTurn)
                    {
                        BattleHUD.Get().defenderInfo.UpdateUnitInfoDisplay(rollOverUnit, showButtons: false);
                    }
                    else
                    {
                        BattleHUD.Get().attackerInfo.UpdateUnitInfoDisplay(rollOverUnit, showButtons: false);
                    }
                }
                if (cursorUpdate && battleUnit != null && battleUnit != rollOverUnit && rollOverUnit.currentlyVisible && rollOverUnit.attackingSide != SelectedUnit().attackingSide)
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
                if (isAttackerTurn)
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
                    if (pos3i != Vector3i.invalid && battleUnit != null && battleUnit.attackingSide && battle.battleWalls != null)
                    {
                        BattleWall battleWall = battle.battleWalls.Find((BattleWall o) => o.position == pos3i);
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
            if (clickedUnit.attackingSide == battle.playerIsAttacker)
            {
                if (clickedUnit == SelectedUnit())
                {
                    UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup);
                    List<BattleUnit> units = battle.GetUnits(clickedUnit.attackingSide);
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
                List<BattleUnit> units2 = battle.GetUnits(clickedUnit.attackingSide);
                unitInfo2.SetData(units2, clickedUnit);
            }
        }

        private void EndState()
        {
            Debug.LogFormat("FSMBattleTurn::EndState() AttentionAvailable:{0} IsFinished:{1} turn:{2} lastTurn:{3} ",
                 battle.IsAttentionAvaliable(), battle.IsFinished(), battle.turn, battle.lastTurn);

            if (!battle.IsAttentionAvaliable())
            {
                return;
            }
            FsmEventTarget fsmEventTarget = new FsmEventTarget();
            fsmEventTarget.target = FsmEventTarget.EventTarget.Self;
            if (battle.IsFinished() || battle.turn == battle.lastTurn)
            {
                TriggerEvent(EEnchantmentType.BattleEndEffect, ESkillType.BattleEndEffect);

                for (int num = battle.attackerUnits.Count - 1; num >= 0; num--)
                {
                    BattleUnit battleUnit = battle.attackerUnits[num];
                    Unit unit = battle.buToSource[battleUnit];
                    if (!battleUnit.isHopingToJoin && 
                        unit.GetWizardOwner() != battleUnit.GetWizardOwner() && 
                        unit.group != null)
                    {
                        battle.attackerUnits.Remove(battleUnit);
                        battle.defenderUnits.Add(battleUnit);
                        battle.UnitListsDirty();
                        battleUnit.ownerID = battle.defender.GetID();
                        battleUnit.attackingSide = !battleUnit.attackingSide;
                        battleUnit.currentFigureHP = 0;
                        battleUnit.figureCount = 0;
                    }
                }
                for (int num2 = battle.defenderUnits.Count - 1; num2 >= 0; num2--)
                {
                    BattleUnit battleUnit2 = battle.defenderUnits[num2];
                    Unit unit2 = battle.buToSource[battleUnit2];
                    if (!battleUnit2.isHopingToJoin && 
                        unit2.GetWizardOwner() != battleUnit2.GetWizardOwner() && 
                        unit2.group != null)
                    {
                        battle.defenderUnits.Remove(battleUnit2);
                        battle.attackerUnits.Add(battleUnit2);
                        battleUnit2.ownerID = battle.attacker.GetID();
                        battleUnit2.attackingSide = !battleUnit2.attackingSide;
                        battleUnit2.currentFigureHP = 0;
                        battleUnit2.figureCount = 0;
                    }
                }
                base.Fsm.Event(fsmEventTarget, "End Battle");
            }
            else
            {
                battle.BattleCountdownUpdate(isAttackerTurn);
                battle.attacker.BattleCountdownUpdate(isAttackerTurn);
                battle.defender.BattleCountdownUpdate(isAttackerTurn);
                for (int num3 = battle.attackerUnits.Count - 1; num3 >= 0; num3--)
                {
                    battle.attackerUnits[num3].BattleCountdownUpdate(isAttackerTurn);
                }
                for (int num4 = battle.defenderUnits.Count - 1; num4 >= 0; num4--)
                {
                    battle.defenderUnits[num4].BattleCountdownUpdate(isAttackerTurn);
                }
                if (isAttackerTurn)
                {
                    TriggerEventForSide(attacker: true, includeBattle: true, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
                    NewTurn(battle.attackerUnits);
                }
                else
                {
                    TriggerEventForSide(attacker: false, includeBattle: false, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
                    NewTurn(battle.defenderUnits);
                }
                base.Fsm.Event(fsmEventTarget, "NextTurn");
            }
        }

        public IEnumerator AttackUnit(BattleUnit bu)
        {
            battle.GainAttention(SelectedUnit().GetOrCreateFormation());
            yield return battle.AttackUnit(SelectedUnit(), bu);
            yield return battle.WaitForAttention();
            if (SelectedUnit() == null || SelectedUnit().Mp == 0 || !SelectedUnit().IsAlive())
            {
                SelectNextUnit();
            }
            attacking = null;
        }

        public IEnumerator AttackWall(BattleWall bw)
        {
            yield return battle.AttackWall(SelectedUnit(), bw);
            yield return battle.WaitForAttention();
            if (SelectedUnit() == null || SelectedUnit().Mp == 0)
            {
                SelectNextUnit();
            }
            BattleHUD.RefreshSelection();
            attacking = null;
        }

        private void MoveTo(Vector3i pos)
        {
            SelectedUnit().MoveAnimatedTo(pos, battle);
            if (SelectedUnit().Mp == 0)
            {
                HUDEvent(null, "Skip");
            }
            else
            {
                BattleHUD.Get().SetUnitDirty(SelectedUnit());
            }
        }

        private BattleUnit SelectedUnit()
        {
            return BattleHUD.GetSelectedUnit();
        }

        private void TriggerEvent(EEnchantmentType eEnchType, ESkillType eSkillType)
        {
            /*
             * consider TriggerEvent(EEnchantmentType.BattleEndEffect, ESkillType.BattleEndEffect);
             */

            if (eEnchType == EEnchantmentType.BattleEndEffect)
            {
                Debug.Log("TriggerEvent-BattleEndEffect");
            }

            if (battle.buToSource == null)
            {
                return;
            }
            List<BattleUnit> list = new List<BattleUnit>(battle.buToSource.Keys);
            List<BattleUnit> list2 = Serializer.DeepClone(list);
            battle.TriggerScripts(eEnchType);
            battle.GetPlayer(attacker: true).TriggerScripts(eEnchType);
            battle.GetPlayer(attacker: false).TriggerScripts(eEnchType);
            if (battle.buToSource != null)
            {
                foreach (BattleUnit bu in list)
                {
                    if (bu.IsAlive())
                    {
                        bu.TriggerScripts(eEnchType);
                        if (bu.IsAlive())
                        {
                            bu.TriggerSkillScripts(eSkillType);
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

        private void TriggerEventForSide(bool attacker, bool includeBattle, EEnchantmentType eEnchType, ESkillType eSkillType)
        {
            if (battle.buToSource == null)
            {
                return;
            }
            List<BattleUnit> list = new List<BattleUnit>(battle.buToSource.Keys);
            List<BattleUnit> list2 = Serializer.DeepClone(list);
            if (includeBattle)
            {
                battle.TriggerScripts(eEnchType);
            }
            battle.GetPlayer(attacker).TriggerScripts(eEnchType);
            if (battle.buToSource != null)
            {
                foreach (BattleUnit item in new List<BattleUnit>(attacker ? battle.attackerUnits : battle.defenderUnits))
                {
                    item.TriggerScripts(eEnchType);
                    item.TriggerSkillScripts(eSkillType);
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
            return !busy;
        }
    }
}
