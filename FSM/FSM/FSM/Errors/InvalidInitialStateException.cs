using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public class InvalidInitialStateException : Exception
    {
        public InvalidInitialStateException() : base("Initial state is invalid")
        {

        }
    }
}
