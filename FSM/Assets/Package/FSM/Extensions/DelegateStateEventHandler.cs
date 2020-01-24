using System;

namespace Paps.FSM.Extensions
{
    public class DelegateStateEventHandler : IStateEventHandler
    {
        private Func<IEvent, bool> _method;

        public DelegateStateEventHandler(Func<IEvent, bool> method)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public override bool Equals(object obj)
        {
            if(obj is DelegateStateEventHandler cast)
            {
                return _method == cast._method;
            }

            return false;
        }

        public static bool operator ==(DelegateStateEventHandler first, DelegateStateEventHandler second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(DelegateStateEventHandler first, DelegateStateEventHandler second)
        {
            return !first.Equals(second);
        }

        public bool HandleEvent(IEvent ev)
        {
            return _method(ev);
        }

        public override int GetHashCode()
        {
            return _method.GetHashCode();
        }
    }
}