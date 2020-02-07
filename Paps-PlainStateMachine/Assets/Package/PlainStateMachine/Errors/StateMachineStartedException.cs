using System;

namespace Paps.StateMachines
{
    public class StateMachineStartedException : Exception
    {
        public StateMachineStartedException() : base("State machine is started, so the requested operation cannot be done")
        {

        }
    }
}
