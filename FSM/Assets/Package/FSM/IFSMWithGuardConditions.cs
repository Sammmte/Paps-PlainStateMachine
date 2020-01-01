using System.Collections.Generic;

namespace Paps.FSM
{
    public interface IFSMWithGuardConditions<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition<TState, TTrigger> guardCondition);
        void RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition<TState, TTrigger> guardCondition);

        bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition<TState, TTrigger> guardCondition);

        KeyValuePair<Transition<TState, TTrigger>, IGuardCondition<TState, TTrigger>[]>[] GetGuardConditions();
    }
}
