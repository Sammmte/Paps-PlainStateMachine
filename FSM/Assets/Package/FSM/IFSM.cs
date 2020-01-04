using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public delegate void StateChange<TState, TTrigger>(IFSM<TState, TTrigger> fsm);

    public interface IFSM<TState, TTrigger>
    {
        int StateCount { get; }
        int TransitionCount { get; }

        event StateChange<TState, TTrigger> OnBeforeStateChanges;
        event StateChange<TState, TTrigger> OnStateChanged;

        bool IsStarted { get; }

        TState InitialState { get; }
        TState CurrentState { get; }

        void AddState(TState stateId, IState state);
        void RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        TState[] GetStates();

        void AddTransition(Transition<TState, TTrigger> transition);
        void RemoveTransition(Transition<TState, TTrigger> transition);

        bool ContainsTransition(Transition<TState, TTrigger> transition);

        Transition<TState, TTrigger>[] GetTransitions();

        void SetInitialState(TState stateId);

        IState GetStateById(TState stateId);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);

        bool SendEvent(IEvent messageEvent);
    }
}