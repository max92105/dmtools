using System;

namespace Data.Objects
{
    public class EncounterEntry : ObjectBase
    {
        private Guid _EncounterId;
        private Guid _MonsterId;
        private string _MonsterName;
        private int _Quantity;

        public Guid EncounterId
        {
            get { return _EncounterId; }
            set
            {
                if (value != _EncounterId)
                {
                    _EncounterId = value;
                    NotifyPropertyChanged("EncounterId");
                }
            }
        }

        public Guid MonsterId
        {
            get { return _MonsterId; }
            set
            {
                if (value != _MonsterId)
                {
                    _MonsterId = value;
                    NotifyPropertyChanged("MonsterId");
                }
            }
        }

        public string MonsterName
        {
            get { return _MonsterName; }
            set
            {
                if (value != _MonsterName)
                {
                    _MonsterName = value;
                    NotifyPropertyChanged("MonsterName");
                }
            }
        }

        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (value != _Quantity)
                {
                    _Quantity = value;
                    NotifyPropertyChanged("Quantity");
                }
            }
        }
    }
}
