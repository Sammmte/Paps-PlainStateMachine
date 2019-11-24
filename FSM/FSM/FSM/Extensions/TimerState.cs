using System;
using System.Timers;

namespace Paps.FSM.Extensions
{
    public class TimerState<TState, TTrigger> : IFSMState<TState, TTrigger>
    {
        private Timer _timer;

        public IFSM<TState, TTrigger> StateMachine { get; private set; }

        private Action<TState> _onTimerElapsed;

        public TimerState(IFSM<TState, TTrigger> fsm, double milliseconds, Action<TState> onTimerElapsed)
        {
            _onTimerElapsed = onTimerElapsed;

            StateMachine = fsm;

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

        public void Update()
        {
            if(_timer.Enabled == false)
            {
                _onTimerElapsed(StateMachine.GetIdOf(this));
            }
        }
    }
}
