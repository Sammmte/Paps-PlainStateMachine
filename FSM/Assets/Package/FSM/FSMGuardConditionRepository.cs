using System;
using System.Collections.Generic;
using System.Linq;

namespace Paps.FSM
{
    public class FSMGuardConditionRepository<TState, TTrigger>
    {
        protected Dictionary<IFSMTransition<TState, TTrigger>, Func<TState, TTrigger, TState, bool>> guardConditions;

        public FSMGuardConditionRepository(IEqualityComparer<IFSMTransition<TState, TTrigger>> equalityComparer)
        {
            guardConditions = new Dictionary<IFSMTransition<TState, TTrigger>, Func<TState, TTrigger, TState, bool>>(equalityComparer);
        }

        public FSMGuardConditionRepository()
        {
            guardConditions = new Dictionary<IFSMTransition<TState, TTrigger>, Func<TState, TTrigger, TState, bool>>();
        }

        public void Add(IFSMTransition<TState, TTrigger> transition, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            if (guardConditions.ContainsKey(transition) == false)
            {
                guardConditions.Add(transition, null);
            }

            guardConditions[transition] += guardCondition;
        }

        public void Remove(IFSMTransition<TState, TTrigger> transition, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            if (guardConditions.ContainsKey(transition))
            {
                guardConditions[transition] -= guardCondition;

                if (guardConditions[transition] == null)
                {
                    guardConditions.Remove(transition);
                }
            }
        }

        public void Clear(IFSMTransition<TState, TTrigger> transition)
        {
            guardConditions.Remove(transition);
        }

        public void Clear()
        {
            guardConditions.Clear();
        }

        public bool Contains(IFSMTransition<TState, TTrigger> transition, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            if (guardConditions.ContainsKey(transition))
            {
                return guardConditions[transition].GetInvocationList().Contains(guardCondition);
            }

            return false;
        }

        public bool ContainsAny(IFSMTransition<TState, TTrigger> transition)
        {
            return guardConditions.ContainsKey(transition);
        }

        public bool AnyTrue(IFSMTransition<TState, TTrigger> transition)
        {
            Func<TState, TTrigger, TState, bool>[] guardConditionsCast =
                Array.ConvertAll(guardConditions[transition].GetInvocationList(),
                func => (Func<TState, TTrigger, TState, bool>)func);

            foreach (var guardCondition in guardConditionsCast)
            {
                if (guardCondition(transition.StateFrom, transition.Trigger, transition.StateTo) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AllTrue(IFSMTransition<TState, TTrigger> transition)
        {
            Func<TState, TTrigger, TState, bool>[] guardConditionsCast =
                Array.ConvertAll(guardConditions[transition].GetInvocationList(),
                func => (Func<TState, TTrigger, TState, bool>)func);

            foreach (var guardCondition in guardConditionsCast)
            {
                if (guardCondition(transition.StateFrom, transition.Trigger, transition.StateTo) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
