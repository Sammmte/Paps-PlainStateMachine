using System;

namespace Paps.FSM
{
    public class FSMStartedException : Exception
    {
        public FSMStartedException() : base("State machine is started, so the requested operation cannot be done")
        {

        }
    }
}
