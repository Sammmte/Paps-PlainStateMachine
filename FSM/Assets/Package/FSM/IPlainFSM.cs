namespace Paps.FSM
{
    public interface IPlainFSM<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        TState CurrentState { get; }

        event StateChanged<TState, TTrigger> OnBeforeStateChanges;
        event StateChanged<TState, TTrigger> OnStateChanged;
    }
}
