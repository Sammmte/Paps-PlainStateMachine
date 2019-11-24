using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class IFSMExtensions
    {
        public static IFSM<TState, TTrigger> AddStateBuilder<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId, IFSMState<TState, TTrigger> state)
        {
            fsm.AddState(stateId, state);

            return fsm;
        }

        public static IFSM<TState, TTrigger> RemoveStateBuilder<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId)
        {
            fsm.RemoveState(stateId);

            return fsm;
        }

        public static IFSM<TState, TTrigger> AddTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, FSMTransition<TState, TTrigger> transition)
        {
            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

            return fsm;
        }

        public static IFSM<TState, TTrigger> RemoveTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, FSMTransition<TState, TTrigger> transition)
        {
            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

            return fsm;
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

        public static bool TryGetIdOf<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, IFSMState<TState, TTrigger> state, out TState stateId)
        {
            try
            {
                stateId = fsm.GetIdOf(state);
                return true;
            }
            catch (StateNotAddedException e)
            {
                stateId = default;
                return false;
            }
        }

        public static IFSM<TState, TTrigger> AddTimerState<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, TState stateId, 
            double milliseconds, Action<TState> onTimerElapsed)
        {
            fsm.AddState(stateId, new TimerState<TState, TTrigger>(fsm, milliseconds, onTimerElapsed));

            return fsm;
        }


    }
}
