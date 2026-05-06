using System;

namespace Data.Objects
{
    public class Action : ObjectBase
    {
        private Guid _MonsterId;
        private String _Name;
        private String _Description;
        private Int16 _AttackBonus;
        private String _DamageDice;
        private Int16 _DamageBonus;
        private Boolean _IsLegendary;
        private Boolean _IsBonus;
        private Boolean _IsReaction;
        private String _Range;
        private String _DamageType;
        private String _AttackAbility; // STR, DEX, etc. - used for auto-calculating attack bonus
        private Boolean _OverrideAttackBonus; // true = use manual AttackBonus, false = calculate from ability

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

        public String Name
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

        public String Description
        {
            get { return _Description; }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public Int16 AttackBonus
        {
            get { return _AttackBonus; }
            set
            {
                if (value != _AttackBonus)
                {
                    _AttackBonus = value;
                    NotifyPropertyChanged("AttackBonus");
                }
            }
        }

        public String DamageDice
        {
            get { return _DamageDice; }
            set
            {
                if (value != _DamageDice)
                {
                    _DamageDice = value;
                    NotifyPropertyChanged("DamageDice");
                }
            }
        }

        public Int16 DamageBonus
        {
            get { return _DamageBonus; }
            set
            {
                if (value != _DamageBonus)
                {
                    _DamageBonus = value;
                    NotifyPropertyChanged("DamageBonus");
                }
            }
        }

        public Boolean IsLegendary
        {
            get { return _IsLegendary; }
            set
            {
                if (value != _IsLegendary)
                {
                    _IsLegendary = value;
                    NotifyPropertyChanged("IsLegendary");
                }
            }
        }

        public Boolean IsBonus
        {
            get { return _IsBonus; }
            set
            {
                if (value != _IsBonus)
                {
                    _IsBonus = value;
                    NotifyPropertyChanged("IsBonus");
                }
            }
        }

        public Boolean IsReaction
        {
            get { return _IsReaction; }
            set
            {
                if (value != _IsReaction)
                {
                    _IsReaction = value;
                    NotifyPropertyChanged("IsReaction");
                }
            }
        }

        /// <summary>
        /// The range of the attack (e.g. "Melee (5 ft.)", "Ranged (80/320 ft.)")
        /// </summary>
        public String Range
        {
            get { return _Range; }
            set
            {
                if (value != _Range)
                {
                    _Range = value;
                    NotifyPropertyChanged("Range");
                }
            }
        }

        /// <summary>
        /// The damage type (e.g. "Slashing", "Fire")
        /// </summary>
        public String DamageType
        {
            get { return _DamageType; }
            set
            {
                if (value != _DamageType)
                {
                    _DamageType = value;
                    NotifyPropertyChanged("DamageType");
                }
            }
        }

        /// <summary>
        /// Which ability modifier to use for attack/damage (e.g. "STR", "DEX")
        /// If empty, uses manual AttackBonus/DamageBonus.
        /// </summary>
        public String AttackAbility
        {
            get { return _AttackAbility; }
            set
            {
                if (value != _AttackAbility)
                {
                    _AttackAbility = value;
                    NotifyPropertyChanged("AttackAbility");
                }
            }
        }

        /// <summary>
        /// If true, the AttackBonus field is used as-is. 
        /// If false, it is calculated from AttackAbility + proficiency.
        /// </summary>
        public Boolean OverrideAttackBonus
        {
            get { return _OverrideAttackBonus; }
            set
            {
                if (value != _OverrideAttackBonus)
                {
                    _OverrideAttackBonus = value;
                    NotifyPropertyChanged("OverrideAttackBonus");
                }
            }
        }
    }
}
