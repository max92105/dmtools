using System;

namespace Data.Objects
{
    public class SpecialAbility : ObjectBase
    {
        private Guid _MonsterId;
        private String _Name;
        private String _Description;
        private Int16 _AttackBonus;
        private Int32 _SortOrder;

        public Guid MonsterId
        {
            get
            {
                return _MonsterId;
            }

            set
            {
                if (value != _MonsterId)
                {
                    _MonsterId = value;
                    NotifyPropertyChanged("MonsterId");
                }
            }
        }

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

        public String Description
        {
            get
            {
                return _Description;
            }

            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public Int16 AttackBonus
        {
            get
            {
                return _AttackBonus;
            }

            set
            {
                if (value != _AttackBonus)
                {
                    _AttackBonus = value;
                    NotifyPropertyChanged("AttackBonus");
                }
            }
        }

        public Int32 SortOrder
        {
            get { return _SortOrder; }
            set
            {
                if (value != _SortOrder)
                {
                    _SortOrder = value;
                    NotifyPropertyChanged("SortOrder");
                }
            }
        }
    }
}