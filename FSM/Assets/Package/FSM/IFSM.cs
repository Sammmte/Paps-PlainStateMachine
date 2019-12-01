using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public interface IFSM<TState, TTrigger>
    {
        int StateCount { get; }
        int TransitionCount { get; }

        bool IsStarted { get; }

        TState InitialState { get; }

        void AddState(TState stateId, IFSMState state);
        void RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        IFSMState[] GetStates();

        void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        IFSMTransition<TState, TTrigger>[] GetTransitions();

        void SetInitialState(TState stateId);

        bool IsInState(TState stateId);

        TState GetIdOf(IFSMState state);

        void Start();
        void Update();
        void Stop();

        void Trigger(TTrigger trigger);
    }
}
