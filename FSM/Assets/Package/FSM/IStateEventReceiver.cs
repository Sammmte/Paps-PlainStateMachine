namespace Paps.FSM
{
    public interface IStateEventReceiver<TEvent> : IState
    {
        bool HandleEvent(TEvent messageEvent);
    }
}