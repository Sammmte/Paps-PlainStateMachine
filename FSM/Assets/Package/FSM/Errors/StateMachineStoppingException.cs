using System;

namespace Paps.FSM
{
    public class StateMachineStoppingException : Exception
    {
        public StateMachineStoppingException()
        {

        }

        public StateMachineStoppingException(string message) : base(message)
        {

        }
    }
}