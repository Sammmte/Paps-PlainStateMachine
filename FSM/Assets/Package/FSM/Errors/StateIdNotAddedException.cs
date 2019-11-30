using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public class StateIdNotAddedException : Exception
    {
        public StateIdNotAddedException(string stateIdString) : base("No state with id " + stateIdString + " was added to state machine")
        {

        }

        public StateIdNotAddedException()
        {

        }
    }
}
