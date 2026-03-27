using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    protected State currentState;
    protected Dictionary<NPCState, State> states = new();

    protected virtual void Start() { }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(NPCState stateName)
    {
        if (!states.ContainsKey(stateName)) return;

        currentState?.Exit();
        currentState = states[stateName];
        currentState.Enter();
    }

    public State GetCurrentState()
    {
        return currentState;
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

