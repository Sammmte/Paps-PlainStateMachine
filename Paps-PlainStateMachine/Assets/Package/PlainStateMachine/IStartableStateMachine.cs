namespace Paps.StateMachines
{
    public interface IStartableStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        bool IsStarted { get; }

        void Start();
        void Stop();
    }
}