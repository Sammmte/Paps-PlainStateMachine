namespace Paps.StateMachines
{
    public interface IStateMachine<TState, TTrigger>
    {
        int StateCount { get; }
        int TransitionCount { get; }

        TState InitialState { get; set; }

        void AddState(TState stateId, IState state);
        bool RemoveState(TState stateId);

        bool ContainsState(TState stateId);

        TState[] GetStates();

        void AddTransition(Transition<TState, TTrigger> transition);
        bool RemoveTransition(Transition<TState, TTrigger> transition);

        bool ContainsTransition(Transition<TState, TTrigger> transition);

        Transition<TState, TTrigger>[] GetTransitions();

        IState GetStateById(TState stateId);

        void Trigger(TTrigger trigger);

        bool IsInState(TState stateId);
    }
}