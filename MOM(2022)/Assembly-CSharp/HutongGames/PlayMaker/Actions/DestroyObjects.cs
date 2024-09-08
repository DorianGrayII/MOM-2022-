using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Destroys GameObjects in an array.")]
    public class DestroyObjects : FsmStateAction
    {
        [RequiredField]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        [Tooltip("The GameObjects to destroy.")]
        public FsmArray gameObjects;

        [HasFloatSlider(0f, 5f)]
        [Tooltip("Optional delay before destroying the Game Objects.")]
        public FsmFloat delay;

        [Tooltip("Detach children before destroying the Game Objects.")]
        public FsmBool detachChildren;

        public override void Reset()
        {
            this.gameObjects = null;
            this.delay = 0f;
        }

        public override void OnEnter()
        {
            if (this.gameObjects.Values != null)
            {
                object[] values = this.gameObjects.Values;
                for (int i = 0; i < values.Length; i++)
                {
                    GameObject gameObject = (GameObject)values[i];
                    if (gameObject != null)
                    {
                        if (this.delay.Value <= 0f)
                        {
                            Object.Destroy(gameObject);
                        }
                        else
                        {
                            Object.Destroy(gameObject, this.delay.Value);
                        }
                        if (this.detachChildren.Value)
                        {
                            gameObject.transform.DetachChildren();
                        }
                    }
                }
            }
            base.Finish();
        }
    }
}
