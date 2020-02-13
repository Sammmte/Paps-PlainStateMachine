namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public interface IStateBehaviour
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}