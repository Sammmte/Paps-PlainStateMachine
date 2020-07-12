using Paps.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    public class StateCollection<TState>
    {
        private readonly Dictionary<TState, IState> _states = new Dictionary<TState, IState>();
        private readonly IEqualityComparer<TState> _stateComparer;

        public Maybe<TState> InitialState { get; private set; }
        public int StateCount => _states.Count;

        public StateCollection(IEqualityComparer<TState> stateComparer)
        {
            _stateComparer = stateComparer;
        }

        public void AddState(TState stateId, IState state)
        {
            ValidateCanAddState(stateId, state);

            _states.Add(stateId, state);
        }

        private void ValidateCanAddState(TState stateId, IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            else if (_states.ContainsKey(stateId))
            {
                throw new StateIdAlreadyAddedException(stateId.ToString());
            }
        }

        public void SetInitialState(TState stateId)
        {
            ValidateHasStateWithId(stateId);

            InitialState = stateId.ToMaybe();
        }

        private void ValidateHasStateWithId(TState stateId)
        {
            if (ContainsState(stateId) == false)
            {
                throw new StateIdNotAddedException(stateId.ToString());
            }
        }

        public bool ContainsState(TState stateId)
        {
            try
            {
                return _states.ContainsKey(stateId);
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        public void RemoveState(TState stateId)
        {
            if(_states.Remove(stateId))
            {
                if (AreEquals(InitialState.Value, stateId))
                    InitialState = Maybe<TState>.Nothing;
            }
        }

        public IState GetStateById(TState stateId)
        {
            if (_states.ContainsKey(stateId))
            {
                return _states[stateId];
            }

            throw new StateIdNotAddedException(stateId.ToString());
        }

        public TState[] GetStates()
        {
            return _states.Keys.ToArray();
        }

        private bool AreEquals(TState stateId1, TState stateId2)
        {
            return _stateComparer.Equals(stateId1, stateId2);
        }
    }
}