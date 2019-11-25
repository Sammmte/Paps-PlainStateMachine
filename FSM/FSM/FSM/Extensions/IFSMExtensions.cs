using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class IFSMExtensions
    {
        public static IFSM<TState, TTrigger> AddTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            return fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static IFSM<TState, TTrigger> RemoveTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            return fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static bool ContainsStateByReference<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, IFSMState<TState, TTrigger> stateRef)
        {
            bool contains = false;

            fsm.ForeachState(
                state =>
                {
                    if (state == stateRef)
                    {
                        contains = true;
                        return true;
                    }

                    return false;
                });

            return contains;
        }
        
        public static T GetState<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm) where T : IFSMState<TState, TTrigger>
        {
            T candidate = default;

            fsm.ForeachState(
                state =>
                {
                    if(state is T cast)
                    {
                        candidate = cast;
                        return true;
                    }

                    return false;
                }
                );

            return candidate;
        }

        public static T[] GetStates<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm) where T : IFSMState<TState, TTrigger>
        {
            List<T> states = null;

            fsm.ForeachState(
                state =>
                {
                    if(state is T cast)
                    {
                        if(states == null)
                        {
                            states = new List<T>();
                        }

                        states.Add(cast);
                    }

                    return false;
                }
                );

            if(states != null)
            {
                return states.ToArray();
            }

            return null;
        }

        public static T GetStateById<T, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId) where T : IFSMState<TState, TTrigger>
        {
            T candidate = default;

            fsm.ForeachState(
                state =>
                {
                    if(state is T cast)
                    {
                        candidate = cast;
                        return true;
                    }

                    return false;
                }
                );

            return candidate;
        }

        public static IFSM<TState, TTrigger> AddTimerState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId, 
            double milliseconds, Action<TState> onTimerElapsed)
        {
            return fsm.AddState(stateId, new TimerState<TState, TTrigger>(fsm, milliseconds, onTimerElapsed));
        }

        public static IFSM<TState, TTrigger> AddEmpty<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            return fsm.AddState(stateId, new FSMState<TState, TTrigger>(fsm));
        }
        
        public static IFSM<TState, TTrigger> AddWithEvents<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId,
            Action onEnter, Action onUpdate = null, Action onExit = null)
        {
            return fsm.AddState(stateId, new DelegateFSMState<TState, TTrigger>(fsm, onEnter, onUpdate, onExit));
        }

        public static IFSM<TState, TTrigger> FromAny<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            fsm.ForeachState(
                state =>
                {
                    fsm.AddTransition(fsm.GetIdOf(state), trigger, stateTo);

                    return false;
                }
                );

            return fsm;
        }

        public static IFSM<TState, TTrigger> FromAnyExceptTarget<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger, TState stateTo)
        {
            fsm.ForeachState(
                state =>
                {
                    TState stateId = fsm.GetIdOf(state);

                    if (stateId.Equals(stateTo) == false)
                    {
                        fsm.AddTransition(fsm.GetIdOf(state), trigger, stateTo);
                    }

                    return false;
                }
                );

            return fsm;
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithTrigger<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTrigger trigger)
        {
            List<IFSMTransition<TState, TTrigger>> transitions = null;

            fsm.ForeachTransition(
                transition =>
                {
                    if(transition.Trigger.Equals(trigger))
                    {
                        if(transitions == null)
                        {
                            transitions = new List<IFSMTransition<TState, TTrigger>>();
                        }

                        transitions.Add(transition);
                    }

                    return false;
                }
                );

            if(transitions == null)
            {
                return null;
            }

            return transitions.ToArray();
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithStateFrom<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateFrom)
        {
            List<IFSMTransition<TState, TTrigger>> transitions = null;

            fsm.ForeachTransition(
                transition =>
                {
                    if (transition.StateFrom.Equals(stateFrom))
                    {
                        if (transitions == null)
                        {
                            transitions = new List<IFSMTransition<TState, TTrigger>>();
                        }

                        transitions.Add(transition);
                    }

                    return false;
                }
                );

            if (transitions == null)
            {
                return null;
            }

            return transitions.ToArray();
        }

        public static IFSMTransition<TState, TTrigger>[] GetTransitionsWithStateTo<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateTo)
        {
            List<IFSMTransition<TState, TTrigger>> transitions = null;

            fsm.ForeachTransition(
                transition =>
                {
                    if (transition.StateTo.Equals(stateTo))
                    {
                        if (transitions == null)
                        {
                            transitions = new List<IFSMTransition<TState, TTrigger>>();
                        }

                        transitions.Add(transition);
                    }

                    return false;
                }
                );

            if (transitions == null)
            {
                return null;
            }

            return transitions.ToArray();
        }
    }
}
