using System;

namespace Paps.FSM.Extensions
{
    public class CompositeState : IState
    {
        private IState[] _states;

        public CompositeState(IState[] innerStates)
        {
            _states = innerStates ?? throw new ArgumentNullException(nameof(innerStates));
        }

        public void Enter()
        {
            for(int i = 0; i < _states.Length; i++)
            {
                _states[i].Enter();
            }
        }

        public void Exit()
        {
            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].Exit();
            }
        }

        public void Update()
        {
            for (int i = 0; i < _states.Length; i++)
            {
                _states[i].Update();
            }
        }
    }
}