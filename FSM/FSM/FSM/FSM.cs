using System;
using System.Collections;
using System.Collections.Generic;

namespace Paps.FSM
{
    public class FSM<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        public int StateCount { get => _states.Count; }

        private HashSet<IFSMState<TState, TTrigger>> _states;

        private Func<TState, TState, bool> _stateComparer;
        private Func<TTrigger, TTrigger, bool> _triggerComparer;

        private FSMStateEqualityComparer _stateEqualityComparer;

        public FSM(Func<TState, TState, bool> stateComparer, Func<TTrigger, TTrigger, bool> triggerComparer)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;

            _stateEqualityComparer = new FSMStateEqualityComparer(stateComparer);

            _states = new HashSet<IFSMState<TState, TTrigger>>(_stateEqualityComparer);
        }

        public FSM() : this(DefaultComparer, DefaultComparer)
        {
            
        }

        public void SetStateComparer(Func<TState, TState, bool> comparer)
        {
            _stateComparer = comparer;
            _stateEqualityComparer._stateComparer = comparer;
        }

        public void SetTriggerComparer(Func<TTrigger, TTrigger, bool> comparer)
        {
            _triggerComparer = comparer;
        }

        private static bool DefaultComparer<T>(T first, T second)
        {
            return first.Equals(second);
        }

        public void AddState(IFSMState<TState, TTrigger> state)
        {
            if(state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            else if(_states.Contains(state))
            {
                throw new StateIdAlreadyAddedException("State id " + state.StateId + " was already added to state machine");
            }

            InternalAddState(state);
        }

        private void InternalAddState(IFSMState<TState, TTrigger> state)
        {
            _states.Add(state);
        }

        public IEnumerator<IFSMState<TState, TTrigger>> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        public void RemoveState(IFSMState<TState, TTrigger> state)
        {
            _states.Remove(state);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class FSMStateEqualityComparer : IEqualityComparer<IFSMState<TState, TTrigger>>
        {
            public Func<TState, TState, bool> _stateComparer;

            public FSMStateEqualityComparer(Func<TState, TState, bool> stateComparer)
            {
                _stateComparer = stateComparer;
            }

            public bool Equals(IFSMState<TState, TTrigger> x, IFSMState<TState, TTrigger> y)
            {
                return _stateComparer(x.StateId, y.StateId);
            }

            public int GetHashCode(IFSMState<TState, TTrigger> obj)
            {
                return obj.StateId.GetHashCode();
            }
        }
    }
}
