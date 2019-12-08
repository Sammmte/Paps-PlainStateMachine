using System;

namespace Paps.FSM.Extensions
{
    public static class FSMWithGuardConditionsExtensions
    {
        public static void AddGuardConditionTo<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
                TState stateFrom, TTrigger trigger, TState stateTo, Func<bool> predicate)
        {
            fsm.AddGuardConditionTo(stateFrom, trigger, stateTo, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }

        public static void RemoveGuardConditionFrom<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<bool> predicate)
        {
            fsm.RemoveGuardConditionFrom(stateFrom, trigger, stateTo, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }

        public static bool ContainsGuardConditionOn<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<bool> predicate)
        {
            return fsm.ContainsGuardConditionOn(stateFrom, trigger, stateTo, new PredicateGuardCondition<TState, TTrigger>(predicate));
        }

        public static void AddGuardConditionTo<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> predicate)
        {
            fsm.AddGuardConditionTo(stateFrom, trigger, stateTo, new PredicateWithParametersGuardCondition<TState, TTrigger>(predicate));
        }

        public static void RemoveGuardConditionFrom<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> predicate)
        {
            fsm.RemoveGuardConditionFrom(stateFrom, trigger, stateTo, new PredicateWithParametersGuardCondition<TState, TTrigger>(predicate));
        }

        public static bool ContainsGuardConditionOn<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> predicate)
        {
            return fsm.ContainsGuardConditionOn(stateFrom, trigger, stateTo, new PredicateWithParametersGuardCondition<TState, TTrigger>(predicate));
        }
    }
}