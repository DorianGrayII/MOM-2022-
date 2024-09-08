using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Stops the animator record mode. It will lock the recording buffer's contents in its current state. The data get saved for subsequent playback with StartPlayback.")]
    public class AnimatorStopRecording : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The recorder StartTime")]
        public FsmFloat recorderStartTime;

        [UIHint(UIHint.Variable)]
        [Tooltip("The recorder StopTime")]
        public FsmFloat recorderStopTime;

        public override void Reset()
        {
            this.gameObject = null;
            this.recorderStartTime = null;
            this.recorderStopTime = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            Animator component = ownerDefaultTarget.GetComponent<Animator>();
            if (component != null)
            {
                component.StopRecording();
                this.recorderStartTime.Value = component.recorderStartTime;
                this.recorderStopTime.Value = component.recorderStopTime;
            }
            base.Finish();
        }
    }
}
