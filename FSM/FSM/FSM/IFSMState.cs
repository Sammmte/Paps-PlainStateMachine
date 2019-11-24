﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paps.FSM
{
    public interface IFSMState<TState, TTrigger>
    {
        IFSM<TState, TTrigger> StateMachine { get; }

        void Enter();
        void Update();
        void Exit();
    }
}
