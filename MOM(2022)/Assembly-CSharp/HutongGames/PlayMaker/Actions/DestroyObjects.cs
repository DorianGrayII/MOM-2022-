namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Destroys GameObjects in an array.")]
    public class DestroyObjects : FsmStateAction
    {
        [RequiredField, ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000), HutongGames.PlayMaker.Tooltip("The GameObjects to destroy.")]
        public FsmArray gameObjects;
        [HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Optional delay before destroying the Game Objects.")]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Detach children before destroying the Game Objects.")]
        public FsmBool detachChildren;

        public override void OnEnter()
        {
            if (this.gameObjects.Values != null)
            {
                foreach (GameObject obj2 in this.gameObjects.Values)
                {
                    if (obj2 != null)
                    {
                        if (this.delay.Value <= 0f)
                        {
                            UnityEngine.Object.Destroy(obj2);
                        }
                        else
                        {
                            UnityEngine.Object.Destroy(obj2, this.delay.Value);
                        }
                        if (this.detachChildren.Value)
                        {
                            obj2.transform.DetachChildren();
                        }
                    }
                }
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObjects = null;
            this.delay = 0f;
        }
    }
}

