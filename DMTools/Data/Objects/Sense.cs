using System;

namespace Data.Objects
{
    public class Sense : ObjectBase
    {
        private Guid _MonsterId;
        private String _SenseType;
        private Int16 _Value;
        private Int32 _SortOrder;

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
                    NotifyPropertyChanged("Display");
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
                    NotifyPropertyChanged("Display");
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

        public String Display
        {
            get { return SenseType + " " + Value + " ft."; }
        }
    }
}
