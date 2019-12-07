namespace Paps.FSM
{
    public interface IFSMState
    {
        void Enter();
        void Update();
        void Exit();
    }
}
