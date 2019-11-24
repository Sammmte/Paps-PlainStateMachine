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

        void AddState(TState stateId, IFSMState<TState, TTrigger> state);
        void RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        void ForeachState(ReturnTrueToFinishIteration<IFSMState<TState, TTrigger>> action);

        void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        void ForeachTransition(ReturnTrueToFinishIteration<FSMTransition<TState, TTrigger>> action);

        void SetInitialState(TState stateId);

        bool IsInState(TState stateId);

        TState GetIdOf(IFSMState<TState, TTrigger> state);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);
    }
}
