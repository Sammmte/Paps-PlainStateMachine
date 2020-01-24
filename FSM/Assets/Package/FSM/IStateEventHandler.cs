namespace Paps.FSM
{
    public interface IStateEventHandler
    {
        bool HandleEvent(IEvent ev);
    }

}