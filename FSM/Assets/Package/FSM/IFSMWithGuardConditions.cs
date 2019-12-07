using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public interface IFSMWithGuardConditions<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        void AddGuardConditionTo(TState stateFrom, TTrigger trigger, TState stateTo, IGuardCondition<TState, TTrigger> guardCondition);
        void RemoveGuardConditionFrom(TState stateFrom, TTrigger trigger, TState stateTo, IGuardCondition<TState, TTrigger> guardCondition);

        bool ContainsGuardConditionOn(TState stateFrom, TTrigger trigger, TState stateTo, IGuardCondition<TState, TTrigger> guardCondition);

        KeyValuePair<IFSMTransition<TState, TTrigger>, IGuardCondition<TState, TTrigger>[]>[] GetGuardConditions();
    }
}
