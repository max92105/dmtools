using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class PlayerCharacterFactory
    {
        public PlayerCharacter GetObject(Guid playerCharacterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<PlayerCharacter>("playerCharacters");
                var playerCharacter = collection.FindById(playerCharacterId);
                if (playerCharacter != null)
                {
                    playerCharacter.SetInternalState(InternalStates.UnModified, true);
                }
                return playerCharacter ?? new PlayerCharacter();
            }
        }

        public List<PlayerCharacter> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<PlayerCharacter>("playerCharacters");
                var playerCharacters = collection.FindAll().OrderBy(p => p.Name).ToList();
                foreach (var playerCharacter in playerCharacters)
                {
                    playerCharacter.SetInternalState(InternalStates.UnModified, true);
                }
                return playerCharacters;
            }
        }

        public List<PlayerCharacter> GetObjectsBySearchCriteria(String name)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<PlayerCharacter>("playerCharacters");
                var playerCharacters = collection.Find(p => p.Name.Contains(name)).OrderBy(p => p.Name).ToList();
                foreach (var playerCharacter in playerCharacters)
                {
                    playerCharacter.SetInternalState(InternalStates.UnModified, true);
                }
                return playerCharacters;
            }
        }

        public void SaveObject(PlayerCharacter playerCharacter)
        {
            if (playerCharacter.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<PlayerCharacter>("playerCharacters");

                    switch (playerCharacter.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(playerCharacter);
                            break;

                        case InternalStates.Modified:
                            collection.Update(playerCharacter);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(playerCharacter.Id);
                            break;
                    }

                    playerCharacter.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
