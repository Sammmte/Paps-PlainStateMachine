namespace Paps.StateMachines.Extensions
{
    public class EmptyState : IState
    {
        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }

        public bool HandleEvent(IEvent messageEvent)
        {
            return false;
        }

        public void Update()
        {
            
        }
    }
}