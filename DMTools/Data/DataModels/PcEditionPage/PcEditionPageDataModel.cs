using Data.Objects;
using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.PcEditionPage
{
    public class PcEditionPageDataModel : DataModel
    {
        private PlayerCharacter _PlayerCharacter;

        public PlayerCharacter PlayerCharacter
        {
            get
            {
                return _PlayerCharacter;
            }
            set
            {
                if (_PlayerCharacter != value)
                {
                    _PlayerCharacter = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
