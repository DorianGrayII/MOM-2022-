// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.StateMachine
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<State, Coroutine> stateCollection = new Dictionary<State, Coroutine>();

    private List<State> openStates = new List<State>();

    private List<State> statesRequestedToClose = new List<State>();

    private List<int> busy = new List<int>();

    public void OnDestroy()
    {
        while (this.openStates.Count > 0)
        {
            Object.Destroy(this.openStates[this.openStates.Count - 1]);
            this.openStates.RemoveAt(this.openStates.Count - 1);
        }
    }

    public T OpenState<T>(bool closeUnrelevant, State parent = null) where T : State
    {
        T val = base.gameObject.AddComponent(typeof(T)) as T;
        if (val is ScreenBase)
        {
            Debug.LogError("[ERROR]Screen base manages its own states locally with visual components");
        }
        Coroutine value = base.StartCoroutine(this.StateOpening(val, closeUnrelevant, parent));
        this.stateCollection[val] = value;
        return val;
    }

    public void OpenState(State state, bool closeUnrelevant, State parent = null)
    {
        Coroutine value = base.StartCoroutine(this.StateOpening(state, closeUnrelevant, parent));
        this.stateCollection[state] = value;
    }

    public void CloseState(State state)
    {
        if (this.stateCollection.ContainsKey(state))
        {
            if (state.stateStatus != State.StateStatus.Active)
            {
                this.statesRequestedToClose.Add(state);
                return;
            }
            base.StopCoroutine(this.stateCollection[state]);
        }
        Coroutine value = base.StartCoroutine(this.StateClosing(state));
        this.stateCollection[state] = value;
    }

    public IEnumerator StateOpening(State state, bool closeUnrelevant, State parent = null)
    {
        while (this.busy.Count > 0)
        {
            yield return null;
        }
        state.Initialize(this);
        this.busy.Add(state.localId);
        if (closeUnrelevant)
        {
            List<State> toRemove;
            if (parent != null)
            {
                yield return parent.RemoveChildren();
                toRemove = this.openStates;
                List<State> protectedStates = parent.GetWholeFamily();
                if (protectedStates != null)
                {
                    toRemove = toRemove.FindAll((State o) => protectedStates.Contains(o));
                }
            }
            else
            {
                toRemove = this.openStates;
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                State state2 = toRemove[i];
                if (!(state2 == null) && state2.stateStatus <= State.StateStatus.Active && this.openStates.Contains(state2))
                {
                    yield return this.StateClosing(state2);
                }
            }
        }
        this.openStates.Add(state);
        if (parent != null)
        {
            parent.AddChild(state);
        }
        yield return state.PreStart();
        yield return state.Starting();
        yield return state.PostStart();
        state.stateStatus = State.StateStatus.Active;
        state.Activated();
        if (state.stateStatus == State.StateStatus.Active)
        {
            if (this.statesRequestedToClose.Contains(state))
            {
                this.statesRequestedToClose.Remove(state);
                this.CloseState(state);
                this.busy.Remove(state.localId);
                yield break;
            }
            this.stateCollection[state] = base.StartCoroutine(this.UpdateState(state));
        }
        this.busy.Remove(state.localId);
    }

    public IEnumerator UpdateState(State state)
    {
        while (state.stateStatus == State.StateStatus.Active)
        {
            state.UpdateState();
            yield return null;
        }
    }

    public IEnumerator StateClosing(State state)
    {
        this.busy.Add(state.localId);
        yield return state.PreClose();
        yield return state.Closing();
        yield return state.PostClose();
        state.stateStatus = State.StateStatus.Dead;
        this.openStates.Remove(state);
        if (!(state is ScreenBase))
        {
            Object.Destroy(state);
        }
        if (this.stateCollection.ContainsKey(state))
        {
            this.stateCollection.Remove(state);
        }
        this.busy.Remove(state.localId);
    }

    public virtual bool HandleEvent(object o)
    {
        foreach (State item in new List<State>(this.openStates))
        {
            if (item != null && item.stateStatus < State.StateStatus.Closing && item.GetParent() == null && item.HandleEvent(o))
            {
                return true;
            }
        }
        return false;
    }

    public T GetState<T>() where T : State
    {
        return this.openStates.Find((State o) => o is T) as T;
    }
}
