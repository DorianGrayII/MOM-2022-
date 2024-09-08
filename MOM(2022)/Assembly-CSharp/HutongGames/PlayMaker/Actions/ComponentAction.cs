using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public abstract class ComponentAction<T> : FsmStateAction where T : Component
    {
        protected GameObject cachedGameObject;

        protected T cachedComponent;

        protected Rigidbody rigidbody => this.cachedComponent as Rigidbody;

        protected Rigidbody2D rigidbody2d => this.cachedComponent as Rigidbody2D;

        protected Renderer renderer => this.cachedComponent as Renderer;

        protected Animation animation => this.cachedComponent as Animation;

        protected AudioSource audio => this.cachedComponent as AudioSource;

        protected Camera camera => this.cachedComponent as Camera;

        protected Light light => this.cachedComponent as Light;

        protected bool UpdateCache(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            if (this.cachedComponent == null || this.cachedGameObject != go)
            {
                this.cachedComponent = go.GetComponent<T>();
                this.cachedGameObject = go;
                if (this.cachedComponent == null)
                {
                    base.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
                }
            }
            return this.cachedComponent != null;
        }

        protected bool UpdateCacheAddComponent(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            if (this.cachedComponent == null || this.cachedGameObject != go)
            {
                this.cachedComponent = go.GetComponent<T>();
                this.cachedGameObject = go;
                if (this.cachedComponent == null)
                {
                    this.cachedComponent = go.AddComponent<T>();
                    this.cachedComponent.hideFlags = HideFlags.DontSaveInEditor;
                }
            }
            return this.cachedComponent != null;
        }

        protected void SendEvent(FsmEventTarget eventTarget, FsmEvent fsmEvent)
        {
            base.Fsm.Event(this.cachedGameObject, eventTarget, fsmEvent);
        }
    }
}
