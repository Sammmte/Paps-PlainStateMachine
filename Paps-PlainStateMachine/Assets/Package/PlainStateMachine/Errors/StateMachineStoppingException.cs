using System;

namespace Paps.StateMachines
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