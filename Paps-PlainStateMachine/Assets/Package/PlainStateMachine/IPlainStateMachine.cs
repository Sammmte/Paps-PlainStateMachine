namespace Paps.StateMachines
{
    public interface IPlainStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        TState CurrentState { get; }

        event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        event StateChanged<TState, TTrigger> OnStateChanged;
    }
}
