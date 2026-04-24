using Controllers.Factories;
using Data.DataModels.PcEditionPage;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public class PcEditionPageDataController
    {
        public static PcEditionPageDataModel LoadNewPlayerCharacter()
        {
            PcEditionPageDataModel pcEditionPageDataModel = new PcEditionPageDataModel();

            NewPlayerCharacter(pcEditionPageDataModel);

            return pcEditionPageDataModel;
        }

        public static PcEditionPageDataModel LoadPlayerCharacter(Guid playerCharacterId)
        {
            PcEditionPageDataModel pcEditionPageDataModel = new PcEditionPageDataModel();

            pcEditionPageDataModel.PlayerCharacter = new PlayerCharacterFactory().GetObject(playerCharacterId);

            return pcEditionPageDataModel;
        }

        public static PcEditionPageDataModel NewPlayerCharacter(PcEditionPageDataModel pcEditionPageDataModel)
        {
            pcEditionPageDataModel.PlayerCharacter = new PlayerCharacter()
            {
                Id = Guid.NewGuid(),
                Name = "New Player Character",
                ArmorClass = 10,
                InitiativeBonus = 0,
                Level = 1
            };

            return pcEditionPageDataModel;
        }

        public static void Save(PcEditionPageDataModel pcEditionPageDataModel)
        {
            if (pcEditionPageDataModel.PlayerCharacter != null)
                new PlayerCharacterFactory().SaveObject(pcEditionPageDataModel.PlayerCharacter);
        }
    }
}
