namespace Paps.FSM
{
    public interface IFSMTransition<TState, TTrigger>
    {
        TState StateFrom { get; }
        TTrigger Trigger { get; }
        TState StateTo { get; }
    }
}
