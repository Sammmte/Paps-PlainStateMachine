using System;

namespace Paps.FSM
{
    public class StateMachineExitingException : Exception
    {
        public StateMachineExitingException()
        {

        }

        public StateMachineExitingException(string message) : base(message)
        {

        }
    }
}