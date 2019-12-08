namespace Paps.FSM
{
    public interface IFSMEventSender<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        bool SendEvent<TEvent>(TEvent messageEvent);
    }
}