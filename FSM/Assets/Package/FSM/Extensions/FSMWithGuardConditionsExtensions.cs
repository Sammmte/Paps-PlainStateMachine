using System;

namespace Paps.FSM.Extensions
{
    public static class FSMWithGuardConditionsExtensions
    {
        public static void AddGuardConditionTo<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
                Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            fsm.AddGuardConditionTo(transition, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }

        public static void RemoveGuardConditionFrom<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            fsm.RemoveGuardConditionFrom(transition, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }

        public static bool ContainsGuardConditionOn<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            return fsm.ContainsGuardConditionOn(transition, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }
    }
}