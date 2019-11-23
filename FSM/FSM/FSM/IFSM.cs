using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public interface IFSM<TState, TTrigger>
    {
        int StateCount { get; }
        int TransitionCount { get; }

        void AddState(IFSMState<TState, TTrigger> state);
        void RemoveState(IFSMState<TState, TTrigger> state);

        void ForeachState(Action<IFSMState<TState, TTrigger>> action);

        void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo);
        void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        void ForeachTransition(Action<FSMTransition<TState, TTrigger>> action);
    }
}
