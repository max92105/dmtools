using System;

namespace Data.Objects
{
    public class Speed : ObjectBase
    {
        private Guid _MonsterId;
        private String _SpeedType;
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

        public String SpeedType
        {
            get { return _SpeedType; }
            set
            {
                if (value != _SpeedType)
                {
                    _SpeedType = value;
                    NotifyPropertyChanged("SpeedType");
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
            get { return SpeedType + " " + Value + " ft."; }
        }
    }
}
