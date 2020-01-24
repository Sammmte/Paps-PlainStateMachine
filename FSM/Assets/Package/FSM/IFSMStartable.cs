namespace Paps.FSM
{
    public interface IFSMStartable<TState, TTrigger> : IFSM<TState, TTrigger>
    {
        bool IsStarted { get; }

        void Start();
        void Stop();
    }
}