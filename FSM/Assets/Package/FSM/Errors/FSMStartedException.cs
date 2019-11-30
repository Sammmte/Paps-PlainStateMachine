using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public class FSMStartedException : Exception
    {
        public FSMStartedException() : base("State machine is started, so the requested operation cannot be done")
        {

        }
    }
}
