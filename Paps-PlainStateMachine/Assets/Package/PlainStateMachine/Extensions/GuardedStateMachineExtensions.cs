using System;

namespace Paps.StateMachines.Extensions
{
    public static class FSMWithGuardConditionsExtensions
    {
        public static void AddGuardConditionTo<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
                Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            fsm.AddGuardConditionTo(transition, new PredicateGuardCondition(predicate));
        }

        public static void RemoveGuardConditionFrom<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            fsm.RemoveGuardConditionFrom(transition, new PredicateGuardCondition(predicate));
        }

        public static bool ContainsGuardConditionOn<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            return fsm.ContainsGuardConditionOn(transition, new PredicateGuardCondition(predicate));
        }
    }
}