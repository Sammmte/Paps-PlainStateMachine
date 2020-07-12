namespace Paps.StateMachines
{
    internal interface ITransitionValidator<TState, TTrigger>
    {
        bool IsValid(Transition<TState, TTrigger> transition);
    }
}