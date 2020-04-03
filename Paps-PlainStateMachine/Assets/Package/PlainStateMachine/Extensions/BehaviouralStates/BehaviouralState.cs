using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Security.Cryptography;

namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public class BehaviouralState : IBehaviouralState
    {
        private int _currentBehaviourEnterIndex;
        private int _currentBehaviourUpdateIndex;
        private int _currentBehaviourExitIndex;

        private List<IStateBehaviour> _behaviours;

        public int BehaviourCount => _behaviours.Count;

        public BehaviouralState()
        {
            _behaviours = new List<IStateBehaviour>();
        }

        public BehaviouralState(params IStateBehaviour[] behaviours) : this()
        {
            if (behaviours.Any(behaviour => behaviour == null))
                throw new ArgumentNullException("some behaviour objects were null");

            foreach (var behaviour in behaviours)
                AddBehaviour(behaviour);
        }

        public void AddBehaviour(IStateBehaviour behaviour)
        {
            if(_behaviours.Contains(behaviour) == false)
                _behaviours.Add(behaviour);
        }

        public bool RemoveBehaviour(IStateBehaviour behaviour)
        {
            int indexOfBehaviour = _behaviours.IndexOf(behaviour);

            if(_behaviours.Remove(behaviour))
            {
                if (indexOfBehaviour <= _currentBehaviourEnterIndex && _currentBehaviourEnterIndex > 0)
                    _currentBehaviourEnterIndex--;

                if (indexOfBehaviour <= _currentBehaviourUpdateIndex && _currentBehaviourUpdateIndex > 0)
                    _currentBehaviourUpdateIndex--;

                if (indexOfBehaviour <= _currentBehaviourExitIndex && _currentBehaviourExitIndex > 0)
                    _currentBehaviourExitIndex--;

                return false;
            }

            return false;
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
            for(_currentBehaviourEnterIndex = 0; _currentBehaviourEnterIndex < _behaviours.Count; _currentBehaviourEnterIndex++)
                _behaviours[_currentBehaviourEnterIndex].OnEnter();
        }

        public void Update()
        {
            for (_currentBehaviourUpdateIndex = 0; _currentBehaviourUpdateIndex < _behaviours.Count; _currentBehaviourUpdateIndex++)
                _behaviours[_currentBehaviourUpdateIndex].OnUpdate();
        }

        public void Exit()
        {
            for (_currentBehaviourExitIndex = 0; _currentBehaviourExitIndex < _behaviours.Count; _currentBehaviourExitIndex++)
                _behaviours[_currentBehaviourExitIndex].OnExit();
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