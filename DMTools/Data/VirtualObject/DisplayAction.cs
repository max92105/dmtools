using System;

namespace Data.VirtualObject
{
    public class DisplayAction
    {
        public Guid   Id          { get; set; }
        public string MonsterName { get; set; }
        public string ActionName  { get; set; }
        public string Description { get; set; }
        public string DisplayName => $"{MonsterName} — {ActionName}";
    }
}
