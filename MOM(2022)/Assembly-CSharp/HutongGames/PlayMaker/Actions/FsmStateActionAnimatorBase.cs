// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.FsmStateActionAnimatorBase
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

public abstract class FsmStateActionAnimatorBase : FsmStateAction
{
    public enum AnimatorFrameUpdateSelector
    {
        OnUpdate = 0,
        OnAnimatorMove = 1,
        OnAnimatorIK = 2
    }

    [Tooltip("Repeat every frame.")]
    public bool everyFrame;

    [Tooltip("Select when to perform the action, during OnUpdate, OnAnimatorMove, OnAnimatorIK")]
    public AnimatorFrameUpdateSelector everyFrameOption;

    protected int IklayerIndex;

    public abstract void OnActionUpdate();

    public override void Reset()
    {
        this.everyFrame = false;
        this.everyFrameOption = AnimatorFrameUpdateSelector.OnUpdate;
    }

    public override void OnPreprocess()
    {
        if (this.everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorMove)
        {
            base.Fsm.HandleAnimatorMove = true;
        }
        if (this.everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorIK)
        {
            base.Fsm.HandleAnimatorIK = true;
        }
    }

    public override void OnUpdate()
    {
        if (this.everyFrameOption == AnimatorFrameUpdateSelector.OnUpdate)
        {
            this.OnActionUpdate();
        }
        if (!this.everyFrame)
        {
            base.Finish();
        }
    }

    public override void DoAnimatorMove()
    {
        if (this.everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorMove)
        {
            this.OnActionUpdate();
        }
        if (!this.everyFrame)
        {
            base.Finish();
        }
    }

    public override void DoAnimatorIK(int layerIndex)
    {
        this.IklayerIndex = layerIndex;
        if (this.everyFrameOption == AnimatorFrameUpdateSelector.OnAnimatorIK)
        {
            this.OnActionUpdate();
        }
        if (!this.everyFrame)
        {
            base.Finish();
        }
    }
}
