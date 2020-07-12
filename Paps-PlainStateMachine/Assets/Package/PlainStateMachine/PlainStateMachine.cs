using System;
using System.Collections.Generic;
using System.Linq;
using Paps.Maybe;

namespace Paps.StateMachines
{
    public class PlainStateMachine<TState, TTrigger> : IPlainStateMachine<TState, TTrigger>
    {
        public int StateCount => _states.Count;
        public int TransitionCount => _transitions.Count;
        public bool IsStarted { get; private set; }

        public Maybe<TState> InitialState { get; private set; }

        public event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public event StateChanged<TState, TTrigger> OnStateChanged;

        private Dictionary<TState, IState> _states;
        private HashSet<Transition<TState, TTrigger>> _transitions;
        private Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>> _guardConditions;
        private Dictionary<TState, HashSet<IStateEventHandler>> _stateEventHandlers;

        private Queue<TransitionCommand> _transitionCommandQueue;

        private TState _protectedNextState;

        private TState _currentState { get; set; }
        public Maybe<TState> CurrentState
        {
            get
            {
                if (IsStarted)
                    return _currentState.ToMaybe();
                else
                    return Maybe<TState>.Nothing;
            }
        }
        private IState _currentStateObject;

        private StateEqualityComparer _stateComparer;
        private TriggerEqualityComparer _triggerComparer;
        
        private TransitionEqualityComparer<TState, TTrigger> _transitionEqualityComparer;

        public PlainStateMachine(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer)
        {
            if (stateComparer == null) throw new ArgumentNullException(nameof(stateComparer));
            if (triggerComparer == null) throw new ArgumentNullException(nameof(triggerComparer));

            _stateComparer = new StateEqualityComparer(stateComparer);
            _triggerComparer = new TriggerEqualityComparer(triggerComparer);

            _transitionEqualityComparer = new TransitionEqualityComparer<TState, TTrigger>(_stateComparer, _triggerComparer);

            _states = new Dictionary<TState, IState>(_stateComparer);
            _transitions = new HashSet<Transition<TState, TTrigger>>(_transitionEqualityComparer);
            _guardConditions = new Dictionary<Transition<TState, TTrigger>, List<IGuardCondition>>(_transitionEqualityComparer);
            _stateEventHandlers = new Dictionary<TState, HashSet<IStateEventHandler>>(_stateComparer);
            _transitionCommandQueue = new Queue<TransitionCommand>();
        }

        public PlainStateMachine() : this(EqualityComparer<TState>.Default, EqualityComparer<TTrigger>.Default)
        {
            
        }

        public void SetInitialState(TState stateId)
        {
            ValidateHasStateWithId(stateId);

            InitialState = stateId.ToMaybe();
        }

        public void SetStateComparer(IEqualityComparer<TState> stateComparer)
        {
            if (stateComparer == null) throw new ArgumentNullException(nameof(stateComparer));

            _stateComparer.SetEqualityComparer(stateComparer);
        }

        public void SetTriggerComparer(IEqualityComparer<TTrigger> triggerComparer)
        {
            if (triggerComparer == null) throw new ArgumentNullException(nameof(triggerComparer));

            _triggerComparer.SetEqualityComparer(triggerComparer);
        }

        public void Start()
        {
            ValidateCanStart();

            //SetInternalState(PlainStateMachineInternalState.Idle);
            IsStarted = true;

            _currentState = InitialState.Value;
            _currentStateObject = GetStateById(_currentState);
            
            EnterCurrentState();
        }

        private void ValidateCanStart()
        {
            //ValidateIsNotIn(PlainStateMachineInternalState.Stopping);
            ValidateIsNotStarted();
            ValidateIsNotEmpty();
            ValidateInitialState();
        }

        private void ValidateIsNotEmpty()
        {
            if (_states.Count == 0) throw new EmptyStateMachineException("State machine cannot be started because it has no states");
        }
        
        private void EnterCurrentState()
        {
            _currentStateObject.Enter();
        }

        public void Update()
        {
            UpdateCurrentState();
        }

        private void UpdateCurrentState()
        {
            ValidateIsStarted();

            _currentStateObject.Update();
        }

        public void Stop()
        {
            if(IsStarted)
            {
                ExitCurrentState();
            }

            IsStarted = false;

            /*if(IsIn(PlainStateMachineInternalState.Idle))
            {
                _internalState = PlainStateMachineInternalState.Stopping;
                ExitCurrentState();
                _internalState = PlainStateMachineInternalState.Stopped;
            }
            else
            {
                ThrowByInternalState();
            }*/
        }

        private void ExitCurrentState()
        {
            _currentStateObject.Exit();
            _currentStateObject = null;
        }

        private void ValidateInitialState()
        {
            if (InitialState.HasValue == false || ContainsState(InitialState.Value) == false)
                throw new InvalidInitialStateException("State machine does not contains current initial state");
        }

        private void ValidateIsStarted()
        {
            if (IsStarted == false)
            {
                throw new StateMachineNotStartedException();
            }
        }

        private void ValidateIsNotStarted()
        {
            if (IsStarted)
            {
                throw new StateMachineStartedException();
            }
        }

        public TState[] GetStates()
        {
            return _states.Keys.ToArray();
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            return _transitions.ToArray();
        }

        public void AddState(TState stateId, IState state)
        {
            ValidateCanAddState(stateId, state);

            InternalAddState(stateId, state);
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

        private void InternalAddState(TState stateId, IState state)
        {
            _states.Add(stateId, state);

            if (_states.Count == 1)
            {
                InitialState = stateId.ToMaybe();
            }
        }

        public bool RemoveState(TState stateId)
        {
            if(ContainsState(stateId))
            {
                //ValidateIsNotIn(PlainStateMachineInternalState.EvaluatingTransitions);
                ValidateIsNotCurrentIfIsStarted(stateId);
                ValidateIsNotNextStateOnTransition(stateId);

                _states.Remove(stateId);

                RemoveTransitionsRelatedTo(stateId);
                RemoveEventHandlersOf(stateId);

                if(_states.Count == 0)
                {
                    InitialState = Maybe<TState>.Nothing;
                }

                return true;
            }

            return false;
        }

        private void ValidateIsNotNextStateOnTransition(TState stateId)
        {
            /*if (IsIn(PlainStateMachineInternalState.Transitioning))
            {
                if (_stateComparer.Equals(stateId, _protectedNextState))
                    throw new ProtectedStateException("Cannot remove protected state " + stateId + " because it takes part on the current transition");
            }*/
        }

        private void ValidateIsNotCurrentIfIsStarted(TState stateId)
        {
            if(IsStarted && _stateComparer.Equals(_currentState, stateId))
            {
                throw new InvalidOperationException("Cannot remove current state");
            }
        }

        private void RemoveEventHandlersOf(TState stateId)
        {
            _stateEventHandlers.Remove(stateId);
        }

        private void RemoveTransitionsRelatedTo(TState stateId)
        {
            Transition<TState, TTrigger>[] transitions = GetTransitionsRelatedTo(stateId);

            for(int i = 0; i < transitions.Length; i++)
            {
                var transition = transitions[i];

                _transitions.Remove(transition);

                RemoveAllGuardConditionsRelatedTo(transition);
            }
        }

        private Transition<TState, TTrigger>[] GetTransitionsRelatedTo(TState stateId)
        {
            return _transitions.Where(
                (transition) => _stateComparer.Equals(transition.StateFrom, stateId) || _stateComparer.Equals(transition.StateTo, stateId)
                ).ToArray();
        }

        public IState GetStateById(TState stateId)
        {
            if(_states.ContainsKey(stateId))
            {
                return _states[stateId];
            }

            throw new StateIdNotAddedException(stateId.ToString());
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            ValidateHasStateWithId(transition.StateFrom);
            ValidateHasStateWithId(transition.StateTo);
            //ValidateIsNotIn(PlainStateMachineInternalState.EvaluatingTransitions);

            _transitions.Add(transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            if(ContainsTransition(transition))
            {
                //ValidateIsNotIn(PlainStateMachineInternalState.EvaluatingTransitions);

                _transitions.Remove(transition);

                RemoveAllGuardConditionsRelatedTo(transition);
                return true;
            }

            return false;
        }

        private void RemoveAllGuardConditionsRelatedTo(Transition<TState, TTrigger> transition)
        {
            _guardConditions.Remove(transition);
        }

        private void ValidateHasStateWithId(TState stateId)
        {
            if(ContainsState(stateId) == false)
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

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Contains(transition);
        }

        public void Trigger(TTrigger trigger)
        {
            ValidateIsStarted();
            //ValidateIsNotIn(PlainStateMachineInternalState.Stopping);

            bool shouldStart = _transitionCommandQueue.Count == 0;

            _transitionCommandQueue.Enqueue(new TransitionCommand() { trigger = trigger });

            if(shouldStart)
            {
                TriggerQueued();
            }
            
            /*if (IsIn(PlainStateMachineInternalState.EvaluatingTransitions) == false &&
                IsIn(PlainStateMachineInternalState.Transitioning) == false)
            {
                SetInternalState(PlainStateMachineInternalState.EvaluatingTransitions);
                TriggerQueued();
                SetInternalState(PlainStateMachineInternalState.Idle);
            }*/
        }

        private void TriggerQueued()
        {
            while(_transitionCommandQueue.Count > 0)
            {
                //SetInternalState(PlainStateMachineInternalState.EvaluatingTransitions);

                TransitionCommand transition = _transitionCommandQueue.Dequeue();

                TState stateTo = default;

                try
                {
                    if(TryGetStateTo(transition.trigger, out stateTo))
                    {
                        Transition(transition.trigger, stateTo);
                    }
                }
                catch(MultipleValidTransitionsFromSameStateException)
                {
                    _transitionCommandQueue.Clear();
                    //SetInternalState(PlainStateMachineInternalState.Idle);
                    throw;
                }
                catch
                {
                    throw;
                }
            }
        }

        private bool TryGetStateTo(TTrigger trigger, out TState stateTo)
        {
            stateTo = default;

            bool modifiedFlag = false;
            bool multipleValidGuardsFlag = false;

            foreach (Transition<TState, TTrigger> transition in _transitions)
            {
                if (_stateComparer.Equals(transition.StateFrom, _currentState) 
                    && _triggerComparer.Equals(transition.Trigger, trigger)
                    && IsValidTransition(transition))
                {
                    if(multipleValidGuardsFlag)
                    {
                        throw new MultipleValidTransitionsFromSameStateException(_currentState.ToString(), trigger.ToString());
                    }
                    
                    stateTo = transition.StateTo;

                    modifiedFlag = true;
                    multipleValidGuardsFlag = true;
                }
            }

            return modifiedFlag;
        } 

        private void Transition(TTrigger trigger, TState stateTo)
        {
            //SetInternalState(PlainStateMachineInternalState.Transitioning);

            _protectedNextState = stateTo;
            
            TState previous = _currentState;
            
            OnBeforeStateChanges?.Invoke(previous, trigger, stateTo);
            
            ExitCurrentState();

            _currentState = stateTo;
            _currentStateObject = GetStateById(_currentState);
            
            OnStateChanged?.Invoke(previous, trigger, stateTo);
            
            EnterCurrentState();

            _protectedNextState = default;
        }

        private bool IsValidTransition(Transition<TState, TTrigger> transition)
        {
            return AllGuardConditionsTrueOrHasNone(transition);
        }

        private bool AllGuardConditionsTrueOrHasNone(Transition<TState, TTrigger> transition)
        {
            if(_guardConditions.ContainsKey(transition))
            {
                var guardConditionsOfTransition = _guardConditions[transition];

                foreach (var guardCondition in guardConditionsOfTransition)
                {
                    if (guardCondition.IsValid() == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateHasTransition(transition);
            ValidateGuardConditionIsNotNull(guardCondition);
            //ValidateIsNotIn(PlainStateMachineInternalState.EvaluatingTransitions);

            if(_guardConditions.ContainsKey(transition) == false)
            {
                _guardConditions.Add(transition, new List<IGuardCondition>());
            }

            _guardConditions[transition].Add(guardCondition);
        }

        public bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            ValidateHasTransition(transition);
            ValidateGuardConditionIsNotNull(guardCondition);

            if (ContainsGuardConditionOn(transition, guardCondition))
            {
                //ValidateIsNotIn(PlainStateMachineInternalState.EvaluatingTransitions);

                _guardConditions[transition].Remove(guardCondition);

                if (_guardConditions[transition].Count == 0)
                {
                    _guardConditions.Remove(transition);
                }

                return true;
            }

            return false;
        }

        public bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition)
        {
            if(_guardConditions.ContainsKey(transition))
            {
                return _guardConditions[transition].Contains(guardCondition);
            }

            return false;
        }

        public IGuardCondition[] GetGuardConditionsOf(Transition<TState, TTrigger> transition)
        {
            ValidateHasTransition(transition);

            if (_guardConditions.ContainsKey(transition))
                return _guardConditions[transition].ToArray();
            else
                return null;
        }

        private void ValidateHasTransition(Transition<TState, TTrigger> transition)
        {
            if(ContainsTransition(transition) == false)
            {
                throw new TransitionNotAddedException(transition.StateFrom.ToString(), transition.Trigger.ToString(), transition.StateTo.ToString());
            }
        }

        private void ValidateGuardConditionIsNotNull(object guardCondition)
        {
            if(guardCondition == null)
            {
                throw new ArgumentNullException("Guard condition was null");
            }
        }

        public void SubscribeEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateHasStateWithId(stateId);

            if (_stateEventHandlers.ContainsKey(stateId) == false)
                _stateEventHandlers.Add(stateId, new HashSet<IStateEventHandler>());

            _stateEventHandlers[stateId].Add(eventHandler);
        }

        public bool UnsubscribeEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateHasStateWithId(stateId);

            if(_stateEventHandlers.ContainsKey(stateId))
            {
                _stateEventHandlers[stateId].Remove(eventHandler);

                if (_stateEventHandlers[stateId].Count == 0)
                {
                    _stateEventHandlers.Remove(stateId);
                }

                return true;
            }

            return false;
        }

        public bool HasEventHandlerOn(TState stateId, IStateEventHandler eventListener)
        {
            return _stateEventHandlers.ContainsKey(stateId) && _stateEventHandlers[stateId].Contains(eventListener);
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            ValidateHasStateWithId(stateId);

            if (_stateEventHandlers.ContainsKey(stateId))
                return _stateEventHandlers[stateId].ToArray();
            else
                return null;
        }

        public bool SendEvent(IEvent messageEvent)
        {
            ValidateIsStarted();

            if(_stateEventHandlers.ContainsKey(_currentState))
            {
                var eventHandlers = _stateEventHandlers[_currentState];

                foreach(IStateEventHandler eventHandler in eventHandlers)
                {
                    if (eventHandler.HandleEvent(messageEvent))
                        return true;
                }
            }

            return false;
        }

        public bool IsInState(TState stateId)
        {
            return IsStarted && _stateComparer.Equals(_currentState, stateId);
        }

        private struct TransitionCommand
        {
            public TTrigger trigger;
        }

        private class StateEqualityComparer : IEqualityComparer<TState>
        {
            private IEqualityComparer<TState> _equalityComparer;

            public StateEqualityComparer(IEqualityComparer<TState> equalityComparer)
            {
                SetEqualityComparer(equalityComparer);
            }

            public bool Equals(TState x, TState y)
            {
                return _equalityComparer.Equals(x, y);
            }

            public int GetHashCode(TState obj)
            {
                return _equalityComparer.GetHashCode(obj);
            }

            public void SetEqualityComparer(IEqualityComparer<TState> equalityComparer)
            {
                _equalityComparer = equalityComparer ?? EqualityComparer<TState>.Default;
            }
        }

        private class TriggerEqualityComparer : IEqualityComparer<TTrigger>
        {
            private IEqualityComparer<TTrigger> _equalityComparer;

            public TriggerEqualityComparer(IEqualityComparer<TTrigger> equalityComparer)
            {
                SetEqualityComparer(equalityComparer);
            }

            public bool Equals(TTrigger x, TTrigger y)
            {
                return _equalityComparer.Equals(x, y);
            }

            public int GetHashCode(TTrigger obj)
            {
                return _equalityComparer.GetHashCode(obj);
            }

            public void SetEqualityComparer(IEqualityComparer<TTrigger> equalityComparer)
            {
                _equalityComparer = equalityComparer ?? EqualityComparer<TTrigger>.Default;
            }
        }
    }
}
