using Data.Objects;
using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.InitiativeManagerPage
{
    public class InitiativeManagerPageDataModel : DataModel
    {
        private ObservableCollection<Monster> _Monsters;
        private Characteristic _Dexterity;
        private ObservableCollection<InitiativeEntry> _InitiativeEntries;
        private ObservableCollection<DisplayPlayerCharacter> _DisplayPlayerCharacters;

        public ObservableCollection<Monster> Monsters
        {
            get
            {
                return _Monsters;
            }
            set
            {
                if (_Monsters != value)
                {
                    _Monsters = value;
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

        public ObservableCollection<InitiativeEntry> InitiativeEntries
        {
            get
            {
                return _InitiativeEntries;
            }
            set
            {
                if (_InitiativeEntries != value)
                {
                    _InitiativeEntries = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DisplayPlayerCharacter> DisplayPlayerCharacters
        {
            get
            {
                return _DisplayPlayerCharacters;
            }
            set
            {
                if (_DisplayPlayerCharacters != value)
                {
                    _DisplayPlayerCharacters = value;
                    OnPropertyChanged();
                }
            }
        }

        public InitiativeManagerPageDataModel()
        {
            _Monsters = new ObservableCollection<Monster>();
            _InitiativeEntries = new ObservableCollection<InitiativeEntry>();
            _DisplayPlayerCharacters = new ObservableCollection<DisplayPlayerCharacter>();
        }
    }
}
