namespace Paps.FSM
{
    public static class IFSMStateCoreExtensions
    {
        public static TState GetStateId<TState, TTrigger>(this IFSMState<TState, TTrigger> state)
        {
            if(state.StateMachine == null)
            {
                throw new UnboundFSMException();
            }

            return state.StateMachine.GetIdOf(state);
        }
    }
}
