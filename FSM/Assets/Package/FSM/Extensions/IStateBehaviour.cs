namespace Paps.FSM.Extensions
{
    public interface IStateBehaviour
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}