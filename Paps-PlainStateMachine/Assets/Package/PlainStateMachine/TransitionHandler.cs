using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines
{
    internal class TransitionHandler<TState, TTrigger>
    {
        public int TransitionCount => _transitions.Count;

        private IEqualityComparer<TState> _stateComparer;
        private IEqualityComparer<TTrigger> _triggerComparer;
        private TransitionEqualityComparer<TState, TTrigger> _transitionEqualityComparer;

        private ITransitionValidator<TState, TTrigger> _transitionValidator;

        private HashSet<Transition<TState, TTrigger>> _transitions = new HashSet<Transition<TState, TTrigger>>();

        private StateBehaviourScheduler<TState> _stateBehaviourScheduler;

        public StateChanged<TState, TTrigger> OnBeforeStateChanges;
        public StateChanged<TState, TTrigger> OnStateChanged;

        public TransitionHandler(IEqualityComparer<TState> stateComparer, IEqualityComparer<TTrigger> triggerComparer,
            ITransitionValidator<TState, TTrigger> transitionValidator, StateBehaviourScheduler<TState> stateBehaviourScheduler)
        {
            _stateBehaviourScheduler = stateBehaviourScheduler;
        }

        public void AddTransition(Transition<TState, TTrigger> transition)
        {
            _transitions.Add(transition);
        }

        public bool RemoveTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Remove(transition);
        }

        public bool ContainsTransition(Transition<TState, TTrigger> transition)
        {
            return _transitions.Contains(transition);
        }

        public Transition<TState, TTrigger>[] GetTransitions()
        {
            if (_transitions.Count > 0)
                return _transitions.ToArray();
            else
                return null;
        }

        public void Trigger(TTrigger trigger)
        {
            if (TryGetStateTo(trigger, out TState stateTo))
            {
                OnBeforeStateChanges?.Invoke(_stateBehaviourScheduler._currentState, trigger, stateTo);
                _stateBehaviourScheduler.SwitchTo(stateTo, 
                    () => OnStateChanged?.Invoke(_stateBehaviourScheduler._currentState, trigger, stateTo));
            }
        }

        private bool TryGetStateTo(TTrigger trigger, out TState stateTo)
        {
            stateTo = default;

            bool modifiedFlag = false;
            bool multipleValidGuardsFlag = false;

            foreach (Transition<TState, TTrigger> transition in _transitions)
            {
                if (_stateComparer.Equals(transition.StateFrom, _stateBehaviourScheduler._currentState)
                    && _triggerComparer.Equals(transition.Trigger, trigger)
                    && _transitionValidator.IsValid(transition))
                {
                    if (multipleValidGuardsFlag)
                    {
                        throw new MultipleValidTransitionsFromSameStateException(
                            _stateBehaviourScheduler._currentState.ToString(), trigger.ToString());
                    }

                    stateTo = transition.StateTo;

                    modifiedFlag = true;
                    multipleValidGuardsFlag = true;
                }
            }

            return modifiedFlag;
        }
    }
}