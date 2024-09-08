namespace HutongGames.PlayMaker.Actions
{
    using System;
    using UnityEngine;

    public abstract class BaseAnimationAction : ComponentAction<Animation>
    {
        protected BaseAnimationAction()
        {
        }

        public override void OnActionTargetInvoked(object targetObject)
        {
            AnimationClip clip = targetObject as AnimationClip;
            if (clip != null)
            {
                Animation component = base.get_Owner().GetComponent<Animation>();
                if (component != null)
                {
                    component.AddClip(clip, clip.name);
                }
            }
        }
    }
}

