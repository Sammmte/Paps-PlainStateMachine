namespace Paps.FSM
{
    public interface IFSMEventDispatcher<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        void SubscribeEventHandlerTo(TState stateId, IStateEventHandler eventHandler);
        void UnsubscribeEventHandlerOf(TState stateId);

        bool HasEventListener(TState stateId, IStateEventHandler eventHandler);
        bool HasEventListener(TState stateId);

        bool SendEvent(IEvent ev);
    }
}