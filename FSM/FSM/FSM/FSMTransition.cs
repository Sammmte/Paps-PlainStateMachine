using System;

namespace Paps.FSM
{
    public readonly struct FSMTransition<TState, TTrigger> : IEquatable<FSMTransition<TState, TTrigger>>
    {
        public readonly TState StateFrom;
        public readonly TTrigger Trigger;
        public readonly TState StateTo;

        private readonly Func<TState, TState, bool> _stateComparer;
        private readonly Func<TTrigger, TTrigger, bool> _triggerComparer;

        public FSMTransition(TState stateFrom, TTrigger trigger, TState stateTo, 
            Func<TState, TState, bool> stateComparer = null, Func<TTrigger, TTrigger, bool> triggerComparer = null)
        {
            this.StateFrom = stateFrom;
            this.Trigger = trigger;
            this.StateTo = stateTo;

            _stateComparer = stateComparer == null ? DefaultComparer : stateComparer;
            _triggerComparer = triggerComparer == null ? DefaultComparer : triggerComparer;
        }

        private static bool DefaultComparer<T>(T first, T second)
        {
            return first.Equals(second);
        }

        public bool Equals(FSMTransition<TState, TTrigger> other)
        {
            return _stateComparer(StateFrom, other.StateFrom) && _triggerComparer(Trigger, other.Trigger) && _stateComparer(StateTo, other.StateTo);
        }

        public override int GetHashCode()
        {
            return (StateFrom, Trigger, StateTo).GetHashCode();
        }

        public static bool operator ==(FSMTransition<TState, TTrigger> transition1, FSMTransition<TState, TTrigger> transition2)
        {
            return transition1.Equals(transition2);
        }

        public static bool operator !=(FSMTransition<TState, TTrigger> transition1, FSMTransition<TState, TTrigger> transition2)
        {
            return transition1.Equals(transition2) == false;
        }
    }
}
