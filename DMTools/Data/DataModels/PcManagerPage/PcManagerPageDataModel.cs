using Data.Objects;
using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.PcManagerPage
{
    public class PcManagerPageDataModel : DataModel
    {
        private ObservableCollection<PlayerCharacter> _PlayerCharaters;

        public ObservableCollection<PlayerCharacter> PlayerCharaters
        {
            get
            {
                return _PlayerCharaters;
            }
            set
            {
                if (_PlayerCharaters != value)
                {
                    _PlayerCharaters = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
