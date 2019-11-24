using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class IFSMExtensions
    {
        public static FSMBuilder<TState, TTrigger> Build<TState, TTrigger>(this IFSM<TState, TTrigger> fsm)
        {
            return new FSMBuilder<TState, TTrigger>(fsm);
        }

        public static void AddTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static void RemoveTransitionWithValuesOf<TTransition, TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TTransition transition) where TTransition : IFSMTransition<TState, TTrigger>
        {
            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
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
            fsm.AddState(stateId, new TimerState<TState, TTrigger>(fsm, milliseconds, onTimerElapsed));

            return fsm;
        }
        

    }
}
