using System;

namespace Data.Objects
{
    public class Characteristic : ObjectBase
    {
        private Guid _MonsterId;
        private Guid _CharacteristicTypeId;
        private Int16 _Score;
        private Int16 _Save;

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

        public Guid CharacteristicTypeId
        {
            get
            {
                return _CharacteristicTypeId;
            }

            set
            {
                if (value != _CharacteristicTypeId)
                {
                    _CharacteristicTypeId = value;
                    NotifyPropertyChanged("CharacteristicTypeId");
                }
            }
        }

        public Int16 Score
        {
            get
            {
                return _Score;
            }

            set
            {
                if (value != _Score)
                {
                    _Score = value;
                    NotifyPropertyChanged("Score");
                }
            }
        }

        public Int32 Modifier
        {
            get
            {
                return ((_Score - 10) / 2);
            }
        }

        public Int16 Save
        {
            get
            {
                return _Save;
            }

            set
            {
                if (value != _Save)
                {
                    _Save = value;
                    NotifyPropertyChanged("Save");
                }
            }
        }
    }
}