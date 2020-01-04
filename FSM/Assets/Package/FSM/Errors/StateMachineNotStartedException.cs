using System;

namespace Paps.FSM
{
    public class StateMachineNotStartedException : Exception
    {
        public StateMachineNotStartedException() : base("State machine is not started. Try using Start() method")
        {

        }
    }
}
