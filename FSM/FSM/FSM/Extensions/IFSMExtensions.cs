using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class IFSMExtensions
    {
        public static void AddTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, FSMTransition<TState, TTrigger> transition)
        {
            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static void RemoveTransition<TState, TTrigger>(this IFSM<TState, TTrigger> fsm, FSMTransition<TState, TTrigger> transition)
        {
            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }
    }
}
