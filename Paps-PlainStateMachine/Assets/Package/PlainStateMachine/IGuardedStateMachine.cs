namespace Paps.StateMachines
{
    public interface IGuardedStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        void AddGuardConditionTo(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);
        bool RemoveGuardConditionFrom(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        bool ContainsGuardConditionOn(Transition<TState, TTrigger> transition, IGuardCondition guardCondition);

        IGuardCondition[] GetGuardConditionsOf(Transition<TState, TTrigger> transition);
    }
}
