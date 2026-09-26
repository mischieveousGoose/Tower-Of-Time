using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;


public abstract class Boss<TState> : MonoBehaviour
    where TState : Enum
{
    protected TState CurrentState { get; private set; }

    private Dictionary<TState, Action> stateActions = new();

    protected void RegisterState(TState state, Action action)
    {
        stateActions[state] = action;
    }

    protected void ChangeState(TState newState)
    {
        CurrentState = newState;
    }

    protected void GetNewState()
    {
        if (stateActions.Count <= 1)
        {
            if (stateActions.Count == 1)
            {
                // only one state exists, just re-enter it
                ChangeState(new List<TState>(stateActions.Keys)[0]);
            }
            return;
        }

        TState lastState = CurrentState;
        TState newState;

        do
        {
            newState = GetRandomState();
        }
        while (newState.Equals(lastState));

        ChangeState(newState);
    }

    private TState GetRandomState()
    {
        var keys = new List<TState>(stateActions.Keys);
        return keys[UnityEngine.Random.Range(0, keys.Count)];
    }

    void Update()
    {
        if (stateActions.TryGetValue(CurrentState, out var action))
        {
            action();
        }
    }
}