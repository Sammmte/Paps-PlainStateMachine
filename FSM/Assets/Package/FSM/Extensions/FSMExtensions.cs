using System;
using System.Collections.Generic;

namespace Paps.FSM.Extensions
{
    public delegate bool ReturnTrueToFinishIteration<T>(T current);

    public static class IFSMExtensions
    {
        public static void AddTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : ITransition<TState, TTrigger>
        {
            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static void RemoveTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : ITransition<TState, TTrigger>
        {
            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static bool ContainsStateByReference<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, IState stateRef)
        {
            TState[] states = fsm.GetStates();

            foreach(TState state in states)
            {
                if (fsm.GetStateById(state) == stateRef)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static T GetState<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm)
        {
            T candidate = default;

            TState[] states = fsm.GetStates();

            foreach(TState state in states)
            {
                if (fsm.GetStateById(state) is T cast)
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

            TState[] states = fsm.GetStates();

            foreach(TState state in states)
            {
                if (fsm.GetStateById(state) is T cast)
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

        public static void AddTimerState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId, 
            double milliseconds, Action<TState> onTimerElapsed)
        {
            fsm.AddState(stateId, new TimerState<TState, TTrigger>(fsm, stateId, milliseconds, onTimerElapsed));
        }

        public static void AddEmpty<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            fsm.AddState(stateId, new EmptyState());
        }
        
        public static void AddWithEvents<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onUpdate, Action onExit)
        {
            fsm.AddState(stateId, new DelegateState<TState, TTrigger>(onEnter, onUpdate, onExit));
        }

        public static void AddWithEvents<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId,
            Action onEnter)
        {
            fsm.AddState(stateId, new DelegateState<TState, TTrigger>(onEnter, null, null));
        }

        public static void AddWithEvents<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onExit)
        {
            fsm.AddState(stateId, new DelegateState<TState, TTrigger>(onEnter, null, onExit));
        }

        public static void ForeachState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, ReturnTrueToFinishIteration<IState> finishable)
        {
            TState[] states = fsm.GetStates();

            foreach(TState state in states)
            {
                if(finishable(fsm.GetStateById(state)))
                {
                    return;
                }
            }
        }

        public static void ForeachTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, ReturnTrueToFinishIteration<ITransition<TState, TTrigger>> finishable)
        {
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (ITransition<TState, TTrigger> transition in transitions)
            {
                if (finishable(transition))
                {
                    return;
                }
            }
        }

        public static void FromAny<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            TState[] states = fsm.GetStates();

            foreach(TState state in states)
            {
                fsm.AddTransition(state, trigger, stateTo);
            }
        }

        public static void FromAnyExceptTarget<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            TState[] states = fsm.GetStates();

            foreach (TState stateId in states)
            {
                if (stateId.Equals(stateTo) == false)
                {
                    fsm.AddTransition(stateId, trigger, stateTo);
                }
            }
        }

        public static ITransition<TState, TTrigger>[] GetTransitionsWithTrigger<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger)
        {
            List<ITransition<TState, TTrigger>> transitionsList = null;

            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<ITransition<TState, TTrigger>>();
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

        public static ITransition<TState, TTrigger>[] GetTransitionsWithStateFrom<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateFrom)
        {
            List<ITransition<TState, TTrigger>> transitionsList = null;

            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateFrom))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<ITransition<TState, TTrigger>>();
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

        public static ITransition<TState, TTrigger>[] GetTransitionsWithStateTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateTo)
        {
            List<ITransition<TState, TTrigger>> transitionsList = null;

            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.StateTo.Equals(stateTo))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<ITransition<TState, TTrigger>>();
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
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();
            
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
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

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
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach (var transition in transitions)
            {
                if (transition.Trigger.Equals(trigger))
                {
                    return true;
                }
            }

            return false;
        }

        public static ITransition<TState, TTrigger>[] GetTransitionsRelatedTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            List<ITransition<TState, TTrigger>> transitionsList = null;

            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                if (transition.StateFrom.Equals(stateId) || transition.StateTo.Equals(stateId))
                {
                    if (transitionsList == null)
                    {
                        transitionsList = new List<ITransition<TState, TTrigger>>();
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
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitions();

            foreach(var transition in transitions)
            {
                fsm.RemoveTransitionWithValuesOf(transition);
            }
        }

        public static void RemoveAllTransitionsRelatedTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            ITransition<TState, TTrigger>[] transitions = fsm.GetTransitionsRelatedTo(stateId);

            foreach (var transition in transitions)
            {
                fsm.RemoveTransitionWithValuesOf(transition);
            }
        }

        
    }
}
