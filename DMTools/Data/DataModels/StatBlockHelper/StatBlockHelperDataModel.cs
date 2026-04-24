using Data.Objects;
using System.Collections.ObjectModel;

namespace Data.DataModels.StatBlockHelper
{
    public class StatBlockHelperDataModel : DataModel
    {
        private Monster _Monster;
        private ObservableCollection<Characteristic> _Characteristics;
        private ObservableCollection<CharacteristicType> _CharacteristicTypes;
        private ObservableCollection<Skill> _Skills;
        private ObservableCollection<SkillType> _SkillTypes;
        private ObservableCollection<SpecialAbility> _SpecialAbilities;
        private ObservableCollection<Action> _Actions;

        public Monster Monster
        {
            get
            {
                return _Monster;
            }
            set
            {
                if (_Monster != value)
                {
                    _Monster = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Characteristic> Characteristics
        {
            get
            {
                return _Characteristics;
            }
            set
            {
                if (_Characteristics != value)
                {
                    _Characteristics = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CharacteristicType> CharacteristicTypes
        {
            get
            {
                return _CharacteristicTypes;
            }
            set
            {
                if (_CharacteristicTypes != value)
                {
                    _CharacteristicTypes = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Skill> Skills
        {
            get
            {
                return _Skills;
            }
            set
            {
                if (_Skills != value)
                {
                    _Skills = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SkillType> SkillTypes
        {
            get
            {
                return _SkillTypes;
            }
            set
            {
                if (_SkillTypes != value)
                {
                    _SkillTypes = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SpecialAbility> SpecialAbilities
        {
            get
            {
                return _SpecialAbilities;
            }
            set
            {
                if (_SpecialAbilities != value)
                {
                    _SpecialAbilities = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Action> Actions
        {
            get
            {
                return _Actions;
            }
            set
            {
                if (_Actions != value)
                {
                    _Actions = value;
                    OnPropertyChanged();
                }
            }
        }

        public StatBlockHelperDataModel()
        {
            _Characteristics = new ObservableCollection<Characteristic>();
            _CharacteristicTypes = new ObservableCollection<CharacteristicType>();
            _Skills = new ObservableCollection<Skill>();
            _SkillTypes = new ObservableCollection<SkillType>();
            _SpecialAbilities = new ObservableCollection<SpecialAbility>();
            _Actions = new ObservableCollection<Action>();
        }
    }
}
