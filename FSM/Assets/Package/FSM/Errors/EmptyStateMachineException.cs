using System;

namespace Paps.FSM
{
    public class EmptyStateMachineException : Exception
    {
        public EmptyStateMachineException()
        {

        }

        public EmptyStateMachineException(string message) : base(message)
        {

        }
    }
}


