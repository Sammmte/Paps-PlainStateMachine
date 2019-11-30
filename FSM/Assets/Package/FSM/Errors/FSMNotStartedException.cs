using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public class FSMNotStartedException : Exception
    {
        public FSMNotStartedException() : base("State machine is not started. Try using Start() method")
        {

        }
    }
}
