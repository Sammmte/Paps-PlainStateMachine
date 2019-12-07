using System;

namespace Paps.FSM
{
    public class FSMNotStartedException : Exception
    {
        public FSMNotStartedException() : base("State machine is not started. Try using Start() method")
        {

        }
    }
}
