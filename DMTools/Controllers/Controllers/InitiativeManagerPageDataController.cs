using Controllers.Factories;
using Controllers.VirtualFactories;
using Data.Constants;
using Data.DataModels.InitiativeManagerPage;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public class InitiativeManagerPageDataController
    {
        public static InitiativeManagerPageDataModel Load()
        {
            InitiativeManagerPageDataModel initiativeManagerPageDataModel = new InitiativeManagerPageDataModel();

            initiativeManagerPageDataModel.Monsters = new ObservableCollection<Monster>(new MonsterFactory().GetObjects());
            initiativeManagerPageDataModel.DisplayPlayerCharacters = new ObservableCollection<DisplayPlayerCharacter>(new DisplayPlayerCharacterFactory().GetObjects());

            return initiativeManagerPageDataModel;
        }

        public static InitiativeManagerPageDataModel LoadDexterity(Guid monsterId)
        {
            InitiativeManagerPageDataModel initiativeManagerPageDataModel = new InitiativeManagerPageDataModel();

            initiativeManagerPageDataModel.Dexterity = new CharacteristicFactory().GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.DexterityId, monsterId);

            return initiativeManagerPageDataModel;
        }

        public static InitiativeManagerPageDataModel Search(String name)
        {
            InitiativeManagerPageDataModel initiativeManagerPageDataModel = new InitiativeManagerPageDataModel();

            initiativeManagerPageDataModel.Monsters = new ObservableCollection<Monster>(new MonsterFactory().GetObjectsBySearchCriteria(name));

            return initiativeManagerPageDataModel;
        }

        public static void SavePlayerCharacter(ObservableCollection<PlayerCharacter> playerCharacters)
        {
            try
            {
                foreach (PlayerCharacter playerCharacter in playerCharacters)
                {
                    new PlayerCharacterFactory().SaveObject(playerCharacter);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
