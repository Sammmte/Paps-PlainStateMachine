namespace Paps.FSM
{
    public interface IGuardCondition<TState, TTrigger>
    {
        bool IsValid(TState stateFrom, TTrigger trigger, TState stateTo);
    }
}


