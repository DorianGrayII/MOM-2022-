using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4758.0")]
    [Tooltip("Move a GameObject to another GameObject. Works like iTween Move To, but with better performance.")]
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

        public override void Reset()
        {
            base.Reset();
            this.fromValue = null;
            this.toVector = null;
            this.finishInNextStep = false;
            this.fromVector = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.objectToMove);
            this.fromVector = ownerDefaultTarget.transform.position;
            this.toVector = this.destination.Value.transform.position;
            base.fromFloats = new float[3];
            base.fromFloats[0] = this.fromVector.Value.x;
            base.fromFloats[1] = this.fromVector.Value.y;
            base.fromFloats[2] = this.fromVector.Value.z;
            base.toFloats = new float[3];
            base.toFloats[0] = this.toVector.Value.x;
            base.toFloats[1] = this.toVector.Value.y;
            base.toFloats[2] = this.toVector.Value.z;
            base.resultFloats = new float[3];
            base.resultFloats[0] = this.fromVector.Value.x;
            base.resultFloats[1] = this.fromVector.Value.y;
            base.resultFloats[2] = this.fromVector.Value.z;
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
                ownerDefaultTarget.transform.position = new Vector3(base.reverse.IsNone ? this.toVector.Value.x : (base.reverse.Value ? this.fromValue.Value.x : this.toVector.Value.x), base.reverse.IsNone ? this.toVector.Value.y : (base.reverse.Value ? this.fromValue.Value.y : this.toVector.Value.y), base.reverse.IsNone ? this.toVector.Value.z : (base.reverse.Value ? this.fromValue.Value.z : this.toVector.Value.z));
                this.finishInNextStep = true;
            }
        }
    }
}
