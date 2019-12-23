namespace Paps.FSM
{
    public interface IEvent
    {
        object GetEventData();
    }

    public interface IEvent<T> : IEvent
    {
        new T GetEventData();
    }
}