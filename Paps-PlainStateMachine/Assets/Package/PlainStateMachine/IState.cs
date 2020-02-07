namespace Paps.StateMachines
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }
}
