using System;

namespace Paps.StateMachines
{
    public class StateMachineNotStartedException : Exception
    {
        public StateMachineNotStartedException() : base("State machine is not started. Try using Start() method")
        {

        }
    }
}
