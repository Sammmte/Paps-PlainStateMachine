using System;

namespace Paps.FSM
{
    public readonly struct FSMTransition<TState, TTrigger> : IEquatable<FSMTransition<TState, TTrigger>>, IFSMTransition<TState, TTrigger>
    {
        private readonly TState stateFrom;
        private readonly TTrigger trigger;
        private readonly TState stateTo;

        public TState StateFrom { get => stateFrom; }
        public TTrigger Trigger { get => trigger; }
        public TState StateTo { get => stateTo; }

        private readonly Func<TState, TState, bool> _stateComparer;
        private readonly Func<TTrigger, TTrigger, bool> _triggerComparer;

        public FSMTransition(TState stateFrom, TTrigger trigger, TState stateTo, 
            Func<TState, TState, bool> stateComparer = null, Func<TTrigger, TTrigger, bool> triggerComparer = null)
        {
            this.stateFrom = stateFrom;
            this.trigger = trigger;
            this.stateTo = stateTo;

            _stateComparer = stateComparer == null ? DefaultComparer : stateComparer;
            _triggerComparer = triggerComparer == null ? DefaultComparer : triggerComparer;
        }

        

        private static bool DefaultComparer<T>(T first, T second)
        {
            return first.Equals(second);
        }

        public bool Equals(FSMTransition<TState, TTrigger> other)
        {
            return _stateComparer(stateFrom, other.stateFrom) && _triggerComparer(trigger, other.trigger) && _stateComparer(stateTo, other.stateTo);
        }

        public override int GetHashCode()
        {
            return (stateFrom, trigger, stateTo).GetHashCode();
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
