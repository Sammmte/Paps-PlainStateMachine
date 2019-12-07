using System;

namespace Paps.FSM.Extensions
{
    public class PredicateWithParametersGuardCondition<TState, TTrigger> : IGuardCondition<TState, TTrigger>
    {
        private Func<TState, TTrigger, TState, bool> predicate;

        public PredicateWithParametersGuardCondition(Func<TState, TTrigger, TState, bool> predicate)
        {
            this.predicate = predicate;
        }

        public override bool Equals(object obj)
        {
            if(obj is PredicateWithParametersGuardCondition<TState, TTrigger> cast)
            {
                return predicate == cast.predicate;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return predicate.GetHashCode();
        }

        public bool IsValid(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            return predicate(stateFrom, trigger, stateTo);
        }
    }
    
}