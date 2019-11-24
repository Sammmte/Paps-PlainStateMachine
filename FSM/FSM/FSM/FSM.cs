using System;
using System.Collections;
using System.Collections.Generic;

namespace Paps.FSM
{
    public class FSM<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        public int StateCount => _states.Count;
        public int TransitionCount => _transitions.Count;
        public bool IsStarted { get; private set; }
        public TState InitialState { get; private set; }

        private IFSMState<TState, TTrigger> _currentState;

        private Dictionary<TState, IFSMState<TState, TTrigger>> _states;
        private HashSet<FSMTransition<TState, TTrigger>> _transitions;

        private Func<TState, TState, bool> _stateComparer;
        private Func<TTrigger, TTrigger, bool> _triggerComparer;

        private FSMStateEqualityComparer _stateEqualityComparer;

        public event StateChanged<TState, TTrigger> OnStateChanged;

        public FSM(Func<TState, TState, bool> stateComparer, Func<TTrigger, TTrigger, bool> triggerComparer)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;

            _stateEqualityComparer = new FSMStateEqualityComparer(stateComparer);

            _states = new Dictionary<TState, IFSMState<TState, TTrigger>>(_stateEqualityComparer);
            _transitions = new HashSet<FSMTransition<TState, TTrigger>>();
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

        public void Start()
        {
            ValidateCanStart();

            _currentState = GetStateById(InitialState);
            EnterCurrentState();

            IsStarted = true;
        }

        private void ValidateCanStart()
        {
            ThrowExceptionIfIsStarted();
            ThrowExceptionIfInitialStateIsInvalid();
        }
        
        private void EnterCurrentState()
        {
            _currentState.Enter();
        }

        public void Update()
        {
            UpdateCurrentState();
        }

        private void UpdateCurrentState()
        {
            ThrowExceptionIfIsNotStarted();

            _currentState.Update();
        }

        public void Stop()
        {
            if(_currentState != null)
            {
                ExitCurrentState();
            }

            IsStarted = false;
        }

        private void ExitCurrentState()
        {
            _currentState.Exit();
        }

        public void SetInitialState(TState stateId)
        {
            ValidateCanSetInitialState(stateId);

            InternalSetInitialState(stateId);
        }

        private void ValidateCanSetInitialState(TState initialStateId)
        {
            ThrowExceptionIfIsStarted();
        }

        private void ThrowExceptionIfIsNotStarted()
        {
            if (IsStarted == false)
            {
                throw new FSMNotStartedException();
            }
        }

        private void ThrowExceptionIfIsStarted()
        {
            if (IsStarted)
            {
                throw new FSMStartedException();
            }
        }

        private void ThrowExceptionIfDoesNotHaveStateId(TState stateId)
        {
            if(ContainsState(stateId) == false)
            {
                throw new StateNotAddedException(stateId.ToString());
            }
        }

        private void ThrowExceptionIfInitialStateIsInvalid()
        {
            if(ContainsState(InitialState) == false)
            {
                throw new InvalidInitialStateException();
            }
        }

        private void InternalSetInitialState(TState stateId)
        {
            InitialState = stateId;
        }

        public void AddState(TState stateId, IFSMState<TState, TTrigger> state)
        {
            ValidateInputStateForAddOperation(stateId, state);

            InternalAddState(stateId, state);
        }

        private void ValidateInputStateForAddOperation(TState stateId, IFSMState<TState, TTrigger> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            else if (_states.ContainsKey(stateId))
            {
                throw new StateIdAlreadyAddedException("State id " + state.GetStateId() + " was already added to state machine");
            }
        }

        private void InternalAddState(TState stateId, IFSMState<TState, TTrigger> state)
        {
            _states.Add(stateId, state);
        }

        public void ForeachState(ReturnTrueToFinishIteration<IFSMState<TState, TTrigger>> finishable)
        {
            foreach(IFSMState<TState, TTrigger> state in _states.Values)
            {
                if(finishable(state))
                {
                    break;
                }
            }
        }

        public void RemoveState(TState stateId)
        {
            _states.Remove(stateId);
        }

        private IFSMState<TState, TTrigger> GetStateById(TState stateId)
        {
            foreach(IFSMState<TState, TTrigger> state in _states.Values)
            {
                if(_stateComparer(state.GetStateId(), stateId))
                {
                    return state;
                }
            }

            throw new StateNotAddedException(stateId.ToString());
        }

        private IFSMState<TState, TTrigger> TryGetStateById(TState stateId)
        {
            try
            {
                var state = GetStateById(stateId);
                return state;
            }
            catch(StateNotAddedException e)
            {
                return null;
            }
        }

        public void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            _transitions.Add(new FSMTransition<TState, TTrigger>(stateFrom, trigger, stateTo));
        }

        public void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            _transitions.Remove(new FSMTransition<TState, TTrigger>(stateFrom, trigger, stateTo));
        }

        public void ForeachTransition(ReturnTrueToFinishIteration<FSMTransition<TState, TTrigger>> finishable)
        {
            foreach(FSMTransition<TState, TTrigger> transition in _transitions)
            {
                if(finishable(transition))
                {
                    break;
                }
            }
        }

        public bool IsInState(TState stateId)
        {
            if(_currentState != null)
            {
                return _stateComparer(_currentState.GetStateId(), stateId);
            }

            return false;
        }

        public bool ContainsState(TState stateId)
        {
            foreach(IFSMState<TState, TTrigger> state in _states.Values)
            {
                if(_stateComparer(state.GetStateId(), stateId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            FSMTransition<TState, TTrigger> comparisonTransition = new FSMTransition<TState, TTrigger>(stateFrom, trigger, stateTo);

            foreach(FSMTransition<TState, TTrigger> transition in _transitions)
            {
                if(transition == comparisonTransition)
                {
                    return true;
                }
            }

            return false;
        }

        public void Trigger(TTrigger trigger)
        {
            ThrowExceptionIfIsNotStarted();

            var stateTo = GetStateTo(trigger);

            if (stateTo != null)
            {
                Transitionate(_currentState, trigger, stateTo);
            }
        }

        private IFSMState<TState, TTrigger> GetStateTo(TTrigger trigger)
        {
            foreach(FSMTransition<TState, TTrigger> transition in _transitions)
            {
                if(_stateComparer(transition.StateFrom, _currentState.GetStateId()) 
                    && _triggerComparer(transition.Trigger, trigger))
                {
                    return TryGetStateById(transition.StateTo);
                }
            }

            return null;
        }

        private void Transitionate(IFSMState<TState, TTrigger> stateFrom, TTrigger trigger, IFSMState<TState, TTrigger> stateTo)
        {
            ExitCurrentState();

            _currentState = null;

            CallOnStateChangedEvent(stateFrom.GetStateId(), trigger, stateTo.GetStateId());

            _currentState = stateTo;

            EnterCurrentState();
        }

        private void CallOnStateChangedEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            if(OnStateChanged != null)
            {
                OnStateChanged(stateFrom, trigger, stateTo);
            }
        }

        public TState GetIdOf(IFSMState<TState, TTrigger> state)
        {
            foreach(KeyValuePair<TState, IFSMState<TState, TTrigger>> entry in _states)
            {
                if(entry.Value == state)
                {
                    return entry.Key;
                }
            }

            throw new StateNotAddedException();
        }

        private class FSMStateEqualityComparer : IEqualityComparer<TState>
        {
            public Func<TState, TState, bool> _stateComparer;

            public FSMStateEqualityComparer(Func<TState, TState, bool> stateComparer)
            {
                _stateComparer = stateComparer;
            }

            public bool Equals(TState x, TState y)
            {
                return _stateComparer(x, y);
            }

            public int GetHashCode(TState obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
