using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Paps.FSM.Extensions
{
    public sealed class PredicateGuardCondition : IGuardCondition
    {
        private Func<bool> predicate;

        public PredicateGuardCondition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public override bool Equals(object obj)
        {
            if(obj is PredicateGuardCondition cast)
            {
                return predicate == cast.predicate;
            }

            return false;
        }

        public bool IsValid()
        {
            return predicate();
        }
    }
}