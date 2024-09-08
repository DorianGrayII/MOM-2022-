namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    public abstract class ComponentAction<T> : FsmStateAction where T: Component
    {
        protected GameObject cachedGameObject;
        protected T cachedComponent;

        protected ComponentAction()
        {
        }

        protected void SendEvent(FsmEventTarget eventTarget, FsmEvent fsmEvent)
        {
            base.Fsm.Event(this.cachedGameObject, eventTarget, fsmEvent);
        }

        protected bool UpdateCache(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            if ((this.cachedComponent == null) || (this.cachedGameObject != go))
            {
                this.cachedComponent = go.GetComponent<T>();
                this.cachedGameObject = go;
                if (this.cachedComponent == null)
                {
                    base.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
                }
            }
            return (this.cachedComponent != null);
        }

        protected bool UpdateCacheAddComponent(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            if ((this.cachedComponent == null) || (this.cachedGameObject != go))
            {
                this.cachedComponent = go.GetComponent<T>();
                this.cachedGameObject = go;
                if (this.cachedComponent == null)
                {
                    this.cachedComponent = go.AddComponent<T>();
                    this.cachedComponent.hideFlags = HideFlags.DontSaveInEditor;
                }
            }
            return (this.cachedComponent != null);
        }

        protected Rigidbody rigidbody
        {
            get
            {
                return (this.cachedComponent as Rigidbody);
            }
        }

        protected Rigidbody2D rigidbody2d
        {
            get
            {
                return (this.cachedComponent as Rigidbody2D);
            }
        }

        protected Renderer renderer
        {
            get
            {
                return (this.cachedComponent as Renderer);
            }
        }

        protected Animation animation
        {
            get
            {
                return (this.cachedComponent as Animation);
            }
        }

        protected AudioSource audio
        {
            get
            {
                return (this.cachedComponent as AudioSource);
            }
        }

        protected Camera camera
        {
            get
            {
                return (this.cachedComponent as Camera);
            }
        }

        protected Light light
        {
            get
            {
                return (this.cachedComponent as Light);
            }
        }
    }
}

