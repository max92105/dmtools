using Data.Objects;
using System;

namespace Data.VirtualObject
{
    public class DisplaySkill : Skill
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
