using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHUtils
{
    public class State : MonoBehaviour
    {
        public enum StateStatus
        {
            Born = 0,
            PreStart = 1,
            Starting = 2,
            PostStart = 3,
            Active = 4,
            PreClose = 5,
            Closing = 6,
            PostClose = 7,
            Dead = 8,
            MAX = 9
        }

        [HideInInspector]
        public StateStatus stateStatus;

        protected State parent;

        protected List<State> children;

        protected StateMachine stateMachine;

        protected bool haveStarted;

        private static int idMain;

        public int localId;

        public void Initialize(StateMachine sm)
        {
            this.localId = State.idMain++;
            this.stateMachine = sm;
        }

        public void AddChild(State s)
        {
            if (s.parent != null)
            {
                s.parent.RemoveChild(s);
            }
            if (this.children == null)
            {
                this.children = new List<State>();
            }
            this.children.Add(s);
            s.parent = this;
        }

        public void RemoveChild(State s)
        {
            if (this.children != null && this.children.Contains(s))
            {
                this.children.Remove(s);
                s.parent = null;
            }
        }

        public IEnumerator RemoveChildren()
        {
            if (this.children != null)
            {
                while (this.children != null && this.children.Count > 0)
                {
                    yield return this.stateMachine.StateClosing(this.children[this.children.Count - 1]);
                }
                this.children = null;
            }
        }

        public virtual IEnumerator PreStart()
        {
            this.stateStatus = StateStatus.PreStart;
            yield break;
        }

        protected virtual void Start()
        {
            this.haveStarted = true;
        }

        public virtual void OnStart()
        {
        }

        public virtual IEnumerator Starting()
        {
            this.stateStatus = StateStatus.Starting;
            while (!this.haveStarted)
            {
                yield return null;
            }
            this.OnStart();
        }

        public virtual IEnumerator PostStart()
        {
            this.stateStatus = StateStatus.PostStart;
            yield break;
        }

        public virtual void Activated()
        {
        }

        public virtual void UpdateState()
        {
        }

        public virtual IEnumerator PreClose()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
            this.stateStatus = StateStatus.PreClose;
            if (this.parent != null)
            {
                this.parent.RemoveChild(this);
                this.parent = null;
            }
            yield return this.RemoveChildren();
        }

        public virtual IEnumerator Closing()
        {
            this.stateStatus = StateStatus.Closing;
            yield break;
        }

        public virtual IEnumerator PostClose()
        {
            this.stateStatus = StateStatus.PostClose;
            yield break;
        }

        public bool IsInChildrenTree(State state)
        {
            if (this.children == null || this.children.Count == 0)
            {
                return false;
            }
            foreach (State child in this.children)
            {
                if (child == state || child.IsInChildrenTree(state))
                {
                    return true;
                }
            }
            return false;
        }

        public List<State> GetAllChildren()
        {
            if (this.children == null || this.children.Count == 0)
            {
                return null;
            }
            List<State> list = new List<State>(this.children);
            foreach (State child in this.children)
            {
                List<State> allChildren = child.GetAllChildren();
                if (allChildren != null)
                {
                    list.AddRange(allChildren);
                }
            }
            return list;
        }

        public State GetRoot()
        {
            State state = this;
            while (state.parent != null)
            {
                state = state.parent;
            }
            return state;
        }

        public State GetParent()
        {
            return this.parent;
        }

        public List<State> GetWholeFamily()
        {
            return this.GetRoot().GetAllChildren();
        }

        public virtual bool HandleEvent(object o)
        {
            if (this.stateStatus != StateStatus.Active)
            {
                return false;
            }
            if (this.children != null)
            {
                foreach (State child in this.children)
                {
                    if (child.stateStatus == StateStatus.Active && child.HandleEvent(o))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
