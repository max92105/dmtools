using System;

namespace Data.Objects
{
    public class InitiativeEntry : ObjectBase
    {
        private Guid    _MonsterId;
        private Guid    _PlayerCharacterId;
        private Boolean _ItsTurn;
        private String  _Name;
        private Int16   _DexterityModifier;
        private Int16   _ArmorClass;
        private Int16   _HitPoints;
        private Int16   _MaxHitPoints;
        private Int32   _Initiative;
        private Int32   _TieBreaker;
        private Int32   _Wave;
        private Int32   _ExhaustionLevel;
        private String  _ApplyValue = "0";

        private Boolean _IsBlinded;
        private Boolean _IsCharmed;
        private Boolean _IsDeafened;
        private Boolean _IsFrightened;
        private Boolean _IsGrappled;
        private Boolean _IsIncapacitated;
        private Boolean _IsInvisible;
        private Boolean _IsParalyzed;
        private Boolean _IsPetrified;
        private Boolean _IsPoisoned;
        private Boolean _IsProne;
        private Boolean _IsRestrained;
        private Boolean _IsStunned;
        private Boolean _IsUnconscious;

        public String ConditionImmunities { get; set; }
        public String WaveColor { get; set; } = "#E69A28";

        public Boolean IsBlinded       { get { return _IsBlinded;       } set { if (value != _IsBlinded)       { _IsBlinded       = value; NotifyPropertyChanged("IsBlinded");       } } }
        public Boolean IsCharmed       { get { return _IsCharmed;       } set { if (value != _IsCharmed)       { _IsCharmed       = value; NotifyPropertyChanged("IsCharmed");       } } }
        public Boolean IsDeafened      { get { return _IsDeafened;      } set { if (value != _IsDeafened)      { _IsDeafened      = value; NotifyPropertyChanged("IsDeafened");      } } }
        public Boolean IsFrightened    { get { return _IsFrightened;    } set { if (value != _IsFrightened)    { _IsFrightened    = value; NotifyPropertyChanged("IsFrightened");    } } }
        public Boolean IsGrappled      { get { return _IsGrappled;      } set { if (value != _IsGrappled)      { _IsGrappled      = value; NotifyPropertyChanged("IsGrappled");      } } }
        public Boolean IsIncapacitated { get { return _IsIncapacitated; } set { if (value != _IsIncapacitated) { _IsIncapacitated = value; NotifyPropertyChanged("IsIncapacitated"); } } }
        public Boolean IsInvisible     { get { return _IsInvisible;     } set { if (value != _IsInvisible)     { _IsInvisible     = value; NotifyPropertyChanged("IsInvisible");     } } }
        public Boolean IsParalyzed     { get { return _IsParalyzed;     } set { if (value != _IsParalyzed)     { _IsParalyzed     = value; NotifyPropertyChanged("IsParalyzed");     } } }
        public Boolean IsPetrified     { get { return _IsPetrified;     } set { if (value != _IsPetrified)     { _IsPetrified     = value; NotifyPropertyChanged("IsPetrified");     } } }
        public Boolean IsPoisoned      { get { return _IsPoisoned;      } set { if (value != _IsPoisoned)      { _IsPoisoned      = value; NotifyPropertyChanged("IsPoisoned");      } } }
        public Boolean IsProne         { get { return _IsProne;         } set { if (value != _IsProne)         { _IsProne         = value; NotifyPropertyChanged("IsProne");         } } }
        public Boolean IsRestrained    { get { return _IsRestrained;    } set { if (value != _IsRestrained)    { _IsRestrained    = value; NotifyPropertyChanged("IsRestrained");    } } }
        public Boolean IsStunned       { get { return _IsStunned;       } set { if (value != _IsStunned)       { _IsStunned       = value; NotifyPropertyChanged("IsStunned");       } } }
        public Boolean IsUnconscious   { get { return _IsUnconscious;   } set { if (value != _IsUnconscious)   { _IsUnconscious   = value; NotifyPropertyChanged("IsUnconscious");   } } }

        public Guid MonsterId
        {
            get { return _MonsterId; }
            set { if (value != _MonsterId) { _MonsterId = value; NotifyPropertyChanged("MonsterId"); } }
        }

        public Guid PlayerCharacterId
        {
            get { return _PlayerCharacterId; }
            set { if (value != _PlayerCharacterId) { _PlayerCharacterId = value; NotifyPropertyChanged("PlayerCharacterId"); } }
        }

        public Boolean ItsTurn
        {
            get { return _ItsTurn; }
            set { if (value != _ItsTurn) { _ItsTurn = value; NotifyPropertyChanged("ItsTurn"); } }
        }

        public String Name
        {
            get { return _Name; }
            set { if (value != _Name) { _Name = value; NotifyPropertyChanged("Name"); } }
        }

        public Int16 DexterityModifier
        {
            get { return _DexterityModifier; }
            set { if (value != _DexterityModifier) { _DexterityModifier = value; NotifyPropertyChanged("DexterityModifier"); } }
        }

        public Int16 ArmorClass
        {
            get { return _ArmorClass; }
            set { if (value != _ArmorClass) { _ArmorClass = value; NotifyPropertyChanged("ArmorClass"); } }
        }

        public Int16 HitPoints
        {
            get { return _HitPoints; }
            set { if (value != _HitPoints) { _HitPoints = value; NotifyPropertyChanged("HitPoints"); NotifyPropertyChanged("HpPercent"); } }
        }

        public Int16 MaxHitPoints
        {
            get { return _MaxHitPoints; }
            set { if (value != _MaxHitPoints) { _MaxHitPoints = value; NotifyPropertyChanged("MaxHitPoints"); NotifyPropertyChanged("HpPercent"); } }
        }

        public Double HpPercent
        {
            get
            {
                if (_MaxHitPoints <= 0) return 1.0;
                return Math.Min(1.0, Math.Max(0.0, (double)_HitPoints / _MaxHitPoints));
            }
        }

        public Int32 Initiative
        {
            get { return _Initiative; }
            set { if (value != _Initiative) { _Initiative = value; NotifyPropertyChanged("Initiative"); } }
        }

        public Int32 TieBreaker
        {
            get { return _TieBreaker; }
            set { if (value != _TieBreaker) { _TieBreaker = value; NotifyPropertyChanged("TieBreaker"); } }
        }

        public Int32 Wave
        {
            get { return _Wave; }
            set { if (value != _Wave) { _Wave = value; NotifyPropertyChanged("Wave"); } }
        }

        public Int32 ExhaustionLevel
        {
            get { return _ExhaustionLevel; }
            set { if (value != _ExhaustionLevel) { _ExhaustionLevel = Math.Max(0, Math.Min(6, value)); NotifyPropertyChanged("ExhaustionLevel"); } }
        }

        public String ApplyValue
        {
            get { return _ApplyValue; }
            set { if (value != _ApplyValue) { _ApplyValue = value; NotifyPropertyChanged("ApplyValue"); } }
        }
    }
}
