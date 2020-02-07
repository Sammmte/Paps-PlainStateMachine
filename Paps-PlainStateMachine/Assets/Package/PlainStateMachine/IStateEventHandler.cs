namespace Paps.StateMachines
{
    public interface IStateEventHandler
    {
        bool HandleEvent(IEvent ev);
    }

}