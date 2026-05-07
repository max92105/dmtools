using System;

namespace Data.VirtualObject
{
    public class DisplaySpecialAbility
    {
        public Guid Id { get; set; }
        public string MonsterName  { get; set; }
        public string AbilityName  { get; set; }
        public string Description  { get; set; }
        public string DisplayName  => $"{MonsterName} — {AbilityName}";
    }
}
