namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
    public class Translate : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The game object to translate.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), Readonly, HutongGames.PlayMaker.Tooltip("A translation vector. NOTE: You can override individual axis below.")]
        public FsmVector3 vector;
        [HutongGames.PlayMaker.Tooltip("Translation along x axis.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Translation along y axis.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Translation along z axis.")]
        public FsmFloat z;
        [HutongGames.PlayMaker.Tooltip("Translate in local or world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Translate over one second")]
        public bool perSecond;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;
        [HutongGames.PlayMaker.Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
        public bool fixedUpdate;

        private void DoTranslate()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 translation = this.vector.IsNone ? new Vector3(this.x.Value, this.y.Value, this.z.Value) : this.vector.get_Value();
                if (!this.x.IsNone)
                {
                    translation.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    translation.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    translation.z = this.z.Value;
                }
                if (!this.perSecond)
                {
                    ownerDefaultTarget.transform.Translate(translation, this.space);
                }
                else
                {
                    ownerDefaultTarget.transform.Translate(translation * Time.deltaTime, this.space);
                }
            }
        }

        public override void OnEnter()
        {
            if (!this.everyFrame && (!this.lateUpdate && !this.fixedUpdate))
            {
                this.DoTranslate();
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (this.fixedUpdate)
            {
                this.DoTranslate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoTranslate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnPreprocess()
        {
            if (this.fixedUpdate)
            {
                base.Fsm.HandleFixedUpdate = true;
            }
            if (this.lateUpdate)
            {
                base.Fsm.HandleLateUpdate = true;
            }
        }

        public override void OnUpdate()
        {
            if (!this.lateUpdate && !this.fixedUpdate)
            {
                this.DoTranslate();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
            this.space = Space.Self;
            this.perSecond = true;
            this.everyFrame = true;
            this.lateUpdate = false;
            this.fixedUpdate = false;
        }
    }
}

