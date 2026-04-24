using System;

namespace Data.Objects
{
    public class PlayerCharacter : ObjectBase
    {
        private String _Name;
        private Int16 _ArmorClass;
        private Int16 _InitiativeBonus;
        private Int16 _Level;

        public String Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public Int16 ArmorClass
        {
            get
            {
                return _ArmorClass;
            }

            set
            {
                if (value != _ArmorClass)
                {
                    _ArmorClass = value;
                    NotifyPropertyChanged("ArmorClass");
                }
            }
        }

        public Int16 InitiativeBonus
        {
            get
            {
                return _InitiativeBonus;
            }

            set
            {
                if (value != _InitiativeBonus)
                {
                    _InitiativeBonus = value;
                    NotifyPropertyChanged("InitiativeBonus");
                }
            }
        }

        public Int16 Level
        {
            get
            {
                return _Level;
            }

            set
            {
                if (value != _Level)
                {
                    _Level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }
    }
}
