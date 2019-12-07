using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public delegate bool ReturnTrueToFinishIteration<T>(T current);

    public static class IFSMExtensions
    {
        public static void AddTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static void RemoveTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static bool ContainsStateByReference<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, IFSMState stateRef)
        {
            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                if (state == stateRef)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static T GetState<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm)
        {
            T candidate = default;

            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                if (state is T cast)
                {
                    candidate = cast;
                    break;
                }
            }

            return candidate;
        }

        public static T[] GetStates<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm)
        {
            List<T> statesList = null;

            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                if (state is T cast)
                {
                    if (statesList == null)
                    {
                        statesList = new List<T>();
                    }

                    statesList.Add(cast);
                }
            }

            if(statesList != null)
            {
                return statesList.ToArray();
            }

            return null;
        }

        public static T GetStateById<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            T candidate = default;

            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                if (state is T cast)
                {
                    candidate = cast;
                    break;
                }
            }

            return candidate;
        }

        public static void AddTimerState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId, 
            double milliseconds, Action<TState> onTimerElapsed)
        {
            fsm.AddState(stateId, new TimerState<TState, TTrigger>(fsm, milliseconds, onTimerElapsed));
        }

        public static void AddEmpty<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            fsm.AddState(stateId, new FSMState<TState, TTrigger>(fsm));
        }
        
        public static void AddWithEvents<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onUpdate, Action onExit)
        {
            fsm.AddState(stateId, new DelegateFSMState<TState, TTrigger>(fsm, onEnter, onUpdate, onExit));
        }

        public static void ForeachState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, ReturnTrueToFinishIteration<IFSMState> finishable)
        {
            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                if(finishable(state))
                {
                    return;
                }
            }
        }

        public static void ForeachTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, ReturnTrueToFinishIteration<IFSMTransition<TState, TTrigger>> finishable)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (IFSMTransition<TState, TTrigger> transition in transitions)
            {
                if (finishable(transition))
                {
                    return;
                }
            }
        }

        public static void FromAny<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            IFSMState[] states = fsm.GetStates();

            foreach(IFSMState state in states)
            {
                fsm.AddTransition(fsm.GetIdOf(state), trigger, stateTo);
            }
        }

        public static void FromAnyExceptTarget<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            IFSMState[] states = fsm.GetStates();

            foreach (IFSMState state in states)
            {
                TState stateId = fsm.GetIdOf(state);

                if (stateId.Equals(stateTo) == false)
                {
                    fsm.AddTransition(fsm.GetIdOf(state), trigger, stateTo);
                }
            }
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithTrigger<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger)
        {
            List<IFSMTransition<TState, TTrigger>> transitionsList = null;

            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<IFSMTransition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if(transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithStateFrom<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateFrom)
        {
            List<IFSMTransition<TState, TTrigger>> transitionsList = null;

            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateFrom))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<IFSMTransition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithStateTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateTo)
        {
            List<IFSMTransition<TState, TTrigger>> transitionsList = null;

            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateTo.Equals(stateTo))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<IFSMTransition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static bool ContainsTransitionWithStateTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateTo)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();
            
            foreach(var transition in transitions)
            {
                if (transition.StateTo.Equals(stateTo))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsTransitionWithStateFrom<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateFrom)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateFrom))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsTransitionWithTrigger<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    return true;
                }
            }

            return false;
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsRelatedTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            List<IFSMTransition<TState, TTrigger>> transitionsList = null;

            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateId) || transition.StateTo.Equals(stateId))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<IFSMTransition<TState, TTrigger>>();
                    }

                    transitionsList.Add(transition);
                }
            }

            if (transitionsList == null)
            {
                return null;
            }

            return transitionsList.ToArray();
        }

        public static bool ContainsTransitionRelatedTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            var transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateId) || transition.StateTo.Equals(stateId))
                {
                    return true;
                }
            }

            return false;
        }

        public static void RemoveAllTransitions<TState, TTrigger>(this IFSM<TState, TTrigger> fsm)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                fsm.RemoveTransitionWithValuesOf(transition);
            }
        }

        public static void RemoveAllTransitionsRelatedTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            IFSMTransition<TState, TTrigger>[] transitions = fsm.GetTransitionsRelatedTo(stateId);

            foreach (var transition in transitions)
            {
                fsm.RemoveTransitionWithValuesOf(transition);
            }
        }

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
