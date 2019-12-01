using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Paps.FSM
{
    public class GuardConditionOrderedCollection<TState, TTrigger>
    {
        private List<KeyValuePair<IFSMTransition<TState, TTrigger>, List<IGuardCondition>>> _orderedGuardConditions;
        private IEqualityComparer<IFSMTransition<TState, TTrigger>> _keyEqualityComparer;

        public GuardConditionOrderedCollection(IEqualityComparer<IFSMTransition<TState, TTrigger>> equalityComparer)
        {
            _keyEqualityComparer = equalityComparer;
            _orderedGuardConditions = new List<KeyValuePair<IFSMTransition<TState, TTrigger>, List<IGuardCondition>>>();
        }

        public void Add(IFSMTransition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if(ContainsKey(transition) == false)
            {
                _orderedGuardConditions.Add(new KeyValuePair<IFSMTransition<TState, TTrigger>, List<IGuardCondition>>(transition, new List<IGuardCondition>()));
            }

            GetListOf(transition).Add(guardCondition);
        }

        public bool RemoveFrom(IFSMTransition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if(ContainsKey(transition))
            {
                var list = GetListOf(transition);
                bool removed = list.Remove(guardCondition);

                if(list.Count == 0)
                {
                    Remove(transition);
                }

                return removed;
            }

            return false;
        }

        public bool Remove(IFSMTransition<TState, TTrigger> transition)
        {
            int index = IndexOf(transition);

            if(index != -1)
            {
                _orderedGuardConditions.RemoveAt(index);

                return true;
            }

            return false;
        }

        public bool ContainsKey(IFSMTransition<TState, TTrigger> transition)
        {
            for(int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                if(_keyEqualityComparer.Equals(_orderedGuardConditions[i].Key, transition))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsGuardCondition(IGuardCondition guardCondition)
        {
            for(int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                if(_orderedGuardConditions[i].Value.Equals(guardCondition))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsGuardConditionOn(IFSMTransition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            var list = GetListOf(transition);

            if(list != null)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    if(list[i].Equals(guardCondition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<IGuardCondition> GetListOf(IFSMTransition<TState, TTrigger> transition)
        {
            for(int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                if(_keyEqualityComparer.Equals(_orderedGuardConditions[i].Key, transition))
                {
                    return _orderedGuardConditions[i].Value;
                }
            }

            return null;
        }

        public IGuardCondition[] GetGuardConditionsOf(IFSMTransition<TState, TTrigger> transition)
        {
            if (ContainsKey(transition) == false)
                throw new ArgumentException("key does not exist in the collection");

            for (int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                if (_keyEqualityComparer.Equals(_orderedGuardConditions[i].Key, transition))
                {
                    return _orderedGuardConditions[i].Value.ToArray();
                }
            }

            return null;
        }

        public int IndexOf(IFSMTransition<TState, TTrigger> transition)
        {
            for(int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                if(_keyEqualityComparer.Equals(_orderedGuardConditions[i].Key, transition))
                {
                    return i;
                }
            }

            return -1;
        }

        public KeyValuePair<IFSMTransition<TState, TTrigger>, IGuardCondition[]>[] GetGuardConditions()
        {
            var guardConditions = new KeyValuePair<IFSMTransition<TState, TTrigger>, IGuardCondition[]>[_orderedGuardConditions.Count];

            for(int i = 0; i < _orderedGuardConditions.Count; i++)
            {
                guardConditions[i] = new KeyValuePair<IFSMTransition<TState, TTrigger>, IGuardCondition[]>(_orderedGuardConditions[i].Key, _orderedGuardConditions[i].Value.ToArray());
            }

            return guardConditions;
        }
    }

}