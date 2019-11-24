using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM.Extensions
{
    public class FSMWithGuardConditionsBuilder<TState, TTrigger> : FSMBuilder<TState, TTrigger>
    {
        public new IFSMWithGuardConditions<TState, TTrigger> InnerFSM { get; protected set; }

        public FSMWithGuardConditionsBuilder(IFSMWithGuardConditions<TState, TTrigger> fsm) : base(fsm)
        {
            InnerFSM = fsm;
        }

        public FSMWithGuardConditionsBuilder<TState, TTrigger> AddANDGuardConditionTo(
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            InnerFSM.AddANDGuardConditionTo(stateFrom, trigger, stateTo, guardCondition);

            return this;
        }

        public FSMWithGuardConditionsBuilder<TState, TTrigger> AddORGuardConditionTo(
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            InnerFSM.AddORGuardConditionTo(stateFrom, trigger, stateTo, guardCondition);

            return this;
        }

        public FSMWithGuardConditionsBuilder<TState, TTrigger> RemoveANDGuardConditionFrom(
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            InnerFSM.RemoveANDGuardConditionFrom(stateFrom, trigger, stateTo, guardCondition);

            return this;
        }

        public FSMWithGuardConditionsBuilder<TState, TTrigger> RemoveORGuardConditionFrom(
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            InnerFSM.RemoveORGuardConditionFrom(stateFrom, trigger, stateTo, guardCondition);

            return this;
        }
    }
}
