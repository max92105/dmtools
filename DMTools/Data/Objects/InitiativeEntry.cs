using System;

namespace Data.Objects
{
    public class InitiativeEntry : ObjectBase
    {       
        private Guid _MonsterId;
        private Guid _PlayerCharacterId;
        private Boolean _ItsTurn;
        private String _Name;
        private Int16 _DexterityModifier;
        private Int16 _ArmorClass;
        private Int16 _HitPoints;
        private Int32 _Initiative;
        private Int32 _TieBreaker;

        public Boolean IsBlinded { get; set; }
        public Boolean IsCharmed { get; set; }
        public Boolean IsDeafened { get; set; }
        public Boolean IsFrightened { get; set; }
        public Boolean IsIncapacitated { get; set; }
        public Boolean IsInvisible { get; set; }
        public Boolean IsParalyzed { get; set; }
        public Boolean IsPetrified { get; set; }
        public Boolean IsPoisoned { get; set; }
        public Boolean IsRestrained { get; set; }
        public Boolean IsStunned { get; set; }

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

        public Guid PlayerCharacterId
        {
            get
            {
                return _PlayerCharacterId;
            }

            set
            {
                if (value != _PlayerCharacterId)
                {
                    _PlayerCharacterId = value;
                    NotifyPropertyChanged("PlayerCharacterId");
                }
            }
        }

        public Boolean ItsTurn
        {
            get
            {
                return _ItsTurn;
            }

            set
            {
                if (value != _ItsTurn)
                {
                    _ItsTurn = value;
                    NotifyPropertyChanged("ItsTurn");
                }
            }
        }

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

        public Int16 DexterityModifier
        {
            get
            {
                return _DexterityModifier;
            }

            set
            {
                if (value != _DexterityModifier)
                {
                    _DexterityModifier = value;
                    NotifyPropertyChanged("DexterityModifier");
                }
            }
        }

        public Int16 ArmorClass
        {
            get
            {
                return _ArmorClass;
            }

            set
            {
                if (value != _ArmorClass)
                {
                    _ArmorClass = value;
                    NotifyPropertyChanged("ArmorClass");
                }
            }
        }

        public Int16 HitPoints
        {
            get
            {
                return _HitPoints;
            }

            set
            {
                if (value != _HitPoints)
                {
                    _HitPoints = value;
                    NotifyPropertyChanged("HitPoints");
                }
            }
        }

        public Int32 Initiative
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

        public Int32 TieBreaker
        {
            get
            {
                return _TieBreaker;
            }

            set
            {
                if (value != _TieBreaker)
                {
                    _TieBreaker = value;
                    NotifyPropertyChanged("TieBreaker");
                }
            }
        }
    }
}
