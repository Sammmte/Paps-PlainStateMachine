using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public interface IFSMWithGuardConditions<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        IFSMWithGuardConditions<TState, TTrigger> AddANDGuardConditionTo(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);
        IFSMWithGuardConditions<TState, TTrigger> RemoveANDGuardConditionFrom(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);

        IFSMWithGuardConditions<TState, TTrigger> AddORGuardConditionTo(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);
        IFSMWithGuardConditions<TState, TTrigger> RemoveORGuardConditionFrom(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);

        bool ContainsANDGuardConditionOn(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);
        bool ContainsORGuardConditionOn(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition);
        
    }
}
