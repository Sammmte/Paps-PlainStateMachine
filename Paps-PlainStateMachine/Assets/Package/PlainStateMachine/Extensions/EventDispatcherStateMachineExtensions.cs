using System;

namespace Paps.StateMachines.Extensions
{
    public static class FSMEventDispatcherExtensions
    {
        public static void SubscribeEventHandlerTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            fsm.SubscribeEventHandlerTo(stateId, new DelegateStateEventHandler(method));
        }

        public static void UnsubscribeEventHandlerFrom<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            fsm.UnsubscribeEventHandlerFrom(stateId, new DelegateStateEventHandler(method));
        }

        public static bool HasEventHandler<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, Func<IEvent, bool> method)
        {
            return fsm.HasEventHandler(stateId, new DelegateStateEventHandler(method));
        }

        public static void SubscribeEventHandlersTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, params IStateEventHandler[] eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                fsm.SubscribeEventHandlerTo(stateId, eventHandler);
            }
        }

        public static void SubscribeEventHandlersTo<TState, TTrigger>(this IEventDispatcherStateMachine<TState, TTrigger> fsm, TState stateId, params Func<IEvent, bool>[] methods)
        {
            foreach(var method in methods)
            {
                fsm.SubscribeEventHandlerTo(stateId, new DelegateStateEventHandler(method));
            }
        }
    }
}