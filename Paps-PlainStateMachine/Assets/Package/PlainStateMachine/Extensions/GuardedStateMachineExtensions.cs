using System;

namespace Paps.StateMachines.Extensions
{
    public static class GuardedStateMachineExtensions
    {
        public static void AddGuardConditionsTo<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, params IGuardCondition[] guardConditions)
        {
            foreach (var guardCondition in guardConditions)
            {
                fsm.AddGuardConditionTo(transition, guardCondition);
            }
        }
        
        public static void AddGuardConditionTo<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
                Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            fsm.AddGuardConditionTo(transition, new PredicateGuardCondition(predicate));
        }

        public static bool RemoveGuardConditionFrom<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            return fsm.RemoveGuardConditionFrom(transition, new PredicateGuardCondition(predicate));
        }

        public static bool ContainsGuardConditionOn<TState, TTrigger>(this IGuardedStateMachine<TState, TTrigger> fsm,
            Transition<TState, TTrigger> transition, Func<bool> predicate)
        {
            return fsm.ContainsGuardConditionOn(transition, new PredicateGuardCondition(predicate));
        }
    }
}