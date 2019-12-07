using System;

namespace Paps.FSM
{
    public class InvalidInitialStateException : Exception
    {
        public InvalidInitialStateException() : base("Initial state is invalid")
        {

        }
    }
}
