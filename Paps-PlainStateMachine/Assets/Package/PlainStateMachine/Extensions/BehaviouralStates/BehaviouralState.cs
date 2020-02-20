using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public class BehaviouralState : IBehaviouralState
    {
        private HashSet<IStateBehaviour> _behaviours;

        public int BehaviourCount => _behaviours.Count;

        public BehaviouralState()
        {
            _behaviours = new HashSet<IStateBehaviour>();
        }

        public BehaviouralState(params IStateBehaviour[] behaviours)
        {
            if (behaviours.Any(behaviour => behaviour == null)) throw new ArgumentNullException("some behaviour objects were null");

            _behaviours = new HashSet<IStateBehaviour>(behaviours);
        }

        public void AddBehaviour(IStateBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public bool RemoveBehaviour(IStateBehaviour behaviour)
        {
            return _behaviours.Remove(behaviour);
        }

        public bool ContainsBehaviour(IStateBehaviour behaviour)
        {
            return _behaviours.Contains(behaviour);
        }

        public T GetBehaviour<T>()
        {
            foreach(var behaviour in _behaviours)
            {
                if (behaviour is T candidate)
                    return candidate;
            }

            return default;
        }

        public T[] GetBehaviours<T>()
        {
            List<T> candidates = null;

            foreach(var behaviour in _behaviours)
            {
                if(behaviour is T candidate)
                {
                    if(candidates == null)
                    {
                        candidates = new List<T>();
                    }

                    candidates.Add(candidate);
                }
            }

            if (candidates != null && candidates.Count > 0) return candidates.ToArray();
            else return null;
        }

        public void Enter()
        {
            foreach(var behaviour in _behaviours)
            {
                behaviour.OnEnter();
            }
        }

        public void Update()
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.OnUpdate();
            }
        }

        public void Exit()
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.OnExit();
            }
        }

        public IEnumerator<IStateBehaviour> GetEnumerator()
        {
            return _behaviours.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}