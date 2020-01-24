using System;

namespace Paps.FSM.Extensions
{
    public class PredicateGuardCondition : IGuardCondition
    {
        private Func<bool> predicate;

        public PredicateGuardCondition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public override bool Equals(object obj)
        {
            if (obj is PredicateGuardCondition cast)
            {
                return predicate == cast.predicate;
            }

            return false;
        }

        public static bool operator ==(PredicateGuardCondition first, PredicateGuardCondition second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(PredicateGuardCondition first, PredicateGuardCondition second)
        {
            return !first.Equals(second);
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


