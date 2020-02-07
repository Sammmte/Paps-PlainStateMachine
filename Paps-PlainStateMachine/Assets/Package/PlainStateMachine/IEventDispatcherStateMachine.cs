namespace Paps.StateMachines
{
    public interface IEventDispatcherStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void SubscribeEventHandlerTo(TState stateId, IStateEventHandler eventHandler);
        void UnsubscribeEventHandlerFrom(TState stateId, IStateEventHandler eventHandler);

        bool HasEventHandler(TState stateId, IStateEventHandler eventHandler);
        bool HasEventListener(TState stateId);

        bool SendEvent(IEvent ev);
    }
}