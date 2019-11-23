using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public delegate void StateChanged<TState, TTrigger>(TState stateFrom, TTrigger trigger, TState stateTo);

    public interface IFSM<TState, TTrigger>
    {
        event StateChanged<TState, TTrigger> OnStateChanged;

        int StateCount { get; }
        int TransitionCount { get; }

        bool IsStarted { get; }

        TState InitialState { get; }

        void AddState(IFSMState<TState, TTrigger> state);
        void RemoveState(IFSMState<TState, TTrigger> state);

        bool ContainsState(TState stateId);

        void ForeachState(Action<IFSMState<TState, TTrigger>> action);

        void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        void ForeachTransition(Action<FSMTransition<TState, TTrigger>> action);

        void SetInitialState(TState stateId);

        bool IsInState(TState stateId);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);
    }
}
