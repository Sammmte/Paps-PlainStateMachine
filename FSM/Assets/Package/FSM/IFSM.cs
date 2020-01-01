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

        void AddState(TState stateId, IState state);
        void RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        TState[] GetStates();

        void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        ITransition<TState, TTrigger>[] GetTransitions();

        void SetInitialState(TState stateId);

        bool IsInState(TState stateId);

        IState GetStateById(TState stateId);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);

        bool SendEvent(IEvent messageEvent);
    }
}