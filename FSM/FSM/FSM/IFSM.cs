using System;
using System.Collections.Generic;

namespace Paps.FSM
{
    public interface IFSM<TState, TTrigger> : IEnumerable<IFSMState<TState, TTrigger>>
    {
        int StateCount { get; }

        void AddState(IFSMState<TState, TTrigger> state);
        void RemoveState(IFSMState<TState, TTrigger> state);
    }
}
