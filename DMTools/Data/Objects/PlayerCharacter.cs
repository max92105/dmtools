using System;

namespace Data.Objects
{
    public class PlayerCharacter : ObjectBase
    {
        private String _Name;
        private Int16 _ArmorClass;
        private Int16 _InitiativeBonus;
        private Int16 _Level;
        private Int16 _MaxHp;
        private Int16 _PassivePerception;

        public String Name
        {
            get { return _Name; }
            set
            {
                if (value != _Name) { _Name = value; NotifyPropertyChanged("Name"); }
            }
        }

        public Int16 ArmorClass
        {
            get { return _ArmorClass; }
            set
            {
                if (value != _ArmorClass) { _ArmorClass = value; NotifyPropertyChanged("ArmorClass"); }
            }
        }

        public Int16 InitiativeBonus
        {
            get { return _InitiativeBonus; }
            set
            {
                if (value != _InitiativeBonus) { _InitiativeBonus = value; NotifyPropertyChanged("InitiativeBonus"); }
            }
        }

        public Int16 Level
        {
            get { return _Level; }
            set
            {
                if (value != _Level) { _Level = value; NotifyPropertyChanged("Level"); }
            }
        }

        public Int16 MaxHp
        {
            get { return _MaxHp; }
            set
            {
                if (value != _MaxHp) { _MaxHp = value; NotifyPropertyChanged("MaxHp"); }
            }
        }

        public Int16 PassivePerception
        {
            get { return _PassivePerception; }
            set
            {
                if (value != _PassivePerception) { _PassivePerception = value; NotifyPropertyChanged("PassivePerception"); }
            }
        }
    }
}
