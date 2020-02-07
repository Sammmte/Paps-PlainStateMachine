using System.Collections.Generic;

namespace Paps.StateMachines
{
    public interface IGuardedStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);
        void RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        KeyValuePair<Transition<TState, TTrigger>, IGuardCondition[]>[] GetGuardConditions();
    }
}
