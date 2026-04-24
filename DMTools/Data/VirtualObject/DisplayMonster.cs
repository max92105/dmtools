using System;

namespace Data.VirtualObject
{
    public class DisplayMonster
    {
        public Guid Id { get; set; }
        public Boolean IsSelected { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public String Subtype { get; set; }
        public Int16 ArmorClass { get; set; }
        public Int16 HitPoints { get; set; }
        public Decimal ChallengeRating { get; set; }
    }
}
