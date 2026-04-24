using System;

namespace Data.Objects
{
    public class Type : ObjectBase
    {
        private String _Name;

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
    }
}
