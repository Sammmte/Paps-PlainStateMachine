using System;

namespace Paps.FSM.Extensions
{
    public class PredicateGuardCondition<TState, TTrigger> : IGuardCondition
    {
        private Func<bool> predicate;

        public PredicateGuardCondition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public override bool Equals(object obj)
        {
            if (obj is PredicateGuardCondition<TState, TTrigger> cast)
            {
                return predicate == cast.predicate;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return predicate.GetHashCode();
        }

        public bool IsValid()
        {
            return predicate();
        }
    }
}


