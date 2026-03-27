using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    protected State currentState;
    protected NPCState currentStateKey;
    protected Dictionary<NPCState, State> states = new();

    protected virtual void Start() { }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(NPCState stateName)
    {
        if (states == null) return;
        if (!states.ContainsKey(stateName)) return;

        if (currentState != null && currentStateKey == stateName) return;

        currentState?.Exit();

        currentState = states[stateName];
        currentStateKey = stateName;

        if (currentState == null) return;

        currentState.Enter();
    }

    public bool IsInState(NPCState state)
    {
        return currentStateKey == state;
    }

    public NPCState GetCurrentStateKey()
    {
        return currentStateKey;
    }
}
public enum NPCState
{
    Idle,
    Roam,
    Investigate,
    Hunt,
    FinalChase
}

