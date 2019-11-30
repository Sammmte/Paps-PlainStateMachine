using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public interface IFSMTransition<TState, TTrigger>
    {
        TState StateFrom { get; }
        TTrigger Trigger { get; }
        TState StateTo { get; }
    }
}
