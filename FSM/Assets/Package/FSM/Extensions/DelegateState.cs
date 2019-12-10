using System;

namespace Paps.FSM.Extensions
{
    public class DelegateState<TState, TTrigger> : IState
    {
        protected Action onEnter;
        protected Action onUpdate;
        protected Action onExit;

        public DelegateState(Action onEnter, Action onUpdate, Action onExit)
        {
            this.onEnter = onEnter;
            this.onUpdate = onUpdate;
            this.onExit = onExit;
        }

        public void Enter()
        {
            if(onEnter != null)
            {
                onEnter();
            }
        }

        public void Update()
        {
            if(onUpdate != null)
            {
                onUpdate();
            }
        }

        public void Exit()
        {
            if(onExit != null)
            {
                onExit();
            }
        }
    }
}
