using System;

namespace Paps.StateMachines
{
    public class InvalidInitialStateException : Exception
    {
        public InvalidInitialStateException() : base("Initial state is invalid")
        {

        }

        public InvalidInitialStateException(string message) : base(message)
        {

        }
    }
}
