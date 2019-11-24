using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public static class IFSMWithGuardConditionsExtensions
    {
        public static FSMWithGuardConditionsBuilder<TState, TTrigger> Build<TState, TTrigger>(this IFSMWithGuardConditions<TState, TTrigger> fsm)
        {
            return new FSMWithGuardConditionsBuilder<TState, TTrigger>(fsm);
        }
    }
}
