public delegate void StateChange<TState, TTrigger>(TState stateFrom, TTrigger trigger, TState stateTo);

public interface IFSMEventNotifier<TState, TTrigger>
{
    event StateChange<TState, TTrigger> OnBeforeStateChanges;
    event StateChange<TState, TTrigger> OnStateChanged;
}
