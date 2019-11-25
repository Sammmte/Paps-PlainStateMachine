using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public delegate void StateChanged<TState, TTrigger>(TState stateFrom, TTrigger trigger, TState stateTo);
    public delegate bool ReturnTrueToFinishIteration<T>(T current);

    public interface IFSM<TState, TTrigger>
    {
        event StateChanged<TState, TTrigger> OnStateChanged;

        int StateCount { get; }
        int TransitionCount { get; }

        bool IsStarted { get; }

        TState InitialState { get; }

        IFSM<TState, TTrigger> AddState(TState stateId, IFSMState<TState, TTrigger> state);
        IFSM<TState, TTrigger> RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        void ForeachState(ReturnTrueToFinishIteration<IFSMState<TState, TTrigger>> action);

        IFSM<TState, TTrigger> AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        IFSM<TState, TTrigger> RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        void ForeachTransition(ReturnTrueToFinishIteration<IFSMTransition<TState, TTrigger>> action);

        IFSM<TState, TTrigger> SetInitialState(TState stateId);

        bool IsInState(TState stateId);

        TState GetIdOf(IFSMState<TState, TTrigger> state);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);
    }
}
