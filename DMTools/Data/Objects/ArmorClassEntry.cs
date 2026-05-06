using System;

namespace Data.Objects
{
    /// <summary>
    /// Represents an armor class value for a specific condition/phase.
    /// e.g. "With Shield: 18", "Without Shield: 16", "Phase 2: 14"
    /// </summary>
    public class ArmorClassEntry : ObjectBase
    {
        private Guid _MonsterId;
        private String _Label;
        private Int16 _Value;

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
        /// Description of when this AC applies (e.g. "Default", "With Shield", "Phase 2")
        /// </summary>
        public String Label
        {
            get { return _Label; }
            set
            {
                if (value != _Label)
                {
                    _Label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        public Int16 Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public String Display
        {
            get
            {
                if (String.IsNullOrEmpty(_Label) || _Label == "Default")
                    return Value.ToString();
                return Label + ": " + Value;
            }
        }
    }
}
