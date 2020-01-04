using System;

namespace Paps.FSM
{
    public class StateMachineStartedException : Exception
    {
        public StateMachineStartedException() : base("State machine is started, so the requested operation cannot be done")
        {

        }
    }
}
