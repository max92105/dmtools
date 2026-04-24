using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.MonsterManagerPage
{
    public class MonsterManagerPageDataModel : DataModel
    {
        private ObservableCollection<DisplayMonster> _Monsters;       

        public ObservableCollection<DisplayMonster> Monsters
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

        public MonsterManagerPageDataModel()
        {
            _Monsters = new ObservableCollection<DisplayMonster>();
        }
    }
}
