using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.VirtualFactories
{
    public class DisplayPlayerCharacterFactory
    {
        public List<DisplayPlayerCharacter> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<PlayerCharacter>("playerCharacters");
                var playerCharacters = collection.FindAll().OrderBy(p => p.Name).ToList();

                var displayPlayerCharacters = playerCharacters.Select(p => new DisplayPlayerCharacter
                {
                    Id = p.Id,
                    Name = p.Name,
                    ArmorClass = p.ArmorClass,
                    InitiativeBonus = p.InitiativeBonus,
                    Level = p.Level
                }).ToList();

                foreach (var dpc in displayPlayerCharacters)
                {
                    dpc.SetInternalState(InternalStates.UnModified, true);
                }

                return displayPlayerCharacters;
            }
        }
    }
}
