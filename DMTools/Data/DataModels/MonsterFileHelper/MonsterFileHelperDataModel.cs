using Data.Objects;
using System.Collections.Generic;

namespace Data.DataModels.MonsterFileHelper
{
    public class MonsterFileHelperDataModel : DataModel
    {
        private List<Monster> _Monsters;
        private List<Characteristic> _Characteristics;
        private List<Skill> _Skills;
        private List<SpecialAbility> _SpecialAbilities;
        private List<Action> _Actions;
        private List<ArmorClassEntry> _ArmorClassEntries;
        private List<Speed> _Speeds;
        private List<Sense> _Senses;
        private List<DamageModifier> _DamageModifiers;

        public List<Monster> Monsters
        {
            get { return _Monsters; }
            set { if (_Monsters != value) { _Monsters = value; OnPropertyChanged(); } }
        }

        public List<Characteristic> Characteristics
        {
            get { return _Characteristics; }
            set { if (_Characteristics != value) { _Characteristics = value; OnPropertyChanged(); } }
        }

        public List<Skill> Skills
        {
            get { return _Skills; }
            set { if (_Skills != value) { _Skills = value; OnPropertyChanged(); } }
        }

        public List<SpecialAbility> SpecialAbilities
        {
            get { return _SpecialAbilities; }
            set { if (_SpecialAbilities != value) { _SpecialAbilities = value; OnPropertyChanged(); } }
        }

        public List<Action> Actions
        {
            get { return _Actions; }
            set { if (_Actions != value) { _Actions = value; OnPropertyChanged(); } }
        }

        public List<ArmorClassEntry> ArmorClassEntries
        {
            get { return _ArmorClassEntries; }
            set { if (_ArmorClassEntries != value) { _ArmorClassEntries = value; OnPropertyChanged(); } }
        }

        public List<Speed> Speeds
        {
            get { return _Speeds; }
            set { if (_Speeds != value) { _Speeds = value; OnPropertyChanged(); } }
        }

        public List<Sense> Senses
        {
            get { return _Senses; }
            set { if (_Senses != value) { _Senses = value; OnPropertyChanged(); } }
        }

        public List<DamageModifier> DamageModifiers
        {
            get { return _DamageModifiers; }
            set { if (_DamageModifiers != value) { _DamageModifiers = value; OnPropertyChanged(); } }
        }

        public MonsterFileHelperDataModel()
        {
            _Monsters = new List<Monster>();
            _Characteristics = new List<Characteristic>();
            _Skills = new List<Skill>();
            _SpecialAbilities = new List<SpecialAbility>();
            _Actions = new List<Action>();
            _ArmorClassEntries = new List<ArmorClassEntry>();
            _Speeds = new List<Speed>();
            _Senses = new List<Sense>();
            _DamageModifiers = new List<DamageModifier>();
        }
    }
}
