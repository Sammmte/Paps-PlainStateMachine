using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public class UnboundFSMException : Exception
    {
        public UnboundFSMException() : base("the state does not have an associated state machine")
        {

        }
    }
}
