using System;

namespace Data.Objects
{
    public class Sense : ObjectBase
    {
        private Guid _MonsterId;
        private String _SenseType;
        private Int16 _Value;

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

        public String SenseType
        {
            get { return _SenseType; }
            set
            {
                if (value != _SenseType)
                {
                    _SenseType = value;
                    NotifyPropertyChanged("SenseType");
                }
            }
        }

        public Int16 Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public String Display
        {
            get { return SenseType + " " + Value + " ft."; }
        }
    }
}
