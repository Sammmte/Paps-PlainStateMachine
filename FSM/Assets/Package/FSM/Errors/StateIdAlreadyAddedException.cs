using System;

namespace Paps.FSM
{
    public class StateIdAlreadyAddedException : Exception
    {
        public StateIdAlreadyAddedException()
        {

        }

        public StateIdAlreadyAddedException(string message) : base(message)
        {

        }
    }
}
