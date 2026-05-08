using System;

namespace Data.Objects
{
    /// <summary>
    /// Represents a damage modifier (resistance or vulnerability) with a dice pool.
    /// Instead of classic half/double, the creature rolls dice to reduce or amplify damage.
    /// </summary>
    public class DamageModifier : ObjectBase
    {
        private Guid _MonsterId;
        private String _DamageType;
        private String _ModifierType; // "Resistance", "Vulnerability", "Immunity"
        private Int16 _DiceCount;
        private Int16 _DiceSize;
        private Int32 _SortOrder;

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

        /// <summary>
        /// The damage type (e.g. Fire, Cold, Slashing)
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
        /// "Resistance", "Vulnerability", or "Immunity"
        /// </summary>
        public String ModifierType
        {
            get { return _ModifierType; }
            set
            {
                if (value != _ModifierType)
                {
                    _ModifierType = value;
                    NotifyPropertyChanged("ModifierType");
                }
            }
        }

        /// <summary>
        /// Number of dice to roll (e.g. 2 in "2d6")
        /// </summary>
        public Int16 DiceCount
        {
            get { return _DiceCount; }
            set
            {
                if (value != _DiceCount)
                {
                    _DiceCount = value;
                    NotifyPropertyChanged("DiceCount");
                }
            }
        }

        /// <summary>
        /// Size of dice to roll (e.g. 6 in "2d6")
        /// </summary>
        public Int16 DiceSize
        {
            get { return _DiceSize; }
            set
            {
                if (value != _DiceSize)
                {
                    _DiceSize = value;
                    NotifyPropertyChanged("DiceSize");
                }
            }
        }

        public Int32 SortOrder
        {
            get { return _SortOrder; }
            set
            {
                if (value != _SortOrder)
                {
                    _SortOrder = value;
                    NotifyPropertyChanged("SortOrder");
                }
            }
        }

        public String DiceNotation
        {
            get
            {
                if (ModifierType == "Immunity") return "Immune";
                return DiceCount + "d" + DiceSize;
            }
        }

        public String Display
        {
            get { return DamageType + " " + ModifierType + " (" + DiceNotation + ")"; }
        }
    }
}
