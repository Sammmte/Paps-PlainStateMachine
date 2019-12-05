using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultipleValidTransitionsFromSameStateException : Exception
{
    public MultipleValidTransitionsFromSameStateException(string stateFrom, string trigger) 
        : base("State " + stateFrom + " has more than one valid transition with trigger " + trigger + 
            ". You may want to check your guard conditions or add some for preventing this exception")
    {

    }
}
