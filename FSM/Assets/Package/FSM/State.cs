using System;

namespace Paps.FSM
{
    public class State<TState, TTrigger> : IState
    {
        public IFSM<TState, TTrigger> StateMachine { get; private set; }

        public State(IFSM<TState, TTrigger> stateMachine)
        {
            if (stateMachine == null) throw new ArgumentNullException(nameof(stateMachine));

            StateMachine = stateMachine;
        }
               
        public void Enter()
        {
            OnEnter();
        }      
               
        public void Exit()
        {
            OnExit();
        }      
               
        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }
}
