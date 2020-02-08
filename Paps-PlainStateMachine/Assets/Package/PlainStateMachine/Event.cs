namespace Paps.StateMachines
{
    public class Event<T> : IEvent<T>
    {
        private T _value;

        public Event(T value)
        {
            _value = value;
        }

        public T GetEventData()
        {
            return _value;
        }

        object IEvent.GetEventData()
        {
            return _value;
        }
    }
}