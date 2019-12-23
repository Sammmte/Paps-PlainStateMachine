using System;
using System.Timers;

namespace Paps.FSM.Extensions
{
    public class TimerState<TState, TTrigger> : IState
    {
        private Timer _timer;

        private Action<TState> _onTimerElapsed;

        private IFSM<TState, TTrigger> _stateMachine;

        public TimerState(IFSM<TState, TTrigger> fsm, double milliseconds, Action<TState> onTimerElapsed)
        {
            _stateMachine = fsm;

            _onTimerElapsed = onTimerElapsed;

            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = milliseconds;
        }

        public void Enter()
        {
            _timer.Start();
        }

        public void Exit()
        {
            _timer.Stop();
        }

        public bool HandleEvent(IEvent messageEvent)
        {
            return false;
        }

        public void Update()
        {
            if (_timer.Enabled == false)
            {
                _onTimerElapsed(_stateMachine.GetIdOf(this));
            }
        }
    }
}