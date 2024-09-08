namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Moves a Vector2 towards a Target. Optionally sends an event when successful.")]
    public class Vector2MoveTowards : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Vector2 to Move")]
        public FsmVector2 source;
        [HutongGames.PlayMaker.Tooltip("A target Vector2 to move towards.")]
        public FsmVector2 target;
        [HasFloatSlider(0f, 20f), HutongGames.PlayMaker.Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
        public FsmFloat maxSpeed;
        [HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
        public FsmFloat finishDistance;
        [HutongGames.PlayMaker.Tooltip("Event to send when the Finish Distance is reached.")]
        public FsmEvent finishEvent;

        private void DoMoveTowards()
        {
            this.source.set_Value(Vector2.MoveTowards(this.source.get_Value(), this.target.get_Value(), this.maxSpeed.Value * Time.deltaTime));
            if ((this.source.get_Value() - this.target.get_Value()).magnitude < this.finishDistance.Value)
            {
                base.Fsm.Event(this.finishEvent);
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMoveTowards();
        }

        public override void Reset()
        {
            this.source = null;
            this.target = null;
            this.maxSpeed = 10f;
            this.finishDistance = 1f;
            this.finishEvent = null;
        }
    }
}

