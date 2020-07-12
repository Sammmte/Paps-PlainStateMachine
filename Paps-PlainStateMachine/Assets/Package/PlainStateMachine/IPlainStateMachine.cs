using Paps.Maybe;

namespace Paps.StateMachines
{
    public interface IPlainStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>, IStartableStateMachine<TState, TTrigger>, IUpdatableStateMachine<TState, TTrigger>,
        IEventDispatcherStateMachine<TState, TTrigger>, IGuardedStateMachine<TState, TTrigger>
    {
        Maybe<TState> CurrentState { get; }

        event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        event StateChanged<TState, TTrigger> OnStateChanged;
    }
}
