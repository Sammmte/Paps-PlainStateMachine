namespace Paps.StateMachines
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