using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class FSMBuilderExtensions
    {
        public static FSMBuilder<TState, TTrigger> AddTransitionWithValuesOf<TState, TTrigger>(this FSMBuilder<TState, TTrigger> fsmBuilder, FSMTransition<TState, TTrigger> transition)
        {
            return fsmBuilder.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }

        public static FSMBuilder<TState, TTrigger> RemoveTransitionWithValuesOf<TState, TTrigger>(this FSMBuilder<TState, TTrigger> fsmBuilder, FSMTransition<TState, TTrigger> transition)
        {
            return fsmBuilder.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
        }
    }
}
