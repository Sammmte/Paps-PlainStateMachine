using System;

namespace Paps.FSM.Extensions
{
    public class DelegateFSMState<TState, TTrigger> : State<TState, TTrigger>
    {
        protected Action onEnter;
        protected Action onUpdate;
        protected Action onExit;

        public DelegateFSMState(IFSM<TState, TTrigger> fsm, Action onEnter, Action onUpdate, Action onExit) : base(fsm)
        {
            this.onEnter = onEnter;
            this.onUpdate = onUpdate;
            this.onExit = onExit;
        }

        protected override void OnEnter()
        {
            if(onEnter != null)
            {
                onEnter();
            }
        }

        protected override void OnUpdate()
        {
            if(onUpdate != null)
            {
                onUpdate();
            }
        }

        protected override void OnExit()
        {
            if(onExit != null)
            {
                onExit();
            }
        }
    }
}
