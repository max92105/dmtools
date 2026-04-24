using Controllers.Factories;
using Controllers.VirtualFactories;
using Data.DataModels.InitiativeManagerPage;
using Data.DataModels.PcManagerPage;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public class PcManagerPageDataController
    {
        public static PcManagerPageDataModel Load()
        {
            PcManagerPageDataModel pcManagerPageDataModel = new PcManagerPageDataModel();

            pcManagerPageDataModel.PlayerCharaters = new ObservableCollection<PlayerCharacter>(new PlayerCharacterFactory().GetObjects());

            return pcManagerPageDataModel;
        }

        public static PcManagerPageDataModel SearchMonster(String name)
        {
            PcManagerPageDataModel pcManagerPageDataModel = new PcManagerPageDataModel();

            pcManagerPageDataModel.PlayerCharaters = new ObservableCollection<PlayerCharacter>(new PlayerCharacterFactory().GetObjectsBySearchCriteria(name));

            return pcManagerPageDataModel;
        }
    }
}
