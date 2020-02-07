namespace Paps.StateMachines
{
    public interface IUpdatableStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void Update();
    }
}
