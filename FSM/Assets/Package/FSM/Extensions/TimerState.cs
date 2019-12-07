using System;
using System.Timers;

namespace Paps.FSM.Extensions
{
    public class TimerState<TState, TTrigger> : State<TState, TTrigger>
    {
        private Timer _timer;

        private Action<TState> _onTimerElapsed;

        public TimerState(IFSM<TState, TTrigger> fsm, double milliseconds, Action<TState> onTimerElapsed) : base(fsm)
        {
            _onTimerElapsed = onTimerElapsed;

            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = milliseconds;
        }

        protected override void OnEnter()
        {
            _timer.Start();
        }

        protected override void OnExit()
        {
            _timer.Stop();
        }

        protected override void OnUpdate()
        {
            if (_timer.Enabled == false)
            {
                _onTimerElapsed(StateMachine.GetIdOf(this));
            }
        }
    }
}
