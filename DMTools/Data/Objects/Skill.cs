using System;

namespace Data.Objects
{
    public class Skill : ObjectBase
    {
        private Guid _MonsterId;
        private Guid _SkillTypeId;
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

        public Guid SkillTypeId
        {
            get
            {
                return _SkillTypeId;
            }

            set
            {
                if (value != _SkillTypeId)
                {
                    _SkillTypeId = value;
                    NotifyPropertyChanged("SkillTypeId");
                }
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