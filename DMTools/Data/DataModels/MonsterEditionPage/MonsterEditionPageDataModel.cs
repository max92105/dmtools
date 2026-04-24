using Data.Objects;
using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.MonsterEditionPage
{
    public class MonsterEditionPageDataModel : DataModel
    {
        private Monster _Monster;
        private Characteristic _Strength;
        private Characteristic _Dexterity;
        private Characteristic _Constitution;
        private Characteristic _Intelligence;
        private Characteristic _Wisdom;
        private Characteristic _Charisma;
        private ObservableCollection<DisplaySkill> _DisplaySkills;
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

        public Characteristic Strength
        {
            get
            {
                return _Strength;
            }
            set
            {
                if (_Strength != value)
                {
                    _Strength = value;
                    OnPropertyChanged();
                }
            }
        }

        public Characteristic Dexterity
        {
            get
            {
                return _Dexterity;
            }
            set
            {
                if (_Dexterity != value)
                {
                    _Dexterity = value;
                    OnPropertyChanged();
                }
            }
        }

        public Characteristic Constitution
        {
            get
            {
                return _Constitution;
            }
            set
            {
                if (_Constitution != value)
                {
                    _Constitution = value;
                    OnPropertyChanged();
                }
            }
        }

        public Characteristic Intelligence
        {
            get
            {
                return _Intelligence;
            }
            set
            {
                if (_Intelligence != value)
                {
                    _Intelligence = value;
                    OnPropertyChanged();
                }
            }
        }

        public Characteristic Wisdom
        {
            get
            {
                return _Wisdom;
            }
            set
            {
                if (_Wisdom != value)
                {
                    _Wisdom = value;
                    OnPropertyChanged();
                }
            }
        }

        public Characteristic Charisma
        {
            get
            {
                return _Charisma;
            }
            set
            {
                if (_Charisma != value)
                {
                    _Charisma = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DisplaySkill> DisplaySkills
        {
            get
            {
                return _DisplaySkills;
            }
            set
            {
                if (_DisplaySkills != value)
                {
                    _DisplaySkills = value;
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

        public MonsterEditionPageDataModel()
        {
            _DisplaySkills = new ObservableCollection<DisplaySkill>();
            _SpecialAbilities = new ObservableCollection<SpecialAbility>();
            _Actions = new ObservableCollection<Action>();
        }
    }
}
