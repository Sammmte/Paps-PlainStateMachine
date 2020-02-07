namespace Paps.StateMachines.Extensions
{
    public interface IStateBehaviour
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}