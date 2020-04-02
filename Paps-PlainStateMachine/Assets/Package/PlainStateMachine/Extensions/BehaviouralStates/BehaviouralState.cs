using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Security.Cryptography;

namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public class BehaviouralState : IBehaviouralState
    {
        private enum State
        {
            Idle,
            Enter,
            Update,
            Exit
        }

        private State _internalState;

        private int _currentBehaviourIndex;

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
                if (indexOfBehaviour <= _currentBehaviourIndex && _currentBehaviourIndex > 0)
                    _currentBehaviourIndex--;

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
            if (_internalState != State.Idle)
                InvalidateIteration(State.Enter);

            _internalState = State.Enter;

            for(_currentBehaviourIndex = 0; _currentBehaviourIndex < _behaviours.Count; _currentBehaviourIndex++)
                _behaviours[_currentBehaviourIndex].OnEnter();

            _internalState = State.Idle;
        }

        public void Update()
        {
            if (_internalState != State.Idle)
                InvalidateIteration(State.Update);

            _internalState = State.Update;

            for (_currentBehaviourIndex = 0; _currentBehaviourIndex < _behaviours.Count; _currentBehaviourIndex++)
                _behaviours[_currentBehaviourIndex].OnUpdate();

            _internalState = State.Idle;
        }

        public void Exit()
        {
            if (_internalState != State.Idle)
                InvalidateIteration(State.Exit);

            _internalState = State.Exit;

            for (_currentBehaviourIndex = 0; _currentBehaviourIndex < _behaviours.Count; _currentBehaviourIndex++)
                _behaviours[_currentBehaviourIndex].OnExit();

            _internalState = State.Idle;
        }

        private void InvalidateIteration(State desired)
        {
            throw new InvalidOperationException("Cannot " + desired.ToString().ToLower() + " while in " + _internalState);
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