namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.GUILayout), Tooltip("Turn GUILayout on/off. If you don't use GUILayout actions you can get some performance back by turning GUILayout off. This can make a difference on iOS platforms.")]
    public class UseGUILayout : FsmStateAction
    {
        [RequiredField]
        public bool turnOffGUIlayout;

        public override void OnEnter()
        {
            base.Fsm.get_Owner().useGUILayout = !this.turnOffGUIlayout;
            base.Finish();
        }

        public override void Reset()
        {
            this.turnOffGUIlayout = true;
        }
    }
}

