namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4758.0"), HutongGames.PlayMaker.Tooltip("Move a GameObject to another GameObject. Works like iTween Move To, but with better performance.")]
    public class MoveObject : EaseFsmAction
    {
        [RequiredField]
        public FsmOwnerDefault objectToMove;
        [RequiredField]
        public FsmGameObject destination;
        private FsmVector3 fromValue;
        private FsmVector3 toVector;
        private FsmVector3 fromVector;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.objectToMove);
            this.fromVector = (FsmVector3) ownerDefaultTarget.transform.position;
            this.toVector = (FsmVector3) this.destination.get_Value().transform.position;
            base.fromFloats = new float[] { this.fromVector.get_Value().x, this.fromVector.get_Value().y, this.fromVector.get_Value().z };
            base.toFloats = new float[] { this.toVector.get_Value().x, this.toVector.get_Value().y, this.toVector.get_Value().z };
            base.resultFloats = new float[] { this.fromVector.get_Value().x, this.fromVector.get_Value().y, this.fromVector.get_Value().z };
            this.finishInNextStep = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.objectToMove);
            ownerDefaultTarget.transform.position = new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]);
            if (this.finishInNextStep)
            {
                base.Finish();
                if (base.finishEvent != null)
                {
                    base.Fsm.Event(base.finishEvent);
                }
            }
            if (base.finishAction && !this.finishInNextStep)
            {
                ownerDefaultTarget.transform.position = new Vector3(base.reverse.IsNone ? this.toVector.get_Value().x : (base.reverse.Value ? this.fromValue.get_Value().x : this.toVector.get_Value().x), base.reverse.IsNone ? this.toVector.get_Value().y : (base.reverse.Value ? this.fromValue.get_Value().y : this.toVector.get_Value().y), base.reverse.IsNone ? this.toVector.get_Value().z : (base.reverse.Value ? this.fromValue.get_Value().z : this.toVector.get_Value().z));
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.fromValue = null;
            this.toVector = null;
            this.finishInNextStep = false;
            this.fromVector = null;
        }
    }
}

