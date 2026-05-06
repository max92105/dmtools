using System;
using System.Collections.Generic;

namespace DMTools.Helpers
{
    public class EncounterPendingEntry
    {
        public Guid MonsterId { get; set; }
        public int Quantity { get; set; }
    }

    public static class EncounterService
    {
        public static List<EncounterPendingEntry> PendingEntries { get; set; }
    }
}
