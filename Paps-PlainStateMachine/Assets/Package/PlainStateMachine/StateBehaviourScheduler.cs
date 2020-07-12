using Paps.Maybe;
using System;

namespace Paps.StateMachines
{
    internal class StateBehaviourScheduler<TState>
    {
        private readonly StateCollection<TState> _states;

        private IState _currentStateObject;
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
        public bool IsStarted { get; private set; }

        public StateBehaviourScheduler(StateCollection<TState> stateCollection)
        {
            _states = stateCollection;
        }

        public void Start()
        {
            ValidateCanStart();

            _currentState = _states.InitialState.Value;
            _currentStateObject = _states.GetStateById(_states.InitialState.Value);

            IsStarted = true;

            _currentStateObject.Enter();
        }

        private void ValidateCanStart()
        {
            ValidateIsNotStarted();
            ValidateIsNotEmpty();
            ValidateInitialState();
        }

        private void ValidateIsNotStarted()
        {
            if (IsStarted)
                throw new StateMachineStartedException();
        }

        private void ValidateIsNotEmpty()
        {
            if (_states.StateCount == 0)
                throw new EmptyStateMachineException("State machine has no states");
        }

        private void ValidateInitialState()
        {
            if (_states.InitialState.HasValue == false)
                throw new InvalidInitialStateException("Initial state is not set");
        }

        public void Update()
        {
            if(IsStarted)
            {
                _currentStateObject.Update();
            }
        }

        public void Stop()
        {
            if(IsStarted)
            {
                var lastStateObject = _currentStateObject;

                IsStarted = false;
                _currentState = default;
                _currentStateObject = default;

                lastStateObject.Exit();
            }
        }

        public void SwitchTo(TState stateId, Action onStateChanged)
        {
            var nextState = stateId;
            var nextStateObj = _states.GetStateById(stateId);

            _currentStateObject.Exit();

            _currentState = nextState;
            _currentStateObject = nextStateObj;

            onStateChanged();

            _currentStateObject.Enter();
        }
    }
}