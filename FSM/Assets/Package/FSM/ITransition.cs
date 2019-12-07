namespace Paps.FSM
{
    public interface ITransition<TState, TTrigger>
    {
        TState StateFrom { get; }
        TTrigger Trigger { get; }
        TState StateTo { get; }
    }
}
