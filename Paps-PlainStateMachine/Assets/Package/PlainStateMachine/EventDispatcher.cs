using System;
using System.Collections.Generic;

namespace Paps.StateMachines
{
    internal class EventDispatcher<TState>
    {
        private IEqualityComparer<TState> _stateComparer;
        private Dictionary<TState, List<IStateEventHandler>> _eventHandlers;

        private StateBehaviourScheduler<TState> _stateBehaviourScheduler;

        public EventDispatcher(IEqualityComparer<TState> stateComparer, StateBehaviourScheduler<TState> stateBehaviourScheduler)
        {
            _stateComparer = stateComparer ?? throw new ArgumentNullException(nameof(stateComparer));

            _eventHandlers = new Dictionary<TState, List<IStateEventHandler>>(_stateComparer);
            _stateBehaviourScheduler = stateBehaviourScheduler;
        }

        public void AddEventHandlerTo(TState stateId, IStateEventHandler eventHandler)
        {
            ValidateIsNotNull(eventHandler);

            if (_eventHandlers.ContainsKey(stateId) == false)
                _eventHandlers.Add(stateId, new List<IStateEventHandler>());

            _eventHandlers[stateId].Add(eventHandler);
        }

        private void ValidateIsNotNull(IStateEventHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException(nameof(eventHandler));
        }

        public bool RemoveEventHandlerFrom(TState stateId, IStateEventHandler eventHandler)
        {
            if (_eventHandlers.ContainsKey(stateId))
            {
                bool removed = _eventHandlers[stateId].Remove(eventHandler);

                if (_eventHandlers[stateId].Count == 0)
                    _eventHandlers.Remove(stateId);

                return removed;
            }

            return false;
        }

        public void RemoveEventHandlersFrom(TState stateId)
        {
            _eventHandlers.Remove(stateId);
        }

        public bool HasEventHandlerOn(TState stateId, IStateEventHandler eventHandler)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].Contains(eventHandler);

            return false;
        }

        public IStateEventHandler[] GetEventHandlersOf(TState stateId)
        {
            if (_eventHandlers.ContainsKey(stateId))
                return _eventHandlers[stateId].ToArray();
            else
                return null;
        }

        public bool SendEvent(IEvent ev)
        {
            var eventHandlers = _eventHandlers[_stateBehaviourScheduler.CurrentState.Value];

            for(int i = 0; i < eventHandlers.Count; i++)
            {
                if (eventHandlers[i].HandleEvent(ev))
                    return true;
            }

            return false;
        }
    }
}