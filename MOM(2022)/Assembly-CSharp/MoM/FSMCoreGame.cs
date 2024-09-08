using System.Collections;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMCoreGame : FSMStateBase
    {
        public static FSMCoreGame instance;

        private AdventureManager adventureManager;

        private Battle battle;

        public override void OnEnter()
        {
            FSMCoreGame.instance = this;
            base.OnEnter();
            this.adventureManager = new AdventureManager();
            World.GetActivePlane();
        }

        public static FSMCoreGame Get()
        {
            return FSMCoreGame.instance;
        }

        public static AdventureManager GetAdvManager()
        {
            return FSMCoreGame.Get().adventureManager;
        }

        public override void OnExit()
        {
            if (FSMCoreGame.Get() != null && FSMCoreGame.Get().adventureManager != null)
            {
                FSMCoreGame.Get().adventureManager.Destroy();
                FSMCoreGame.Get().adventureManager = null;
            }
            FSMCoreGame.instance = null;
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
            base.OnExit();
        }

        public void StartBattle(Battle b)
        {
            if (!b.attacker.playerOwner && !b.defender.playerOwner)
            {
                b.attacker.GetWizardOwner()?.GetDiplomacy().Attacked(b.defender.GetWizardOwner(), b.gDefender);
                base.StartCoroutine(this.AIvsAIbattle(b));
            }
            else
            {
                base.StartCoroutine(this.PlayerBattle());
            }
        }

        private IEnumerator PlayerBattle(bool enterBattle = true)
        {
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement))
            {
                yield return null;
            }
            FsmEventTarget fsmEventTarget = new FsmEventTarget();
            fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
            base.Fsm.Event(fsmEventTarget, enterBattle ? "EnterBattle" : "PopupTownCaptured");
        }

        private IEnumerator AIvsAIbattle(Battle b)
        {
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement))
            {
                yield return null;
            }
            bool num = b.attacker.wizard != null;
            bool flag = b.defender.wizard != null;
            int num2 = (num ? 1 : 0) + (flag ? (-1) : 0);
            int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
            int iterations = ((num2 == 0) ? 1 : settingAsInt);
            MHTimer t = MHTimer.StartNew();
            BattleResult br = new BattleResult();
            yield return PowerEstimate.SimulatedBattle(b, iterations, br, num2);
            b.ApplyResultsToUnits(br);
            b.ApplyManaUses(br);
            Battle.PrepareChanges(b);
            Battle.ApplyBattleChanges(b);
            Debug.Log("! AI vs AI battle took " + t.GetTime());
        }

        public bool HudButton(string name)
        {
            if (TurnManager.Get() == null || !TurnManager.Get().playerTurn)
            {
                return false;
            }
            switch (name)
            {
            case "ButtonGame":
                UIManager.Open<PauseMenu>(UIManager.Layer.Standard);
                return true;
            case "ButtonResearch":
                if (this.CanResearch())
                {
                    UIManager.Open<ResearchSpells>(UIManager.Layer.Standard);
                }
                return true;
            case "ButtonCast":
                UIManager.Open<CastSpells>(UIManager.Layer.Standard);
                return true;
            case "ButtonMagic":
                UIManager.Open<Magic>(UIManager.Layer.Standard);
                return true;
            case "ButtonArmies":
                UIManager.Open<ArmyManager>(UIManager.Layer.Standard);
                return true;
            case "ButtonDiplomacy":
                UIManager.Open<Diplomacy>(UIManager.Layer.Standard);
                return true;
            case "ButtonInfo":
                UIManager.Open<Stats>(UIManager.Layer.Standard);
                return true;
            case "ButtonCities":
                UIManager.Open<CityManager>(UIManager.Layer.Standard);
                return true;
            default:
                return false;
            }
        }

        public bool CanResearch()
        {
            return GameManager.GetHumanWizard().GetMagicAndResearch().curentResearchOptions.Count > 0;
        }
    }
}
