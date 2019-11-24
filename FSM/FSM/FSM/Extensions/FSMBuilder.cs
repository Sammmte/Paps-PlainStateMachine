using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public class FSMBuilder<TState, TTrigger>
    {
        public IFSM<TState, TTrigger> InnerFSM { get; protected set; }

        public FSMBuilder(IFSM<TState, TTrigger> fsm)
        {
            InnerFSM = fsm;
        }

        public FSMBuilder<TState, TTrigger> AddState(TState stateId, IFSMState<TState, TTrigger> state)
        {
            InnerFSM.AddState(stateId, state);

            return this;
        }

        public FSMBuilder<TState, TTrigger> RemoveState(TState stateId)
        {
            InnerFSM.RemoveState(stateId);

            return this;
        }

        public FSMBuilder<TState, TTrigger> AddTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            InnerFSM.AddTransition(stateFrom, trigger, stateTo);

            return this;
        }

        public FSMBuilder<TState, TTrigger> RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            InnerFSM.RemoveTransition(stateFrom, trigger, stateTo);

            return this;
        }
    }
}
