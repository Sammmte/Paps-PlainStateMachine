using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paps.FSM
{
    public class FSM<TState, TTrigger> : IFSMWithGuardConditions<TState, TTrigger>, IFSMEventNotifier<TState, TTrigger>
    {
        public int StateCount => _states.Count;
        public int TransitionCount => _transitions.Count;
        public bool IsStarted { get; private set; }
        public TState InitialState { get; private set; }

        private Dictionary<TState, IFSMState> _states;
        private HashSet<IFSMTransition<TState, TTrigger>> _transitions;
        private FSMGuardConditionRepository<TState, TTrigger> _ANDguardConditionRepository;
        private FSMGuardConditionRepository<TState, TTrigger> _ORguardConditionRepository;

        private Queue<TransitionRequest> _transitionRequestQueue;

        private IFSMState _currentState;
        private bool _isTransitioning;

        private Func<TState, TState, bool> _stateComparer;
        private Func<TTrigger, TTrigger, bool> _triggerComparer;

        private FSMStateEqualityComparer _stateEqualityComparer;
        private FSMTransitionEqualityComparer _transitionEqualityComparer;

        public event StateChange<TState, TTrigger> OnBeforeStateChanges;
        public event StateChange<TState, TTrigger> OnStateChanged;

        public FSM(Func<TState, TState, bool> stateComparer, Func<TTrigger, TTrigger, bool> triggerComparer)
        {
            _stateComparer = stateComparer;
            _triggerComparer = triggerComparer;

            _stateEqualityComparer = new FSMStateEqualityComparer(stateComparer);
            _transitionEqualityComparer = new FSMTransitionEqualityComparer(stateComparer, triggerComparer);

            _states = new Dictionary<TState, IFSMState>(_stateEqualityComparer);
            _transitions = new HashSet<IFSMTransition<TState, TTrigger>>(_transitionEqualityComparer);
            _ANDguardConditionRepository = new FSMGuardConditionRepository<TState, TTrigger>(_transitionEqualityComparer);
            _ORguardConditionRepository = new FSMGuardConditionRepository<TState, TTrigger>(_transitionEqualityComparer);
            _transitionRequestQueue = new Queue<TransitionRequest>();
        }

        public FSM() : this(DefaultComparer, DefaultComparer)
        {
            
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
            ValidateIsNotStarted();
            ValidateInitialState(InitialState);
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
            ValidateIsStarted();

            _currentState.Update();
        }

        public void Stop()
        {
            if(IsStarted)
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
            ValidateIsNotStarted();
            ValidateInitialState(initialStateId);
        }

        private void ValidateInitialState(TState initialStateId)
        {
            if(ContainsState(initialStateId) == false)
            {
                throw new InvalidInitialStateException();
            }
        }

        private void ValidateIsStarted()
        {
            if (IsStarted == false)
            {
                throw new FSMNotStartedException();
            }
        }

        private void ValidateIsNotStarted()
        {
            if (IsStarted)
            {
                throw new FSMStartedException();
            }
        }

        public IFSMState[] GetStates()
        {
            return _states.Values.ToArray();
        }

        public IFSMTransition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.ToArray();
        }

        private void InternalSetInitialState(TState stateId)
        {
            InitialState = stateId;
        }

        public void AddState(TState stateId, IFSMState state)
        {
            ValidateCanAddState(stateId, state);

            InternalAddState(stateId, state);
        }

        private void ValidateCanAddState(TState stateId, IFSMState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            else if (_states.ContainsKey(stateId))
            {
                throw new StateIdAlreadyAddedException("State id " + stateId + " was already added to state machine");
            }
        }

        private void InternalAddState(TState stateId, IFSMState state)
        {
            _states.Add(stateId, state);
        }

        public void RemoveState(TState stateId)
        {
            if(_states.Remove(stateId))
            {
                RemoveTransitionsRelatedTo(stateId);
            }
        }

        private void RemoveTransitionsRelatedTo(TState stateId)
        {
            IFSMTransition<TState, TTrigger>[] transitions = _transitions.Where(
                (transition) => _stateComparer(transition.StateFrom, stateId) || _stateComparer(transition.StateTo, stateId)
                ).ToArray();

            for(int i = 0; i < transitions.Length; i++)
            {
                var transition = transitions[i];

                InternalRemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);
            }
        }

        private IFSMState GetStateById(TState stateId)
        {
            foreach(IFSMState state in _states.Values)
            {
                if(_stateComparer(GetIdOf(state), stateId))
                {
                    return state;
                }
            }

            throw new StateIdNotAddedException(stateId.ToString());
        }

        private IFSMState TryGetStateById(TState stateId)
        {
            try
            {
                var state = GetStateById(stateId);
                return state;
            }
            catch(StateIdNotAddedException e)
            {
                return null;
            }
        }

        public void AddTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            ValidateHasStateWithId(stateFrom);
            ValidateHasStateWithId(stateTo);

            InternalAddTransition(stateFrom, trigger, stateTo);
        }

        private void InternalAddTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            _transitions.Add(new FSMTransition<TState, TTrigger>(stateFrom, trigger, stateTo));
        }

        public void RemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            ValidateHasStateWithId(stateFrom);
            ValidateHasStateWithId(stateTo);

            InternalRemoveTransition(stateFrom, trigger, stateTo);
        }

        private void InternalRemoveTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            var transition = GetTransition(stateFrom, trigger, stateTo);

            if (transition != null && _transitions.Remove(transition))
            {
                RemoveAllGuardConditionsRelatedTo(transition);
            }
        }

        private void RemoveAllGuardConditionsRelatedTo(IFSMTransition<TState, TTrigger> transition)
        {
            _ANDguardConditionRepository.Clear(transition);
            _ORguardConditionRepository.Clear(transition);
        }

        private void ValidateHasStateWithId(TState stateId)
        {
            if(ContainsState(stateId) == false)
            {
                throw new StateIdNotAddedException(stateId.ToString());
            }
        }

        public bool IsInState(TState stateId)
        {
            return IsStarted && _stateComparer(GetIdOf(_currentState), stateId);
        }

        public bool ContainsState(TState stateId)
        {
            foreach(IFSMState state in _states.Values)
            {
                if(_stateComparer(GetIdOf(state), stateId))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            var transition = GetTransition(stateFrom, trigger, stateTo);

            if(transition != null)
            {
                return true;
            }

            return false;
        }

        public void Trigger(TTrigger trigger)
        {
            ValidateIsStarted();

            TState currentStateId = GetIdOf(_currentState);

            if (HasTransitionFromState(currentStateId, trigger))
            {
                _transitionRequestQueue.Enqueue(new TransitionRequest() { stateFrom = GetIdOf(_currentState), trigger = trigger });

                if(_isTransitioning == false)
                {
                    _isTransitioning = true;
                    TriggerQueued();
                }
            }
        }
        
        private void TriggerQueued()
        {
            while(_transitionRequestQueue.Count > 0)
            {
                TransitionRequest transition = _transitionRequestQueue.Dequeue();

                var stateTo = GetStateTo(transition.trigger);

                if (stateTo != null)
                {
                    Transition(_currentState, transition.trigger, stateTo);
                }
            }

            _isTransitioning = false;
        }

        private bool HasTransitionFromState(TState stateFrom, TTrigger trigger)
        {
            foreach (IFSMTransition<TState, TTrigger> transition in _transitions)
            {
                if (_stateComparer(transition.StateFrom, stateFrom) && _triggerComparer(transition.Trigger, trigger))
                {
                    return true;
                }
            }

            return false;
        }

        private IFSMState GetStateTo(TTrigger trigger)
        {
            foreach(IFSMTransition<TState, TTrigger> transition in _transitions)
            {
                if(_stateComparer(transition.StateFrom, GetIdOf(_currentState)) 
                    && _triggerComparer(transition.Trigger, trigger)
                    && IsValidTransition(transition))
                {
                    return TryGetStateById(transition.StateTo);
                }
            }

            return null;
        }

        private void Transition(IFSMState stateFrom, TTrigger trigger, IFSMState stateTo)
        {
            TState stateFromId = GetIdOf(stateFrom);
            TState stateToId = GetIdOf(stateTo);

            CallOnBeforeStateChangesEvent(stateFromId, trigger, stateToId);

            ExitCurrentState();
            
            _currentState = stateTo;

            CallOnStateChangedEvent(stateFromId, trigger, stateToId);

            EnterCurrentState();
        }

        private bool IsValidTransition(IFSMTransition<TState, TTrigger> transition)
        {
            return (_ANDguardConditionRepository.ContainsAny(transition) == false || _ANDguardConditionRepository.AllTrue(transition))
                && (_ORguardConditionRepository.ContainsAny(transition) == false || _ORguardConditionRepository.AnyTrue(transition));
        }

        private void CallOnStateChangedEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            OnStateChanged?.Invoke(stateFrom, trigger, stateTo);
        }

        private void CallOnBeforeStateChangesEvent(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            OnBeforeStateChanges?.Invoke(stateFrom, trigger, stateTo);
        }

        public TState GetIdOf(IFSMState state)
        {
            foreach(KeyValuePair<TState, IFSMState> entry in _states)
            {
                if(entry.Value == state)
                {
                    return entry.Key;
                }
            }

            throw new StateIdNotAddedException();
        }

        public void AddANDGuardConditionTo(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            AddGuardConditionTo(_ANDguardConditionRepository, stateFrom, trigger, stateTo, guardCondition);
        }

        public void RemoveANDGuardConditionFrom(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            RemoveGuardConditionFrom(_ANDguardConditionRepository, stateFrom, trigger, stateTo, guardCondition);
        }

        public void AddORGuardConditionTo(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            AddGuardConditionTo(_ORguardConditionRepository, stateFrom, trigger, stateTo, guardCondition);
        }

        public void RemoveORGuardConditionFrom(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            RemoveGuardConditionFrom(_ORguardConditionRepository, stateFrom, trigger, stateTo, guardCondition);
        }

        private IFSMTransition<TState, TTrigger> GetTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            foreach(IFSMTransition<TState, TTrigger> transition in _transitions)
            {
                if(_transitionEqualityComparer.Equals(transition, stateFrom, trigger, stateTo))
                {
                    return transition;
                }
            }

            return null;
        }


        private void AddGuardConditionTo(FSMGuardConditionRepository<TState, TTrigger> guardConditions,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            ValidateHasTransition(stateFrom, trigger, stateTo);
            ValidateGuardConditionIsNotNull(guardCondition);

            guardConditions.Add(GetTransition(stateFrom, trigger, stateTo), guardCondition);
        }

        private void RemoveGuardConditionFrom(FSMGuardConditionRepository<TState, TTrigger> guardConditions,
            TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            ValidateHasTransition(stateFrom, trigger, stateTo);
            ValidateGuardConditionIsNotNull(guardCondition);

            guardConditions.Remove(GetTransition(stateFrom, trigger, stateTo), guardCondition);
        }

        public bool ContainsANDGuardConditionOn(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            return ContainsGuardConditionOn(_ANDguardConditionRepository, GetTransition(stateFrom, trigger, stateTo), guardCondition);
        }

        public bool ContainsORGuardConditionOn(TState stateFrom, TTrigger trigger, TState stateTo, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            return ContainsGuardConditionOn(_ORguardConditionRepository, GetTransition(stateFrom, trigger, stateTo), guardCondition);
        }

        private bool ContainsGuardConditionOn(FSMGuardConditionRepository<TState, TTrigger> guardConditionRepository,
            IFSMTransition<TState, TTrigger> transition, Func<TState, TTrigger, TState, bool> guardCondition)
        {
            if (transition != null)
            {
                return guardConditionRepository.Contains(transition, guardCondition);
            }

            return false;
        }

        private void ValidateHasTransition(TState stateFrom, TTrigger trigger, TState stateTo)
        {
            if(ContainsTransition(stateFrom, trigger, stateTo) == false)
            {
                throw new TransitionNotAddedException(stateFrom.ToString(), trigger.ToString(), stateTo.ToString());
            }
        }

        private void ValidateGuardConditionIsNotNull(Delegate guardCondition)
        {
            if(guardCondition == null)
            {
                throw new ArgumentNullException("Guard condition was null");
            }
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

        private struct TransitionRequest
        {
            public TState stateFrom;
            public TTrigger trigger;
        }

        private class FSMTransitionEqualityComparer : IEqualityComparer<IFSMTransition<TState, TTrigger>>
        {
            private Func<TState, TState, bool> _stateComparer;
            private Func<TTrigger, TTrigger, bool> _triggerComparer;

            public FSMTransitionEqualityComparer(Func<TState, TState, bool> stateComparer, Func<TTrigger, TTrigger, bool> triggerComparer)
            {
                _stateComparer = stateComparer;
                _triggerComparer = triggerComparer;
            }

            public bool Equals(IFSMTransition<TState, TTrigger> x, IFSMTransition<TState, TTrigger> y)
            {
                return _stateComparer(x.StateFrom, y.StateFrom) && _triggerComparer(x.Trigger, y.Trigger) && _stateComparer(x.StateTo, y.StateTo);
            }

            public int GetHashCode(IFSMTransition<TState, TTrigger> obj)
            {
                return (obj.StateFrom, obj.Trigger, obj.StateTo).GetHashCode();
            }

            public bool Equals(IFSMTransition<TState, TTrigger> transition, TState stateFrom, TTrigger trigger, TState stateTo)
            {
                return _stateComparer(transition.StateFrom, stateFrom) && _triggerComparer(transition.Trigger, trigger) && _stateComparer(transition.StateTo, stateTo);
            }
        }
    }
}
