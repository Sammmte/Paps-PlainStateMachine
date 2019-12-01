using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paps.FSM
{
    public interface IGuardCondition
    {
        bool IsValid();
    }
}


