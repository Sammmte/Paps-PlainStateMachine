namespace Paps.FSM
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();

        bool HandleEvent(IEvent messageEvent);
    }
}
