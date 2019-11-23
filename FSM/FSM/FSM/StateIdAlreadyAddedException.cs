using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
