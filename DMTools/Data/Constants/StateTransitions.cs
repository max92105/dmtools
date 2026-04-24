using System;
using System.Collections.Generic;

namespace Data.Constant
{
    public static class StateTransitions
    {
        public static List<Tuple<InternalStates, InternalStates>> Transitions =
            new List<Tuple<InternalStates, InternalStates>>()
            {
                new Tuple<InternalStates, InternalStates>(InternalStates.UnModified, InternalStates.Modified),
                new Tuple<InternalStates, InternalStates>(InternalStates.UnModified, InternalStates.Deleted)
            };    
    }
}
