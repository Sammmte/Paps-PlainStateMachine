using System;

namespace Paps.StateMachines
{
    public struct Transition<TState, TTrigger> : IEquatable<Transition<TState, TTrigger>>
    {
        public TState StateFrom { get; private set; }
        public TTrigger Trigger { get; private set; }
        public TState StateTo { get; private set; }

        private readonly Func<TState, TState, bool> _stateComparer;
        private readonly Func<TTrigger, TTrigger, bool> _triggerComparer;

        public Transition(TState stateFrom, TTrigger trigger, TState stateTo, 
            Func<TState, TState, bool> stateComparer = null, Func<TTrigger, TTrigger, bool> triggerComparer = null)
        {
            StateFrom = stateFrom;
            Trigger = trigger;
            StateTo = stateTo;

            _stateComparer = stateComparer == null ? DefaultComparer : stateComparer;
            _triggerComparer = triggerComparer == null ? DefaultComparer : triggerComparer;
        }

        private static bool DefaultComparer<T>(T first, T second)
        {
            return first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if(obj is Transition<TState, TTrigger> cast)
            {
                return Equals(cast);
            }

            return false;
        }

        public bool Equals(Transition<TState, TTrigger> other)
        {
            return _stateComparer(StateFrom, other.StateFrom) && _triggerComparer(Trigger, other.Trigger) && _stateComparer(StateTo, other.StateTo);
        }

        public override int GetHashCode()
        {
            return (StateFrom, Trigger, StateTo).GetHashCode();
        }

        public static bool operator ==(Transition<TState, TTrigger> transition1, Transition<TState, TTrigger> transition2)
        {
            return transition1.Equals(transition2);
        }

        public static bool operator !=(Transition<TState, TTrigger> transition1, Transition<TState, TTrigger> transition2)
        {
            return transition1.Equals(transition2) == false;
        }
    }
}
