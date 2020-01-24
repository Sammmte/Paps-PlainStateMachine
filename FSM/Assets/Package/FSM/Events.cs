namespace Paps.FSM
{
    public delegate void StateChanged<TState, TTrigger>(TState previous, TTrigger trigger, TState current);
}
