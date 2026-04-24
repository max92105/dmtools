using Data.Objects;
using System;

namespace Data.VirtualObject
{
    public class DisplayPlayerCharacter : PlayerCharacter
    {
        private Int16 _Initiative;

        public Int16 Initiative
        {
            get
            {
                return _Initiative;
            }

            set
            {
                if (value != _Initiative)
                {
                    _Initiative = value;
                    NotifyPropertyChanged("Initiative");
                }
            }
        }
    }
}
