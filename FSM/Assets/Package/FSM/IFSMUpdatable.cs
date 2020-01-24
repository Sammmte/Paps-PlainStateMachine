namespace Paps.FSM
{
    public interface IFSMUpdatable<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        void Update();
    }
}
