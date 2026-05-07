using Data.Objects;
using System;

namespace Data.VirtualObject
{
    public class DisplaySkill : Skill
    {
        private String _Name;

        public String Name
        {
            get { return _Name; }
            set { if (value != _Name) { _Name = value; NotifyPropertyChanged("Name"); } }
        }

        // Set by SkillWindow from the monster's current ability modifiers — not persisted.
        public string AbilityKey          { get; set; } = "???";
        public string DefaultModifier     { get; set; } = "+0";
        public int    AbilityModifierValue { get; set; }
    }
}
