using System.Collections.Generic;

namespace Paps.FSM
{
    public interface IFSMWithGuardConditions<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);
        void RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        KeyValuePair<Transition<TState, TTrigger>, IGuardCondition[]>[] GetGuardConditions();
    }
}
