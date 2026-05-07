using System;

namespace Data.Objects
{
    public class Monster : ObjectBase
    {
        private String _Name;
        private String _Size;
        private String _Type;
        private String _Subtype;
        private String _Alignment;
        private Int16 _ArmorClass;
        private Int16 _HitPoints;
        private String _HitDice;
        private String _Speed;
        private String _DamageVulnerabilities;
        private String _DamageResistances;
        private String _DamageImmunities;
        private String _ConditionImmunities;
        private String _Senses;
        private String _Languages;
        private Decimal _ChallengeRating;

        public String Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if(value != _Name)
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public String Size
        {
            get
            {
                return _Size;
            }

            set
            {
                if (value != _Size)
                {
                    _Size = value;
                    NotifyPropertyChanged("Size");
                }
            }
        }

        public String Type
        {
            get
            {
                return _Type;
            }

            set
            {
                if (value != _Type)
                {
                    _Type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        public String Subtype
        {
            get
            {
                return _Subtype;
            }

            set
            {
                if (value != _Subtype)
                {
                    _Subtype = value;
                    NotifyPropertyChanged("Subtype");
                }
            }
        }

        public String Alignment
        {
            get
            {
                return _Alignment;
            }

            set
            {
                if (value != _Alignment)
                {
                    _Alignment = value;
                    NotifyPropertyChanged("Alignment");
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

        public String HitDice
        {
            get
            {
                return _HitDice;
            }

            set
            {
                if (value != _HitDice)
                {
                    _HitDice = value;
                    NotifyPropertyChanged("HitDice");
                }
            }
        }

        public String Speed
        {
            get
            {
                return _Speed;
            }

            set
            {
                if (value != _Speed)
                {
                    _Speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }

        public String DamageVulnerabilities
        {
            get
            {
                return _DamageVulnerabilities;
            }

            set
            {
                if (value != _DamageVulnerabilities)
                {
                    _DamageVulnerabilities = value;
                    NotifyPropertyChanged("DamageVulnerabilities");
                }
            }
        }

        public String DamageResistances
        {
            get
            {
                return _DamageResistances;
            }

            set
            {
                if (value != _DamageResistances)
                {
                    _DamageResistances = value;
                    NotifyPropertyChanged("DamageResistances");
                }
            }
        }

        public String DamageImmunities
        {
            get
            {
                return _DamageImmunities;
            }

            set
            {
                if (value != _DamageImmunities)
                {
                    _DamageImmunities = value;
                    NotifyPropertyChanged("DamageImmunities");
                }
            }
        }

        public String ConditionImmunities
        {
            get
            {
                return _ConditionImmunities;
            }

            set
            {
                if (value != _ConditionImmunities)
                {
                    _ConditionImmunities = value;
                    NotifyPropertyChanged("ConditionImmunities");
                }
            }
        }

        public String Senses
        {
            get
            {
                return _Senses;
            }

            set
            {
                if (value != _Senses)
                {
                    _Senses = value;
                    NotifyPropertyChanged("Senses");
                }
            }
        }

        public String Languages
        {
            get
            {
                return _Languages;
            }

            set
            {
                if (value != _Languages)
                {
                    _Languages = value;
                    NotifyPropertyChanged("Languages");
                }
            }
        }

        public Decimal ChallengeRating
        {
            get
            {
                return _ChallengeRating;
            }

            set
            {
                if (value != _ChallengeRating)
                {
                    _ChallengeRating = value;
                    NotifyPropertyChanged("ChallengeRating");
                }
            }
        }

        private Int16 _ProficiencyBonus;

        // 0 = auto-compute from CR (D&D 5e table). Set to a non-zero value to override.
        public Int16 ProficiencyBonus
        {
            get { return _ProficiencyBonus; }
            set
            {
                if (value != _ProficiencyBonus)
                {
                    _ProficiencyBonus = value;
                    NotifyPropertyChanged("ProficiencyBonus");
                }
            }
        }
    }
}