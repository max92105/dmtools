using System;

namespace Data.Objects
{
    public class Encounter : ObjectBase
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
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
